using GlacierKitCore.Models;
using GlacierKitTestShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.Models
{
	public class ReactiveOptionalTest
	{
		#region Constructor

		[Fact]
		public static void ReactiveOptional_Default_ctor_works()
		{
			Util.AssertDefaultCtorWorks<ReactiveOptional<object>>();
		}

		[Fact]
		public static void ReactiveOptional_Ctor_with_value_dosent_throw()
		{
			Util.AssertCodeDoesNotThrowException(
				() => _ = new ReactiveOptional<object>(GeneralUseData.SmallInt)
			);
		}

		#endregion


		#region HasValue

		[Fact]
		public static void HasValue_is_initially_false_if_no_initial_value_given()
		{
			// Arrange
			ReactiveOptional<object> optional;
			bool actualValue;

			// Act
			optional = new();
			actualValue = optional.HasValue;

			// Assert
			Assert.False(actualValue);
		}

		#endregion


		#region HasValue

		[Fact]
		public static void HasValue_is_initially_true_if_initial_value_given()
		{
			// Arrange
			ReactiveOptional<object> optional;
			object initialOptionalValue = GeneralUseData.SmallInt;
			bool actualValue;

			// Act
			optional = new(initialOptionalValue);
			actualValue = optional.HasValue;

			// Assert
			Assert.True(actualValue);
		}

		[Fact]
		public static void HasValue_becomes_true_after_LastValue_is_set()
		{
			// Arrange
			ReactiveOptional<object> optional;
			object lastValue = GeneralUseData.SmallInt;
			bool actualValue;

			// Act
			optional = new();
			optional.LastValue = lastValue;
			actualValue = optional.HasValue;

			// Assert
			Assert.True(actualValue);
		}

		#endregion


		#region MakeEmpty

		[Fact]
		public static void MakeEmpty_dosent_throw()
		{
			Util.AssertCodeDoesNotThrowException(
				() => _ = ReactiveOptional<object>.MakeEmpty()
			);
		}

		[Fact]
		public static void MakeEmpty_Provides_unique_instances()
		{
			// Arrange
			List<ReactiveOptional<object>> returnedValues = new();
			int timesToInvoke = 10;
			int expectedDistinctReturnedValues = timesToInvoke;
			int actualDistinctReturnedValues;

			// Act
			for (int i = 0; i < timesToInvoke; i++)
				returnedValues.Add(ReactiveOptional<object>.MakeEmpty());
			actualDistinctReturnedValues = returnedValues.Distinct().Count();

			// Assert
			Assert.Equal(expectedDistinctReturnedValues, actualDistinctReturnedValues);
		}

		[Fact]
		public static void MakeEmpty_HasValue_is_initially_false()
		{
			// Arrange
			bool actualValue;

			// Act
			actualValue = ReactiveOptional<object>.MakeEmpty().HasValue;

			// Assert
			Assert.False(actualValue);
		}

		#endregion
	}
}
