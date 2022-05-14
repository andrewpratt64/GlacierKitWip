using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Attributes.DataProviders
{
	/// <summary>
	/// Marks a GK command as publicly available to the app
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ExposeAsGKCommandAttribute : Attribute
	{}
}
