using GlacierKitCore.ViewModels.EditorWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.ViewModels
{
    public class EditorWindowViewModelTest
    {
#pragma warning disable IDE1006 // Naming Styles
        private class _TYPE_UNRELATED_CONCRETE_CLASS
        {
        }
        private abstract class _TYPE_UNRELATED_ABSTRACT_CLASS
        {
        }
        private class _TYPE_RELATED_CONCRETE_CLASS : EditorWindowViewModel
        {
            public static new string EditorName => "Related Concrete Class";
        }
        private abstract class _TYPE_RELATED_ABSTRACT_CLASS : EditorWindowViewModel
        {
        }
#pragma warning restore IDE1006 // Naming Styles


        // This could potentially be rewritten as a [Theory], but it might not matter
        private static void TestIsTypeAnInstantiableEditorWindow(Type? input, bool expectsTrue)
        {
            // Arrange
            bool actualValue;

            // Act
            actualValue = EditorWindowViewModel.IsTypeAnInstantiableEditorWindow(input);

            // Assert

            // Yes, this could technically just test if actualValue and expectsTrue
            // are equal but I like the output format Assert.True/False has.
            if (expectsTrue)
                Assert.True(actualValue);
            else
                Assert.False(actualValue);
        }


        [Fact]
        public static void IsTypeAnInstantiableEditorWindow_with_null_returns_false()
        {
            TestIsTypeAnInstantiableEditorWindow(null, false);
        }

        [Fact]
        public static void IsTypeAnInstantiableEditorWindow_with_unrelated_concrete_type_returns_false()
        {
            TestIsTypeAnInstantiableEditorWindow(typeof(_TYPE_UNRELATED_CONCRETE_CLASS), false);
        }

        [Fact]
        public static void IsTypeAnInstantiableEditorWindow_with_unrelated_abstract_type_returns_false()
        {
            TestIsTypeAnInstantiableEditorWindow(typeof(_TYPE_UNRELATED_ABSTRACT_CLASS), false);
        }

        [Fact]
        public static void IsTypeAnInstantiableEditorWindow_with_related_concrete_type_returns_true()
        {
            TestIsTypeAnInstantiableEditorWindow(typeof(_TYPE_RELATED_CONCRETE_CLASS), true);
        }

        [Fact]
        public static void IsTypeAnInstantiableEditorWindow_with_related_abstract_type_returns_false()
        {
            TestIsTypeAnInstantiableEditorWindow(typeof(_TYPE_RELATED_ABSTRACT_CLASS), false);
        }
    }
}
