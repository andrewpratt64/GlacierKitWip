using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GlacierKitCore.Misc;
using GlacierKitCore.ViewModels.EditorWindows;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Views.EditorWindows
{
	/// <summary>
	/// Base class for an individual editor window's view
	/// </summary>
	/// <typeparam name="TViewModel">The view model for the view</typeparam>
	public abstract class EditorWindowView<TViewModel> :
		ReactiveUserControl<TViewModel>,
		IActivationHandler
		where TViewModel : EditorWindowViewModel
	{
		protected EditorWindowView()
		{ }

		public abstract void HandleActivation(CompositeDisposable disposables);
		public abstract void HandleDeactivation();


		/// <summary>
		/// Performs the final steps for initializing the window, intended to be called at the end of a constructor right before calling <see cref="AvaloniaXamlLoader.Load(object)"/>
		/// </summary>
		protected void FinishSetup()
		{
			this.WhenActivated(disposables =>
			{
				HandleActivation(disposables);
				Disposable.Create(() => HandleDeactivation()).DisposeWith(disposables);
			});
		}
	}
}
