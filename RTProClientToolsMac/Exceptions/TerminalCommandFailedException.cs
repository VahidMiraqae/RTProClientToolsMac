using System.Runtime.Serialization;

namespace RTProClientToolsMac.Exceptions
{
    [Serializable]
    internal class TerminalCommandFailedException : Exception
    {
        private Exception ex;

        public TerminalCommandFailedException()
        {
        }

        public TerminalCommandFailedException(Exception ex)
        {
            this.ex = ex;
        }

        public TerminalCommandFailedException(string? message) : base(message)
        {
        }

        public TerminalCommandFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TerminalCommandFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}