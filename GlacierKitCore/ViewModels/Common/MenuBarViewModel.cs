using DynamicData;
using GlacierKitCore.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GlacierKitCore.ViewModels.Common
{
    public class MenuBarViewModel : ViewModelBase
    {
		private readonly ReadOnlyObservableCollection<MenuBarItemViewModel> _rootItems;


		/// <summary>
		/// The hierarchy of menu items
		/// </summary>
		public MultiRootTree<MenuBarItemViewModel> ItemTree { get; }

		/// <summary>
		/// The menu items at the root level
		/// </summary>
		public ReadOnlyObservableCollection<MenuBarItemViewModel> RootItems => _rootItems;


		public MenuBarViewModel()
		{
			ItemTree = new();
			ItemTree.ConnectToRootNodes()
				.Transform(node => node.Value)
				.Bind(out _rootItems)
				.DeferUntilLoaded()
				.Subscribe();
		}
    }


    public class MenuBarItemViewModel : ViewModelBase
	{
		private readonly ReadOnlyObservableCollection<MenuBarItemViewModel> _childItems;


		/// <summary>
		/// The tree node corresponding to this menu item
		/// </summary>
		public TreeNode<MenuBarItemViewModel> ItemNode { get; }

		/// <summary>
		/// The parent menu item, if any
		/// </summary>
		[ObservableAsProperty]
		public MenuBarItemViewModel? ParentItem { get; }

		/// <summary>
		/// The menu items contained within this item
		/// </summary>
		public ReadOnlyObservableCollection<MenuBarItemViewModel> ChildItems => _childItems;

		/// <summary>
		/// The unique identifier of this item
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// The fully qualified identifier of this item
		/// </summary>
		[ObservableAsProperty]
		public string FullId { get; }


		internal MenuBarItemViewModel(TreeNode<MenuBarItemViewModel> itemNode, string id)
		{
			ItemNode = itemNode;
			Id = id;

			this.WhenAnyValue(x => x.ItemNode.Parent)
				.BindTo(this, x => x.ParentItem);

			ItemNode.ConnectToChildNodes()
				.Transform(node => node.Value)
				.Bind(out _childItems)
				.DeferUntilLoaded()
				.Subscribe();
		}
	}
}
