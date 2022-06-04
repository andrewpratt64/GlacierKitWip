using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PlaceholderModule.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlaceholderModule.Views
{
	public partial class TreeEditorAView : ReactiveUserControl<TreeEditorAViewModel>
	{
		public TreeEditorAView()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
