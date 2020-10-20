using System;
using System.Runtime.Serialization;

namespace MicroCommunication.Api.Exceptions
{
    [Serializable]
    public class DemoException : Exception
    {
        public DemoException() : base("This error was intended.") { }
        protected DemoException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}
