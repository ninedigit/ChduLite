using Microsoft.Extensions.Logging;
using NineDigit.ChduLite.Commands;
using NineDigit.ChduLite.Exceptions;
using NineDigit.SerialTransport;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NineDigit.ChduLite.Transport
{
    internal sealed class CommandTransport : ICommandTransport
    {
        readonly bool ownsTransport;
        readonly ITransport transport;
        readonly ICommandTransportInitializer initializer;
        readonly ILogger logger;

        public CommandTransport(ITransport transport, bool ownsTransport, ICommandTransportInitializer initializer, ILoggerFactory loggerFactory)
        {
            if (loggerFactory is null)
                throw new ArgumentNullException(nameof(loggerFactory));

            this.ownsTransport = ownsTransport;
            this.transport = transport ?? throw new ArgumentNullException(nameof(transport));
            this.initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));
            this.logger = loggerFactory.CreateLogger<CommandTransport>();
        }

        public async Task<TResponse> ExecuteCommandAsync<TResponse>(ChduLiteCommand<TResponse> command, CancellationToken cancellationToken)
        {
            await this.ExecuteCommandAsync((ChduLiteCommand)command, cancellationToken).ConfigureAwait(false);
            
            var response = await this.ReadCommandResponseAsync(command, cancellationToken).ConfigureAwait(false);

            return command.ProcessResponse(response);
        }

        /// <summary>
        /// Writes data to CHDU
        /// </summary>
        /// <param name="command">Command to send</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        public async Task ExecuteCommandAsync(ChduLiteCommand command, CancellationToken cancellationToken)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            cancellationToken.ThrowIfCancellationRequested();

            var dataToWrite = command.GetBytes();
            
            try
            {
                if (this.initializer.IsInitializationPending)
                {
                    this.logger.LogDebug("Executing transport initialization.");
                    await this.initializer.InitializeAsync(this, cancellationToken).ConfigureAwait(false);
                    this.logger.LogDebug("Transport initialization successful.");
                }

                var responseArray = await this.transport.WriteAndReadAsync(dataToWrite, responseLength: 1, cancellationToken).ConfigureAwait(false);
                var response = responseArray[0];

                switch (response)
                {
                    case (byte)ControlChars.ACK:
                        // this.logger.LogDebug("Received ACK from CHDU Lite.");
                        break;
                    case (byte)ControlChars.NAK:
                        this.logger.LogDebug("Received NAK from CHDU Lite.");
                        // after NAK, one more byte containing error code is available.
                        var errCode = (ErrorCode)await this.transport.ReadOneAsync(cancellationToken).ConfigureAwait(false);
                        this.logger.LogError("CHDU Lite device rejected command {commandBytes} with error code {errorCode}.", BitConverter.ToString(dataToWrite), errCode);
                        throw new CommandFailedException(errCode);
                    default:
                        this.logger.LogError("CHDU Lite device responded to command {commandBytes} with unknown response {response}.", BitConverter.ToString(dataToWrite), response);
                        throw new UnexpectedDeviceResponseException("Neočakávaná odpoveď z dátového úložiska. Uistite sa, že v nastaveniach aplikácie je zvolený správny model dátového úložiska.");
                }
            }
            catch (ChduLiteTransportException ex)
            {
                // keep connection open, if error is caused by client (not by device)
                var keepConnectionOpen = ex is CommandFailedException cmdFailedEx && cmdFailedEx.ErrorCode.IsClientError() == true;
                
                if (keepConnectionOpen)
                    this.logger.LogInformation("Chyba nie je vyhodnotená ako chyba zariadenia, spojenie so zariadením ostáva otvorené.");
                else
                    this.transport.Disconnect(ex);

                throw;
            }
            catch (WriteTransportException ex)
            {
                throw new ChduLiteTransportException("Chyba pri odosielaní údajov na dátové úložisko.", ex);
            }
            catch (BusyPortException ex)
            {
                throw new ChduLiteTransportException("Aplikácia nedokáže nadviazať spojenie s dátovým úložiskom, pretože spojenie je obsadené inou aplikáciou.", ex);
            }
            catch (TransportException ex) when (ex.InnerException is FileNotFoundException)
            {
                throw new ChduLiteTransportException($"Aplikácia nedokáže nadviazať spojenie s dátovym úložiskom. Uistite sa, že úložisko je pripojené na porte {ex.PortName}.", ex);
            }
            catch (TransportException ex) when (ex.InnerException is ReadTimeoutException timeoutEx)
            {
                throw new ChduLiteTransportException($"Odpoveď z dátového úložiska neprišla v stanovenom limite ({timeoutEx.Timeout} ms).", ex);
            }
            catch (TransportException ex)
            {
                throw new ChduLiteTransportException("Nastala transportná chyba pri komunikácii s chráneným dátovým úložiskom.", ex);
            }
        }

        private async Task<ResponseMessage[]> ReadCommandResponseAsync(IChduLiteResponseCommand command, CancellationToken cancellationToken)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            cancellationToken.ThrowIfCancellationRequested();

            var expectedBlocksCount = command.ResponseBlocksCount;
            var result = new ResponseMessage[expectedBlocksCount];

            try
            {
                for (int currentBlockIndex = 0; currentBlockIndex < expectedBlocksCount; currentBlockIndex++)
                {
                    var currentBlock = new byte[Block.MaxRawDataLength];

                    // Nacitam start byte
                    var startByte = await this.transport.ReadOneAsync(cancellationToken).ConfigureAwait(false);

                    // Overenie start byte-u. Odpoved musi zacinat STX
                    if (startByte != (byte)ControlChars.STX)
                        throw new UnexpectedDeviceResponseException($"Unexpected start byte: '{startByte}'.");

                    int receivedBytesCount = 0;
                    int totalBytesToRead = (int)command.MinResponseDataBytes + ResponseMessage.HeaderLength;
                    var totalBytesToReadParsePending = true;

                    // nacitame hlavicku (prve dva bajty obsahujuce celkovu dlzku) + minimalnu dlzku odpovede. Nasledne precitame zbytok spravy.
                    while (receivedBytesCount < totalBytesToRead)
                    {
                        receivedBytesCount += await this.transport.ReadAsync(
                            buffer: currentBlock,
                            offset: receivedBytesCount,
                            count: totalBytesToRead - receivedBytesCount,
                            cancellationToken: cancellationToken)
                            .ConfigureAwait(false);

                        if (totalBytesToReadParsePending && ResponseMessage.TryParsePayloadLength(currentBlock, out int payloadLength))
                        {
                            totalBytesToRead = payloadLength + ResponseMessage.HeaderLength;
                            totalBytesToReadParsePending = false;
                        }
                    }

                    // Odpoved na prijatie packetu sa pouziva len v pripade prikazov, ktorych odpovede su rozdelene do viacerych packetov (aktualne len pri READ)
                    var sendPacketReceivedConfirmation = command.SupportsMultiPacketTransfer;
                    if (sendPacketReceivedConfirmation)
                    {
                        // Pri prikaze "READ" musime do 1 sekundy od prijatia bloku odpovedat stavom:
                        // - ACK: ak bol blok v poriadku
                        // - NAK: ak by blok nebol v poriadku a chceme ho prijat znovu (max. 8 krat)
                        // - NUL: ak chceme prerusit prenos
                        if (cancellationToken.IsCancellationRequested)
                        {
                            await this.transport.WriteOneAsync((byte)ControlChars.NUL, CancellationToken.None).ConfigureAwait(false);
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                        else
                        {
                            await this.transport.WriteOneAsync((byte)ControlChars.ACK, cancellationToken).ConfigureAwait(false);
                        }
                    }

                    // Nacitam end byte
                    var endByte = await this.transport.ReadOneAsync(cancellationToken).ConfigureAwait(false);

                    // this.logger.LogDebug("Received end byte: {endbyte} (packet {packet}/{packetsCount})", endByte, currentBlockIndex + 1, expectedBlocksCount);

                    // Overim end byte
                    if (endByte == ControlChars.BEL)
                        result[currentBlockIndex] = new ResponseMessage(currentBlock, isCrcValid: false);
                    else if (endByte == ControlChars.ACK || endByte == ControlChars.EOT)
                        result[currentBlockIndex] = new ResponseMessage(currentBlock, isCrcValid: true);
                    else // (NAK or other)
                        throw new UnexpectedDeviceResponseException($"Unexpected end byte '{endByte}' while reading block {currentBlockIndex + 1} out of {expectedBlocksCount}.");

                    if (endByte == ControlChars.EOT && currentBlockIndex + 1 < expectedBlocksCount)
                        throw new IncompleteDataException($"Bolo prijatých iba {currentBlockIndex} blokov z očakávaných {expectedBlocksCount}.");
                }

                this.transport.DiscardBuffers();
            }
            catch (WriteTransportException ex)
            {
                throw new ChduLiteTransportException("Chyba pri odosielaní údajov na dátové úložisko.", ex);
            }
            catch (BusyPortException ex)
            {
                throw new ChduLiteTransportException("Aplikácia nedokáže nadviazať spojenie s dátovým úložiskom, pretože spojenie je obsadené inou aplikáciou.", ex);
            }
            catch (TransportException ex) when (ex.InnerException is FileNotFoundException)
            {
                throw new ChduLiteTransportException($"Aplikácia nedokáže nadviazať spojenie s dátovym úložiskom. Uistite sa, že úložisko je pripojené na porte {ex.PortName}.", ex);
            }
            catch (TransportException ex)
            {
                this.logger.LogError(ex, "Nastala transportná chyba pri čítaní odpovede z dátového úložiska.");
                throw;
            }
            catch (TimeoutException ex)
            {
                const string message = "Čítanie odpovede z dátového úložiska nebolo dokončené v stanovenom časovom limite.";
                this.logger.LogError(ex, message);
                throw new ChduLiteTransportException(message, ex);
            }
            catch (OperationCanceledException ex)
            {
                const string message = "Čítanie odpovede z dátového úložiska bolo prerušené.";
                this.logger.LogError(ex, message);
                throw new ChduLiteTransportException(message, ex);
            }
            catch (Exception ex)
            {
                const string message = "Nastala chyba pri čítaní odpovede z dátového úložiska.";
                this.logger.LogError(ex, message);
                throw new ChduLiteTransportException(message, ex);
            }

            return result;
        }

        #region IDisposable
        private bool disposedValue;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.ownsTransport)
                    {
                        try
                        {
                            this.transport.Dispose();
                        }
                        // "Port does not exists" occurs if device has been plugged out physically.
                        catch (IOException) { }
                    }
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
