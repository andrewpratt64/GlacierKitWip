using Dock.Model.Core;
using GlacierKit;
using GlacierKit.ViewModels;
using GlacierKitCore.Models;
using GlacierKitCore.ViewModels;
using GlacierKitTestShared;
using PlaceholderModule.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xunit;

namespace GlacierKitTest.Tests.ViewModels
{
    public class MainViewModelTest
    {
        
        private static readonly Type? _DATA_ValidValueForNewDocumentType = typeof(FooViewModel);
        private static readonly Type? _DATA_AbstractValueForNewDocumentType = typeof(BarViewModel);


        #region Constructor

        [Fact]
        public static void Default_ctor_works()
        {
            Util.AssertDefaultCtorWorks<MainViewModel>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Method return value is not tested")]
        [Fact]
        public static void Ctor_with_null_dockable_does_not_throw()
        {
            // Arrange
			EditorContext ctx = new();
            IFactory factory = new MainDockFactory(new());
            IDockable? dockable = null;

            // Assert
            Util.AssertCodeDoesNotThrowException(() => new MainViewModel(ctx, factory, dockable));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Method return value is not tested")]
        [Fact]
        public static void Ctor_with_not_null_dockable_does_not_throw()
        {
			// Arrange
			EditorContext ctx = new();
			IFactory factory = new MainDockFactory(new());
            IDockable dockable = factory.CreateLayout()!;

            // Assert
            Util.AssertCodeDoesNotThrowException(() => new MainViewModel(ctx, factory, dockable));
        }

        #endregion


        #region NewDocumentType

        [Fact]
        public static void NewDocumentType_is_initially_null()
        {
            // Arrange
            MainViewModel vm;
            Type? newDocumentType;

            // Act
            vm = new();
            newDocumentType = vm.NewDocumentType;

            // Assert
            Assert.Null(newDocumentType);
        }

        [Fact]
        public static void NewDocumentType_get_does_not_throw()
        {
            // Arrange
            MainViewModel vm;

            // Act
            vm = new();

            // Assert
            Util.AssertCodeDoesNotThrowException(() => _ = vm.NewDocumentType);
        }

        [Fact]
        public static void NewDocumentType_does_not_throw_when_attempting_to_set_to_valid_value()
        {
            // Arrange
            MainViewModel vm;
            Type? newDocumentType = _DATA_ValidValueForNewDocumentType;

            // Act
            vm = new();

            // Assert
            Util.AssertCodeDoesNotThrowException(() => vm.NewDocumentType = newDocumentType);
        }

        [Fact]
        public static void NewDocumentType_has_expected_value_after_set_to_valid_value()
        {
            // Arrange
            MainViewModel vm;
            Type? newDocumentType = _DATA_ValidValueForNewDocumentType;
            Type? expectedValueAfterSet = newDocumentType;
            Type? actualValueAfterSet;

            // Act
            vm = new();
            vm.NewDocumentType = newDocumentType;
            actualValueAfterSet = vm.NewDocumentType;

            // Assert
            Assert.Equal(expectedValueAfterSet, actualValueAfterSet);
        }

        [Fact]
        public static void NewDocumentType_does_not_throw_when_attempting_to_set_to_abstract_value()
        {
            // Arrange
            MainViewModel vm;
            Type? newDocumentType = _DATA_AbstractValueForNewDocumentType;

            // Act
            vm = new();

            // Assert
            Util.AssertCodeDoesNotThrowException(() => vm.NewDocumentType = newDocumentType);
        }

        [Fact]
        public static void NewDocumentType_does_not_throw_when_attempting_to_set_to_null_value()
        {
            // Arrange
            MainViewModel vm;
            Type? newDocumentType = null;

            // Act
            vm = new();

            // Assert
            Util.AssertCodeDoesNotThrowException(() => vm.NewDocumentType = newDocumentType);
        }

        #endregion


        #region IsNewDocumentTypeValid

        [Fact]
        public static void IsNewDocumentTypeValid_is_initially_false()
        {
            // Arrange
            MainViewModel vm;
            bool actualValue;

            // Act
            vm = new();
            actualValue = vm.IsNewDocumentTypeValid;

            // Assert
            Assert.False(actualValue);
        }

        [Fact]
        public static void IsNewDocumentTypeValid_is_false_when_NewDocumentType_is_abstract()
        {
            // Arrange
            MainViewModel vm;
            Type? initialDocumentType = _DATA_ValidValueForNewDocumentType;
            Type? newDocumentType = _DATA_AbstractValueForNewDocumentType;
            bool actualValue;

            // Act
            vm = new();
            vm.NewDocumentType = initialDocumentType;
            vm.NewDocumentType = newDocumentType;
            actualValue = vm.IsNewDocumentTypeValid;

            // Assert
            Assert.False(actualValue);
        }

        [Fact]
        public static void IsNewDocumentTypeValid_is_false_when_NewDocumentType_is_null()
        {
            // Arrange
            MainViewModel vm;
            Type? initialDocumentType = _DATA_ValidValueForNewDocumentType;
            Type? newDocumentType = null;
            bool actualValue;

            // Act
            vm = new();
            vm.NewDocumentType = initialDocumentType;
            vm.NewDocumentType = newDocumentType;
            actualValue = vm.IsNewDocumentTypeValid;

            // Assert
            Assert.False(actualValue);
        }

        [Fact]
        public static void IsNewDocumentTypeValid_is_true_when_NewDocumentType_is_valid()
        {
            // Arrange
            MainViewModel vm;
            Type? newDocumentType = _DATA_ValidValueForNewDocumentType;
            bool actualValue;

            // Act
            vm = new();
            vm.NewDocumentType = newDocumentType;
            actualValue = vm.IsNewDocumentTypeValid;

            // Assert
            Assert.True(actualValue);
        }

        #endregion


        #region CreateDocument

        [Fact]
        public static void CreateDocument_is_not_null()
        {
            // Arrange
            MainViewModel vm;
            ICommand? actualCommand;

            // Act
            vm = new();
            actualCommand = vm.CreateDocument;

            Assert.NotNull(actualCommand);
        }

        #endregion


        #region CreateDocument

        [Fact]
        public static void VisibleDockables_initial_value_is_not_null()
        {
            // Arrange
            MainViewModel vm;
            EditorContext initialCtx;
            IFactory initialFactory;
            IDockable initialDockable;
            IList<IDockable>? actualValue;

            // Act
            initialCtx = new();
            initialFactory = new MainDockFactory(initialCtx);
            initialDockable = initialFactory.CreateLayout()!;
            initialFactory.InitLayout(initialDockable);

            vm = new(initialCtx, initialFactory, initialDockable);
            actualValue = vm.VisibleDockables;

            // Assert
            Assert.NotNull(actualValue);
            
        }

        [Fact]
        public static void CreateDocument_with_default_NewDocumentType_does_not_throw()
        {
            // Arrange
            MainViewModel vm;

            // Act
            vm = new();

            Util.AssertCodeDoesNotThrowException(() => vm.CreateDocument!.Execute(null));
        }

        [Fact]
        public static void VisibleDockables_count_unchanged_after_CreateDocument_with_default_NewDocumentType()
        {
            // Arrange
            MainViewModel vm;
            int? countBefore;
            int? countAfter;

            // Act
            vm = new();
            countBefore = vm.VisibleDockables?.Count;
            vm.CreateDocument!.Execute(null);
            countAfter = vm.VisibleDockables?.Count;

            Assert.Equal(countBefore, countAfter);
        }

        [Fact]
        public static void CreateDocument_with_null_NewDocumentType_does_not_throw()
        {
            // Arrange
            MainViewModel vm;
            Type? initialDocumentType = _DATA_ValidValueForNewDocumentType;
            Type? newDocumentType = null;

            // Act
            vm = new();
            vm.NewDocumentType = initialDocumentType;
            vm.NewDocumentType = newDocumentType;

            Util.AssertCodeDoesNotThrowException(() => vm.CreateDocument!.Execute(null));
        }

        [Fact]
        public static void VisibleDockables_count_unchanged_after_CreateDocument_with_null_NewDocumentType()
        {
            // Arrange
            MainViewModel vm;
            Type? initialDocumentType = _DATA_ValidValueForNewDocumentType;
            Type? newDocumentType = null;
            int? countBefore;
            int? countAfter;

            // Act
            vm = new();
            countBefore = vm.VisibleDockables?.Count;
            vm.NewDocumentType = initialDocumentType;
            vm.NewDocumentType = newDocumentType;
            vm.CreateDocument!.Execute(null);
            countAfter = vm.VisibleDockables?.Count;

            Assert.Equal(countBefore, countAfter);
        }

        [Fact]
        public static void CreateDocument_with_abstract_NewDocumentType_does_not_throw()
        {
            // Arrange
            MainViewModel vm;
            Type? initialDocumentType = _DATA_ValidValueForNewDocumentType;
            Type? newDocumentType = _DATA_AbstractValueForNewDocumentType;

            // Act
            vm = new();
            vm.NewDocumentType = initialDocumentType;
            vm.NewDocumentType = newDocumentType;

            Util.AssertCodeDoesNotThrowException(() => vm.CreateDocument!.Execute(null));
        }

        [Fact]
        public static void VisibleDockables_count_unchanged_after_CreateDocument_with_abstract_NewDocumentType()
        {
            // Arrange
            MainViewModel vm;
            Type? initialDocumentType = _DATA_ValidValueForNewDocumentType;
            Type? newDocumentType = _DATA_AbstractValueForNewDocumentType;
            int? countBefore;
            int? countAfter;

            // Act
            vm = new();
            countBefore = vm.VisibleDockables?.Count;
            vm.NewDocumentType = initialDocumentType;
            vm.NewDocumentType = newDocumentType;
            vm.CreateDocument!.Execute(null);
            countAfter = vm.VisibleDockables?.Count;

            Assert.Equal(countBefore, countAfter);
        }

        [Fact]
        public static void CreateDocument_with_valid_NewDocumentType_does_not_throw()
        {
            // Arrange
            MainViewModel vm;
            Type? newDocumentType = _DATA_ValidValueForNewDocumentType;

            // Act
            vm = new();
            vm.NewDocumentType = newDocumentType;

            // Assert
            Util.AssertCodeDoesNotThrowException(() => vm.CreateDocument!.Execute(null));
        }

        [Fact]
        public static void VisibleDockables_count_increased_by_one_after_CreateDocument_with_valid_NewDocumentType()
        {
            // Arrange
            MainViewModel vm;
            EditorContext initialCtx;
            IFactory initialFactory;
            IDockable initialDockable;
            Type? newDocumentType = _DATA_ValidValueForNewDocumentType;
            int? countBefore;
            int? countAfter;
            int expectedDifference = 1;

            // Act
            initialCtx = new();
            initialFactory = new MainDockFactory(initialCtx);
            initialDockable = initialFactory.CreateLayout()!;
            initialFactory.InitLayout(initialDockable);

            vm = new(initialCtx, initialFactory, initialDockable);
            countBefore = vm.VisibleDockables?.Count;
            vm.NewDocumentType = newDocumentType;
            vm.CreateDocument!.Execute(null);
            countAfter = vm.VisibleDockables?.Count;

            // Assert
            Assert.Equal(expectedDifference, countAfter - countBefore);
        }

        #endregion
    
    }
}
