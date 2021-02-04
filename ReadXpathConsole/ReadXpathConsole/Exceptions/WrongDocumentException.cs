using System;

namespace ReadXpathConsole.Exceptions
{
    public class WrongDocumentException: Exception
    {
        public WrongDocumentException(string message): base(message)
        {
            
        }
    }
}