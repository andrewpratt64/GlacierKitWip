using System;
using Xunit;
using GlacierKitCore.Services;
using GlacierKitTestShared;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using GlacierKitCore.ViewModels.EditorWindows;

namespace GlacierKitCoreTest.Tests.Services
{
    public class GKModuleLoaderServiceTest
    {
		#region Misc

		[Fact]
		[Trait("TestingMember", "Misc")]
		public static void Assembly_path_has_expected_values()
        {
            // Arrange
            string? assemblyDir;
            string projectName = "GlacierKitCoreTest";
#if DEBUG
            string buildConfig = "Debug";
#else
            string buildConfig = "Release";
#endif
            string? environmentVersion = null;
            string? assemblyDirExpectedSuffix = null;
            string? modulesDir = null;

            // Act
            assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (assemblyDir != null)
            {
                environmentVersion = $"net{Environment.Version.Major}.{Environment.Version.Minor}";
                assemblyDirExpectedSuffix = Path.Combine(projectName, "bin", buildConfig, environmentVersion);
                modulesDir = Path.Combine(assemblyDir, "gkmodules");
            }

            // Assert
            Assert.NotNull(environmentVersion);
            Assert.NotNull(assemblyDirExpectedSuffix);
            Assert.False(assemblyDir == null || modulesDir == null, "Failed to determine the gkmodules filepath");
            Assert.EndsWith(assemblyDirExpectedSuffix, assemblyDir);
        }

		#endregion


		#region Constructor

		[Fact]
		[Trait("TestingMember", "Constructor")]
		public static void Default_ctor_works()
        {
            Util.AssertDefaultCtorWorks<GKModuleLoaderService>();
        }

		#endregion


		#region State

		[Fact]
		[Trait("TestingMember", "Property_State")]
		public static void Initial_state_is_unloaded()
        {
            // Arrange
            GKModuleLoaderService? service;
            GKModuleLoaderService.ELoaderState expected = GKModuleLoaderService.ELoaderState.NotLoaded;

            // Act
            service = new GKModuleLoaderService();

            // Assert
            Assert.Equal(expected, service.State);
        }

		[Fact]
		[Trait("TestingMember", "Property_State")]
		public static void State_is_loaded_after_running_LoadModules()
		{
			// Arrange
			GKModuleLoaderService? service;
			GKModuleLoaderService.ELoaderState expected = GKModuleLoaderService.ELoaderState.Loaded;

			// Act

			service = new GKModuleLoaderService();
			service.LoadModules();

			// Assert
			Assert.Equal(expected, service.State);
		}

		#endregion


		#region ModuleAssemblies

		[Fact]
		[Trait("TestingMember", "Property_ModuleAssemblies")]
		public static void ModuleAssemblies_starts_empty()
		{
			// Arrange
			GKModuleLoaderService? service;

			// Act
			service = new GKModuleLoaderService();

			// Assert
			Assert.Empty(service.ModuleAssemblies);
		}

		[Fact]
		[Trait("TestingMember", "Property_ModuleAssemblies")]
		public static void ModuleAssemblies_has_correct_size_after_running_LoadModules()
		{
			// Arrange
			GKModuleLoaderService? service;
			int expected = MiscData.ExpectedModules;

			// Act
			service = new GKModuleLoaderService();
			service.LoadModules();

			// Assert
			Assert.Equal(expected, service.ModuleAssemblies.Count);
		}

		#endregion


		#region EditorWindowViewModels

		[Fact]
		[Trait("TestingMember", "Property_EditorWindowViewModels")]
		public static void EditorWindowViewModels_starts_empty()
		{
			// Arrange
			GKModuleLoaderService? service;

			// Act
			service = new GKModuleLoaderService();

			// Assert
			Assert.Empty(service.EditorWindowViewModels);
		}

		[Fact]
		[Trait("TestingMember", "Property_EditorWindowViewModels")]
		public static void EditorWindowViewModels_not_empty_after_running_LoadModules()
		{
			// Arrange
			GKModuleLoaderService? service;

			// Act
			service = new GKModuleLoaderService();
			service.LoadModules();

			// Assert
			Assert.NotEmpty(service.EditorWindowViewModels);
		}

		[Fact]
		[Trait("TestingMember", "Property_EditorWindowViewModels")]
		public static void EditorWindowViewModels_contains_only_valid_types_after_running_LoadModules()
		{
			// Arrange
			GKModuleLoaderService? service;
#pragma warning disable IDE0039 // Use local function
			Action<Type?> AssertTypeIsValid = (type) =>
			{
				Assert.True(EditorWindowViewModel.IsTypeAnInstantiableEditorWindow(type));
			};
#pragma warning restore IDE0039 // Use local function

			// Act
			service = new GKModuleLoaderService();
			service.LoadModules();

			// Assert
			Assert.All(service.EditorWindowViewModels, AssertTypeIsValid);
		}

		#endregion


		#region GKCommands

		[Fact]
		[Trait("TestingMember", "Property_GKCommands")]
		public static void GKCommands_starts_empty()
		{
			// Arrange
			GKModuleLoaderService? service;

			// Act
			service = new GKModuleLoaderService();

			// Assert
			Assert.Empty(service.GKCommands);
		}

		[Fact]
		[Trait("TestingMember", "Property_GKCommands")]
		public static void GKCommands_not_empty_after_running_LoadModules()
		{
			// Arrange
			GKModuleLoaderService? service;

			// Act
			service = new GKModuleLoaderService();
			service.LoadModules();

			// Assert
			Assert.NotEmpty(service.GKCommands);
		}

		#endregion


		#region MainMenuBarItemsSetupInfo

		[Fact]
		[Trait("TestingMember", "Property_MainMenuBarItemsSetupInfo")]
		public static void MainMenuBarItemsSetupInfo_starts_empty()
		{
			// Arrange
			GKModuleLoaderService? service;

			// Act
			service = new GKModuleLoaderService();

			// Assert
			Assert.Empty(service.MainMenuBarItemsSetupInfo);
		}

		[Fact]
		[Trait("TestingMember", "Property_MainMenuBarItemsSetupInfo")]
		public static void MainMenuBarItemsSetupInfo_not_empty_after_running_LoadModules()
		{
			// Arrange
			GKModuleLoaderService? service;

			// Act
			service = new GKModuleLoaderService();
			service.LoadModules();

			// Assert
			Assert.NotEmpty(service.MainMenuBarItemsSetupInfo);
		}

		#endregion


		#region GKModulesDirectory

		[Fact]
		[Trait("TestingMember", "Property_GKModulesDirectory")]
		public static void GKModulesDirectory_has_expected_prefix()
		{
			// Arrange
			GKModuleLoaderService? service;
			string expected = "gkmodules";
			string? actualDir;

			// Act
			service = new GKModuleLoaderService();
			actualDir = service.GKModulesDirectory;

			// Assert
			Assert.NotNull(actualDir);
			Assert.EndsWith(expected, actualDir);
			Assert.True(Directory.Exists(actualDir), "GKModulesDirectory has a non-existing path: " + actualDir);
		}

		#endregion
    }
}
