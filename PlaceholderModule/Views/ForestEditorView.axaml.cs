using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PlaceholderModule.ViewModels;
using ReactiveUI;

namespace PlaceholderModule.Views
{
	public partial class ForestEditorView : ReactiveUserControl<ForestEditorViewModel>
	{
		private TreeView? TreeView => this.FindControl<TreeView>("PART_TreeView");


		public ForestEditorView()
		{
			this.WhenActivated(disposables =>
			{
				if (TreeView != null)
				{
					this.OneWayBind(
						viewModel: ViewModel,
						vmProperty: vm => vm.Forests,
						viewProperty: v => v.TreeView!.Items
					);
				}
			});

			AvaloniaXamlLoader.Load(this);
		}
	}
}
