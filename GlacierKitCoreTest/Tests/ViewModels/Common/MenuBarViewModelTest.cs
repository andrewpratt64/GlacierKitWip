using DynamicData;
using GlacierKitCore.Models;
using GlacierKitCore.ViewModels.Common;
using GlacierKitTestShared;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0018:Inline variable declaration", Justification = "Intentional seperation of declaration and assignment in the arrange and act portion of the unit test")]
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

		[Fact]
		[Trait("TestingMember", "Property_RootItems")]
		public static void RootItems_reacts_to_changes()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				MenuBarViewModel viewModel;
				List<MenuBarItemViewModel> expectedValue;
				ReadOnlyObservableCollection<MenuBarItemViewModel> actualValue;

				// Act
				viewModel = new();
				expectedValue = new();
				actualValue = viewModel.RootItems;

				expectedValue.Add(viewModel.ItemTree.CreateRootNode.Execute(new("foo")).Wait().Value);
				TreeNode<MenuBarItemViewModel> itemToDelete = viewModel.ItemTree.CreateRootNode.Execute(new("Delete me")).Wait();
				expectedValue.Add(viewModel.ItemTree.CreateRootNode.Execute(new("bar", "Bar")).Wait().Value);
				itemToDelete.Delete.Execute(false).Wait();
				scheduler.AdvanceBy(2);

				// Assert
				Util.AssertCollectionsHaveSameItems(expectedValue, actualValue);
			});
		}

		[Fact]
		[Trait("TestingMember", "Property_RootItems")]
		public static void RootItems_follows_order()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				MenuBarViewModel viewModel;
				ReadOnlyObservableCollection<MenuBarItemViewModel> actualValue;
				List<MenuBarItemViewModel> rootItems = new()
				{
					new("a", "Node A", GeneralUseData.StubGKCommand, null, 0),
					new("b", "Node B", GeneralUseData.StubGKCommand, null, 0),
					new("c", "Node C", GeneralUseData.StubGKCommand, null, 0),
					new("d", "Node D", GeneralUseData.StubGKCommand, null, 1),
					new("e", "Node E", GeneralUseData.StubGKCommand, null, -10),
					new("f", "Node F", GeneralUseData.StubGKCommand, null, 12345),
					new("g", "Node G", GeneralUseData.StubGKCommand, null, -2),
					new("h", "Node H", GeneralUseData.StubGKCommand, null, -10),
					new("i", "Node I", GeneralUseData.StubGKCommand, null, 0),
					new("j", "Node J", GeneralUseData.StubGKCommand, null, 56)
				};

				// Act
				viewModel = new();
				actualValue = viewModel.RootItems;

				foreach (MenuBarItemViewModel rootItem in rootItems)
					viewModel.ItemTree.CreateRootNode.Execute(rootItem).Wait();
				scheduler.AdvanceBy(2);

				// Assert
				int? previousOrderValue = null;
				foreach (MenuBarItemViewModel rootItem in actualValue)
				{
					Assert.True(previousOrderValue == null || previousOrderValue <= rootItem.Order);
					previousOrderValue = rootItem.Order;
				}
			});
		}

		#endregion
	}
}
