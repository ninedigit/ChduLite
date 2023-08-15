using System;
using System.Runtime.Serialization;

namespace NineDigit.ChduLite.Exceptions
{
    [Serializable]
    public class CommandFailedException : ChduLiteTransportException
    {
        public CommandFailedException()
            : this(ErrorCode.Unknown)
        {
        }

        public CommandFailedException(ErrorCode errorCode)
            : this(CreateMessage(errorCode))
        {
            this.ErrorCode = errorCode;
        }

        public CommandFailedException(string message)
            : base(message)
        {
            this.ErrorCode = ErrorCode.Unknown;
        }

        public CommandFailedException(string message, Exception innerException)
            : this(message, ErrorCode.Unknown, innerException)
        {
        }

        public CommandFailedException(string message, ErrorCode errorCode, Exception innerException)
            : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }

        public ErrorCode ErrorCode { get; }

        private static string CreateMessage(ErrorCode errorCode)
            => $"Komunikácia s dátovým úložiskom zlyhala (chyba číslo {(int)errorCode}: {errorCode}).";

        protected CommandFailedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            serializationInfo.GetValue(nameof(ErrorCode), typeof(ErrorCode));
        }
    }
}