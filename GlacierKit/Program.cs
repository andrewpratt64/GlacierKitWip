using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using System;

namespace GlacierKit
{
    internal class Program
    {
		// Initialization code. Don't use any Avalonia, third-party APIs or any
		// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
		// yet and stuff might break.
		[STAThread]
		public static void Main(string[] args)
		{
			BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
		}

		// Avalonia configuration, don't remove; also used by visual designer.
		public static AppBuilder BuildAvaloniaApp()
		{
			return AppBuilder.Configure<App>()
				.UsePlatformDetect()
				// This part is needed to prevent a potential huge memory leak
				// See: https://github.com/AvaloniaUI/Avalonia/issues/6977
				.With(new Win32PlatformOptions
				{
					AllowEglInitialization = false
				})
				.LogToTrace()
				.UseReactiveUI();
		}
	}
}
