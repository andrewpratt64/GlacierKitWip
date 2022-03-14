using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using GlacierKitTestShared;

namespace GlacierKitTestShared.Tests
{
    #region Test that code does/doesn't throw
    /// <summary>
    /// Unit tests for GlacierKitTestShared.Util
    /// </summary>
    public class UtilTest
    {
#pragma warning disable IDE1006 // Naming Styles
        private static object _DATA_NotNullValue => new();

        private static Action _DATA_GoodCode => new(() => {});
        private static Action _DATA_BadCode => new(() => {throw new Exception(); });

        
        private class _DATA_GoodClass
        {
            public _DATA_GoodClass()
            { }
        }

        private class _DATA_BadClass
        {
            public _DATA_BadClass()
            {
                throw new Exception();
            }
        }
#pragma warning restore IDE1006 // Naming Styles


        [Fact]
        public static void Null_value_while_expecting_null_with_AssertNullConditional_passes()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertNullConditional(true, null));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public static void Null_value_while_expecting_not_null_with_AssertNullConditional_fails()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertNullConditional(false, null));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public static void Not_null_value_while_expecting_null_with_AssertNullConditional_fails()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertNullConditional(true, _DATA_NotNullValue));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public static void Not_null_value_while_expecting_not_null_with_AssertNullConditional_passes()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertNullConditional(false, _DATA_NotNullValue));

            // Assert
            Assert.Null(exception);
        }


        [Fact]
        public static void Bad_code_with_DidCodeThrowException_returns_true()
        {
            // Arrange
            bool? returnval;

            // Act
            returnval = Util.DidCodeThrowException(_DATA_BadCode);

            // Assert
            Assert.NotNull(returnval);
            Assert.True(returnval);
        }


        [Fact]
        public static void Good_code_with_DidCodeThrowException_returns_false()
        {
            // Arrange
            bool? returnval;

            // Act
            returnval = Util.DidCodeThrowException(_DATA_GoodCode);

            // Assert
            Assert.NotNull(returnval);
            Assert.False(returnval);
        }


        [Fact]
        public static void Bad_code_with_AssertCodeThrowsException_passes()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertCodeThrowsException(_DATA_BadCode));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public static void Good_code_with_AssertCodeThrowsException_fails()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertCodeThrowsException(_DATA_GoodCode));

            // Assert
            Assert.NotNull(exception);
        }


        [Fact]
        public static void Bad_code_with_AssertCodeDoesNotThrowException_fails()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertCodeDoesNotThrowException(_DATA_BadCode));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public static void Good_code_with_AssertCodeDoesNotThrowException_passes()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertCodeDoesNotThrowException(_DATA_GoodCode));

            // Assert
            Assert.Null(exception);
        }
        #endregion


        #region Test constructor
        [Fact]
        public static void Bad_class_with_AssertDefaultCtorDoesNotThrowException_fails()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertDefaultCtorDoesNotThrowException<_DATA_BadClass>());

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public static void Good_class_with_AssertDefaultCtorDoesNotThrowException_passes()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertDefaultCtorDoesNotThrowException<_DATA_GoodClass>());

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public static void Bad_class_with_AssertDefaultCtorWorks_fails()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertDefaultCtorWorks<_DATA_BadClass>());

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public static void Good_class_with_AssertDefaultCtorWorks_passes()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertDefaultCtorWorks<_DATA_GoodClass>());

            // Assert
            Assert.Null(exception);
        }
        #endregion
    }
}
