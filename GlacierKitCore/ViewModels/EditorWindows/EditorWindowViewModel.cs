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
        public abstract string EditorName { get; }


        // TODO: Shorter name?
        public static bool IsTypeAnInstantiableEditorWindow(Type? type)
        {
            // A type may be instantiated as an editor window if it is:
            //  - Not null
            //  - A subclass of EditorWindowViewModel
            //  - Not abstract
            return (type?.IsSubclassOf(typeof(EditorWindowViewModel)) ?? false) && !type!.IsAbstract;
        }
    }
}
