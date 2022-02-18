using Dock.Model.ReactiveUI.Controls;
using GlacierKitCore.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.ViewModels.EditorWindows
{
    [GKViewModel]
    public abstract class EditorWindowViewModel : Tool
    {
        // TODO: This should be set in some sort of "gk module manifest" object or something
        public abstract string EditorName { get; }
    }
}
