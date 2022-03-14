using Dock.Model.Controls;
using Dock.Model.Core;
using GlacierKit;
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

namespace GlacierKitTest.Tests
{
    public class MainDockFactoryTest
    {
        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Method return value is not tested")]
        public static void Ctor_does_not_throw()
        {
            // Arrange
            EditorContext context;

            // Act
            context = new EditorContext();

            // Assert
            Util.AssertCodeDoesNotThrowException(() => new MainDockFactory(context));
        }

        [Fact]
        public static void CreateLayout_does_not_throw()
        {
            // Arrange
            EditorContext context;
            MainDockFactory factory;

            // Act
            context = new EditorContext();
            factory = new MainDockFactory(context);

            // Assert
            Util.AssertCodeDoesNotThrowException(() => factory.CreateLayout());
        }

        [Fact]
        public static void CreateLayout_does_not_return_null()
        {
            // Arrange
            EditorContext context;
            MainDockFactory factory;
            IRootDock? layout;

            // Act
            context = new EditorContext();
            factory = new MainDockFactory(context);
            layout = factory.CreateLayout();

            // Assert
            Assert.NotNull(layout);
        }

        [Fact]
        public static void CreateLayout_returns_with_expected_misc_values()
        {
            // Arrange
            EditorContext context;
            MainDockFactory factory;
            IRootDock layout;

            string expectedId = "Root";
            //string expectedTitle = "Root";

            bool expectNullForActiveDockable = false;
            bool expectNullForDefaultDockable = false;
            bool expectNullForVisibleDockables = false;

            bool expectedCanClose = false;
            bool expectedCanFloat = false;
            bool expectedIsCollapsable = false;
            bool expectedCanCanPin = false;


            // Act
            context = new EditorContext();
            factory = new MainDockFactory(context);
            layout = factory.CreateLayout();

            // Assert
            Assert.Equal(expectedId, layout.Id);
            //Assert.Equal(expectedTitle, layout.Title);

            Util.AssertNullConditional(expectNullForActiveDockable, layout.ActiveDockable);
            Util.AssertNullConditional(expectNullForDefaultDockable, layout.DefaultDockable);
            Util.AssertNullConditional(expectNullForVisibleDockables, layout.VisibleDockables);

            Assert.Equal(expectedCanClose, layout.CanClose);
            Assert.Equal(expectedCanFloat, layout.CanFloat);
            Assert.Equal(expectedIsCollapsable, layout.IsCollapsable);
            Assert.Equal(expectedCanCanPin, layout.CanPin);
        }

        [Fact]
        public static void CreateLayout_returns_with_at_least_one_visible_dockable()
        {
            // Arrange
            EditorContext context;
            MainDockFactory factory;
            IRootDock layout;
            IList<IDockable>? dockables;

            // Act
            context = new EditorContext();
            factory = new MainDockFactory(context);
            layout = factory.CreateLayout();
            dockables = layout.VisibleDockables;

            // Assert
            Assert.NotEmpty(dockables);
        }

        [Fact]
        public static void InitLayout_does_not_throw()
        {
            // Arrange
            EditorContext context;
            MainDockFactory factory;
            IRootDock layout;

            // Act
            context = new EditorContext();
            factory = new MainDockFactory(context);
            layout = factory.CreateLayout();

            // Assert
            Util.AssertCodeDoesNotThrowException(() => factory.InitLayout(layout));
        }
    }
}
