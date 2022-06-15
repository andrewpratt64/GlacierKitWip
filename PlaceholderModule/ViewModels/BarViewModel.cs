using GlacierKitCore.Models;
using GlacierKitCore.ViewModels;
using GlacierKitCore.ViewModels.EditorWindows;
using System;
using System.Globalization;

namespace PlaceholderModule.ViewModels
{
	public abstract class BarViewModel : EditorWindowViewModel
	{
		public BarViewModel(EditorContext ctx) : base(ctx)
		{}
	}
}
