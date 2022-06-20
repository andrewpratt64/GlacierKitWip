using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GlacierKitCore.Views.EditorWindows;
using PlaceholderModule.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace PlaceholderModule.Views
{
	public partial class ForestEditorView : EditorWindowView<ForestEditorViewModel>
	{
		private TreeView? TreeView => this.FindControl<TreeView>("PART_TreeView");


		public ForestEditorView()
		{
			FinishSetup();
			AvaloniaXamlLoader.Load(this);
		}

		public override void HandleActivation(CompositeDisposable disposables)
		{
			if (TreeView != null)
			{
				this.OneWayBind(
					viewModel: ViewModel,
					vmProperty: vm => vm.Forests,
					viewProperty: v => v.TreeView!.Items
				);
			}
		}

		public override void HandleDeactivation()
		{}
	}
}
