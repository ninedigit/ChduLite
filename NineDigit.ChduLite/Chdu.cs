using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NineDigit.ChduLite.Commands;
using NineDigit.ChduLite.Transport;
using NineDigit.SerialTransport;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NineDigit.ChduLite
{
    /// <summary>
    /// Represents CHDU Lite device.
    /// </summary>
    public sealed class Chdu : IDisposable
    {
        readonly ITransport serialTransport;
        readonly ICommandTransport commandTransport;
        readonly ICommandTransportInitializer commandTransportInitializer;
        readonly SemaphoreSlim commandLock = new(1, 1);

        /// <summary>
        /// </summary>
        /// <param name="portName">Name of serial port.</param>
        /// <param name="printerBaudRate">Serial printer communication speed.</param>
        /// <param name="defaultTimeout"></param>
        public Chdu(string portName, int printerBaudRate, TimeSpan defaultTimeout)
            : this(portName, printerBaudRate, defaultTimeout, NullLoggerFactory.Instance)
        { }

        /// <summary>
        /// </summary>
        /// <param name="portName">Name of serial port.</param>
        /// <param name="printerBaudRate">Serial printer communication speed.</param>
        /// <param name="defaultTimeout"></param>
        /// <param name="loggerFactory">The logger factory</param>
        public Chdu(string portName, int printerBaudRate, TimeSpan defaultTimeout, ILoggerFactory loggerFactory)
            : this(CreateSerialTransport(portName, printerBaudRate, defaultTimeout, loggerFactory), ownsTransport: true, loggerFactory)
        {
        }
        
        internal Chdu(ITransport transport, bool ownsTransport, ILoggerFactory loggerFactory)
        {
            this.serialTransport = transport ?? throw new ArgumentNullException(nameof(transport));
            this.commandTransportInitializer = new CommandTransportInitializer(transport);
            this.commandTransport = new CommandTransport(transport, ownsTransport, this.commandTransportInitializer, loggerFactory);
        }

        /// <summary>
        /// Získa objekt poskytujúci stav transportnej vrstvy.
        /// </summary>
        public ITransportConnection TransportConnection => this.serialTransport;

        /// <summary>
        /// Vyčítanie bloku na danej adrese.
        /// </summary>
        /// <param name="address">Adresa bloku</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Blok</returns>
        public async Task<Block> ReadBlockAsync(BlockAddress address, CancellationToken cancellationToken)
        {
            var blocks = await ReadBlocksAsync(address, blocksCount: 1, cancellationToken).ConfigureAwait(false);
            return blocks[0];
        }

        /// <summary>
        /// Vyčítanie blokov podľa počiatočnej adresy.
        /// </summary>
        /// <param name="address">Adresa prvého bloku</param>
        /// <param name="blocksCount">Počet blokov na vyčítanie</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Pole blokov</returns>
        public async Task<Block[]> ReadBlocksAsync(BlockAddress address, uint blocksCount, CancellationToken cancellationToken)
        {
            await this.commandLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (blocksCount < ReadBlocksCommand.MaxBlocksCount)
                {
                    var cmd = new ReadBlocksCommand(address, blocksCount);
                    return await this.commandTransport.ExecuteCommandAsync(cmd, cancellationToken).ConfigureAwait(false);
                }

                var result = new Block[blocksCount];
                var blocksProcessed = 0u;
                while (blocksProcessed < blocksCount)
                {
                    var blocksToRead = Math.Min(blocksCount - blocksProcessed, ReadBlocksCommand.MaxBlocksCount);
                    var addr = address + blocksProcessed;
                    var cmd = new ReadBlocksCommand(addr, blocksToRead);
                    var blocks = await this.commandTransport.ExecuteCommandAsync(cmd, cancellationToken).ConfigureAwait(false);
                    blocks.CopyTo(result, index: blocksProcessed);
                    blocksProcessed += blocksToRead;
                }

                return result;
            }
            finally
            {
                this.commandLock.Release();
            }
        }

        /// <summary>
        /// Zapíše blok dát.
        /// </summary>
        /// <param name="blockContent">Blok dát na zapísanie</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Adresa</returns>
        public Task<BlockWriteResult> WriteBlockAsync(BlockContent blockContent, CancellationToken cancellationToken)
        {
            var cmd = new WriteBlockCommand(blockContent, BlockWriteMode.Save);
            return LockAndExecuteSingleCommand(cmd, cancellationToken);
        }

        /// <summary>
        /// Zapíše a kompletne vytlačí blok dát.
        /// </summary>
        /// <param name="blockContent">Blok dát na zapísanie a kompletné vytlačenie</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<BlockWriteResult> WriteAndPrintBlockAsync(BlockContent blockContent, CancellationToken cancellationToken)
        {
            var cmd = new WriteBlockCommand(blockContent, BlockWriteMode.SaveAndPrint);
            return LockAndExecuteSingleCommand(cmd, cancellationToken);
        }

        /// <summary>
        /// Zapíše a čiastočne vytlačí blok dát.
        /// </summary>
        /// <param name="blockContent">Blok dát na zapísanie a čiastočné vytlačenie.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<BlockWriteResult> WriteAndPrintBlockAsync(OffsettedBlockContent blockContent, CancellationToken cancellationToken)
        {
            var cmd = new WriteBlockCommand(blockContent);
            return LockAndExecuteSingleCommand(cmd, cancellationToken);
        }

        /// <summary>
        /// Načítanie statusu
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ChduLiteStatus> GetStatusAsync(CancellationToken cancellationToken)
        {
            var cmd = new GetDeviceStatusCommand();
            return LockAndExecuteSingleCommand(cmd, cancellationToken);
        }

        /// <summary>
        /// Otvorenie peňažnej zásuvky
        /// </summary>
        /// <returns></returns>
        public Task OpenDrawerAsync(DrawerPin drawerPin, CancellationToken cancellationToken)
        {
            var cmd = new OpenDrawerCommand(drawerPin);
            return LockAndExecuteSingleCommand(cmd, cancellationToken);
        }

        /// <summary>
        /// Nevratné zamknutie CHDÚ
        /// <para>
        /// Funkcia je dostupná od firmvéru verzie 1.1 a novšej.
        /// </para>
        /// </summary>
        /// <param name="magicNumber">Náhodné číslo</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task LockMemoryAsync(uint magicNumber, CancellationToken cancellationToken)
        {
            await this.commandLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                var requestLockCommand = new RequestLockCommand(magicNumber);
                var authCode = await this.commandTransport.ExecuteCommandAsync(requestLockCommand, cancellationToken).ConfigureAwait(false);
                
                var activateLockCommand = new ActivateLockCommand(magicNumber, authCode);
                await this.commandTransport.ExecuteCommandAsync(activateLockCommand, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                this.commandLock.Release();
            }
        }

        /// <summary>
        /// Získa informácie o logickom zväzku dát.
        /// <para>
        /// Funkcia je dostupná od firmvéru verzie 1.1 a novšej.
        /// </para>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ChduLiteVolumeInfo> GetVolumeInfoAsync(CancellationToken cancellationToken)
        {
            var cmd = new GetVolumeInfoCommand();
            return LockAndExecuteSingleCommand(cmd, cancellationToken);
        }

        /// <summary>
        /// Navráti úplnú textovú reprezentáciu verzie firmvéru, vrátane tzv. patch segmentu.
        /// V prípade developerskej verzie môže obsahovať aj git hash.
        /// <para>
        /// Napríklad: <code>v1.3.1.-4-gXXXXXXXX</code> označuje 4 aplikované patche, pričom git hash posledného z nich je gXXXXXXXX.
        /// </para>
        /// <para>
        /// Funkcia je dostupná od firmvéru verzie 1.3 a novšej.
        /// </para>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <example>v1.3.1.-4-gXXXXXXX</example>
        public Task<string> GetFirmwareVersionDescriptionAsync(CancellationToken cancellationToken)
        {
            var cmd = new GetFirmwareVersionDescriptionCommand();
            return LockAndExecuteSingleCommand(cmd, cancellationToken);
        }

        #region Helpers

        private async Task<TResponse> LockAndExecuteSingleCommand<TResponse>(ChduLiteCommand<TResponse> command, CancellationToken cancellationToken)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            await this.commandLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                return await this.commandTransport.ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                this.commandLock.Release();
            }
        }

        private async Task LockAndExecuteSingleCommand(ChduLiteCommand command, CancellationToken cancellationToken)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            await this.commandLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                await this.commandTransport.ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                this.commandLock.Release();
            }
        }

        private static ITransport CreateSerialTransport(string portName, int printerBaudRate, TimeSpan defaultTimeout, ILoggerFactory loggerFactory)
        {
            var opts = new SerialPortOptions()
            {
                BaudRate = printerBaudRate,
                ReadTimeout = defaultTimeout,
                WriteTimeout = defaultTimeout
            };

            return TransportFactory.CreateSerialTransport(portName, opts, loggerFactory);
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue;

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.commandTransportInitializer.Dispose();
                    this.commandTransport.Dispose();
                    this.commandLock.Dispose();
                }

                disposedValue = true;
            }
        }
        
        /// <summary>
        /// Disposes current instance and its internal transport layer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
