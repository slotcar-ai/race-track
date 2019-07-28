using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Scai.Driver.Data;

namespace Scai.Driver
{
    public class Ai : IAi
    {
        private readonly Script<int> _script;

        public Ai(Script<int> script)
        {
            _script = script;
        }

        public async Task<AiResult> RunAsync(TrackState state)
        {
            return await RunAsync(state, CancellationToken.None);
        }

        public async Task<AiResult> RunAsync(TrackState state, CancellationToken token)
        {
            return await TryRunAsync(state, token);
        }

        private async Task<AiResult> TryRunAsync(TrackState state, CancellationToken token)
        {
            // TODO: CTS not canceling RunAsync properly.
            using (var tokenSource = new CancellationTokenSource())
            {
                tokenSource.CancelAfter(100);
                try
                {
                    var scriptState = await _script.RunAsync(state, tokenSource.Token);

                    return MapResult(scriptState);
                }
                catch (Exception)
                {
                    return new AiResult(RuntimeError.Exception);
                }
            }
        }

        private AiResult MapResult(ScriptState<int> state)
        {
            return new AiResult(
                speed: state.ReturnValue,
                variables: state.Variables
                    .Select(variable => new ResultVariable
                    {
                        Name = variable.Name,
                        Type = variable.Type.ToString(),
                        Value = variable.Value == null ? "null" : variable.Value.ToString(),
                    })
                    .ToArray());
        }
    }
}
