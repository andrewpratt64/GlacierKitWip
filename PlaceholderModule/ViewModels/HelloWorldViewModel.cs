using Avalonia.Markup.Xaml;
using GlacierKitCore.Models;
using GlacierKitCore.ViewModels;
using GlacierKitCore.ViewModels.EditorWindows;
using System;
using System.Reactive.Disposables;

namespace PlaceholderModule.ViewModels
{
    public class HelloWorldViewModel : EditorWindowViewModel
    {
        public static new string DisplayName => "Welcomer of Worlds";

        
		public HelloWorldViewModel() :
			this(new())
		{ }
		
		public HelloWorldViewModel(EditorContext ctx) :
			base(ctx)
        {
			Title = DisplayName;

			FinishSetup();
		}


		public override void HandleActivation(CompositeDisposable disposables)
		{}

		public override void HandleDeactivation()
		{}
	}
}
