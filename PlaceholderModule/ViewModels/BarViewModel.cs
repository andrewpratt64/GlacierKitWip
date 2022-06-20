using GlacierKitCore.Models;
using GlacierKitCore.ViewModels;
using GlacierKitCore.ViewModels.EditorWindows;
using System;
using System.Globalization;

namespace PlaceholderModule.ViewModels
{
	public abstract class BarViewModel : EditorWindowViewModel
	{
		protected BarViewModel(EditorContext ctx) : base(ctx)
		{}
	}
}
