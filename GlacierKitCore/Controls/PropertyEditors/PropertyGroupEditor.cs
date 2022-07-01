using Avalonia;
using Avalonia.Controls.Primitives;
using GlacierKitCore.Misc;
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
	/// <remarks>This control should be contained within either a root <see cref="PropertyEditor"/> or another <see cref="PropertyGroupEditor"/></remarks>
	public class PropertyGroupEditor :
		TemplatedControl,
		IPropertyGroupEditor,
		IVisiblyDocumentable
	{
		#region Private_fields

		// DirectProperty fields for IVisiblyDocumentable members
		private string? _printName;
		private string? _description;
		private string? _verboseDescription;
		private Uri? _helpUri;

		// DirectProperty fields for IPropertyGroupEditor members
		private IEnumerable<PropertyGroupEditor>? _subgroupPropertyEditors;
		private IEnumerable<SinglePropertyEditor>? _propertyEditors;

		#endregion


		#region Public_property_overrides

		#region IVisiblyDocumentable

		/// <inheritdoc/>
		public string? PrintName
		{
			get => _printName;
			set => SetAndRaise(PrintNameProperty, ref _printName, value);
		}

		/// <inheritdoc/>
		public string? Description
		{
			get => _description;
			set => SetAndRaise(DescriptionProperty, ref _description, value);
		}

		/// <inheritdoc/>
		public string? VerboseDescription
		{
			get => _verboseDescription;
			set => SetAndRaise(VerboseDescriptionProperty, ref _verboseDescription, value);
		}

		/// <inheritdoc/>
		public Uri? HelpUri
		{
			get => _helpUri;
			set => SetAndRaise(HelpUriProperty, ref _helpUri, value);
		}

		#endregion


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

		#region IVisiblyDocumentable

		/// <summary>
		/// Defines the <see cref="PrintName"/> property
		/// </summary>
		public static readonly DirectProperty<PropertyGroupEditor, string?> PrintNameProperty =
			AvaloniaProperty.RegisterDirect<PropertyGroupEditor, string?>(
				name: nameof(PrintName),
				getter: o => o.PrintName,
				setter: (o, v) => o.PrintName = v
			);

		/// <summary>
		/// Defines the <see cref="Description"/> property
		/// </summary>
		public static readonly DirectProperty<PropertyGroupEditor, string?> DescriptionProperty =
			AvaloniaProperty.RegisterDirect<PropertyGroupEditor, string?>(
				name: nameof(Description),
				getter: o => o.Description,
				setter: (o, v) => o.Description = v
			);

		/// <summary>
		/// Defines the <see cref="VerboseDescription"/> property
		/// </summary>
		public static readonly DirectProperty<PropertyGroupEditor, string?> VerboseDescriptionProperty =
			AvaloniaProperty.RegisterDirect<PropertyGroupEditor, string?>(
				name: nameof(VerboseDescription),
				getter: o => o.VerboseDescription,
				setter: (o, v) => o.VerboseDescription = v
			);

		/// <summary>
		/// Defines the <see cref="HelpUri"/> property
		/// </summary>
		public static readonly DirectProperty<PropertyGroupEditor, Uri?> HelpUriProperty =
			AvaloniaProperty.RegisterDirect<PropertyGroupEditor, Uri?>(
				name: nameof(HelpUri),
				getter: o => o.HelpUri,
				setter: (o, v) => o.HelpUri = v
			);

		#endregion


		#region IPropertyGroupEditor

		/// <summary>
		/// Defines the <see cref="PropertyEditors"/> property
		/// </summary>
		public static readonly DirectProperty<PropertyGroupEditor, IEnumerable<SinglePropertyEditor>?> PropertyEditorsProperty =
			PropertyEditor.PropertyEditorsProperty.AddOwner<PropertyGroupEditor>(
				getter: o => o.PropertyEditors,
				setter: (o, v) => o.PropertyEditors = v
			);

		/// <summary>
		/// Defines the <see cref="SubgroupPropertyEditors"/> property
		/// </summary>
		public static readonly DirectProperty<PropertyGroupEditor, IEnumerable<PropertyGroupEditor>?> SubgroupPropertyEditorsProperty =
			PropertyEditor.SubgroupPropertyEditorsProperty.AddOwner<PropertyGroupEditor>(
				getter: o => o.SubgroupPropertyEditors,
				setter: (o, v) => o.SubgroupPropertyEditors = v
			);

		#endregion

		#endregion
	}
}
