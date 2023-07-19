using System;
using System.Runtime.Serialization;

namespace NineDigit.ChduLite.Exceptions
{
    [Serializable]
    public class UnexpectedDeviceResponseException : ChduLiteTransportException
    {
        public UnexpectedDeviceResponseException()
        {
        }

        public UnexpectedDeviceResponseException(string message)
            : base(message)
        {
        }

        public UnexpectedDeviceResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UnexpectedDeviceResponseException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
