using DynamicData;
using DynamicData.Binding;
using GlacierKitCore.Commands;
using GlacierKitCore.Models;
using GlacierKitCore.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.ViewModels.Common
{
	public class MenuBarItemViewModel : ViewModelBase
	{
		#region Private_fields

		private ReadOnlyObservableCollection<MenuBarItemViewModel>? _childItems;

		#endregion


		#region Public_properties

		/// <summary>
		/// The unique identifier of this item
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// The print-friendly name of this item
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// The command to execute when the menu item is clicked, or
		/// null if the menu item should be a group
		/// </summary>
		public GKCommand<Unit, Unit>? ItemCommand { get; }

		/// <summary>
		/// The tree node corresponding to this menu item
		/// </summary>
		[Reactive]
		public TreeNode<MenuBarItemViewModel>? ItemNode { get; internal set; }

		/// <summary>
		/// The menu items contained within this items, or null if this item has no tree node
		/// </summary>
		public ReadOnlyObservableCollection<MenuBarItemViewModel>? ChildItems => _childItems;

		#endregion


		#region Public_commands

		/// <summary>
		/// Clicks the menu item, which may execute <see cref="ItemCommand"/> if it's set
		/// </summary>
		public ReactiveCommand<Unit, Unit> Click { get; }

		#endregion


		#region Constructor

		/// <summary>
		/// Creates a new menu item as a child of another menu item
		/// </summary>
		/// <param name="parentItemNode">The tree node of the parent menu item</param>
		/// <param name="id">The unique identifier of this item</param>
		/// <param name="title">The print-friendly name of this item</param>
		public MenuBarItemViewModel(string id, string title, GKCommand<Unit, Unit>? itemCommand = null)
		{
			Id = id;
			Title = title;
			ItemCommand = itemCommand;

			// Bind the child nodes of ItemNode to ChildItems, when possible
			this.WhenAnyValue(x => x.ItemNode)
				.WhereNotNull()
				.Subscribe
				(x =>
					x.ConnectToChildNodes()
					.ObserveOn(RxApp.MainThreadScheduler)
					.Transform(x => x.Value)
					.ObserveOn(RxApp.MainThreadScheduler)
					.Bind(out _childItems)
					.Subscribe()
				);

			Click = ReactiveCommand.Create(() =>
			{
				_ = ItemCommand?.Command.Execute().Subscribe();
				return Unit.Default;
			});
			
		}

		/// <summary>
		/// <inheritdoc cref="MenuBarItemViewModel(string, string)"/>
		/// </summary>
		/// <param name="id"><inheritdoc/></param>
		public MenuBarItemViewModel(string id):
			this(id, id)
		{}

		#endregion
	}
}
