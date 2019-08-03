using System;

namespace Scai.Driver.Data
{
    public class AiResult
    {
        public bool IsOk { get; }

        public int Speed { get; }

        public ResultVariable[] Variables { get; }

        public TimeSpan ExecutionTime { get; }

        public Exception RuntimeException { get; }

        public AiResult(int speed, ResultVariable[] variables, TimeSpan executionTime)
        {
            IsOk = true;
            Speed = speed;
            Variables = variables;
            ExecutionTime = executionTime;
        }

        public AiResult(Exception runtimeException, TimeSpan executionTime)
        {
            IsOk = false;
            RuntimeException = runtimeException;
            ExecutionTime = executionTime;
        }
    }
}
