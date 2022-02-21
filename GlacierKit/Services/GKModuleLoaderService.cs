using GlacierKitCore.ViewModels.EditorWindows;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKit.Services
{
    public class GKModuleLoaderService
    {
        private string? _gkModulesDir = null;


        /// <summary>
        /// Enumerates possible states of the loader
        /// </summary>
        public enum ELoaderState : byte
        {
            /// <summary>
            /// Loader has not attempted to load modules yet
            /// </summary>
            NotLoaded,
            /// <summary>
            /// Loader is currently attempting to load modules
            /// </summary>
            Loading,
            /// <summary>
            /// Loader has completed loading modules
            /// </summary>
            Loaded,
            /// <summary>
            /// An error occured while loader was attempting to load modules
            /// </summary>
            Failed
        }


        private List<Assembly> _moduleAssemblies = new();
        private List<Type> _editorWindowViewModels = new();


        /// <summary>
        /// Current state of this loader
        /// </summary>
        public ELoaderState State
        { get; private set; } = ELoaderState.NotLoaded;

        /// <summary>
        /// Collection of loaded GK module assemblies
        /// </summary>
        public IEnumerable<Assembly> ModuleAssemblies => _moduleAssemblies;

        /// <summary>
        /// Collection of view model types for editor windows loaded from GK modules
        /// </summary>
        public IEnumerable<Type> EditorWindowViewModels => _editorWindowViewModels;


        /// <summary>
        /// Filepath of the "gkmodules" directory
        /// </summary>
        public string GKModulesDirectory
        {
            get
            {
                // Calculate the filepath if it's unknown
                if (_gkModulesDir == null)
                {
                    string? assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    Debug.Assert(assemblyDir != null);
                    _gkModulesDir = Path.Combine(assemblyDir, "gkmodules");
                }
                // Return the cached filepath
                return _gkModulesDir;
            }
        }



        /// <summary>
        /// Synchronously loads all dlls from the "gkmodules" directory
        /// </summary>
        public void LoadModules()
        {
            Debug.Assert(State == ELoaderState.NotLoaded);

            // Update the loader state to show we're currently loading
            State = ELoaderState.Loading;

            // Iterate over every .dll file in the modules directory
            foreach (string moduleFile in Directory.GetFiles(GKModulesDirectory, "*.dll"))
            {
                // Load the assembly for this dll
                Assembly moduleAssembly = Assembly.LoadFile(moduleFile);

                // Remember this module's assembly
                _moduleAssemblies.Add(moduleAssembly);

                // Iterate over all exported types assembly
                foreach (var type in moduleAssembly.GetExportedTypes())
                {
                    // Remember this exported type if it's a view model for an editor window
                    if (type.IsSubclassOf(typeof(EditorWindowViewModel)))
                        _editorWindowViewModels.Add(type);
                }
            }

            // Update the loader state to show we've finished loading
            State = ELoaderState.Loaded;
        }
    }
}
