using NineDigit.ChduLite.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NineDigit.ChduLite.Transport
{
    internal interface ICommandTransport : IDisposable
    {
        Task ExecuteCommandAsync(ChduLiteCommand command, CancellationToken cancellationToken);
        Task<TResponse> ExecuteCommandAsync<TResponse>(ChduLiteCommand<TResponse> command, CancellationToken cancellationToken);
    }
}
