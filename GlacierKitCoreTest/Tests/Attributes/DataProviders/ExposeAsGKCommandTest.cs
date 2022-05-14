using GlacierKitCore.Attributes.DataProviders;
using GlacierKitTestShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.Attributes.DataProviders
{
	public class ExposeAsGKCommandTest
	{
		#region Constructor

		[Fact]
		public static void ExposeAsGKCommand_default_ctor_works()
		{
			Util.AssertDefaultCtorWorks<ExposeAsGKCommandAttribute>();
		}

		#endregion
	}
}
