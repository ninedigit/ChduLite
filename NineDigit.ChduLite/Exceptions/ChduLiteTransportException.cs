using System;
using System.Runtime.Serialization;

namespace NineDigit.ChduLite.Exceptions
{
    [Serializable]
    public class ChduLiteTransportException : ChduLiteException
    {
        internal ChduLiteTransportException()
        {
        }

        internal ChduLiteTransportException(string message)
            : base(message)
        {
        }

        internal ChduLiteTransportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ChduLiteTransportException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
