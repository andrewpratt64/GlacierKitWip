using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static GlacierKitCore.Utility.CollectionUtils;

namespace GlacierKitCoreTest.Tests.Utility
{
	public class CollectionUtilsTest
	{
		private static ICollection? CreateCollectionWithTypeSizeOf(ECollectionSizeType type)
		{
			switch (type)
			{
				case ECollectionSizeType.Null:
					return null;
				case ECollectionSizeType.Empty:
					return new List<string>();
				case ECollectionSizeType.SingleItem:
					return new List<string>
					{
						"First"
					};
				case ECollectionSizeType.SeveralItems:
					return new List<string>
					{
						"First",
						"Second",
						"Third"
					};
				default:
					Debug.Fail("If this code is ran, then CreateCollectionWithTypeSizeOf dosen't cover all possible types!");
					return null;
			}
		}


		[Fact]
		public static void ECollectionSizeType_and_ECollectionSizeFlags_have_same_underlying_type()
		{
			// Arrange
			Type UnderlyingTypeOfECollectionSizeType;
			Type UnderlyingTypeOfECollectionSizeFlags;

			// Act
			UnderlyingTypeOfECollectionSizeType = typeof(ECollectionSizeType).GetEnumUnderlyingType();
			UnderlyingTypeOfECollectionSizeFlags = typeof(ECollectionSizeType).GetEnumUnderlyingType();

			// Assert
			Assert.Equal(UnderlyingTypeOfECollectionSizeType, UnderlyingTypeOfECollectionSizeFlags);
		}

		[Theory]
		[InlineData(ECollectionSizeType.Null, ECollectionSizeFlags.Null)]
		[InlineData(ECollectionSizeType.Empty, ECollectionSizeFlags.Empty)]
		[InlineData(ECollectionSizeType.SingleItem, ECollectionSizeFlags.SingleItem)]
		[InlineData(ECollectionSizeType.SeveralItems, ECollectionSizeFlags.SeveralItems)]
		public static void ECollectionSizeType_matches_ECollectionSizeFlags_values(
			ECollectionSizeType sizeType,
			ECollectionSizeFlags sizeFlags
		)
		{
			// Arrange
			ulong sizeTypeValue;
			ulong sizeFlagsValue;

			// Act
			sizeTypeValue = (ulong)sizeType;
			sizeFlagsValue = (ulong)sizeFlags;

			Assert.Equal(sizeTypeValue, sizeFlagsValue);
		}

		[Theory]
		[InlineData(	ECollectionSizeType.Null,			ECollectionSizeFlags.Null			)]
		[InlineData(	ECollectionSizeType.Empty,			ECollectionSizeFlags.Empty			)]
		[InlineData(	ECollectionSizeType.SingleItem,		ECollectionSizeFlags.SingleItem		)]
		[InlineData(	ECollectionSizeType.SeveralItems,	ECollectionSizeFlags.SeveralItems	)]
		public static void Calling_SizeFlagsOfType_returns_expected_value(
			ECollectionSizeType sizeType,
			ECollectionSizeFlags expectedReturnValue
		)
		{
			// Arrange
			ECollectionSizeFlags actualReturnValue;

			// Act
			actualReturnValue = SizeFlagsOfType(sizeType);

			// Assert
			Assert.Equal(expectedReturnValue, actualReturnValue);
		}

		[Theory]
		[InlineData(ECollectionSizeType.Null,
			ECollectionSizeFlags.Null,
			ECollectionSizeFlags.NullOrEmpty
		)]
		[InlineData(ECollectionSizeType.Empty,
			ECollectionSizeFlags.Empty,
			ECollectionSizeFlags.NullOrEmpty,
			ECollectionSizeFlags.NotNull
		)]
		[InlineData(ECollectionSizeType.SingleItem,
			ECollectionSizeFlags.SingleItem,
			ECollectionSizeFlags.AnyItems,
			ECollectionSizeFlags.NotNull
		)]
		[InlineData(ECollectionSizeType.SeveralItems,
			ECollectionSizeFlags.SeveralItems,
			ECollectionSizeFlags.AnyItems,
			ECollectionSizeFlags.NotNull
		)]
		public static void Calling_GetCollectionSizeFlagsOf_returns_expected_value(
			ECollectionSizeType sizeType,
			params ECollectionSizeFlags[] expectedReturnValues
		)
		{
			// Arrange
			ICollection? collection;
			ECollectionSizeFlags actualReturnValue;

			// Act
			collection = CreateCollectionWithTypeSizeOf(sizeType);
			actualReturnValue = GetCollectionSizeFlagsOf(collection);

			// Assert
			foreach (var expectedReturnValue in expectedReturnValues)
				Assert.NotEqual<ECollectionSizeFlags>(0, actualReturnValue & expectedReturnValue);
		}
	}
}
