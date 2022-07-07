using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Controls
{
	[TemplatePart("PART_Container", typeof(IPanel))]
	public class ContentPair : TemplatedControl
	{
		#region Private_fields

		private Avalonia.Controls.Controls _childControls;
		private IControl? _firstShownControl, _lastShownControl;

		#endregion


		#region Public_properties

		/// <summary>
		/// The first of two controls
		/// </summary>
		public IControl? FirstControl
		{
			get => GetValue(FirstControlProperty);
			set => SetValue(FirstControlProperty, value);
		}

		/// <summary>
		/// The first to appear of two controls
		/// </summary>
		public IControl? FirstShownControl
		{
			get => _firstShownControl;
			private set => SetAndRaise(FirstShownControlProperty, ref _firstShownControl, value);
		}

		/// <summary>
		/// The second, and also last, of two controls
		/// </summary>
		public IControl? LastControl
		{
			get => GetValue(LastControlProperty);
			set => SetValue(LastControlProperty, value);
		}

		/// <summary>
		/// The last to appear of two controls
		/// </summary>
		public IControl? LastShownControl
		{
			get => _lastShownControl;
			private set => SetAndRaise(LastShownControlProperty, ref _lastShownControl, value);
		}

		/// <summary>
		/// When true, reverses the visual order of <see cref="FirstControl"/> and <see cref="LastControl"/>
		/// </summary>
		public bool IsReversed
		{
			get => GetValue(IsReversedProperty);
			set => SetValue(IsReversedProperty, value);
		}

		/// <summary>
		/// The two child controls in order of appearance
		/// </summary>
		public Avalonia.Controls.Controls ChildControls
		{
			get => _childControls;
			private set => SetAndRaise(ChildControlsProperty, ref _childControls, value);
		}

		#endregion


		#region Avalonia_properties

		/// <summary>
		/// Defines the <see cref="FirstControl"/> property
		/// </summary>
		public static readonly StyledProperty<IControl?> FirstControlProperty =
			AvaloniaProperty.Register<ContentPair, IControl?>(nameof(FirstControl));

		/// <summary>
		/// Defines the <see cref="FirstShownControl"/> property
		/// </summary>
		public static readonly DirectProperty<ContentPair, IControl?> FirstShownControlProperty =
			AvaloniaProperty.RegisterDirect<ContentPair, IControl?>(
				name: nameof(FirstShownControl),
				getter: o => o.FirstShownControl
			);

		/// <summary>
		/// Defines the <see cref="LastControl"/> property
		/// </summary>
		public static readonly StyledProperty<IControl?> LastControlProperty =
			AvaloniaProperty.Register<ContentPair, IControl?>(nameof(LastControl));

		/// <summary>
		/// Defines the <see cref="LastShownControl"/> property
		/// </summary>
		public static readonly DirectProperty<ContentPair, IControl?> LastShownControlProperty =
			AvaloniaProperty.RegisterDirect<ContentPair, IControl?>(  
				name: nameof(LastShownControl),
				getter: o => o.LastShownControl
			);

		/// <summary>
		/// Defines the <see cref="IsReversed"/> property
		/// </summary>
		public static readonly StyledProperty<bool> IsReversedProperty =
			AvaloniaProperty.Register<ContentPair, bool>(nameof(IsReversed));

		/// <summary>
		/// Defines the <see cref="ChildControls"/> property
		/// </summary>
		public static readonly DirectProperty<ContentPair, Avalonia.Controls.Controls> ChildControlsProperty =
			AvaloniaProperty.RegisterDirect<ContentPair, Avalonia.Controls.Controls>(
				name: nameof(ChildControls),
				getter: o => o.ChildControls
			);

		#endregion


		#region Constructor

		/// <summary>
		/// Creates a new instance of <see cref="ContentPair"/>
		/// </summary>
		public ContentPair() :
			base()
		{
			// Init _childControls
			_childControls = new();

			// Update everything based on the initial state of the object
			UpdateControlPair();
		}

		#endregion


		#region Protected_overrides

		/// <inheritdoc/>
		override protected void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
		{
			base.OnPropertyChanged(change);

			// Handle changes in child controls and the order of child controls
			if (change.Property == FirstControlProperty || change.Property == LastControlProperty || change.Property == IsReversedProperty)
				UpdateControlPair();
		}

		#endregion


		#region Private_methods

		/// <summary>
		/// Updates the order of <see cref="_childControls"/> based on the value of <see cref="IsReversed"/>
		/// </summary>
		private void UpdateChildControlsOrder()
		{
			// Don't bother updating if one or both controls are missing
			if (FirstControl == null || LastControl == null)
				return;

			// Clear ChildControls
			_childControls.Clear();

			// Populate _childControls, based on IsReversed
			if (IsReversed)
			{
				_childControls.Add(LastControl);
				_childControls.Add(FirstControl);
			}
			else
			{
				_childControls.Add(FirstControl);
				_childControls.Add(LastControl);
			}

			// Assert ChildControls contains only the first and last control with no duplicates
			Debug.Assert(_childControls.Count == 2);
			Debug.Assert(_childControls.Contains(FirstControl));
			Debug.Assert(_childControls.Contains(LastControl));
		}


		/// <summary>
		/// Updates the value of <see cref="FirstShownControl"/> and <see cref="LastShownControl"/>
		/// </summary>
		private void UpdateFirstAndLastShownControls()
		{
			FirstShownControl = _childControls.FirstOrDefault();
			IControl? lastShownControl = _childControls.LastOrDefault();

			if (FirstShownControl == lastShownControl)
				LastShownControl = null;
			else
				LastShownControl = lastShownControl;
		}


		/// <summary>
		/// Updates the state of the control pair in response to a change in child controls or order of child controls
		/// </summary>
		private void UpdateControlPair()
		{
			UpdateChildControlsOrder();
			UpdateFirstAndLastShownControls();
		}

		#endregion
	}
}
