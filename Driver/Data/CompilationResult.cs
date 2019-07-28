namespace Scai.Driver.Data
{
    public class CompilationResult
    {
        public bool IsOk { get; }

        public IAi Ai { get; }

        public CompilationError[] Errors { get; }

        public CompilationResult(IAi ai)
        {
            IsOk = true;
            Ai = ai;
        }

        public CompilationResult(CompilationError[] errors)
        {
            IsOk = false;
            Errors = errors;
        }
    }
}