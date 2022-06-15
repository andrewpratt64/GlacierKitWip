using GlacierKitCore.ViewModels;
using GlacierKitCore.ViewModels.EditorWindows;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Media;
using Avalonia;
using GlacierKitCore.Models;

namespace PlaceholderModule.ViewModels
{
    public class PushyButtonsViewModel : EditorWindowViewModel
    {
        public static new string DisplayName => "Pushy Buttons";


        public PushyButtonsViewModel(EditorContext ctx) :
			base(ctx)
        {
            Title = DisplayName;
        }
    }
}
