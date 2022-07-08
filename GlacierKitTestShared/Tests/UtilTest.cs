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
        private static object NotNullValue => new();

        private static Action GoodCode => new(() => {});
        private static Action BadCode => new(() => {throw new Exception(); });

        
        private class GoodClass
        {
            public GoodClass()
            { }
        }

        private class BadClass
        {
            public BadClass()
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
            exception = Record.Exception(() => Util.AssertNullConditional(true, NotNullValue));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public static void Not_null_value_while_expecting_not_null_with_AssertNullConditional_passes()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertNullConditional(false, NotNullValue));

            // Assert
            Assert.Null(exception);
        }


        [Fact]
        public static void Bad_code_with_DidCodeThrowException_returns_true()
        {
            // Arrange
            bool? returnval;

            // Act
            returnval = Util.DidCodeThrowException(BadCode);

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
            returnval = Util.DidCodeThrowException(GoodCode);

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
            exception = Record.Exception(() => Util.AssertCodeThrowsException(BadCode));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public static void Good_code_with_AssertCodeThrowsException_fails()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertCodeThrowsException(GoodCode));

            // Assert
            Assert.NotNull(exception);
        }


        [Fact]
        public static void Bad_code_with_AssertCodeDoesNotThrowException_fails()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertCodeDoesNotThrowException(BadCode));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public static void Good_code_with_AssertCodeDoesNotThrowException_passes()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertCodeDoesNotThrowException(GoodCode));

            // Assert
            Assert.Null(exception);
        }
        #endregion


        #region Test_constructor
        [Fact]
        public static void Bad_class_with_AssertDefaultCtorDoesNotThrowException_fails()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertDefaultCtorDoesNotThrowException<BadClass>());

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public static void Good_class_with_AssertDefaultCtorDoesNotThrowException_passes()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertDefaultCtorDoesNotThrowException<GoodClass>());

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public static void Bad_class_with_AssertDefaultCtorWorks_fails()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertDefaultCtorWorks<BadClass>());

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public static void Good_class_with_AssertDefaultCtorWorks_passes()
        {
            // Arrange
            Exception? exception;

            // Act
            exception = Record.Exception(() => Util.AssertDefaultCtorWorks<GoodClass>());

            // Assert
            Assert.Null(exception);
        }
        #endregion

        [Fact]
        public static void AssertCollectionsHaveSameItems_with_two_empty_collections_passes()
        {
            // Arrange
            Exception? exception;
            IEnumerable<string> expectedCollection = new List<string>{};
            IEnumerable<string> actualCollection = new List<string>{};

            // Act
            exception = Record.Exception(() => Util.AssertCollectionsHaveSameItems(expectedCollection, actualCollection));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public static void AssertCollectionsHaveSameItems_with_different_collections_fails()
        {
            // Arrange
            Exception? exception;
            IEnumerable<string> expectedCollection = new List<string>
            {
                "Apple",
                "Banana",
                "Pear"
            };
            IEnumerable<string> actualCollection = new List<string>
            {
                "Red",
                "Orange",
                "Yellow",
                "Lime"
            };

            // Act
            exception = Record.Exception(() => Util.AssertCollectionsHaveSameItems(expectedCollection, actualCollection));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public static void AssertCollectionsHaveSameItems_with_different_size_collections_fails()
        {
            // Arrange
            Exception? exception;
            IEnumerable<string> expectedCollection = new List<string>
            {
                "Apple",
                "Banana",
                "Pear"
            };
            IEnumerable<string> actualCollection = new List<string>
            {
                "Apple",
                "Banana",
                "Pear",
                "Banana"
            };

            // Act
            exception = Record.Exception(() => Util.AssertCollectionsHaveSameItems(expectedCollection, actualCollection));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public static void AssertCollectionsHaveSameItems_with_identical_collections_passes()
        {
            // Arrange
            Exception? exception;
            IEnumerable<string> expectedCollection = new List<string>
            {
                "Apple",
                "Banana",
                "Pear"
            };
            IEnumerable<string> actualCollection = new List<string>
            {
                "Apple",
                "Banana",
                "Pear"
            };

            // Act
            exception = Record.Exception(() => Util.AssertCollectionsHaveSameItems(expectedCollection, actualCollection));

            // Assert
            Assert.Null(exception);
        }
    }
}
