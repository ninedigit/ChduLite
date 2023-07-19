using System;
using System.Runtime.Serialization;

namespace NineDigit.ChduLite.Exceptions
{
    public abstract class ChduLiteException : Exception
    {
        protected ChduLiteException()
        {
        }

        protected ChduLiteException(string message)
            : base(message)
        {
        }

        protected ChduLiteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ChduLiteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
