using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Scai.Driver.Data;

/*
 * This is pretty good! https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples
 * Also, this could be base of web-based editor: https://codemirror.net/
 */
namespace Scai.Driver
{
    public class AiCompiler : IAiCompiler
    {
        private const string References = @"
using System.Linq;
using System.Collections.Generic;

var output = new List<string>();

void Print(string message)
{
    output.Add(message);
}
        ";

        private const string Entrypoint = "SetSpeed()";

        public CompilationResult Compile(string code)
        {
            var script = CSharpScript
                .Create(
                    code: References,
                    globalsType: typeof(TrackState),
                    options: Options())
                .ContinueWith(code: code)
                .ContinueWith<int>(code: Entrypoint);

            var diagnostics = script.Compile();
            if (diagnostics.Any())
            {
                var errors = diagnostics
                    .Select(MapDiagnostic)
                    .ToArray();

                return new CompilationResult(errors);
            }
            else
            {
                var ai = new Ai(script);

                return new CompilationResult(ai);
            }
        }

        private ScriptOptions Options() =>
            ScriptOptions.Default
                .WithImports("System.Math")
                .WithReferences(typeof(System.Linq.Enumerable).Assembly);

        private CompilationError MapDiagnostic(Diagnostic diagnostic)
        {
            var location = diagnostic.Location.GetMappedLineSpan();

            return new CompilationError
            {
                Severity = MapSeverity(diagnostic.Severity),
                Message = diagnostic.GetMessage(),
                StartLinePosition = location.StartLinePosition.Line,
                EndLinePosition = location.EndLinePosition.Line,
            };
        }

        private ErrorLevel MapSeverity(DiagnosticSeverity severity)
        {
            switch (severity)
            {
                case DiagnosticSeverity.Error: return ErrorLevel.Error;
                case DiagnosticSeverity.Warning: return ErrorLevel.Warning;
                case DiagnosticSeverity.Info: return ErrorLevel.Info;
                case DiagnosticSeverity.Hidden: return ErrorLevel.Info;
                default: return ErrorLevel.Info;
            }
        }
    }
}
