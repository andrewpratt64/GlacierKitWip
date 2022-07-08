using GlacierKitCore.Attributes;
using GlacierKitCore.Attributes.DataProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.Attributes.DataProviders
{
	public class GKDataProviderTest
	{
		#region Theory_data

		public class NotAGKCommandProvider
		{ }

		[GKDataProviderAttribute]
		public static class AGKCommandProvider
		{ }

		private static readonly Type _DATA_InvalidTypeForIsTypeAGKCommandProvider = typeof(NotAGKCommandProvider);
		private static readonly Type _DATA_ValidTypeForIsTypeAGKCommandProvider = typeof(AGKCommandProvider);

		#endregion


		#region IsTypeAGKDataProvider

		[Fact]
		public static void IsTypeAGKDataProvider_with_invalid_type_returns_false()
		{
			// Arrange
			Type type = _DATA_InvalidTypeForIsTypeAGKCommandProvider;

			// Assert
			Assert.False(GKDataProviderAttribute.IsTypeAGKDataProvider(type));
		}

		[Fact]
		public static void IsTypeAGKDataProvider_with_valid_type_returns_true()
		{
			// Arrange
			Type type = _DATA_ValidTypeForIsTypeAGKCommandProvider;

			// Assert
			Assert.True(GKDataProviderAttribute.IsTypeAGKDataProvider(type));
		}

		#endregion
	}
}
