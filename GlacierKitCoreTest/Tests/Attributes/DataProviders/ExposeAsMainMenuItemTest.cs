using GlacierKitCore.Attributes.DataProviders;
using GlacierKitCore.Commands;
using GlacierKitTestShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.Attributes.DataProviders
{
	public class ExposeAsMainMenuItemTest
	{
		#region Constructor

		[Fact]
		[Trait("TestingMember", "Constructor")]
		public static void ExposeAsMainMenuItem_constructor_doesnt_throw()
		{
			Util.AssertCodeDoesNotThrowException(
				() => _ = new ExposeAsMainMenuItemAttribute()
			);
		}

		#endregion
	}
}
