using Avalonia;
using Avalonia.Controls;
using GlacierKitCore.ViewModels.Debugging;
using GlacierKitTestShared;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.ViewModels.Debugging
{
	public class StyleClassesViewerViewModelTest
	{
		#region Theory_data

		private static readonly string[][] _classNamesTheoryDataSet =
		{
			Array.Empty<string>(),
			new string[]
			{
				"foo"
			},
			new string[]
			{
				":foo"
			},
			new string[]
			{
				"foo",
				"bar",
				"baz"
			},
			new string[]
			{
				":foo",
				":bar",
				":baz"
			},
			new string[]
			{
				":foo",
				"bar",
				"baz"
			},
		};


		public class StyleClassesViewerViewModelConstructorTheoryData : TheoryData<IEnumerable<string>>
		{
			public StyleClassesViewerViewModelConstructorTheoryData()
			{
				foreach (string[] classNames in _classNamesTheoryDataSet)
					Add(classNames);
			}
		}
		public static StyleClassesViewerViewModelConstructorTheoryData
			StyleClassesViewerViewModelConstructorTheoryDataValue { get; } = new();

		#endregion


		#region Constructor

		[Theory]
		[MemberData(nameof(StyleClassesViewerViewModelConstructorTheoryDataValue))]
		[Trait("TestingMember", "Constructor")]
		public static void Constructor_doesnt_throw(IEnumerable<string> classNames)
		{
			Util.AssertCodeDoesNotThrowException(
				() => _ = new StyleClassesViewerViewModel(classNames)
			);
		}

		#endregion


		#region ClassItems

		[Theory]
		[MemberData(nameof(StyleClassesViewerViewModelConstructorTheoryDataValue))]
		[Trait("TestingMember", "Property_ClassItems")]
		public static void ClassItems_has_expected_classNames(IEnumerable<string> classNames)
		{
			// Arrange
			StyleClassesViewerViewModel instance;
			IEnumerable<string> expected = classNames;
			IEnumerable<string> actual;

			// Act
			instance = new(classNames);
			actual =
				from classItem in instance.ClassItems
				select classItem.ClassName;

			// Assert
			Util.AssertCollectionsHaveSameItems(expected, actual);
		}

		[Theory]
		[MemberData(nameof(StyleClassesViewerViewModelConstructorTheoryDataValue))]
		[Trait("TestingMember", "Property_ClassItems")]
		public static void All_items_in_ClassItems_start_inactive(IEnumerable<string> classNames)
		{
			// Arrange
			StyleClassesViewerViewModel instance;
			IEnumerable<bool> actual;

			// Act
			instance = new(classNames);
			actual =
				from classItem in instance.ClassItems
				select classItem.IsActive;

			// Assert
			Assert.False(actual.Any(isActive => isActive));
		}

		#endregion


		#region PreviewControl

		[Theory]
		[MemberData(nameof(StyleClassesViewerViewModelConstructorTheoryDataValue))]
		[Trait("TestingMember", "Property_PreviewControl")]
		public static void Changes_to_PreviewControl_may_be_reacted_to(IEnumerable<string> classNames)
		{
			// Arrange
			StyleClassesViewerViewModel instance;
			IStyledElement? reactor = null;
			IDisposable disposable;

			TextBlock textBlockA = new()
			{ Name = "TextBlockA" };

			TextBlock textBlockB = new()
			{ Name = "TextBlockB" };

			Button button = new()
			{ Name = "Button" };

			List<IStyledElement?> valuesToChangeTo = new()
			{
				textBlockA,
				textBlockB,
				button,
				null,
				button,
				textBlockA,
				null,
				button
			};


			// Act
			instance = new(classNames);
			disposable = instance
				.WhenAnyValue(x => x.PreviewControl)
				.Subscribe(x => reactor = x);

			// Act/Assert
			foreach (IStyledElement? valueToChangeTo in valuesToChangeTo)
			{
				instance.PreviewControl = valueToChangeTo;
				Assert.Equal(valueToChangeTo, reactor);
			}

			// Cleanup
			disposable.Dispose();
		}

		#endregion


		#region IsPreviewControlBackgroundInverted

		

		#endregion
	}
}
