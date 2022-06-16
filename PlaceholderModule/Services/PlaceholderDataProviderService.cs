using GlacierKitCore.Attributes;
using GlacierKitCore.Attributes.DataProviders;
using GlacierKitCore.Commands;
using GlacierKitCore.Models;
using GlacierKitCore.Services;
using GlacierKitCore.ViewModels.Common;
using PlaceholderModule.Models;
using PlaceholderModule.ViewModels;
using PlaceholderModule.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;

namespace PlaceholderModule.Services
{
    public class PlaceholderDataProviderService : GKDataProviderService
	{
		#region Commands


		#region Generic_commands
		
		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> PrintHi { get; }

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> UselessCommand1 { get; }

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> UselessCommand2 { get; }

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> UselessCommand3 { get; }

		#endregion


		#region Open_editor_window_commands

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> OpenFooView { get; }

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> OpenHelloWorldView { get; }

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> OpenPushyButtonsView { get; }

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> OpenTreeEditorAView { get; }

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> OpenForestEditorView { get; }

		#endregion


		#region Forest_commands

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> CreateNewForest { get; }

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> DestroyForest { get; }

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> CreateNewTreeInForest { get; }

		[ExposeAsGKCommand]
		public GKCommand<Unit, Unit> DestroyTreeInForestForest { get; }

		#endregion


		#endregion


		#region Menu_items

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo PlaceholderMenuItemGroupInfo { get; }

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo StuffToDoMenuItemGroupInfo { get; }

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo PrintHiMenuItemInfo => new
		(
			title: "Print Hi",
			path: new string[] { "placeholder", "stuffToDo", "PlaceholderModule_printHi" },
			command: PrintHi
		);

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo OpenFooViewMenuItemInfo { get; }

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo OpenHelloWorldViewMenuItemInfo { get; }

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo OpenPushyButtonsViewMenuItemInfo { get; }

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo OpenForestEditorViewMenuItemInfo { get; }

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo OpenTreeEditorAViewMenuItemInfo { get; }

		#endregion


		#region Constructor

		public PlaceholderDataProviderService(EditorContext ctx) :
			base(ctx)
		{
			#region Init_commands

			#region Generic_commands

			PrintHi = new
			(
				commandId: "PlaceholderModule_PrintHi",
				displayName: "Print \"Hi\"",
				command: ReactiveCommand.Create<Unit, Unit>(_ =>
				{
					Trace.WriteLine("Hi");
					return Unit.Default;
				})
			);

			UselessCommand1 = new
			(
				commandId: "PlaceholderModule_UselessCommand1",
				displayName: "Useless Command",
				command: ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default)
			);

			UselessCommand2 = new
			(
				commandId: "PlaceholderModule_UselessCommand2",
				displayName: "Yet Another Useless Command",
				command: ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default)
			);

			UselessCommand3 = new
			(
				commandId: "PlaceholderModule_UselessCommand3",
				displayName: "Pretend I do something",
				command: ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default)
			);
			#endregion

			#region Open_editor_window_commands

			OpenFooView = new
			(
				commandId: "PlaceholderModule_OpenFooView",
				displayName: "Open Foo view",
				command: ReactiveCommand.Create<Unit, Unit>(_ =>
				{
					Ctx.CreateEditorWindow.Execute(typeof(FooViewModel)).Subscribe();
					return Unit.Default;
				})
			);

			OpenHelloWorldView = new
			(
				commandId: "PlaceholderModule_OpenHelloWorldView",
				displayName: "Open Hello World view",
				command: ReactiveCommand.Create<Unit, Unit>(_ =>
				{
					Ctx.CreateEditorWindow.Execute(typeof(HelloWorldViewModel)).Subscribe();
					return Unit.Default;
				})
			);

			OpenPushyButtonsView = new
			(
				commandId: "PlaceholderModule_OpenPushyButtonsView",
				displayName: "Open Pushy Buttons view",
				command: ReactiveCommand.Create<Unit, Unit>(_ =>
				{
					Ctx.CreateEditorWindow.Execute(typeof(PushyButtonsViewModel)).Subscribe();
					return Unit.Default;
				})
			);

			OpenForestEditorView = new
			(
				commandId: "PlaceholderModule_OpenForestEditorView",
				displayName: "Open Forest Editor view",
				command: ReactiveCommand.Create<Unit, Unit>(_ =>
				{
					Ctx.CreateEditorWindow.Execute(typeof(ForestEditorViewModel)).Subscribe();
					return Unit.Default;
				})
			);

			OpenTreeEditorAView = new
			(
				commandId: "PlaceholderModule_OpenTreeEditorAView",
				displayName: "Open Tree Editor A view",
				command: ReactiveCommand.Create<Unit, Unit>(_ =>
				{
					Ctx.CreateEditorWindow.Execute(typeof(TreeEditorAViewModel)).Subscribe();
					return Unit.Default;
				})
			);

			#endregion

			#region Forest_commands

			CreateNewForest = new
			(
				commandId: "PlaceholderModule_CreateNewForest",
				displayName: "Create new forest",
				command: ReactiveCommand.Create<Unit, Unit>(_ =>
				{
					ForestModel forest = new(Ctx, "Unnamed");
					Ctx.AddItem.Execute(forest).Subscribe();
					return Unit.Default;
				})
			);

			DestroyForest = new
			(
				commandId: "PlaceholderModule_DestroyForest",
				displayName: "Destroy forest",
				command: ReactiveCommand.Create<Unit, Unit>
				(
					execute: _ =>
					{
						ForestModel forest = ForestModel.GetFocusedForest(Ctx);
						Ctx.RemoveItem.Execute(forest).Subscribe();
						return Unit.Default;
					},
					canExecute: this.WhenAnyValue(x => x.Ctx.FocusedItem)
						.Select(item => ForestModel.IsItemIndirectlyAForest(item))
				)
			);

			CreateNewTreeInForest = new
			(
				commandId: "PlaceholderModule_CreateNewTreeInForest",
				displayName: "Plant tree",
				command: ReactiveCommand.Create<Unit, Unit>
				(
					execute: _ =>
					{
						ForestModel.GetFocusedForest(Ctx).PlantTree
							.Execute().Subscribe();
						return Unit.Default;
					},
					canExecute: this.WhenAnyValue(x => x.Ctx.FocusedItem)
						.Select(item => ForestModel.IsItemIndirectlyAForest(item))
				)
			);

			DestroyTreeInForestForest = new
			(
				commandId: "PlaceholderModule_DestroyTreeInForestForest",
				displayName: "Chop tree",
				command: ReactiveCommand.Create<Unit, Unit>
				(
					execute: _ =>
					{
						TreeModel tree = (TreeModel)Ctx.FocusedItem!;
						tree.ContainingForest.ChopTree
							.Execute(tree).Subscribe();
						
						return Unit.Default;
					},
					canExecute: this.WhenAnyValue(x => x.Ctx.FocusedItem)
						.Select(item => item is TreeModel)
				)
			);

			#endregion

			#endregion


			#region Init_menu_items

			#region Placeholder

			PlaceholderMenuItemGroupInfo = new
			(
				title: "Placeholder",
				path: new string[] { "placeholder" },
				command: null
			);

			StuffToDoMenuItemGroupInfo = new
			(
				title: "Stuff to do",
				path: new string[] { "placeholder", "stuffToDo" },
				command: null
			);

			#endregion

			#region View

			#region View_editorWindows

			OpenFooViewMenuItemInfo = new
			(
				title: "Foo",
				path: new string[] { "view", "editorWindows", "PlaceholderModule_openFooView" },
				command: OpenFooView
			);

			OpenHelloWorldViewMenuItemInfo = new
			(
				title: "Hello World",
				path: new string[] { "view", "editorWindows", "PlaceholderModule_openHelloWorldView" },
				command: OpenHelloWorldView
			);

			OpenPushyButtonsViewMenuItemInfo = new
			(
				title: "Pushy Buttons",
				path: new string[] { "view", "editorWindows", "PlaceholderModule_openPushyButtonsView" },
				command: OpenPushyButtonsView
			);

			OpenForestEditorViewMenuItemInfo = new
			(
				title: "Forest Editor",
				path: new string[] { "view", "editorWindows", "PlaceholderModule_openForestEditorView" },
				command: OpenForestEditorView
			);

			OpenTreeEditorAViewMenuItemInfo = new
			(
				title: "Tree Editor A",
				path: new string[] { "view", "editorWindows", "PlaceholderModule_openTreeEditorAView" },
				command: OpenTreeEditorAView
			);

			#endregion

			#endregion

			#endregion
		}

		#endregion
	}
}
