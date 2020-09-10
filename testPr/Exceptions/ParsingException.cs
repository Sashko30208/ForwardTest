using System;

namespace testPr.Exceptions
{
    public class ParsingException : Exception
    {
        public ParsingException(string message) : base(message) { }
    }
}
