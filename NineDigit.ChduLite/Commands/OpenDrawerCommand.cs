using System;

namespace NineDigit.ChduLite.Commands
{
    internal sealed class OpenDrawerCommand : ChduLiteCommand
    {
        public OpenDrawerCommand(DrawerPin drawerPin)
            : base(GetCommandId(drawerPin))
        { }

        private static ChduLiteCommandId GetCommandId(DrawerPin drawerPin)
            => drawerPin switch
            {
                DrawerPin.Pin2 => ChduLiteCommandId.OpenDrawer,
                DrawerPin.Pin5 => ChduLiteCommandId.OpenDrawer2,
                _ => throw new NotImplementedException(),
            };
    }
}
