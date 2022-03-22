using GlacierKitCore.Commands;
using GlacierKitCore.Models;
using GlacierKitCore.Services;
using GlacierKitTestShared;
using PlaceholderModule.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.Models
{
    public class EditorContextTest
    {
        private static readonly Type? _DATA_ValidInputForCreateEditorWindow = typeof(FooViewModel);
        private static readonly Type? _DATA_InvalidInputForCreateEditorWindow = typeof(BarViewModel);
        private static readonly string _DATA_ValidIdForGetCommand = "PlaceholderModule_PrintHi";
        private static readonly string _DATA_InvalidIdForGetCommand = "IDontExist";



        [Fact]
        public static void Default_ctor_works()
        {
            Util.AssertDefaultCtorWorks<EditorContext>();
        }

        [Fact]
        public static void ModuleLoader_not_null()
        {
            // Arrange
            EditorContext? ctx;
            GKModuleLoaderService? actualValue;

            // Act
            ctx = new();
            actualValue = ctx.ModuleLoader;

            // Assert
            Assert.NotNull(actualValue);
        }

        [Fact]
        public static void CreateEditorWindow_not_null()
        {
            // Arrange
            EditorContext? ctx;
            object? actualValue;

            // Act
            ctx = new();
            actualValue = ctx.CreateEditorWindow;

            // Assert
            Assert.NotNull(actualValue);
        }

        [Fact]
        public static void CreateEditorWindow_with_invalid_type_does_not_throw()
        {
            // Arrange
            EditorContext? ctx;
            Type? input = _DATA_InvalidInputForCreateEditorWindow;

            // Act
            ctx = new();

            // Assert
            Util.AssertCodeDoesNotThrowException(
                () =>
                ctx.CreateEditorWindow
                .Execute(input)
                .Subscribe()
                .Dispose()
            );
        }

        [Fact]
        public static void CreateEditorWindow_with_invalid_type_returns_null()
        {
            // Arrange
            EditorContext? ctx;
            Type? input = _DATA_InvalidInputForCreateEditorWindow;
            Type? expectedValue = null;
            Type? actualValue;
            IObservable<Type?> commandObservable;
            IDisposable commandDisposable;

            // Act
            ctx = new();
            commandObservable = ctx.CreateEditorWindow.Execute(input).ObserveOn(RxApp.MainThreadScheduler);
            commandDisposable = commandObservable.Subscribe();
            actualValue = commandObservable.Wait();


            // Assert
            Assert.Equal(expectedValue, actualValue);
            commandDisposable.Dispose();
        }

        [Fact]
        public static void CreateEditorWindow_with_null_type_does_not_throw()
        {
            // Arrange
            EditorContext? ctx;
            Type? input = null;

            // Act
            ctx = new();

            // Assert
            Util.AssertCodeDoesNotThrowException(
                () =>
                ctx.CreateEditorWindow
                .Execute(input)
                .Subscribe()
                .Dispose()
            );
        }

        [Fact]
        public static void CreateEditorWindow_with_null_type_returns_null()
        {
            // Arrange
            EditorContext? ctx;
            Type? input = null;
            Type? expectedValue = null;
            Type? actualValue;
            IObservable<Type?> commandObservable;
            IDisposable commandDisposable;

            // Act
            ctx = new();
            commandObservable = ctx.CreateEditorWindow.Execute(input).ObserveOn(RxApp.MainThreadScheduler);
            commandDisposable = commandObservable.Subscribe();
            actualValue = commandObservable.Wait();


            // Assert
            Assert.Equal(expectedValue, actualValue);
            commandDisposable.Dispose();
        }

        [Fact]
        public static void CreateEditorWindow_with_valid_type_does_not_throw()
        {
            // Arrange
            EditorContext? ctx;
            Type? input = _DATA_ValidInputForCreateEditorWindow;

            // Act
            ctx = new();

            // Assert
            Util.AssertCodeDoesNotThrowException(
                () =>
                ctx.CreateEditorWindow
                .Execute(input)
                .Subscribe()
                .Dispose()
            );
        }

        [Fact]
        public static void CreateEditorWindow_with_valid_type_returns_same_type_as_input()
        {
            // Arrange
            EditorContext? ctx;
            Type? input = _DATA_ValidInputForCreateEditorWindow;
            Type? expectedValue = input;
            Type? actualValue;
            IObservable<Type?> commandObservable;
            IDisposable commandDisposable;

            // Act
            ctx = new();
            commandObservable = ctx.CreateEditorWindow.Execute(input).ObserveOn(RxApp.MainThreadScheduler);
            commandDisposable = commandObservable.Subscribe();
            actualValue = commandObservable.Wait();


            // Assert
            Assert.Equal(expectedValue, actualValue);
            commandDisposable.Dispose();
        }

        [Fact]
        public static void GetCommand_with_valid_id_before_loading_returns_null()
        {
            // Arrange
            EditorContext? ctx;
            string commandId = _DATA_ValidIdForGetCommand;
            GKCommand<Unit, Unit>? returnValue;

            // Act
            ctx = new();
            returnValue = ctx.GetCommand<Unit, Unit>(commandId);

            // Assert
            Assert.Null(returnValue);
        }

        [Fact]
        public static void GetCommand_with_invalid_id_before_loading_returns_null()
        {
            // Arrange
            EditorContext? ctx;
            string commandId = _DATA_InvalidIdForGetCommand;
            GKCommand<Unit, Unit>? returnValue;

            // Act
            ctx = new();
            returnValue = ctx.GetCommand<Unit, Unit>(commandId);

            // Assert
            Assert.Null(returnValue);
        }

        [Fact]
        public static void GetCommand_with_valid_id_after_loading_dosent_return_null()
        {
            // Arrange
            EditorContext? ctx;
            string commandId = _DATA_ValidIdForGetCommand;
            GKCommand<Unit, Unit>? returnValue;

            // Act
            ctx = new();
            ctx.ModuleLoader.LoadModules();
            returnValue = ctx.GetCommand<Unit, Unit>(commandId);

            // Assert
            Assert.NotNull(returnValue);
        }

        [Fact]
        public static void GetCommand_with_invalid_id_after_loading_returns_null()
        {
            // Arrange
            EditorContext? ctx;
            string commandId = _DATA_InvalidIdForGetCommand;
            GKCommand<Unit, Unit>? returnValue;

            // Act
            ctx = new();
            ctx.ModuleLoader.LoadModules();
            returnValue = ctx.GetCommand<Unit, Unit>(commandId);

            // Assert
            Assert.Null(returnValue);
        }

        [Fact]
        public static void Executing_command_with_assumed_value_from_GetCommand_does_not_throw()
        {
            // Arrange
            EditorContext? ctx;
            string commandId = _DATA_ValidIdForGetCommand;
            GKCommand<Unit, Unit>? returnValue;

            // Act
            ctx = new();
            ctx.ModuleLoader.LoadModules();
            returnValue = ctx.GetCommand<Unit, Unit>(commandId);

            // Assert
            Util.AssertCodeDoesNotThrowException(() =>
            {
                returnValue!.Command.Execute().Subscribe();
            });
        }
    }
}
