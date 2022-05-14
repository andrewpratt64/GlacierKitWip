using GlacierKitCore.Attributes;
using GlacierKitCore.Attributes.DataProviders;
using GlacierKitCore.Commands;
using GlacierKitCore.ViewModels.Common;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Text;

namespace Models
{
    [GKDataProviderAttribute]
    public static class PlaceholderData
    {
		#region Commands

		[ExposeAsGKCommand]
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

		[ExposeAsGKCommand]
		public static GKCommand<Unit, Unit> UselessCommand1
		{ get; } = new GKCommand<Unit, Unit>
		(
			commandId: "PlaceholderModule_UselessCommand1",
			displayName: "Useless Command",
			command: ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default)
		);

		[ExposeAsGKCommand]
		public static GKCommand<Unit, Unit> UselessCommand2
		{ get; } = new GKCommand<Unit, Unit>
		(
			commandId: "PlaceholderModule_UselessCommand2",
			displayName: "Yet Another Useless Command",
			command: ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default)
		);

		[ExposeAsGKCommand]
		public static GKCommand<Unit, Unit> UselessCommand3
		{ get; } = new GKCommand<Unit, Unit>
		(
			commandId: "PlaceholderModule_UselessCommand3",
			displayName: "Pretend I do something",
			command: ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default)
		);

		#endregion


		#region Menu_items

		[ExposeAsMainMenuItem]
		public static MainMenuItemSetupInfo Placeholder_MenuItemGroupInfo
		{ get; } = new
		(
			title: "Placeholder",
			path: new string[] { "placeholder" },
			command: null
		);

		[ExposeAsMainMenuItem]
		public static MainMenuItemSetupInfo Placeholder_StuffToDo_MenuItemGroupInfo
		{ get; } = new
		(
			title: "Stuff to do",
			path: new string[] { "placeholder", "stuffToDo" },
			command: null
		);

		[ExposeAsMainMenuItem]
		public static MainMenuItemSetupInfo PrintHiMenuItemInfo
		{ get; } = new
		(
			title: "Print Hi",
			path: new string[] { "placeholder", "stuffToDo", "PlaceholderModule_printHi" },
			command: PrintHi
		);

		#endregion
	}
}
