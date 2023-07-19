namespace NineDigit.ChduLite
{
    public static class ErrorCodeExtensions
    {
        /// <summary>
        /// Gets whether error code represents error caused
        /// by client side (PC), not the server side (CHDU device)
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns>
        /// <c>null</c>, if error code is <see cref="ErrorCode.Unknown"/>,
        /// <c>true</c>, if error code represents client error,
        /// <c>false</c> otherwise.
        /// </returns>
        public static bool? IsClientError(this ErrorCode errorCode)
        {
            if (errorCode == ErrorCode.Unknown)
                return null;

            return errorCode == ErrorCode.InvalidCommand
                || errorCode == ErrorCode.InvalidParameter
                || errorCode == ErrorCode.InvalidReadAddress;
        }
    }
}