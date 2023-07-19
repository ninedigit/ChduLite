using System;
using System.Runtime.Serialization;

namespace NineDigit.ChduLite.Exceptions
{
    [Serializable]
    public class IncompleteDataException : ChduLiteTransportException
    {
        public IncompleteDataException()
        {
        }

        public IncompleteDataException(string message)
            : base(message)
        {
        }

        public IncompleteDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected IncompleteDataException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}