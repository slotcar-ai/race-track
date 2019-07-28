namespace Scai.Driver.Data
{
    public class CompilationError
    {
        public ErrorLevel Severity { get; set; }

        public string Message { get; set; }

        public int StartLinePosition { get; set; }

        public int EndLinePosition { get; set; }
    }
}