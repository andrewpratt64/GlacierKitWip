using System;
using Xunit;

namespace GlacierKitTestShared
{
    public static class Util
    {
        #region Test that code does/doesn't throw
        /// <summary>
        /// Tests if the given code throws an exception
        /// </summary>
        /// <param name="testCode">The code which may throw an exception.</param>
        /// <returns>True if <paramref name="testCode"/> threw an error while running</returns>
        public static bool DidCodeThrowException(Func<object> testCode)
        {
            return Record.Exception(testCode) != null;
        }

        /// <summary>
        /// Verifies that the given code throws an exception when ran
        /// </summary>
        /// <param name="testCode">The code expected to throw an exception</param>
        /// <exception cref="Xunit.Sdk.TrueException">Thrown if <paramref name="testCode"/> did not throw an exception</exception>
        public static void AssertCodeThrowsException(Func<object> testCode)
        {
            Assert.True(DidCodeThrowException(testCode));
        }

        /// <summary>
        /// Verifies that the given code does NOT throw an exception when ran
        /// </summary>
        /// <param name="testCode">The code expected to not throw any exceptions</param>
        /// <exception cref="Xunit.Sdk.FalseException">Thrown if <paramref name="testCode"/> threw an exception</exception>
        public static void AssertCodeDoesNotThrowException(Func<object> testCode)
        {
            Assert.False(DidCodeThrowException(testCode));
        }
        #endregion


        #region Test constructor
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
    }
}
