using GlacierKitCore.Controls.PropertyEditors;
using GlacierKitTestShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.Controls.PropertyEditors
{
	public class PropertyGroupEditorTest
	{
		#region Constructor

		[Fact]
		[Trait("TestingMember", "Constructor")]
		public static void Default_ctor_works()
		{
			Util.AssertDefaultCtorWorks<PropertyGroupEditor>();
		}

		#endregion
	}
}
