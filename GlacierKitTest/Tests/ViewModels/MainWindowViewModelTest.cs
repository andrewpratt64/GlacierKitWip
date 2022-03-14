using GlacierKitCore.Models;
using GlacierKitCore.Services;
using GlacierKit.ViewModels;
using GlacierKitTestShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Reactive.Linq;
using ReactiveUI;

namespace GlacierKitTest.Tests.ViewModels
{
    public class MainWindowViewModelTest
    {
        [Fact]
        public static void Default_ctor_works()
        {
            Util.AssertDefaultCtorWorks<MainWindowViewModel>();
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Method return value is not tested")]
        public static void Ctor_with_null_context_does_not_throw()
        {
            // Arrange
            EditorContext? ctx = null;

            // Assert
            Util.AssertCodeDoesNotThrowException(() => new MainWindowViewModel(ctx));
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Method return value is not tested")]
        public static void Ctor_with_non_null_context_does_not_throw()
        {
            // Arrange
            EditorContext ctx;

            // Act
            ctx = new();

            // Assert
            Util.AssertCodeDoesNotThrowException(() => new MainWindowViewModel(ctx));
        }

        [Fact]
        public static void Ctor_uses_provided_context()
        {
            // Arrange
            EditorContext ctx;
            MainWindowViewModel vm;

            // Act
            ctx = new();
            vm = new(ctx);

            // Assert
            Assert.Equal(ctx, vm.Ctx);
        }
    }
}
