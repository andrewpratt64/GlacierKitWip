using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PlaceholderModule.Views
{
    public partial class FooView : UserControl
    {
        public FooView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
