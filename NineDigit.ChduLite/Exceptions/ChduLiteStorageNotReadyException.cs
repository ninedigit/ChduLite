using System;
using System.Runtime.Serialization;

namespace NineDigit.ChduLite.Exceptions
{
    public sealed class ChduLiteStorageNotReadyException : ChduLiteException
    {
        public ChduLiteStorageNotReadyException()
            : this("Informácie o dátovom úložisku nie je možné získať, pretože vnútorná pamäť zariadenia nie je pripravená na použitie.")
        {
        }

        public ChduLiteStorageNotReadyException(string message)
            : base(message)
        {
        }

        public ChduLiteStorageNotReadyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ChduLiteStorageNotReadyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
