using NineDigit.ChduLite.Commands;
using NineDigit.ChduLite.Exceptions;
using NineDigit.SerialTransport;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NineDigit.ChduLite.Transport
{
    internal sealed class CommandTransportInitializer : ICommandTransportInitializer
    {
        public const int MaxConnectionCheckAttempts = 2;
        
        readonly ITransport serialTransport;

        public CommandTransportInitializer(ITransport serialTransport)
        {
            this.serialTransport = serialTransport ?? throw new ArgumentNullException(nameof(serialTransport));
            this.serialTransport.StateChanged += OnSerialTransportStateChanged;
        }

        #region Event handlers
        private void OnSerialTransportStateChanged(object sender, TransportConnectionStateChange e)
        {
            if (e.NewState == TransportConnectionState.Disconnected)
                this.IsInitialized = false;
        }
        #endregion

        public bool IsInitialized { get; private set; }
        public bool IsInitializing { get; private set; }
        public bool IsInitializationPending => !IsInitialized && !IsInitializing;

        public async Task InitializeAsync(ICommandTransport commandTransport, CancellationToken cancellationToken)
        {
            try
            {
                this.IsInitializing = true;

                await this.serialTransport.ExecuteSoftResetAsync(cancellationToken).ConfigureAwait(false);

                // use device status command execution as "connection check"
                var command = new GetDeviceStatusCommand();

                for (int i = 0; i < MaxConnectionCheckAttempts; i++)
                {
                    try
                    {
                        await commandTransport.ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false);
                        this.IsInitialized = true;
                        return;
                    }
                    catch (CommandFailedException) when (i < MaxConnectionCheckAttempts - 1)
                    {
                        // ignore command failed exception, unless max attempt is reached.
                    }
                    catch (Exception ex)
                    {
                        throw new ChduLiteTransportException("Inicializácia spojenia s chráneným dátovým úložiskom nebola úspešná.", ex);
                    }
                }
            }
            finally
            {
                this.IsInitializing = false;
            }
        }

        #region IDisposable
        private bool disposedValue;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.serialTransport.StateChanged -= OnSerialTransportStateChanged;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
