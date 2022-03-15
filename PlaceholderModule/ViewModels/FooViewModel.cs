using GlacierKitCore.ViewModels;
using GlacierKitCore.ViewModels.EditorWindows;
using System;
using System.Globalization;

namespace PlaceholderModule.ViewModels
{
    public class FooViewModel : EditorWindowViewModel
    {
        public static new string EditorName => "The Foo View";


        public static string FooData => DateTime.UnixEpoch.AddHours(123.456).TimeOfDay.ToString(@"hh\:mm");

        public FooViewModel()
        {
            Title = EditorName;
        }
    }
}
