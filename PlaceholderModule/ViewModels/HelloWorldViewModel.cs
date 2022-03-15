using GlacierKitCore.ViewModels;
using GlacierKitCore.ViewModels.EditorWindows;
using System;


namespace PlaceholderModule.ViewModels
{
    public class HelloWorldViewModel : EditorWindowViewModel
    {
        public static new string EditorName => "Welcomer of Worlds";

        public HelloWorldViewModel()
        {
            Title = EditorName;
        }
    }
}
