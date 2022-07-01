using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Misc
{
	/// <summary>
	/// Interface for objects that can be documented in a way that is visible to a user
	/// </summary>
	public interface IVisiblyDocumentable
	{
		/// <summary>
		/// The print-friendly name of the object
		/// </summary>
		public abstract string? PrintName { get; }

		/// <summary>
		/// A brief description of the object
		/// </summary>
		public abstract string? Description { get; }

		/// <summary>
		/// A description of the object, should be longer than <see cref="Description"/>
		/// </summary>
		public abstract string? VerboseDescription { get; }

		/// <summary>
		/// The uri of help information regarding the object
		/// </summary>
		public abstract Uri? HelpUri { get; }
	}
}
