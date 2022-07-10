using GlacierKitCore.ViewModels.Debugging;
using GlacierKitTestShared;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.ViewModels.Debugging
{
	public class StyleClassesViewerItemTest
	{
		#region Theory_data

		private static readonly string[] _classNamesTheoryData =
		{
			"foo",
			"bar",
			"baz"
		};


		public class ConstructorParamsInfo
		{
			public Func<StyleClassesViewerItem> Constructor { get; internal init; } = null!;
			public string ClassName { get; internal init; } = null!;
			public bool? IsActive { get; internal init; } = null!;
		}


		public class StyleClassesViewerItemParamConstructorTheoryData : TheoryData<ConstructorParamsInfo>
		{
			public StyleClassesViewerItemParamConstructorTheoryData()
			{
				foreach (string className in _classNamesTheoryData)
				{
					Add
					(
						new ConstructorParamsInfo
						{
							Constructor = () => new(className),
							ClassName = className,
							IsActive = null
						}
					);

					Add
					(
						new ConstructorParamsInfo
						{
							Constructor = () => new(className, true),
							ClassName = className,
							IsActive = true
						}
					);

					Add
					(
						new ConstructorParamsInfo
						{
							Constructor = () => new(className, false),
							ClassName = className,
							IsActive = false
						}
					);
				}
			}
		}
		public static StyleClassesViewerItemParamConstructorTheoryData
			StyleClassesViewerItemParamConstructorTheoryDataValue { get; } = new();

		#endregion


		#region Constructor

		[Theory]
		[MemberData(nameof(StyleClassesViewerItemParamConstructorTheoryDataValue))]
		[Trait("TestingMember", "Constructor")]
		public static void Constructor_doesnt_throw(ConstructorParamsInfo paramsInfo)
		{
			Util.AssertCodeDoesNotThrowException(
				() => _ = paramsInfo.Constructor()
			);
		}

		#endregion


		#region IsActive

		[Theory]
		[MemberData(nameof(StyleClassesViewerItemParamConstructorTheoryDataValue))]
		[Trait("TestingMember", "Property_IsActive")]
		public static void IsActive_is_set_by_constructor(ConstructorParamsInfo paramsInfo)
		{
			// Arrange
			StyleClassesViewerItem instance;
			bool expected, actual;

			// Act
			instance = paramsInfo.Constructor();
			expected = paramsInfo.IsActive ?? false;
			actual = instance.IsActive;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Theory]
		[MemberData(nameof(StyleClassesViewerItemParamConstructorTheoryDataValue))]
		[Trait("TestingMember", "Property_IsActive")]
		public static void Changes_to_IsActive_may_be_reacted_to(ConstructorParamsInfo paramsInfo)
		{
			// Arrange
			StyleClassesViewerItem instance;

			// Act
			instance = paramsInfo.Constructor();

			// Act/Assert
			Util.AssertPropertyIsReactive(
				source: instance,
				propertyAccessExpression: x => x.IsActive,
				propertySetter: (source, value) => source.IsActive = value,
				valuesToChangeTo: GeneralUseData.SetOfBools
			);
		}

		#endregion


		#region ClassName

		[Theory]
		[MemberData(nameof(StyleClassesViewerItemParamConstructorTheoryDataValue))]
		[Trait("TestingMember", "Property_ClassName")]
		public static void ClassName_is_set_by_constructor(ConstructorParamsInfo paramsInfo)
		{
			// Arrange
			StyleClassesViewerItem instance;
			string expected = paramsInfo.ClassName;
			string actual;

			// Act
			instance = paramsInfo.Constructor();
			actual = instance.ClassName;

			// Assert
			Assert.Equal(expected, actual);
		}

		#endregion
	}
}
