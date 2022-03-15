using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PlaceholderModule.Views
{
    public partial class HelloWorldView : UserControl
    {
        public HelloWorldView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
