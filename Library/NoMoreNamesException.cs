namespace NameProvider
{
    using System;

    public class NoMoreNamesException : Exception
    {
        public NoMoreNamesException() {}

        public NoMoreNamesException(string message) : base(message) {}

        public NoMoreNamesException(string message, Exception innerException) : base(message, innerException) {}
    }
}