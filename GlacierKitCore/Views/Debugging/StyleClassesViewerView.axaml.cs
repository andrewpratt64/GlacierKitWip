using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GlacierKitCore.Views.Debugging
{
	/// <summary>
	/// Debug utility for previewing style classes on a control
	/// </summary>
	public partial class StyleClassesViewerView : UserControl
	{
		public StyleClassesViewerView()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
