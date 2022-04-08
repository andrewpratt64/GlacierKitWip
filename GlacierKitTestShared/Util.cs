using GlacierKitTestShared.CommonTestData;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GlacierKitTestShared
{
    public static class Util
    {
        #region Null_check
        /// <summary>
        /// Verifies that a given value either is or isn't null
        /// </summary>
        /// <param name="shouldExpectNull">True if value is expected to be null, false if value is NOT expected to be null</param>
        /// <param name="value">Nullable value to test</param>
        public static void AssertNullConditional(bool shouldExpectNull, object? value)
        {
            if (shouldExpectNull)
                Assert.Null(value);
            else
                Assert.NotNull(value);
        }
        #endregion

        #region Test_that_code_does/doesn't_throw
        /// <summary>
        /// Tests if the given code throws an exception
        /// </summary>
        /// <param name="testCode">The code which may throw an exception.</param>
        /// <returns>True if <paramref name="testCode"/> threw an error while running</returns>
        public static bool DidCodeThrowException(Action testCode)
        {
            return Record.Exception(testCode) != null;
        }

        /// <summary>
        /// Verifies that the given code throws an exception when ran
        /// </summary>
        /// <param name="testCode">The code expected to throw an exception</param>
        /// <exception cref="Xunit.Sdk.FalseException">Thrown if <paramref name="testCode"/> did not throw an exception</exception>
        public static void AssertCodeThrowsException(Action testCode)
        {
            //Assert.True(DidCodeThrowException(testCode));

            Exception? thrownException = Record.Exception(testCode);
            Assert.False(thrownException == null, "testCode was expected to throw an exception but did not throw anything when executed.");
        }

        /// <summary>
        /// Verifies that the given code does NOT throw an exception when ran
        /// </summary>
        /// <param name="testCode">The code expected to not throw any exceptions</param>
        /// <exception cref="Xunit.Sdk.TrueException">Thrown if <paramref name="testCode"/> threw an exception</exception>
        public static void AssertCodeDoesNotThrowException(Action testCode)
        {
            //Assert.False(DidCodeThrowException(testCode));

            Exception? thrownException = Record.Exception(testCode);
            Assert.True(
                thrownException == null,
                "testCode was NOT expected to throw an exception but an exception of type "
                + thrownException?.GetType().Name ?? string.Empty
                + " was thrown when executed.\nException details: "
                + thrownException?.Message ?? string.Empty
            );
        }
        #endregion


        #region Test_constructor
        /// <summary>
        /// Verifies that a class's default constructor doesn't throw an exception
        /// </summary>
        /// <typeparam name="T">The class who's constructor should be tested</typeparam>
        /// <exception cref="Xunit.Sdk.FalseException">Thrown if the default constructor threw an exception</exception>
        public static void AssertDefaultCtorDoesNotThrowException<T>()
            where T : new()
        {
            AssertCodeDoesNotThrowException(() => new T());
        }

        /// <summary>
        /// Verifies that a class's default constructor works
        /// </summary>
        /// <typeparam name="T">The class who's constructor should be tested</typeparam>
        /// <exception cref="Xunit.Sdk.FalseException">Thrown if the default constructor threw an exception</exception>
        public static void AssertDefaultCtorWorks<T>()
            where T : new()
        {
            AssertDefaultCtorDoesNotThrowException<T>();
        }
        #endregion

        #region Test_collections

        /// <summary>
        /// Verifies that two collections are the same size and contain the same items, ignoring the order they appear
        /// </summary>
        /// <typeparam name="TItem">Type of each item</typeparam>
        /// <param name="expectedItems">The items expected to be in <paramref name="actualItems"/></param>
        /// <param name="actualItems">The collection to test</param>
        /// <exception cref="Xunit.Sdk.EmptyException">Thrown if the items of the two collections don't match</exception>
        public static void AssertCollectionsHaveSameItems<TItem>(IEnumerable<TItem> expectedItems, IEnumerable<TItem> actualItems)
        {
			Assert.Equal(expectedItems.Count(), actualItems.Count());
            Assert.Empty(expectedItems.Except(actualItems));
            Assert.Empty(actualItems.Except(expectedItems));
        }

		#endregion


		#region Misc_utility_methods

		

		#endregion
	}
}
