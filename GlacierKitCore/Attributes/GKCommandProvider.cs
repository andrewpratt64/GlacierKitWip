using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Attributes
{
    /// <summary>
    /// Denotes a public static class as a provider of commands for GlacierKit
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GKCommandProvider : Attribute
    {
    }
}
