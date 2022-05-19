using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GlacierKitCore.Models;
using GlacierKitCore.Services;
using GlacierKit.ViewModels;
using GlacierKit.Views;

namespace GlacierKit
{
    public class App : Application
    {
        public EditorContext Ctx
        { get; } = new();


        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
			Ctx.ModuleLoader.LoadModules();

			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindowView
                {
                    DataContext = new MainWindowViewModel(Ctx),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
