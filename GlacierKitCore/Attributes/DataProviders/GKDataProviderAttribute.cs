using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Attributes.DataProviders
{
    /// <summary>
    /// Denotes a public static class as a provider of data for GlacierKit
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GKDataProviderAttribute : Attribute
    {
		/// <summary>
		/// Test if a given type is a GKData Provider
		/// </summary>
		/// <remarks>A type is considered a GKData Provider if it is a public static class with the GKDataProviderAttribute attribute</remarks>
		/// <param name="type">Type to test</param>
		/// <returns>True if <paramref name="type"/> is a GKData Provider, false otherwise</returns>
		public static bool IsTypeAGKDataProvider(Type type)
		{
			return type.IsAbstract
				&& type.IsSealed
				&& type.IsDefined(typeof(GKDataProviderAttribute), false);
		}
	}
}
