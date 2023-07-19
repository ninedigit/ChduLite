using System;

namespace NineDigit.ChduLite.Commands
{
    internal sealed class OpenDrawerCommand : ChduLiteCommand
    {
        public OpenDrawerCommand(DrawerPin drawerPin)
            : base(GetCommandId(drawerPin))
        { }

        private static ChduLiteCommandId GetCommandId(DrawerPin drawerPin)
        {
            switch (drawerPin)
            {
                case DrawerPin.Pin2:
                    return ChduLiteCommandId.OpenDrawer;
                case DrawerPin.Pin5:
                    return ChduLiteCommandId.OpenDrawer2;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
