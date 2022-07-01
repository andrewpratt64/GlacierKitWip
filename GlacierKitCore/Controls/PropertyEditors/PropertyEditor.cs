using Avalonia;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Controls.PropertyEditors
{
	/// <summary>
	/// The root control for editing a set of properties and groups of properties
	/// </summary>
	public class PropertyEditor :
		TemplatedControl,
		IPropertyGroupEditor
	{
		#region Private_fields
		
		// DirectProperty fields for IPropertyGroupEditor members
		private IEnumerable<PropertyGroupEditor>? _subgroupPropertyEditors;
		private IEnumerable<SinglePropertyEditor>? _propertyEditors;

		#endregion


		#region Public_property_overrides

		#region IPropertyGroupEditor

		/// <inheritdoc/>
		public IEnumerable<PropertyGroupEditor>? SubgroupPropertyEditors
		{
			get => _subgroupPropertyEditors;
			set => SetAndRaise(SubgroupPropertyEditorsProperty, ref _subgroupPropertyEditors, value);
		}

		/// <inheritdoc/>
		public IEnumerable<SinglePropertyEditor>? PropertyEditors
		{
			get => _propertyEditors;
			set => SetAndRaise(PropertyEditorsProperty, ref _propertyEditors, value);
		}

		#endregion

		#endregion


		#region Avalonia_properties

		#region IPropertyGroupEditor

		/// <summary>
		/// Defines the <see cref="PropertyEditors"/> property
		/// </summary>
		public static readonly DirectProperty<PropertyEditor, IEnumerable<SinglePropertyEditor>?> PropertyEditorsProperty =
			AvaloniaProperty.RegisterDirect<PropertyEditor, IEnumerable<SinglePropertyEditor>?>(
				name: nameof(PropertyEditors),
				getter: o => o.PropertyEditors,
				setter: (o, v) => o.PropertyEditors = v
			);

		/// <summary>
		/// Defines the <see cref="SubgroupPropertyEditors"/> property
		/// </summary>
		public static readonly DirectProperty<PropertyEditor, IEnumerable<PropertyGroupEditor>?> SubgroupPropertyEditorsProperty =
			AvaloniaProperty.RegisterDirect<PropertyEditor, IEnumerable<PropertyGroupEditor>?>(
				name: nameof(SubgroupPropertyEditors),
				getter: o => o.SubgroupPropertyEditors,
				setter: (o, v) => o.SubgroupPropertyEditors = v
			);

		#endregion

		#endregion
	}
}
