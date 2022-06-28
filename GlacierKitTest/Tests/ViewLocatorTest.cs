using Avalonia.Controls;
using GlacierKit;
using GlacierKit.ViewModels;
using GlacierKit.Views;
using GlacierKitCore.ViewModels;
using GlacierKitTestShared;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitTest.Tests
{
    public class ViewLocatorTest
    {
#pragma warning disable IDE1006 // Naming Styles
		private class _DATA_BadViewModel : ReactiveObject
		{
            public static string Words = "I\'m not a valid Glacier Kit ViewModel";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        private static object _DATA_BadData => new _DATA_BadViewModel();
        private static object _DATA_GoodData => new MainWindowViewModel();

        private static bool IsExpectedReturnValueFromBuild(object returnedData, bool wasGoodInput)
        {
            MainWindowView? returnedView = returnedData as MainWindowView;
            if (wasGoodInput)
                return returnedView != null;
            return returnedView == null;
        }
#pragma warning restore IDE1006 // Naming Styles


		[Fact]
        public static void Default_ctor_works()
        {
            Util.AssertDefaultCtorWorks<GlacierKit.ViewLocator>();
        }

        
        [Fact]
        public static void Build_with_bad_data_returns_expected_value()
        {
            // Arrange
            GlacierKit.ViewLocator viewLocator = new();
            object data = _DATA_BadData;
            object actual;

            // Act
            actual = viewLocator.Build(data);

            // Assert
            Assert.True(IsExpectedReturnValueFromBuild(actual, false));
        }


        // TODO: How to test this? MainWindowViewModel can't be fully setup on it's own
        /*[Fact]
        public static void Build_with_good_data_returns_expected_value()
        {
            // Arrange
            GlacierKit.ViewLocator viewLocator = new();
            object data = _DATA_GoodData;
            object actual;

            // Act
            actual = viewLocator.Build(data);

            // Assert
            Assert.True(IsExpectedReturnValueFromBuild(actual, true));
        }*/


        [Fact]
        public static void Match_with_bad_data_returns_false()
        {
            // Arrange
            GlacierKit.ViewLocator viewLocator = new();
            object data = _DATA_BadData;
            bool returnVal;

            // Act
            returnVal = viewLocator.Match(data);

            // Assert
            Assert.False(returnVal);
        }


        [Fact]
        public static void Match_with_good_data_returns_true()
        {
            // Arrange
            GlacierKit.ViewLocator viewLocator = new();
            object data = _DATA_GoodData;
            bool returnVal;

            // Act
            returnVal = viewLocator.Match(data);

            // Assert
            Assert.True(returnVal);
        }
    }
}
