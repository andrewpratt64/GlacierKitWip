using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.ViewModels.Debugging
{
	/// <summary>
	/// A single class item within a <see cref="StyleClassesViewerViewModel"/>
	/// </summary>
	public class StyleClassesViewerItem : ReactiveObject
	{
		/// <summary>
		/// True if the class is active, false otherwise
		/// </summary>
		[Reactive]
		public bool IsActive { get; set; }
		
		/// <summary>
		/// The name of the style class
		/// </summary>
		public string ClassName { get; }
		

		/// <summary>
		/// Creates a new <see cref="StyleClassesViewerItem"/> instance
		/// </summary>
		/// <param name="className">The value for <see cref="ClassName"/></param>
		/// <param name="isActive">The initial value for <see cref="IsActive"/></param>
		public StyleClassesViewerItem(string className, bool isActive = false)
		{
			IsActive = isActive;
			ClassName = className;
		}
	}


	/// <summary>
	/// View model for style class viewer debugging utility
	/// </summary>
	public class StyleClassesViewerViewModel : ViewModelBase
	{
		#region Private_fields

		private readonly SourceCache<StyleClassesViewerItem, string> _classItems;
		private readonly ReadOnlyObservableCollection<StyleClassesViewerItem> _classItemsView;

		#endregion


		#region Public_properties

		/// <summary>
		/// A read-only view of the current style class items
		/// </summary>
		public ReadOnlyObservableCollection<StyleClassesViewerItem> ClassItems => _classItemsView;


		/// <summary>
		/// The control to preview with style classes
		/// </summary>
		[Reactive]
		public IStyledElement? PreviewControl{ get; set; }


		/// <summary>
		/// True if the background for the visual parent of <see cref="PreviewControl"/> has it's color inverted
		/// </summary>
		[Reactive]
		public bool IsPreviewControlBackgroundInverted { get; set; }

		/// <summary>
		/// The background color to use when <see cref="IsPreviewControlBackgroundInverted"/> is false
		/// </summary>
		[Reactive]
		public IBrush? PreviewControlNormalBackground { get; set; }

		/// <summary>
		/// The background color to use when <see cref="IsPreviewControlBackgroundInverted"/> is true
		/// </summary>
		[Reactive]
		public IBrush? PreviewControlInvertedBackground { get; set; }



		#endregion


		#region Public_OAPHs

		/// <summary>
		/// The currently enabled classes, as it would appear for the Classes property of a control in axaml
		/// </summary>
		[MemberNotNull(nameof(ClassesStringifiedValue))]
		[ObservableAsProperty]
		public string ClassesStringifiedValue { get; } = null!;

		/// <summary>
		/// The current background color for the visual parent of <see cref="PreviewControl"/>
		/// </summary>
		[ObservableAsProperty]
		public IBrush? PreviewControlCurrentBackground { get; }

		#endregion


		#region Constructor

		/// <summary>
		/// Creates a new <see cref="StyleClassesViewerViewModel"/> instance
		/// </summary>
		/// <param name="classNames">The names of the style classes to include</param>
		public StyleClassesViewerViewModel(IEnumerable<string> classNames)
		{
			_classItems = new(item => item.ClassName);

#pragma warning disable IDE0008 // Use explicit type
			var classItemChangeSet = _classItems.Connect()
				.AutoRefresh(item => item.IsActive)
				.Publish();
#pragma warning restore IDE0008 // Use explicit type

			classItemChangeSet.Bind(out _classItemsView).Subscribe();

#pragma warning disable IDE0008 // Use explicit type
			
			var classItemIsActiveChanged = classItemChangeSet
				.WhenPropertyChanged(item => item.IsActive)
				.DistinctUntilChanged();

			var classItemBecomesActive = classItemIsActiveChanged
				.Where(propertyValue => propertyValue.Value);
			
			var classItemBecomesInactive = classItemIsActiveChanged
				.Where(propertyValue => !propertyValue.Value);

			var handlePseudoClassItemBecomesActive = classItemBecomesActive
				.Where(propertyValue => propertyValue.Sender.ClassName.StartsWith(':'))
				.Subscribe(propertyValue => (PreviewControl?.Classes as IPseudoClasses)?.Add(propertyValue.Sender.ClassName));

			var handleClassItemBecomesActive = classItemBecomesActive
				.Where(propertyValue => !propertyValue.Sender.ClassName.StartsWith(':'))
				.Subscribe(propertyValue => PreviewControl?.Classes?.Add(propertyValue.Sender.ClassName));

			var handlePseudoClassItemBecomesInactive = classItemBecomesInactive
				.Where(propertyValue => propertyValue.Sender.ClassName.StartsWith(':'))
				.Subscribe(propertyValue => (PreviewControl?.Classes as IPseudoClasses)?.Remove(propertyValue.Sender.ClassName));

			var handleClassItemBecomesInactive = classItemBecomesInactive
				.Where(propertyValue => !propertyValue.Sender.ClassName.StartsWith(':'))
				.Subscribe(propertyValue => PreviewControl?.Classes?.Remove(propertyValue.Sender.ClassName));

#pragma warning restore IDE0008 // Use explicit type

			classItemChangeSet
				.Filter(item => item.IsActive)
				.ToCollection()
				.Select(items => string.Join(' ', items.Select(item => item.ClassName)))
				.ToPropertyEx(
					source: this,
					property: x => x.ClassesStringifiedValue,
					initialValue: string.Empty,
					deferSubscription: false
				);

			classItemChangeSet.Connect();

			foreach (string className in classNames)
				_classItems.AddOrUpdate(new StyleClassesViewerItem(className));

			this.WhenAnyValue(
					x => x.IsPreviewControlBackgroundInverted,
					x => x.PreviewControlNormalBackground,
					x => x.PreviewControlInvertedBackground,
					(isInverted, normalBackground, invertedBackground) => isInverted ? PreviewControlInvertedBackground : PreviewControlNormalBackground
				)
				.ToPropertyEx(this, x => x.PreviewControlCurrentBackground);
		}

		#endregion
	}
}
