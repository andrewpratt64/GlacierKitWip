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

		public static GKCommand<Unit, Unit> UselessCommand1
		{ get; } = new GKCommand<Unit, Unit>
		(
			commandId: "PlaceholderModule_UselessCommand1",
			displayName: "Useless Command",
			command: ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default)
		);

		public static GKCommand<Unit, Unit> UselessCommand2
		{ get; } = new GKCommand<Unit, Unit>
		(
			commandId: "PlaceholderModule_UselessCommand2",
			displayName: "Yet Another Useless Command",
			command: ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default)
		);

		public static GKCommand<Unit, Unit> UselessCommand3
		{ get; } = new GKCommand<Unit, Unit>
		(
			commandId: "PlaceholderModule_UselessCommand3",
			displayName: "Pretend I do something",
			command: ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default)
		);
	}
}
