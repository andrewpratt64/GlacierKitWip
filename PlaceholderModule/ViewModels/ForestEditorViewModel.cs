using DynamicData;
using GlacierKitCore.Models;
using GlacierKitCore.Services;
using GlacierKitCore.ViewModels;
using GlacierKitCore.ViewModels.EditorWindows;
using PlaceholderModule.Models;
using PlaceholderModule.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace PlaceholderModule.ViewModels
{
	public class ForestEditorViewModel :
		EditorWindowViewModel,
		IActivatableViewModel
	{
		private ReadOnlyObservableCollection<ForestModel>? _forests;
		private ReadOnlyObservableCollection<TreeModel>? _trees;


		public static new string DisplayName => "ForestModel Editor";

		public ReadOnlyObservableCollection<ForestModel>? Forests => _forests;


		[Reactive]
		public IContextualItem? SelectedItem { get; set; }


		[Reactive]
		public int? TotalForestCount { get; private set; }
		[Reactive]
		public int? TotalTreeCount { get; private set; }


		[ObservableAsProperty]
		public ReactiveCommand<Unit, Unit>? ClickAddForestButton { get; }
		[ObservableAsProperty]
		public ReactiveCommand<Unit, Unit>? ClickDestroyForestButton { get; }
		[ObservableAsProperty]
		public ReactiveCommand<Unit, Unit>? ClickAddTreeButton { get; }
		[ObservableAsProperty]
		public ReactiveCommand<Unit, Unit>? ClickDestroyTreeButton { get; }

		public ForestEditorViewModel() :
			this(new())
		{ }

		public ForestEditorViewModel(EditorContext ctx) :
			base(ctx)
		{
			Title = DisplayName;

			// Observable for when modules are loaded
			IObservable<GKModuleLoaderService.ELoaderState> whenModulesLoadedObservable = this
				.WhenAnyValue(x => x.Ctx.ModuleLoader.State)
				.Where(state => state == GKModuleLoaderService.ELoaderState.Loaded);

			// Bind ClickAddForestButton when modules are loaded
			whenModulesLoadedObservable
				.Select(_ => Ctx.GetCommand<Unit, Unit>("PlaceholderModule_CreateNewForest")?.Command)
				.ToPropertyEx(this, x => x.ClickAddForestButton);
			// Bind ClickDestroyForestButton when modules are loaded
			whenModulesLoadedObservable
				.Select(_ => Ctx.GetCommand<Unit, Unit>("PlaceholderModule_DestroyForest")?.Command)
				.ToPropertyEx(this, x => x.ClickDestroyForestButton);
			// Bind ClickAddForestButton when modules are loaded
			whenModulesLoadedObservable
				.Select(_ => Ctx.GetCommand<Unit, Unit>("PlaceholderModule_CreateNewTreeInForest")?.Command)
				.ToPropertyEx(this, x => x.ClickAddTreeButton);
			// Bind ClickDestroyForestButton when modules are loaded
			whenModulesLoadedObservable
				.Select(_ => Ctx.GetCommand<Unit, Unit>("PlaceholderModule_DestroyTreeInForestForest")?.Command)
				.ToPropertyEx(this, x => x.ClickDestroyTreeButton);


			FinishSetup();
		}


		public override void HandleActivation(CompositeDisposable disposables)
		{
			IConnectableObservable<IChangeSet<IContextualItem>> ctxItems = Ctx
				.ConnectToItems()
				.Publish();

			// Bind _forests
			ctxItems
				.Filter(item => item is ForestModel)
				.Transform(item => (ForestModel)item)
				.Bind(out _forests)
				.Subscribe()
				.DisposeWith(disposables);

			// Bind _trees
			ctxItems
				.Filter(item => item is TreeModel)
				.Transform(item => (TreeModel)item)
				.Bind(out _trees)
				.Subscribe()
				.DisposeWith(disposables);

			// Setup TotalForestCount
			ctxItems
				.CountChanged()
				.StartWithEmpty()
				.Subscribe(_ => TotalForestCount = _forests.Count)
				.DisposeWith(disposables);

			// Setup TotalTreeCount
			ctxItems
				.CountChanged()
				.StartWithEmpty()
				.Subscribe(_ => TotalTreeCount = _trees.Count)
				.DisposeWith(disposables);

			ctxItems.Connect().DisposeWith(disposables);

			// Observable for changes to the current context's FocusedItem
			IObservable<IContextualItem?> whenFocusedItemChangesObservable = this
				.WhenAnyValue(x => x.Ctx.FocusedItem)
				.Where(item => item != SelectedItem);

			// SelectedItem reacts to focused forests and trees
			whenFocusedItemChangesObservable
				.Where(item => item is ForestModel || item is TreeModel)
				.Subscribe(item => SelectedItem = item)
				.DisposeWith(disposables);

			// SelectedItem becomes null when unrelated items are focused
			whenFocusedItemChangesObservable
				.Where(item => !(item is ForestModel || item is TreeModel))
				.Subscribe(_ => SelectedItem = null)
				.DisposeWith(disposables);

			// Focused item is affected by SelectedItem
			this.WhenAnyValue(x => x.SelectedItem)
				.Where(item => item != Ctx.FocusedItem)
				.Subscribe(item => Ctx.FocusedItem = item)
				.DisposeWith(disposables);
		}


		public override void HandleDeactivation()
		{}
	}
}
