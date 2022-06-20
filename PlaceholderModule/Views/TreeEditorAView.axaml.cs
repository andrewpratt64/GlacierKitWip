using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using GlacierKitCore.Views.EditorWindows;
using PlaceholderModule.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;

namespace PlaceholderModule.Views
{
	public partial class TreeEditorAView : EditorWindowView<TreeEditorAViewModel>
	{
		public TreeEditorAView()
		{
			FinishSetup();
			AvaloniaXamlLoader.Load(this);
		}

		public override void HandleActivation(CompositeDisposable disposables)
		{}

		public override void HandleDeactivation()
		{}
	}
}
