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
	/// Allows editing the value of a single property
	/// </summary>
	public class SinglePropertyEditor :
		TemplatedControl,
		IVisiblyDocumentable
	{
		#region Private_fields

		// DirectProperty fields for IVisiblyDocumentable members
		private string? _printName;
		private string? _description;
		private string? _verboseDescription;
		private Uri? _helpUri;

		#endregion


		#region Public_properties

		/// <summary>
		/// The current value of the property
		/// </summary>
		public object? Value { get; set; }

		/// <summary>
		/// If true, the property isn't required to always have a value
		/// </summary>
		public bool IsOptional { get; set; }

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

		#endregion


		#region Avalonia_properties

		#region IVisiblyDocumentable

		/// <summary>
		/// Defines the <see cref="PrintName"/> property
		/// </summary>
		public static readonly DirectProperty<SinglePropertyEditor, string?> PrintNameProperty =
			PropertyGroupEditor.PrintNameProperty.AddOwner<SinglePropertyEditor>(
				getter: o => o.PrintName,
				setter: (o, v) => o.PrintName = v
			);

		/// <summary>
		/// Defines the <see cref="Description"/> property
		/// </summary>
		public static readonly DirectProperty<SinglePropertyEditor, string?> DescriptionProperty =
			PropertyGroupEditor.DescriptionProperty.AddOwner<SinglePropertyEditor>(
				getter: o => o.Description,
				setter: (o, v) => o.Description = v
			);

		/// <summary>
		/// Defines the <see cref="VerboseDescription"/> property
		/// </summary>
		public static readonly DirectProperty<SinglePropertyEditor, string?> VerboseDescriptionProperty =
			PropertyGroupEditor.VerboseDescriptionProperty.AddOwner<SinglePropertyEditor>(
				getter: o => o.VerboseDescription,
				setter: (o, v) => o.VerboseDescription = v
			);

		/// <summary>
		/// Defines the <see cref="HelpUri"/> property
		/// </summary>
		public static readonly DirectProperty<SinglePropertyEditor, Uri?> HelpUriProperty =
			PropertyGroupEditor.HelpUriProperty.AddOwner<SinglePropertyEditor>(
				getter: o => o.HelpUri,
				setter: (o, v) => o.HelpUri = v
			);


		#endregion

		#endregion
	}
}
