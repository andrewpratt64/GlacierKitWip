using GlacierKitCore.Attributes;
using GlacierKitTestShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Attributes
{
    public class GKViewModelTest
    {
        [Fact]
        public static void Default_ctor_works()
        {
            Util.AssertDefaultCtorWorks<GKViewModel>();
        }
    }
}
