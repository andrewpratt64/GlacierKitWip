using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Controls.PropertyEditors
{
	/// <summary>
	/// Describes a control for editing a group of properties
	/// </summary>
	public interface IPropertyGroupEditor
	{
		/// <summary>
		/// Sub-categories of properties with editors
		/// </summary>
		public abstract IEnumerable<PropertyGroupEditor>? SubgroupPropertyEditors { get; }

		/// <summary>
		/// The properties to edit
		/// </summary>
		public abstract IEnumerable<SinglePropertyEditor>? PropertyEditors { get; }
	}
}
