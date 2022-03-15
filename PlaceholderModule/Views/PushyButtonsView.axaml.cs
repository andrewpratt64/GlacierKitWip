using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PlaceholderModule.Views
{
    public partial class PushyButtonsView : UserControl
    {
        public PushyButtonsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
