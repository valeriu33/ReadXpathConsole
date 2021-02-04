using System;

namespace ReadXpathConsole
{
    public class WrongPathException: Exception
    {
        public WrongPathException(string message): base(message)
        {
            
        }
    }
}