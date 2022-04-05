using GlacierKitCore.Utility.Tree;
using GlacierKitTestShared.CommonTestData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitTestShared.Tests
{
	public class CommonTestDataTest
	{
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CA2211 // Non-constant fields should not be visible

		public class InfoForEnumTheoryData : TheoryData<IEnumerable<object[]>, IEnumerable<object>>
		{
			public IEnumerable<object[]> TheoryData
			{ get; }
			public IEnumerable<object> EnumValues
			{ get; }

			public InfoForEnumTheoryData(
				IEnumerable<object[]> theoryData,
				IEnumerable<object> enumValues
			)
			{
				TheoryData = theoryData;
				EnumValues = enumValues;
				Add(theoryData, enumValues);
			}
		}

		public enum _TYPE_BasicEnumForEnumTheoryData
        {
            First,
            Second,
            Third,
            Fourth
        }
		public static InfoForEnumTheoryData _DATA_BasicEnumForEnumTheoryData =
		new(
			theoryData: new EnumTheoryData<_TYPE_BasicEnumForEnumTheoryData>(),

			enumValues: new List<object>
			{
				_TYPE_BasicEnumForEnumTheoryData.First,
				_TYPE_BasicEnumForEnumTheoryData.Second,
				_TYPE_BasicEnumForEnumTheoryData.Third,
				_TYPE_BasicEnumForEnumTheoryData.Fourth
			}
		);

		public enum _TYPE_LongValuedEnumForEnumTheoryData : long
		{
            First = 59L,
            Second = 0L,
            Third = -123L,
            Fourth = 999L
        }
		public static InfoForEnumTheoryData _DATA_LongValuedEnumForEnumTheoryData =
		new(
			theoryData: new EnumTheoryData<_TYPE_LongValuedEnumForEnumTheoryData>(),

			enumValues: new List<object>
			{
				_TYPE_LongValuedEnumForEnumTheoryData.First,
				_TYPE_LongValuedEnumForEnumTheoryData.Second,
				_TYPE_LongValuedEnumForEnumTheoryData.Third,
				_TYPE_LongValuedEnumForEnumTheoryData.Fourth
			}
		);


#pragma warning restore CA2211 // Non-constant fields should not be visible
#pragma warning restore IDE1006 // Naming Styles



		[Fact]
        public static void BoolTheoryData_uses_all_values()
        {
            // Arrange
            BoolTheoryData theoryData;
            IEnumerable<object> expectedValues = new List<object>
            {
                false,
				true
            };
			IEnumerable<object> actualValues;

			// Act
			theoryData = new();
			actualValues = (
				from row in theoryData
				from item in row
				select item
			) ?? Enumerable.Empty<object>();


			// Assert
			Util.AssertCollectionsHaveSameItems(
				expectedValues,
				actualValues
			);
		}

		// NOTE: There's an issue with this method and MethodData
        [Theory]
		[InlineData(nameof(_DATA_BasicEnumForEnumTheoryData))]
		[InlineData(nameof(_DATA_LongValuedEnumForEnumTheoryData))]
        public static void EnumTheoryData_uses_all_values(
			string propertyName
			/*IEnumerable<object[]> theoryData,
			IEnumerable<object> expectedValues*/
		)
		{
			// Arrange
			InfoForEnumTheoryData? info;
			IEnumerable<object[]> theoryData;
			IEnumerable<object> expectedValues;
            IEnumerable<object> actualValues;

			// Act
			info = null;
			if (typeof(CommonTestDataTest).GetField(propertyName) is FieldInfo propertyInfo)
			{
				if (propertyInfo.GetValue(null) is InfoForEnumTheoryData infoVal)
					info = infoVal;
				else
					Debug.Fail($"Value of property \"{propertyName}\" did not return an object of type, \"InfoForEnumTheoryData\"");
			}
			else
				Debug.Fail($"Failed to get property named, \"{propertyName}\"");

			theoryData = info.TheoryData;
			expectedValues = info.EnumValues;

			actualValues = (
				from row in theoryData
				from item in row
				select item
			) ?? Enumerable.Empty<object>();


			// Assert
			Util.AssertCollectionsHaveSameItems(
				expectedValues,
				actualValues
			);
        }
	}
}
