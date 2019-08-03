using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.Scripting;
using Scai.Driver.Data;

namespace Scai.Driver
{
    public class Ai : IAi
    {
        private const int TimeoutInMs = 500;
        private readonly Script<int> _script;

        public Ai(Script<int> script)
        {
            _script = script;
        }

        public AiResult Run(TrackState state)
        {
            ScriptState<int> scriptState = null;
            var thread = new Thread(async () =>
            {
                scriptState = await _script.RunAsync(state, ex => true);
            });

            var watch = Stopwatch.StartNew();

            thread.Start();
            var timedOut = !thread.Join(TimeoutInMs);

            watch.Stop();
            var executionTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);

            if (timedOut)
            {
                var exception = new TimeoutException("AI timed out");
                return new AiResult(exception, executionTime);
            }
            else if (scriptState.Exception != null)
            {
                return new AiResult(scriptState.Exception, executionTime);
            }
            else
            {
                return new AiResult(
                    speed: scriptState.ReturnValue,
                    variables: scriptState.Variables
                        .Select(variable => new ResultVariable
                        {
                            Name = variable.Name,
                            Type = variable.Type.ToString(),
                            Value = variable.Value == null ? "null" : variable.Value.ToString(),
                        })
                        .ToArray(),
                    executionTime: executionTime);
            }
        }
    }
}
