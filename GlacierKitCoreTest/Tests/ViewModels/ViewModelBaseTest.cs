using Dock.Model.Core;
using GlacierKitCore.ViewModels;
using GlacierKitTestShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.ViewModels
{
    public class ViewModelBaseTest
    {
        [Fact]
        public static void Default_ctor_works()
        {
            Util.AssertDefaultCtorWorks<ViewModelBase>();
        }
    }
}
