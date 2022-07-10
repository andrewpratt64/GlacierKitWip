using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using GlacierKitCore.Utility;
using GlacierKitCore.ViewModels.Debugging;
using GlacierKitTestShared;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using ReactiveUI.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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


		public class ClassNamesChangesTheoryData : TheoryData<IEnumerable<string>, IEnumerable<string>, string>
		{
#pragma warning disable CA1805 // Do not initialize unnecessarily
			private bool _wasAnyItemsAdded = false;
#pragma warning restore CA1805 // Do not initialize unnecessarily

			private void AddWith(IEnumerable<string> classNames, IEnumerable<string> preactivatedClassNames, string classNameToChange)
			{
				_wasAnyItemsAdded = true;
				Add(classNames, preactivatedClassNames, classNameToChange);
			}

			private void AddWith(IEnumerable<string> classNames, IEnumerable<string> preactivatedClassNames)
			{
				foreach (string className in classNames)
					AddWith(classNames, preactivatedClassNames, className);
			}

			private void AddWith(IEnumerable<string> classNames)
			{
				AddWith(classNames, Enumerable.Empty<string>());
				
				List<string> preactivatedClassNames = new() { classNames.First() };
				AddWith(classNames, preactivatedClassNames);

				int classNamesCount = classNames.Count();
				if (classNamesCount > 1)
				{
					preactivatedClassNames.Add(classNames.Last());
					AddWith(classNames, preactivatedClassNames);

					if (classNamesCount > 2)
					{
						preactivatedClassNames.Add(classNames.ElementAt(1));
						AddWith(classNames, preactivatedClassNames);
					}
				}
			}


			public ClassNamesChangesTheoryData()
			{
				foreach (IEnumerable<string> classNames in _classNamesTheoryDataSet)
				{
					if (!classNames.Any())
						continue;

					AddWith(classNames);
				}

				Debug.Assert(_wasAnyItemsAdded, "No theory data was added");
			}
		}
		public static ClassNamesChangesTheoryData ClassNamesChangesTheoryDataValue { get; } = new();


		public class BrushesTheoryData : TheoryData<IBrush?, IBrush?>
		{
			public BrushesTheoryData()
			{
				foreach (IBrush normalBrush in GeneralUseData.SetOfIBrushes)
				{
					foreach (IBrush invertedBrush in GeneralUseData.SetOfIBrushes.Where(brush => brush != normalBrush))
					{
						Add(normalBrush,	null);
						Add(null,			invertedBrush);
						Add(normalBrush,	invertedBrush);
					}
				}
			}
		}
		public static BrushesTheoryData BrushesTheoryDataValue { get; } = new();

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

			// Act/Assert
			Util.AssertPropertyIsReactive(
				source: instance,
				propertyAccessExpression: x => x.PreviewControl,
				propertySetter: (source, value) => source.PreviewControl = value,
				valuesToChangeTo: valuesToChangeTo
			);
		}

		#endregion


		#region IsPreviewControlBackgroundInverted

		[Theory]
		[MemberData(nameof(StyleClassesViewerViewModelConstructorTheoryDataValue))]
		[Trait("TestingMember", "Property_IsPreviewControlBackgroundInverted")]
		public static void Changes_to_IsPreviewControlBackgroundInverted_may_be_reacted_to(IEnumerable<string> classNames)
		{
			// Arrange
			StyleClassesViewerViewModel instance;

			// Act
			instance = new(classNames);

			// Act/Assert
			Util.AssertPropertyIsReactive(
				source: instance,
				propertyAccessExpression: x => x.IsPreviewControlBackgroundInverted,
				propertySetter: (source, value) => source.IsPreviewControlBackgroundInverted = value,
				valuesToChangeTo: GeneralUseData.SetOfBools
			);
		}

		#endregion


		#region PreviewControlNormalBackground

		[Theory]
		[MemberData(nameof(StyleClassesViewerViewModelConstructorTheoryDataValue))]
		[Trait("TestingMember", "Property_PreviewControlNormalBackground")]
		public static void Changes_to_PreviewControlNormalBackground_may_be_reacted_to(IEnumerable<string> classNames)
		{
			// Arrange
			StyleClassesViewerViewModel instance;

			// Act
			instance = new(classNames);

			// Act/Assert
			Util.AssertPropertyIsReactive(
				source: instance,
				propertyAccessExpression: x => x.PreviewControlNormalBackground,
				propertySetter: (source, value) => source.PreviewControlNormalBackground = value,
				valuesToChangeTo: GeneralUseData.SetOfIBrushes
			);
		}

		#endregion


		#region PreviewControlInvertedBackground

		[Theory]
		[MemberData(nameof(StyleClassesViewerViewModelConstructorTheoryDataValue))]
		[Trait("TestingMember", "Property_PreviewControlInvertedBackground")]
		public static void Changes_to_PreviewControlInvertedBackground_may_be_reacted_to(IEnumerable<string> classNames)
		{
			// Arrange
			StyleClassesViewerViewModel instance;

			// Act
			instance = new(classNames);

			// Act/Assert
			Util.AssertPropertyIsReactive(
				source: instance,
				propertyAccessExpression: x => x.PreviewControlInvertedBackground,
				propertySetter: (source, value) => source.PreviewControlInvertedBackground = value,
				valuesToChangeTo: GeneralUseData.SetOfIBrushes
			);
		}

		#endregion


		#region ClassesStringifiedValue

		[Fact]
		[Trait("TestingMember", "OAPH_ClassesStringifiedValue")]
		public static void ClassesStringifiedValue_is_empty_string_when_no_class_items_are_present()
		{
			// Arrange
			StyleClassesViewerViewModel instance;
			IEnumerable<string> classNames = Array.Empty<string>();
			string expected = string.Empty;
			string? actual;

			// Act
			instance = new(classNames);

			actual = instance.ClassesStringifiedValue;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Theory]
		[MemberData(nameof(StyleClassesViewerViewModelConstructorTheoryDataValue))]
		[Trait("TestingMember", "OAPH_ClassesStringifiedValue")]
		public static void ClassesStringifiedValue_is_empty_string_when_all_class_items_are_inactive(IEnumerable<string> classNames)
		{
			// Arrange
			StyleClassesViewerViewModel instance;
			string expected = string.Empty;
			string? actual;

			// Act
			instance = new(classNames);

			if (!instance.ClassItems.Any())
				return;

			actual = instance.ClassesStringifiedValue;

			// Assert
			Assert.False(
				condition: instance.ClassItems.Any(classItem => classItem.IsActive),
				userMessage: "instance.ClassItems contains items with active classes; this unit test requires all items start inactive"
			);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[MemberData(nameof(StyleClassesViewerViewModelConstructorTheoryDataValue))]
		[Trait("TestingMember", "OAPH_ClassesStringifiedValue")]
		public static void ClassesStringifiedValue_is_non_empty_string_when_one_or_more_class_items_are_active(IEnumerable<string> classNames)
		{
			// Arrange
			StyleClassesViewerViewModel instance;
			string? actual;

			// Act
			instance = new(classNames);

			if (instance.ClassItems.FirstOrDefault() is StyleClassesViewerItem classItem)
				classItem.IsActive = true;
			else
			{
				Assert.False(
					condition: instance.ClassItems.Any(),
					userMessage: $"Failed to get the first class item from non-empty collection via {nameof(instance.ClassItems)} with the extension method {nameof(Enumerable.FirstOrDefault)}"
				);
				return;
			}

			actual = instance.ClassesStringifiedValue;

			// Assert
			Assert.True(
				condition: instance.ClassItems.Any(classItem => classItem.IsActive),
				userMessage: "instance.ClassItems contains no items with active classes; this unit test requires at least one class item is active"
			);
			Assert.NotEmpty(actual);
		}

		[Theory]
		[MemberData(nameof(ClassNamesChangesTheoryDataValue))]
		[Trait("TestingMember", "OAPH_ClassesStringifiedValue")]
		public static void ClassesStringifiedValue_has_expected_value_after_a_class_is_activated_or_deactivated(
			IEnumerable<string> classNames,
			IEnumerable<string> preactivatedClassNames,
			string classNameToChange
		)
		{
			// Arrange
			StyleClassesViewerViewModel instance;
			string? actual;
			IEnumerable<string>? expectedParts, actualParts;

			// Act
			instance = new(classNames);
			foreach (string preactivatedClassName in preactivatedClassNames)
				instance.ClassItems
					.First(classItem => classItem.ClassName == preactivatedClassName)
					.IsActive = true;

			if (preactivatedClassNames.Contains(classNameToChange))
			{
				expectedParts = preactivatedClassNames.Except(new string[] { classNameToChange });
				instance.ClassItems.First(classItem => classItem.ClassName == classNameToChange).IsActive = false;
			}
			else
			{
				expectedParts = preactivatedClassNames.Concat(new string[] { classNameToChange });
				instance.ClassItems.First(classItem => classItem.ClassName == classNameToChange).IsActive = true;
			}

			actual = instance.ClassesStringifiedValue;
			if (actual?.Any() ?? false)
				actualParts = actual!.Split(' ');
			else
				actualParts = Enumerable.Empty<string>();

			// Assert
			Assert.NotNull(actual);
			Assert.NotNull(expectedParts);
			Assert.NotNull(actualParts);
			Util.AssertCollectionsHaveSameItems(expectedParts!, actualParts!);
		}

		#endregion


		#region PreviewControlCurrentBackground

		[Theory]
		[MemberData(nameof(BrushesTheoryDataValue))]
		[Trait("TestingMember", "OAPH_PreviewControlCurrentBackground")]
		public static void PreviewControlCurrentBackground_equals_PreviewControlNormalBackground_when_IsPreviewControlBackgroundInverted_is_false(IBrush? normalBrush, IBrush? invertedBrush)
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				StyleClassesViewerViewModel instance;
				IEnumerable<string> classNames = Enumerable.Empty<string>();
				bool valueOfIsPreviewControlBackgroundInverted = false;
				IBrush? expected, actual;


				// Act
				instance = new(classNames)
				{
					PreviewControlNormalBackground = normalBrush,
					PreviewControlInvertedBackground = invertedBrush
				};
				instance.IsPreviewControlBackgroundInverted = valueOfIsPreviewControlBackgroundInverted;

				scheduler.AdvanceBy(2);

				expected = instance.PreviewControlNormalBackground;
				actual = instance.PreviewControlCurrentBackground;

				// Assert
				Assert.True(
					condition: instance.IsPreviewControlBackgroundInverted == valueOfIsPreviewControlBackgroundInverted,
					userMessage: $"Can't perform unit test; instance.IsPreviewControlBackgroundInverted is supposed to be {valueOfIsPreviewControlBackgroundInverted} but instead is {instance.IsPreviewControlBackgroundInverted.ToString() ?? "null"}"
				);
				Assert.Equal(expected, actual);
			});
		}

		[Theory]
		[MemberData(nameof(BrushesTheoryDataValue))]
		[Trait("TestingMember", "OAPH_PreviewControlCurrentBackground")]
		public static void PreviewControlCurrentBackground_equals_PreviewControlInvertedBackground_when_IsPreviewControlBackgroundInverted_is_true(IBrush? normalBrush, IBrush? invertedBrush)
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				StyleClassesViewerViewModel instance;
				IEnumerable<string> classNames = Enumerable.Empty<string>();
				bool valueOfIsPreviewControlBackgroundInverted = true;
				IBrush? expected, actual;


				// Act
				instance = new(classNames)
				{
					PreviewControlNormalBackground = normalBrush,
					PreviewControlInvertedBackground = invertedBrush
				};
				instance.IsPreviewControlBackgroundInverted = valueOfIsPreviewControlBackgroundInverted;

				scheduler.AdvanceBy(2);

				expected = instance.PreviewControlInvertedBackground;
				actual = instance.PreviewControlCurrentBackground;

				// Assert
				Assert.True(
					condition: instance.IsPreviewControlBackgroundInverted == valueOfIsPreviewControlBackgroundInverted,
					userMessage: $"Can't perform unit test; instance.IsPreviewControlBackgroundInverted is supposed to be {valueOfIsPreviewControlBackgroundInverted} but instead is {instance.IsPreviewControlBackgroundInverted.ToString() ?? "null"}"
				);
				Assert.Equal(expected, actual);
			});
		}

		#endregion
	}
}
