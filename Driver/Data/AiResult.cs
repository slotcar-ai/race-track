namespace Scai.Driver.Data
{
    public class AiResult
    {
        public bool IsOk { get; }

        public int Speed { get; }

        public ResultVariable[] Variables { get; }

        public RuntimeError Error { get; }

        public AiResult(int speed, ResultVariable[] variables)
        {
            IsOk = true;
            Speed = speed;
            Variables = variables;
        }

        public AiResult(RuntimeError runtimeError)
        {
            IsOk = false;
            Error = runtimeError;
        }
    }
}
