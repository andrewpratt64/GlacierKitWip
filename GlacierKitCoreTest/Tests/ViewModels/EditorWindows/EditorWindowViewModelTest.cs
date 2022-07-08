using GlacierKitCore.ViewModels.EditorWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.ViewModels
{
	public class EditorWindowViewModelTest
	{
		private class UNRELATED_CONCRETE_CLASS
		{
		}
		private abstract class UNRELATED_ABSTRACT_CLASS
		{
		}
		private class RELATED_CONCRETE_CLASS : EditorWindowViewModel
		{
			public static new string DisplayName => "Related Concrete Class";

			public RELATED_CONCRETE_CLASS() :
				base(new GlacierKitCore.Models.EditorContext())
			{
				FinishSetup();
			}

			public override void HandleActivation(CompositeDisposable disposables)
			{}

			public override void HandleDeactivation()
			{}
		}
		private abstract class RELATED_ABSTRACT_CLASS : EditorWindowViewModel
		{
			public RELATED_ABSTRACT_CLASS() :
				base(new GlacierKitCore.Models.EditorContext())
			{ }
		}


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
			TestIsTypeAnInstantiableEditorWindow(typeof(UNRELATED_CONCRETE_CLASS), false);
		}

		[Fact]
		public static void IsTypeAnInstantiableEditorWindow_with_unrelated_abstract_type_returns_false()
		{
			TestIsTypeAnInstantiableEditorWindow(typeof(UNRELATED_ABSTRACT_CLASS), false);
		}

		[Fact]
		public static void IsTypeAnInstantiableEditorWindow_with_related_concrete_type_returns_true()
		{
			TestIsTypeAnInstantiableEditorWindow(typeof(RELATED_CONCRETE_CLASS), true);
		}

		[Fact]
		public static void IsTypeAnInstantiableEditorWindow_with_related_abstract_type_returns_false()
		{
			TestIsTypeAnInstantiableEditorWindow(typeof(RELATED_ABSTRACT_CLASS), false);
		}
	}
}
