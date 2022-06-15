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
		private readonly TreeView? _treeView;


		public ForestEditorView()
		{
			InitializeComponent();

			_treeView = this.FindControl<TreeView>("PART_TreeView");

			this.WhenActivated(disposables =>
			{
				if (_treeView != null)
				{
					this.OneWayBind(
						viewModel: ViewModel,
						vmProperty: vm => vm.Forests,
						viewProperty: v => v._treeView!.Items
					);
				}
			});
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
