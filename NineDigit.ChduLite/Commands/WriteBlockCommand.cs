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

        protected override byte[] GetArguments()
            => this.blockContent.GetRawData();

        public sealed override BlockWriteResult ProcessResponse(ResponseMessage[] response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.Length != ResponseBlocksCount)
                throw new InvalidOperationException("Unexpected response blocks count.");

            return new BlockWriteResult(response[0]);
        }

        private static ChduLiteCommandId GetCommandId(BlockWriteMode writeMode)
        {
            switch (writeMode)
            {
                case BlockWriteMode.Save:
                    return ChduLiteCommandId.WriteData;
                case BlockWriteMode.SaveAndPrint:
                    return ChduLiteCommandId.WriteDataAndPrint;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
