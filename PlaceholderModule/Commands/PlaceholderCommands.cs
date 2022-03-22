using GlacierKitCore.Attributes;
using GlacierKitCore.Commands;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Text;

namespace Commands
{
    [GKCommandProvider]
    public static class PlaceholderCommandProvider
    {
        public static GKCommand<Unit, Unit> PrintHi
        { get; } = new GKCommand<Unit, Unit>
        (
            commandId: "PlaceholderModule_PrintHi",
            displayName: "Print \"Hi\"",
            command: ReactiveCommand.Create<Unit, Unit>(_ =>
            {
                Trace.WriteLine("Hi");
                return Unit.Default;
            })
        );
    }
}
