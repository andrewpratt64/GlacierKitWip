using Avalonia.Markup.Xaml;
using GlacierKitCore.Models;
using GlacierKitCore.ViewModels;
using GlacierKitCore.ViewModels.EditorWindows;
using System;
using System.Globalization;
using System.Reactive.Disposables;

namespace PlaceholderModule.ViewModels
{
	public class FooViewModel : EditorWindowViewModel
	{
		public static new string DisplayName => "The Foo View";


		public static string FooData => DateTime.UnixEpoch.AddHours(123.456).TimeOfDay.ToString(@"hh\:mm", null);

		
		public FooViewModel() :
			this(new())
		{}
		
		public FooViewModel(EditorContext ctx) :
			base(ctx)
		{
			Title = DisplayName;

			FinishSetup();
		}


		public override void HandleActivation(CompositeDisposable disposables)
		{ }

		public override void HandleDeactivation()
		{ }
	}
}
