using System;
using Scai.Driver.Data;
using Xunit;

namespace Scai.Driver.Test
{
    public class AiTest
    {
        [Theory]
        [InlineData(@"var aVariable = 42; int SetSpeed() => aVariable;", 42)]
        [InlineData(
            @"int SetSpeed()
            {
                var ns = new int[] { 1, 2, 3, 4, 5 };
                return ns.Aggregate(0, (acc, n) => acc + n);
            }",
            15)]
        [InlineData(
            @"int SetSpeed()
            {
                // This is a comment
                return (int) Sqrt(2);
            }",
            1)]
        [InlineData(
            @"int addTwo(int n)
            {
                Print($""n is set to {n}"");
                return n + 2;
            }

            int SetSpeed()
            {
                return addTwo(7) + Car.Speed;
            }",
            13)]
        public void ShouldCompileAndRun(string code, int speed)
        {
            var compiler = new AiCompiler();
            var compilationResult = compiler.Compile(code);
            Assert.True(compilationResult.IsOk, $"Compilation failed");

            var ai = compilationResult.Ai;
            var state = new TrackState
            {
                Car = new CarInfo
                {
                    Speed = 4,
                    Position = 11
                }
            };

            var aiResult = ai.Run(state);
            Assert.Equal(speed, aiResult.Speed);
            Assert.InRange(aiResult.ExecutionTime, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(100));
        }

        [Theory]
        [InlineData(@"var aVariable = new NotFoundClass();")]
        [InlineData(@"var aVariable = 42; // Missing SetSpeed()")]
        public void ShouldNotCompile(string code)
        {
            var compiler = new AiCompiler();
            var compilationResult = compiler.Compile(code);
            Assert.False(compilationResult.IsOk);
            Assert.NotEmpty(compilationResult.Errors);
            Assert.All(compilationResult.Errors, error =>
                Assert.Equal(ErrorLevel.Error, error.Severity));
        }

        [Theory]
        [InlineData(
            @"int SetSpeed()
            {
                var a = 1;
                while(a > 0) // Infinite loop
                {
                    a = 3;
                }
                return a;
            }",
            typeof(TimeoutException))]
        [InlineData(
            @"string AsString(object o)
            {
                return o.ToString();
            }

            int SetSpeed()
            {
                Print(AsString(null)); // Null-reference exception
                return 3;
            }",
            typeof(System.NullReferenceException))]
        public void ShouldNotRun(string code, Type runtimeError)
        {
            var compiler = new AiCompiler();
            var compilationResult = compiler.Compile(code);
            Assert.True(compilationResult.IsOk, "Compilation failed");

            var ai = compilationResult.Ai;
            var state = new TrackState
            {
                Car = new CarInfo
                {
                    Speed = 4,
                    Position = 11
                }
            };

            var aiResult = ai.Run(state);
            Assert.False(aiResult.IsOk);
            Assert.Equal(runtimeError, aiResult.RuntimeException.GetType());
        }
    }
}
