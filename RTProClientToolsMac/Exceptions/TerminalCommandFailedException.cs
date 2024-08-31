namespace RTProClientToolsMac.Exceptions
{
    [Serializable]
    internal class TerminalCommandFailedException : Exception
    {
        public TerminalCommandFailedException()
        {
        }

        public TerminalCommandFailedException(Exception ex) : base("", ex)
        {

        }
    }
}