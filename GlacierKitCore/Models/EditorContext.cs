using Dock.Model.Controls;
using GlacierKitCore.Services;
using GlacierKitCore.ViewModels.EditorWindows;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Models
{
    public class EditorContext : ReactiveObject
    {
        public GKModuleLoaderService ModuleLoader
        { get; }


        /// <summary>
        /// Creates a new editor window of a given type
        /// </summary>
        public ReactiveCommand<Type?, Type?> CreateEditorWindow;



        public EditorContext()
        {
            ModuleLoader = new();


            CreateEditorWindow = ReactiveCommand.Create((Type? windowType) =>
            {
                // Return null for any type that isn't a concrete subclass of EditorWindowViewModel
                if ((windowType?.IsSubclassOf(typeof(EditorWindowViewModel)) ?? false) && !windowType!.IsAbstract )
                    return windowType;
                return null;
            });
        }
    }
}
