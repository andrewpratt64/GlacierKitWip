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

		private static readonly Type InvalidTypeForIsTypeAGKCommandProvider = typeof(NotAGKCommandProvider);
		private static readonly Type ValidTypeForIsTypeAGKCommandProvider = typeof(AGKCommandProvider);

		#endregion


		#region IsTypeAGKDataProvider

		[Fact]
		public static void IsTypeAGKDataProvider_with_invalid_type_returns_false()
		{
			// Arrange
			Type type = InvalidTypeForIsTypeAGKCommandProvider;

			// Assert
			Assert.False(GKDataProviderAttribute.IsTypeAGKDataProvider(type));
		}

		[Fact]
		public static void IsTypeAGKDataProvider_with_valid_type_returns_true()
		{
			// Arrange
			Type type = ValidTypeForIsTypeAGKCommandProvider;

			// Assert
			Assert.True(GKDataProviderAttribute.IsTypeAGKDataProvider(type));
		}

		#endregion
	}
}
