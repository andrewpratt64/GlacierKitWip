using GlacierKitCore.Attributes.DataProviders;
using GlacierKitCore.Commands;
using GlacierKitCore.Models;
using GlacierKitCore.ViewModels.Common;
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

namespace GlacierKitCore.Services
{
    public class GKModuleLoaderService
    {
		#region Public enums

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

		#endregion

		
		#region Private_fields

		private string? _gkModulesDir;
		private readonly List<Assembly> _moduleAssemblies = new();
		private readonly List<Type> _editorWindowViewModels = new();
		private readonly List<IGKCommand> _gkCommands = new();
		private readonly List<MainMenuItemSetupInfo> _mainMenuItemsSetupInfo = new();

		#endregion


		#region Public_properties

		/// <summary>
		/// Current state of this loader
		/// </summary>
		public ELoaderState State
        { get; private set; } = ELoaderState.NotLoaded;


        /// <summary>
        /// Collection of loaded GK module assemblies
        /// </summary>
        public ICollection<Assembly> ModuleAssemblies => _moduleAssemblies;


        /// <summary>
        /// Collection of view model types for editor windows loaded from GK modules
        /// </summary>
        public ICollection<Type> EditorWindowViewModels => _editorWindowViewModels;


        /// <summary>
        /// Collection of GK commands loaded from GK modules
        /// </summary>
        public ICollection<IGKCommand> GKCommands => _gkCommands;


		/// <summary>
		/// The setup information for the main menu items loaded from GK modules
		/// </summary>
		public ICollection<MainMenuItemSetupInfo> MainMenuBarItemsSetupInfo => _mainMenuItemsSetupInfo;


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
					_gkModulesDir = GetGKModulesDirectoryFor(typeof(GKModuleLoaderService).Assembly);
				}
				// Return the cached filepath
				return _gkModulesDir;
			}
		}

		#endregion


		#region Public_static_methods

		/// <summary>
		/// Calculates the filepath of the "gkmodules" directory
		/// </summary>
		/// <param name="assembly">The assembly containing the gkmodules directory</param>
		/// <returns>The folder's filepath</returns>
		public static string GetGKModulesDirectoryFor(Assembly assembly)
		{
			string? assemblyDir = Path.GetDirectoryName(assembly.Location);
			Debug.Assert(assemblyDir != null);
			return Path.Combine(assemblyDir, "gkmodules");
		}

		#endregion


		#region Public_instance_methods

		/// <summary>
		/// Synchronously loads all dlls from the "gkmodules" directory
		/// </summary>
		public void LoadModules()
		{
			LoadModules(GKModulesDirectory);
		}

		/// <summary>
		/// Synchronously loads all dlls from the "gkmodules" directory
		/// </summary>
		/// <param name="modulesDirectory">Path of the gkmodules directory </param>
		/// <example>LoadModules("C:/MyStuff/GlacierKit/GlacierKit/bin/Debug/net5.0/gkmodules")</example>
		public void LoadModules(string modulesPath)
		{
			Debug.Assert(State == ELoaderState.NotLoaded);

			// Update the loader state to show we're currently loading
			State = ELoaderState.Loading;

			// Iterate over every .dll file in the modules directory
			foreach (string moduleFile in Directory.GetFiles(modulesPath, "*.dll"))
			{
				// Load the assembly for this dll
				Assembly moduleAssembly = Assembly.LoadFile(moduleFile);

				// Remember this module's assembly
				_moduleAssemblies.Add(moduleAssembly);

				// Iterate over all exported types assembly
				foreach (Type type in moduleAssembly.GetExportedTypes())
				{
					// Remember this exported type if it's an instantiable view model for an editor window
					if (EditorWindowViewModel.IsTypeAnInstantiableEditorWindow(type))
						_editorWindowViewModels.Add(type);
					// Load data from this type if it's a data provider
					else if (GKDataProviderAttribute.IsTypeAGKDataProvider(type))
						LoadDataFromProvider(type);
				}
			}

			// Update the loader state to show we've finished loading
			State = ELoaderState.Loaded;
		}

		#endregion


		#region Private_methods

		private void LoadDataFromProvider(Type providerType)
        {
            // Iterate over all of the properties in providerType
            foreach (PropertyInfo property in providerType.GetProperties())
            {
				// Skip any members that aren't static and read-only
				if (!property.IsStatic() || property.CanWrite)
					continue;

				// Iterate over the attributes of the property
				foreach (CustomAttributeData attr in property.CustomAttributes)
				{
					// Remember gk commands
					if (attr.AttributeType == typeof(ExposeAsGKCommandAttribute))
					{
						IGKCommand? command = property.GetValue(null) as IGKCommand;
						Debug.Assert(command != null, $"Only instances of {nameof(IGKCommand)} may be exported as a GK command");
						_gkCommands.Add(command);
					}
					// Remember main menu items
					else if (attr.AttributeType == typeof(ExposeAsMainMenuItemAttribute))
					{
						MainMenuItemSetupInfo? setupInfo = property.GetValue(null) as MainMenuItemSetupInfo;
						Debug.Assert(setupInfo != null, $"Only instances of {nameof(MainMenuItemSetupInfo)} may be exported as a main menu item");
						_mainMenuItemsSetupInfo.Add(setupInfo);
					}
				}
            }
        }

		#endregion
	}
}
