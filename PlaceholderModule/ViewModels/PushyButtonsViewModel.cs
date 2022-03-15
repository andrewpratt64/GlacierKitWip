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

namespace PlaceholderModule.ViewModels
{
    public class PushyButtonsViewModel : EditorWindowViewModel
    {
        public static new string EditorName => "Pushy Buttons";


        public PushyButtonsViewModel()
        {
            Title = EditorName;
        }
    }
}
