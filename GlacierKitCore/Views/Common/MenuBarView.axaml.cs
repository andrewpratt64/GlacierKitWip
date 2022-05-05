using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GlacierKitCore.Views.Common
{
	public partial class MenuBarView : UserControl
	{
		public MenuBarView()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
