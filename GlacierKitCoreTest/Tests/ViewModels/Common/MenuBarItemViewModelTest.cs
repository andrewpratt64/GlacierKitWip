﻿using DynamicData;
using GlacierKitCore.Commands;
using GlacierKitCore.Models;
using GlacierKitCore.ViewModels.Common;
using GlacierKitTestShared;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.ViewModels.Common
{
	public class MenuBarItemViewModelTest
	{
		#region Theory_data

		private static readonly string[] _DATA_ctorValues = new[]
		{
			"",
			"lowercase",
			"UPPERCASE",
			"CamelCase",
			"I have spaces",
			 "I have spaces... and symbols!!! >:)"
		};

		public static readonly IEnumerable<object[]> _DATA_ctorIdParamValues = new Func<IEnumerable<object[]>>(() =>
		{
			List<object[]> data = new();
			for (int i = 0; i < _DATA_ctorValues.Length; i++)
				data.Add(new object[] { _DATA_ctorValues[i] });
			return data;
		})();

		public static readonly IEnumerable<object[]> _DATA_ctorIdAndTitleParamValues = new Func<IEnumerable<object[]>>(() =>
		{
			List<object[]> data = new();
			for (int i = 0; i < _DATA_ctorValues.Length; i++)
				data.Add(new object[] { _DATA_ctorValues[i], _DATA_ctorValues[^(i + 1)] });
			return data;
		})();

		#endregion


		#region Constructor

		[Theory]
		[MemberData(nameof(_DATA_ctorIdParamValues))]
		[Trait("TestingMember", "Constructor")]
		public static void MenuBarViewModel_ctor_with_id_doesnt_throw(string id)
		{
			Util.AssertCodeDoesNotThrowException(
				() => _ = new MenuBarItemViewModel(id: id)
			);
		}

		[Theory]
		[MemberData(nameof(_DATA_ctorIdAndTitleParamValues))]
		[Trait("TestingMember", "Constructor")]
		public static void MenuBarViewModel_ctor_with_id_and_title_doesnt_throw(string id, string title)
		{
			Util.AssertCodeDoesNotThrowException(
				() => _ = new MenuBarItemViewModel(id: id, title: title)
			);
		}

		[Theory]
		[MemberData(nameof(_DATA_ctorIdAndTitleParamValues))]
		[Trait("TestingMember", "Constructor")]
		public static void MenuBarViewModel_ctor_with_id_title_and_null_command_doesnt_throw(string id, string title)
		{
			Util.AssertCodeDoesNotThrowException(
				() => _ = new MenuBarItemViewModel(id: id, title: title, command: null)
			);
		}

		[Theory]
		[MemberData(nameof(_DATA_ctorIdAndTitleParamValues))]
		[Trait("TestingMember", "Constructor")]
		public static void MenuBarViewModel_ctor_with_id_title_and_not_null_command_doesnt_throw(string id, string title)
		{
			Util.AssertCodeDoesNotThrowException(
				() => _ = new MenuBarItemViewModel(id: id, title: title, command: GeneralUseData.StubGKCommand)
			);
		}

		#endregion


		#region Id

		[Fact]
		[Trait("TestingMember", "Property_Id")]
		public static void Id_is_set_by_ctor_without_title()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			string id = GeneralUseData.SmallString;
			string actualValue;

			// Act
			viewModel = new(id);
			actualValue = viewModel.Id;

			// Assert
			Assert.Equal(id, actualValue);
		}

		#endregion


		#region Title

		[Fact]
		[Trait("TestingMember", "Property_Title")]
		public static void Title_isnt_null_when_ctor_without_title_is_used()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			string id = GeneralUseData.SmallString;
			string? actualValue;

			// Act
			viewModel = new(id);
			actualValue = viewModel.Title;

			// Assert
			Assert.NotNull(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Property_Title")]
		public static void Title_isnt_empty_when_ctor_without_title_is_used()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			string id = GeneralUseData.SmallString;
			string actualValue;

			// Act
			viewModel = new(id);
			actualValue = viewModel.Title;

			// Assert
			Assert.True(actualValue.Length > 0);
		}

		[Fact]
		[Trait("TestingMember", "Property_Title")]
		public static void Title_is_set_by_ctor_with_title()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			string id = GeneralUseData.SmallString;
			string title = GeneralUseData.TinyString;
			string actualValue;

			// Act
			viewModel = new(id, title);
			actualValue = viewModel.Id;

			// Assert
			Assert.Equal(id, actualValue);
		}

		#endregion


		#region Command

		[Fact]
		[Trait("TestingMember", "Property_Command")]
		public static void Command_is_null_when_ctor_without_title_and_without_command_is_used()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			string id = GeneralUseData.SmallString;
			IGKCommand? actualValue;

			// Act
			viewModel = new(id);
			actualValue = viewModel.Command;

			// Assert
			Assert.Null(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Property_Command")]
		public static void Command_is_null_when_ctor_with_title_and_without_command_is_used()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			string id = GeneralUseData.SmallString;
			string title = GeneralUseData.TinyString;
			IGKCommand? actualValue;

			// Act
			viewModel = new(id, title);
			actualValue = viewModel.Command;

			// Assert
			Assert.Null(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Property_Command")]
		public static void Command_is_null_when_ctor_with_title_and_with_null_command_is_used()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			string id = GeneralUseData.SmallString;
			string title = GeneralUseData.TinyString;
			IGKCommand? command = null;
			IGKCommand? actualValue;

			// Act
			viewModel = new(id, title, command);
			actualValue = viewModel.Command;

			// Assert
			Assert.Null(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Property_Command")]
		public static void Command_is_not_null_when_ctor_with_title_and_with_non_null_command_is_used()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			string id = GeneralUseData.SmallString;
			string title = GeneralUseData.TinyString;
			IGKCommand? command = GeneralUseData.StubGKCommand;
			IGKCommand? actualValue;

			// Act
			viewModel = new(id, title, command);
			actualValue = viewModel.Command;

			// Assert
			Assert.NotNull(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Property_Command")]
		public static void Command_is_set_by_ctor()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			string id = GeneralUseData.SmallString;
			string title = GeneralUseData.TinyString;
			IGKCommand? command = GeneralUseData.StubGKCommand;
			IGKCommand? actualValue;

			// Act
			viewModel = new(id, title, command);
			actualValue = viewModel.Command;

			// Assert
			Assert.Same(command, actualValue);
		}

		#endregion


		#region ItemNode

		[Fact]
		[Trait("TestingMember", "Property_ItemNode")]
		public static void ItemNode_is_initially_null()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			string id = GeneralUseData.SmallString;
			string title = GeneralUseData.TinyString;
			TreeNode<MenuBarItemViewModel>? actualValue;

			// Act
			viewModel = new(id, title);
			actualValue = viewModel.ItemNode;

			// Assert
			Assert.Null(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Property_ItemNode")]
		public static void ItemNode_is_not_null_after_creating_root_node_in_MenuBarViewModel()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			MenuBarViewModel menuBarViewModel;
			string id = GeneralUseData.SmallString;
			string title = GeneralUseData.TinyString;
			TreeNode<MenuBarItemViewModel>? actualValue;

			// Act
			viewModel = new(id, title);
			menuBarViewModel = new();
			_ = menuBarViewModel.ItemTree.CreateRootNode.Execute(viewModel).Wait();
			actualValue = viewModel.ItemNode;

			// Assert
			Assert.NotNull(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Property_ItemNode")]
		public static void ItemNode_is_not_null_after_creating_non_root_node_in_MenuBarViewModel()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			MenuBarViewModel menuBarViewModel;
			string id = GeneralUseData.SmallString;
			string title = GeneralUseData.TinyString;
			TreeNode<MenuBarItemViewModel>? actualValue;

			// Act
			viewModel = new(id, title);
			menuBarViewModel = new();
			TreeNode<MenuBarItemViewModel> rootItem = menuBarViewModel.ItemTree.CreateRootNode.Execute(new("root", "Root")).Wait();
			_ = rootItem.AddChild.Execute(viewModel).Wait();
			actualValue = viewModel.ItemNode;

			// Assert
			Assert.NotNull(actualValue);
		}

		#endregion


		#region ChildItems

		[Fact]
		[Trait("TestingMember", "Property_ChildItems")]
		public static void ChildItems_without_a_tree_node_is_initially_null()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			string id = GeneralUseData.SmallString;
			string title = GeneralUseData.TinyString;
			ReadOnlyObservableCollection<MenuBarItemViewModel>? actualValue;

			// Act
			viewModel = new(id, title);
			actualValue = viewModel.ChildItems;

			// Assert
			Assert.Null(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Property_ChildItems")]
		public static void ChildItems_with_a_tree_node_is_not_initially_null()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			MenuBarViewModel menuBarViewModel;
			string id = GeneralUseData.SmallString;
			string title = GeneralUseData.TinyString;
			ReadOnlyObservableCollection<MenuBarItemViewModel>? actualValue;

			// Act
			viewModel = new(id, title);
			menuBarViewModel = new();
			_ = menuBarViewModel.ItemTree.CreateRootNode.Execute(viewModel).Wait();
			actualValue = viewModel.ChildItems;

			// Assert
			Assert.NotNull(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Property_ChildItems")]
		public static void ChildItems_is_initially_empty()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			MenuBarViewModel menuBarViewModel;
			string id = GeneralUseData.SmallString;
			string title = GeneralUseData.TinyString;
			ReadOnlyObservableCollection<MenuBarItemViewModel> actualValue;

			// Act
			viewModel = new(id, title);
			menuBarViewModel = new();
			_ = menuBarViewModel.ItemTree.CreateRootNode.Execute(viewModel).Wait();
			actualValue = viewModel.ChildItems!;

			// Assert
			Assert.Empty(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Property_ChildItems")]
		public static void ChildItems_reacts_to_changes()
		{
			// Arrange
			MenuBarItemViewModel viewModel;
			MenuBarViewModel menuBarViewModel;
			string id = GeneralUseData.SmallString;
			string title = GeneralUseData.TinyString;
			List<MenuBarItemViewModel> expectedValue;
			ReadOnlyObservableCollection<MenuBarItemViewModel>? actualValue;

			// Act
			viewModel = new(id, title);
			menuBarViewModel = new();
			_ = menuBarViewModel.ItemTree.CreateRootNode.Execute(new("other_root")).Wait();
			TreeNode<MenuBarItemViewModel> root = menuBarViewModel.ItemTree.CreateRootNode.Execute(new("root")).Wait();
			TreeNode<MenuBarItemViewModel> node = root.AddChild.Execute(viewModel).Wait();
			_ = root.AddChild.Execute(new("sibling")).Wait();
			TreeNode<MenuBarItemViewModel> nodeToDelete = node.AddChild.Execute(new("delete_me")).Wait();
			TreeNode<MenuBarItemViewModel>[] directChildren = new TreeNode<MenuBarItemViewModel>[3]
			{
				node.AddChild.Execute(new("first_direct_child")).Wait(),
				node.AddChild.Execute(new("second_direct_child")).Wait(),
				node.AddChild.Execute(new("third_direct_child")).Wait(),
			};
			_ = directChildren[1].AddChild.Execute(new("indirect_child")).Wait();
			_ = nodeToDelete.Delete.Execute(false).Wait();
			expectedValue = new()
			{
				directChildren[0].Value,
				directChildren[1].Value,
				directChildren[2].Value
			};
			actualValue = viewModel.ChildItems!;

			// Assert
			Util.AssertCollectionsHaveSameItems(expectedValue, actualValue);
		}

		#endregion
	}
}