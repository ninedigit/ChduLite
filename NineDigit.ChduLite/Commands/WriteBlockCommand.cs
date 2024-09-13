using System;

namespace NineDigit.ChduLite.Commands
{
    internal sealed class WriteBlockCommand : ChduLiteCommand<BlockWriteResult>
    {
        readonly BlockContentBase blockContent;

        public WriteBlockCommand(OffsettedBlockContent blockContent)
            : this(ChduLiteCommandId.WriteDataAndPrintWithOffset, blockContent)
        { }

        public WriteBlockCommand(BlockContent blockContent, BlockWriteMode mode)
            : this(GetCommandId(mode), blockContent)
        { }

        private WriteBlockCommand(ChduLiteCommandId id, BlockContentBase blockContent)
            : base(id)
        {
            this.blockContent = blockContent
                ?? throw new ArgumentNullException(nameof(blockContent));
        }

        public sealed override uint ResponseBlocksCount => 1;
        public override uint MinResponseDataBytes => BlockWriteResult.PayloadLength;

        protected override byte[] GetArguments()
            => this.blockContent.GetDataToStore();

        public sealed override BlockWriteResult ProcessResponse(ResponseMessage[] response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            if (response.Length != ResponseBlocksCount)
                throw new InvalidOperationException("Unexpected response blocks count.");

            return new BlockWriteResult(response[0]);
        }

        private static ChduLiteCommandId GetCommandId(BlockWriteMode writeMode)
            => writeMode switch
            {
                BlockWriteMode.Save => ChduLiteCommandId.WriteData,
                BlockWriteMode.SaveAndPrint => ChduLiteCommandId.WriteDataAndPrint,
                _ => throw new NotImplementedException(),
            };
    }
}
