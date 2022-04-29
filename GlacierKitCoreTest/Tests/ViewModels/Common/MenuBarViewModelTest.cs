using DynamicData;
using GlacierKitCore.Models;
using GlacierKitCore.ViewModels.Common;
using GlacierKitTestShared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.ViewModels.Common
{
	public class MenuBarViewModelTest
	{
		#region Constructor

		[Fact]
		[Trait("TestingMember", "Constructor")]
		public static void MenuBarViewModel_Default_ctor_works()
		{
			Util.AssertDefaultCtorWorks<MenuBarViewModel>();
		}

		#endregion


		#region ItemTree

		[Fact]
		[Trait("TestingMember", "Property_ItemTree")]
		public static void ItemTree_is_not_initially_null()
		{
			// Arrange
			MenuBarViewModel viewModel;
			MultiRootTree<MenuBarItemViewModel>? actualValue;

			// Act
			viewModel = new();
			actualValue = viewModel.ItemTree;

			// Assert
			Assert.NotNull(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Property_ItemTree")]
		public static void ItemTree_initially_has_no_nodes()
		{
			// Arrange
			MenuBarViewModel viewModel;
			ReadOnlyObservableCollection<TreeNode<MenuBarItemViewModel>> nodes;

			// Act
			viewModel = new();
			viewModel.ItemTree.ConnectToNodes().Bind(out nodes);

			// Assert
			Assert.Empty(nodes);
		}

		#endregion


		#region RootItems

		[Fact]
		[Trait("TestingMember", "Property_RootItems")]
		public static void RootItems_is_not_initially_null()
		{
			// Arrange
			MenuBarViewModel viewModel;
			ReadOnlyObservableCollection<MenuBarItemViewModel>? actualValue;

			// Act
			viewModel = new();
			actualValue = viewModel.RootItems;

			// Assert
			Assert.NotNull(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Property_RootItems")]
		public static void RootItems_initially_has_no_items()
		{
			// Arrange
			MenuBarViewModel viewModel;
			ReadOnlyObservableCollection<MenuBarItemViewModel> actualValue;

			// Act
			viewModel = new();
			actualValue = viewModel.RootItems;

			// Assert
			Assert.Empty(actualValue);
		}

		#endregion
	}
}
