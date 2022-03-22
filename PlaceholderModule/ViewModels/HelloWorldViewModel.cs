using GlacierKitCore.ViewModels;
using GlacierKitCore.ViewModels.EditorWindows;
using System;


namespace PlaceholderModule.ViewModels
{
    public class HelloWorldViewModel : EditorWindowViewModel
    {
        public static new string DisplayName => "Welcomer of Worlds";

        public HelloWorldViewModel()
        {
            Title = DisplayName;
        }
    }
}
