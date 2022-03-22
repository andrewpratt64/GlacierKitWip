using Dock.Model.Controls;
using GlacierKitCore.Commands;
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


        public GKCommand<TParam, TResult>? GetCommand<TParam, TResult>(string id)
        {
            return (GKCommand<TParam, TResult>?)
                ModuleLoader.GKCommands.FirstOrDefault(cmd =>
                       cmd.GKCommandId == id
                    && cmd.TParamValue == typeof(TParam)
                    && cmd.TResultValue== typeof(TResult)
                );
        }
    }
}
