using System;
using System.Linq;

namespace NineDigit.ChduLite.Commands
{
    internal sealed class ReadBlocksCommand : ChduLiteCommand<Block[]>
    {
        readonly BlockAddress address;
        readonly uint blocksCount;

        // note: tato info nie je sucastou specky, ale zariadenie vrati najviac 127 blokov na jeden prikaz,
        // pricom pri prijati priakzu vrati ACK (na strane fw chyba kontrola vstupov?)
        
        // note2: zariadenie dokaz vratit aj viac, ale priblizne pri 975 bloku to moze spadnut.
        public const uint MaxBlocksCount = 127;

        public ReadBlocksCommand(BlockAddress address, uint blocksCount)
            : base(ChduLiteCommandId.ReadData)
        {
            if (blocksCount < 1)
                throw new ArgumentOutOfRangeException(nameof(blocksCount), "Počet blokov na vyčítanie nesmie byť menší, ako 1.");
            if (blocksCount > MaxBlocksCount)
                throw new ArgumentOutOfRangeException(nameof(blocksCount), $"Počet blokov na vyčítanie nesmie byť väčší, ako {MaxBlocksCount}.");

            this.address = address;
            this.blocksCount = blocksCount;
        }

        public override uint ResponseBlocksCount => this.blocksCount;
        public override uint MinResponseDataBytes => Block.MinPayloadLength;
        public override bool SupportsMultiPacketTransfer => true;

        public override Block[] ProcessResponse(ResponseMessage[] messages)
        {
            if (messages is null)
                throw new ArgumentNullException(nameof(messages));

            if (messages.Length != this.ResponseBlocksCount)
                throw new InvalidOperationException("Unexpected response blocks count.");

            var blocks = messages
                .Select(i => new Block(i))
                .ToArray();

            return blocks;
        }

        protected override byte[] GetArguments()
        {
            var buffer = new byte[8];

            Buffer.BlockCopy(BitConverter.GetBytes(this.address), 0, buffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(this.blocksCount), 0, buffer, 4, 4);

            return buffer;
        }
    }
}
