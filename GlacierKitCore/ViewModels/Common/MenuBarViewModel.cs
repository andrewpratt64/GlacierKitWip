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
}
