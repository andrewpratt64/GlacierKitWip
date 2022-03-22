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
        [Fact]
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

        [Fact]
        public static void Default_ctor_works()
        {
            Util.AssertDefaultCtorWorks<GKModuleLoaderService>();
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public static void ModuleAssemblies_has_correct_size_after_running_LoadModules()
        {
            // Arrange
            GKModuleLoaderService? service;
            var expected = MiscData.ExpectedModules;

            // Act
            service = new GKModuleLoaderService();
            service.LoadModules();

            // Assert
            Assert.Equal(expected, service.ModuleAssemblies.Count);
        }

        [Fact]
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

        [Fact]
        public static void EditorWindowViewModels_contains_only_valid_types_after_running_LoadModules()
        {
            // Arrange
            GKModuleLoaderService? service;
            Action<Type?> AssertTypeIsValid = (type) =>
            {
                Assert.True(EditorWindowViewModel.IsTypeAnInstantiableEditorWindow(type));
            };

            // Act
            service = new GKModuleLoaderService();
            service.LoadModules();

            // Assert
            Assert.All(service.EditorWindowViewModels, AssertTypeIsValid);
        }
    }
}
