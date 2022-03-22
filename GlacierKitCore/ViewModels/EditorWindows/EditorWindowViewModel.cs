using Dock.Model.ReactiveUI.Controls;
using GlacierKitCore.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.ViewModels.EditorWindows
{
    /// <summary>
    /// Base class for an individual editor window
    /// </summary>
    [GKViewModel]
    public abstract class EditorWindowViewModel : Tool
    {
        /// <summary>
        /// Print-friendly name to display in the editor
        /// </summary>
        public static string DisplayName => "Editor Window";


        // TODO: Shorter name?
        /// <summary>
        /// Determines whether or not a given type is a concrete subclass of EditorWindowViewModel
        /// </summary>
        /// <param name="type">Type to test</param>
        /// <returns>True if <paramref name="type"/> is a concrete subclass, false otherwise</returns>
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
