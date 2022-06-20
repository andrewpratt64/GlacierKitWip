using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GlacierKitCore.Views.EditorWindows;
using PlaceholderModule.ViewModels;
using System.Reactive.Disposables;

namespace PlaceholderModule.Views
{
    public partial class HelloWorldView : EditorWindowView<HelloWorldViewModel>
	{
        public HelloWorldView()
        {
			FinishSetup();
			AvaloniaXamlLoader.Load(this);
		}

		public override void HandleActivation(CompositeDisposable disposables)
		{ }

		public override void HandleDeactivation()
		{ }
	}
}
