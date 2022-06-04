using GlacierKitCore.Attributes;
using GlacierKitCore.Attributes.DataProviders;
using GlacierKitCore.Commands;
using GlacierKitCore.Models;
using GlacierKitCore.Services;
using GlacierKitCore.ViewModels.Common;
using PlaceholderModule.ViewModels;
using PlaceholderModule.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Text;

namespace PlaceholderModule.Services
{
    public class PlaceholderDataProviderService : GKDataProviderService
	{
		#region Commands

#pragma warning disable CA1822 // Mark members as static
		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> PrintHi => new
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
		public GKCommand<Unit, Unit> UselessCommand1 => new
		(
			commandId: "PlaceholderModule_UselessCommand1",
			displayName: "Useless Command",
			command: ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default)
		);

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> UselessCommand2 => new
		(
			commandId: "PlaceholderModule_UselessCommand2",
			displayName: "Yet Another Useless Command",
			command: ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default)
		);

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> UselessCommand3 => new
		(
			commandId: "PlaceholderModule_UselessCommand3",
			displayName: "Pretend I do something",
			command: ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default)
		);


		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> OpenFooView => new
		(
			commandId: "PlaceholderModule_OpenFooView",
			displayName: "Open Foo view",
			command: ReactiveCommand.Create<Unit, Unit>(_ =>
			{
				Ctx.CreateEditorWindow.Execute(typeof(FooViewModel)).Subscribe();
				return Unit.Default;
			})
		);

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> OpenHelloWorldView => new
		(
			commandId: "PlaceholderModule_OpenHelloWorldView",
			displayName: "Open Hello World view",
			command: ReactiveCommand.Create<Unit, Unit>(_ =>
			{
				Ctx.CreateEditorWindow.Execute(typeof(HelloWorldViewModel)).Subscribe();
				return Unit.Default;
			})
		);

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> OpenPushyButtonsView => new
		(
			commandId: "PlaceholderModule_OpenPushyButtonsView",
			displayName: "Open Pushy Buttons view",
			command: ReactiveCommand.Create<Unit, Unit>(_ =>
			{
				Ctx.CreateEditorWindow.Execute(typeof(PushyButtonsViewModel)).Subscribe();
				return Unit.Default;
			})
		);

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> TMP_OpenTreeEditorAView => new
		(
			commandId: "PlaceholderModule_OpenTreeEditorAView",
			displayName: "Open Tree Editor A view",
			command: ReactiveCommand.Create<Unit, Unit>(_ =>
			{
				Ctx.CreateEditorWindow.Execute(typeof(TreeEditorAViewModel)).Subscribe();
				return Unit.Default;
			})
		);
#pragma warning restore CA1822 // Mark members as static

		#endregion


		#region Menu_items
#pragma warning disable CA1822 // Mark members as static

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo Placeholder_MenuItemGroupInfo => new
		(
			title: "Placeholder",
			path: new string[] { "placeholder" },
			command: null
		);

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo Placeholder_StuffToDo_MenuItemGroupInfo => new
		(
			title: "Stuff to do",
			path: new string[] { "placeholder", "stuffToDo" },
			command: null
		);

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo PrintHiMenuItemInfo => new
		(
			title: "Print Hi",
			path: new string[] { "placeholder", "stuffToDo", "PlaceholderModule_printHi" },
			command: PrintHi
		);

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo View_EditorWindows_OpenFooView_MenuItemInfo => new
		(
			title: "Foo",
			path: new string[] { "view", "editorWindows", "PlaceholderModule_openFooView" },
			command: OpenFooView
		);

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo View_EditorWindows_OpenHelloWorldView_MenuItemInfo => new
		(
			title: "Hello World",
			path: new string[] { "view", "editorWindows", "PlaceholderModule_openHelloWorldView" },
			command: OpenHelloWorldView
		);

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo View_EditorWindows_OpenPushyButtonsView_MenuItemInfo => new
		(
			title: "Pushy Buttons",
			path: new string[] { "view", "editorWindows", "PlaceholderModule_openPushyButtonsView" },
			command: OpenPushyButtonsView
		);

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo TMP_View_EditorWindows_OpenTreeEditorAView_MenuItemInfo => new
		(
			title: "Tree Editor A",
			path: new string[] { "view", "editorWindows", "PlaceholderModule_OpenTreeEditorAView" },
			command: TMP_OpenTreeEditorAView
		);

#pragma warning restore CA1822 // Mark members as static
		#endregion


		#region Constructor

		public PlaceholderDataProviderService(EditorContext ctx) :
			base(ctx)
		{ }

		#endregion
	}
}
