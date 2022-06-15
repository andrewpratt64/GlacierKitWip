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
using System.Text;

namespace PlaceholderModule.ViewModels
{
	public class ForestEditorViewModel :
		EditorWindowViewModel,
		IActivatableViewModel
	{
		private ReadOnlyObservableCollection<ForestModel>? _forests;


		public static new string DisplayName => "ForestModel Editor";


		public ViewModelActivator Activator { get; }

		public ReadOnlyObservableCollection<ForestModel>? Forests => _forests;


		[Reactive]
		public IContextualItem? SelectedItem { get; set; }


		[ObservableAsProperty]
		public ReactiveCommand<Unit, Unit>? ClickAddForestButton { get; }
		[ObservableAsProperty]
		public ReactiveCommand<Unit, Unit>? ClickDestroyForestButton { get; }

		public ForestEditorViewModel() :
			this(new())
		{ }

		public ForestEditorViewModel(EditorContext ctx) :
			base(ctx)
		{
			Title = DisplayName;

			Activator = new();

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

			
			this.WhenActivated(disposables =>
			{
				HandleActivation(disposables);
				Disposable.Create(() => HandleDeactivation()).DisposeWith(disposables);
			});
		}


		private void HandleActivation(CompositeDisposable disposables)
		{
			// Bind Forests property
			Ctx.ConnectToItems()
				.Filter(item => item is ForestModel)
				.Transform(item => (ForestModel)item)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Bind(out _forests)
				.Subscribe()
				.DisposeWith(disposables);

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


#pragma warning disable CA1822 // Mark members as static
		private void HandleDeactivation()
#pragma warning restore CA1822 // Mark members as static
		{
		}
	}
}
