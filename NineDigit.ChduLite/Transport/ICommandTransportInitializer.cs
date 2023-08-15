using System;
using System.Threading;
using System.Threading.Tasks;

namespace NineDigit.ChduLite.Transport
{
    /// <summary>
    /// Represents initializer that will be invoked before each command is sent, if <see cref="IsInitializationPending"/> is <c>true</c>.
    /// </summary>
    internal interface ICommandTransportInitializer : IDisposable
    {
        bool IsInitializationPending { get; }
        Task InitializeAsync(ICommandTransport transport, CancellationToken cancellationToken);
    }
}
