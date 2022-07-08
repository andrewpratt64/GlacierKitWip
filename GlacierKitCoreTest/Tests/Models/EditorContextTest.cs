using DynamicData;
using GlacierKitCore.Commands;
using GlacierKitCore.Models;
using GlacierKitCore.Services;
using GlacierKitCore.ViewModels.Common;
using GlacierKitTestShared;
using Microsoft.Reactive.Testing;
using PlaceholderModule.ViewModels;
using ReactiveUI;
using ReactiveUI.Testing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.Models
{
	public class EditorContextTest
	{
		#region Theory_data

		private static readonly Type? _DATA_ValidInputForCreateEditorWindow = typeof(FooViewModel);
		private static readonly Type? _DATA_InvalidInputForCreateEditorWindow = typeof(BarViewModel);
		private static readonly string _DATA_ValidIdForGetCommand = "PlaceholderModule_PrintHi";
		private static readonly string _DATA_InvalidIdForGetCommand = "IDontExist";

		private class ContextualItem : IContextualItem
		{
			public EditorContext Ctx { get; }
			public string Name { get; set; }

			public ContextualItem(EditorContext ctx, string name)
			{
				Ctx = ctx;
				Name = name;
			}
		}

		#endregion


		#region Constructor

		[Fact]
		[Trait("TestingMember", "Constructor")]
		public static void Default_ctor_works()
		{
			Util.AssertDefaultCtorWorks<EditorContext>();
		}

		#endregion

		
		#region ModuleLoader

		[Fact]
		[Trait("TestingMember", "Property_ModuleLoader")]
		public static void ModuleLoader_not_null()
		{
			// Arrange
			EditorContext? ctx;
			GKModuleLoaderService? actualValue;

			// Act
			ctx = new();
			actualValue = ctx.ModuleLoader;

			// Assert
			Assert.NotNull(actualValue);
		}

		#endregion


		#region MainMenuBar

		[Fact]
		[Trait("TestingMember", "Property_MainMenuBar")]
		public static void MainMenuBar_not_null()
		{
			// Arrange
			EditorContext? ctx;
			MenuBarViewModel? actualValue;

			// Act
			ctx = new();
			actualValue = ctx.MainMenuBar;

			// Assert
			Assert.NotNull(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Property_MainMenuBar")]
		public static void MainMenuBar_initially_has_no_nodes()
		{
			// Arrange
			EditorContext? ctx;
#pragma warning disable IDE0018 // Inline variable declaration
			ReadOnlyObservableCollection<TreeNode<MenuBarItemViewModel>> actualValue;
#pragma warning restore IDE0018 // Inline variable declaration
			IDisposable disposable;

			// Act
			ctx = new();
			disposable = ctx.MainMenuBar.ItemTree
				.ConnectToNodes()
				.Bind(out actualValue)
				.Subscribe();

			// Assert
			Assert.Empty(actualValue);

			// Cleanup
			disposable.Dispose();
		}

		[Fact]
		[Trait("TestingMember", "Property_MainMenuBar")]
		public static void MainMenuBar_has_nodes_after_loading_modules()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				EditorContext? ctx;
#pragma warning disable IDE0018 // Inline variable declaration
				ReadOnlyObservableCollection<TreeNode<MenuBarItemViewModel>> actualValue;
#pragma warning restore IDE0018 // Inline variable declaration
				IDisposable disposable;

				// Act
				ctx = new();
				disposable = ctx.MainMenuBar.ItemTree
					.ConnectToNodes()
					.Bind(out actualValue)
					.Subscribe();
				ctx.ModuleLoader.LoadModules();

				scheduler.AdvanceBy(2);

				// Assert
				Assert.NotEmpty(actualValue);

				// Cleanup
				disposable.Dispose();
			});
		}

		#endregion


		#region FocusedItem

		[Fact]
		[Trait("TestingMember", "Property_FocusedItem")]
		public static void FocusedItem_is_initially_null ()
		{
			// Arrange
			EditorContext ctx;
			object? actualValue;

			// Act
			ctx = new();
			actualValue = ctx.FocusedItem;

			// Assert
			Assert.Null(actualValue);
		}

		#endregion


		#region CreateEditorWindow

		[Fact]
		[Trait("TestingMember", "Command_CreateEditorWindow")]
		public static void CreateEditorWindow_isnt_null()
		{
			// Arrange
			EditorContext? ctx;
			object? actualValue;

			// Act
			ctx = new();
			actualValue = ctx.CreateEditorWindow;

			// Assert
			Assert.NotNull(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Command_CreateEditorWindow")]
		public static void CreateEditorWindow_with_invalid_type_does_not_throw()
		{
			// Arrange
			EditorContext? ctx;
			Type? input = _DATA_InvalidInputForCreateEditorWindow;

			// Act
			ctx = new();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() =>
				ctx.CreateEditorWindow
				.Execute(input)
				.Subscribe()
				.Dispose()
			);
		}

		[Fact]
		[Trait("TestingMember", "Command_CreateEditorWindow")]
		public static void CreateEditorWindow_with_invalid_type_returns_null()
		{
			// Arrange
			EditorContext? ctx;
			Type? input = _DATA_InvalidInputForCreateEditorWindow;
			Type? expectedValue = null;
			Type? actualValue;
			IObservable<Type?> commandObservable;
			IDisposable commandDisposable;

			// Act
			ctx = new();
			commandObservable = ctx.CreateEditorWindow.Execute(input).ObserveOn(RxApp.MainThreadScheduler);
			commandDisposable = commandObservable.Subscribe();
			actualValue = commandObservable.Wait();


			// Assert
			Assert.Equal(expectedValue, actualValue);
			commandDisposable.Dispose();
		}

		[Fact]
		[Trait("TestingMember", "Command_CreateEditorWindow")]
		public static void CreateEditorWindow_with_null_type_does_not_throw()
		{
			// Arrange
			EditorContext? ctx;
			Type? input = null;

			// Act
			ctx = new();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() =>
				ctx.CreateEditorWindow
				.Execute(input)
				.Subscribe()
				.Dispose()
			);
		}

		[Fact]
		[Trait("TestingMember", "Command_CreateEditorWindow")]
		public static void CreateEditorWindow_with_null_type_returns_null()
		{
			// Arrange
			EditorContext? ctx;
			Type? input = null;
			Type? expectedValue = null;
			Type? actualValue;
			IObservable<Type?> commandObservable;
			IDisposable commandDisposable;

			// Act
			ctx = new();
			commandObservable = ctx.CreateEditorWindow.Execute(input).ObserveOn(RxApp.MainThreadScheduler);
			commandDisposable = commandObservable.Subscribe();
			actualValue = commandObservable.Wait();


			// Assert
			Assert.Equal(expectedValue, actualValue);
			commandDisposable.Dispose();
		}

		[Fact]
		[Trait("TestingMember", "Command_CreateEditorWindow")]
		public static void CreateEditorWindow_with_valid_type_does_not_throw()
		{
			// Arrange
			EditorContext? ctx;
			Type? input = _DATA_ValidInputForCreateEditorWindow;

			// Act
			ctx = new();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() =>
				ctx.CreateEditorWindow
				.Execute(input)
				.Subscribe()
				.Dispose()
			);
		}

		[Fact]
		[Trait("TestingMember", "Command_CreateEditorWindow")]
		public static void CreateEditorWindow_with_valid_type_returns_same_type_as_input()
		{
			// Arrange
			EditorContext? ctx;
			Type? input = _DATA_ValidInputForCreateEditorWindow;
			Type? expectedValue = input;
			Type? actualValue;
			IObservable<Type?> commandObservable;
			IDisposable commandDisposable;

			// Act
			ctx = new();
			commandObservable = ctx.CreateEditorWindow.Execute(input).ObserveOn(RxApp.MainThreadScheduler);
			commandDisposable = commandObservable.Subscribe();
			actualValue = commandObservable.Wait();


			// Assert
			Assert.Equal(expectedValue, actualValue);
			commandDisposable.Dispose();
		}

		#endregion


		#region AddItem

		[Fact]
		[Trait("TestingMember", "Command_AddItem")]
		public static void AddItem_isnt_null()
		{
			// Arrange
			EditorContext? ctx;
			object? actualValue;

			// Act
			ctx = new();
			actualValue = ctx.AddItem;

			// Assert
			Assert.NotNull(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Command_AddItem")]
		public static void First_execution_of_AddItem_doesnt_throw()
		{
			// Arrange
			EditorContext? ctx;
			IContextualItem firstItem;

			// Act
			ctx = new();
			firstItem = new ContextualItem(ctx, "Foo");

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() =>
				_ = ctx.AddItem
				.Execute(firstItem)
				.Wait()
			);
		}

		[Fact]
		[Trait("TestingMember", "Command_AddItem")]
		public static void First_execution_of_AddItem_returns_true()
		{
			// Arrange
			EditorContext? ctx;
			IContextualItem firstItem;
			bool returnValue;

			// Act
			ctx = new();
			firstItem = new ContextualItem(ctx, "Foo");
			returnValue = ctx.AddItem.Execute(firstItem).Wait();

			// Assert
			Assert.True(returnValue);
		}

		[Fact]
		[Trait("TestingMember", "Command_AddItem")]
		public static void AddItem_with_unique_item_when_other_items_exist_doesnt_throw()
		{
			// Arrange
			EditorContext? ctx;
			IContextualItem firstItem, secondItem;

			// Act
			ctx = new();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			Debug.Assert(
				firstItem != secondItem,
				$"Can't finish unit test; {nameof(firstItem)} and {nameof(secondItem)} are supposed to be two seperate instances."
			);
			_ = ctx.AddItem.Execute(firstItem).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() =>
				_ = ctx.AddItem
				.Execute(secondItem)
				.Wait()
			);
		}

		[Fact]
		[Trait("TestingMember", "Command_AddItem")]
		public static void Items_has_one_item_after_first_execution_of_AddItem()
		{
			// Arrange
			EditorContext? ctx;
#pragma warning disable IDE0018 // Inline variable declaration
			ReadOnlyObservableCollection<IContextualItem> items;
#pragma warning restore IDE0018 // Inline variable declaration
			IDisposable ctxItemsSubscription;
			IContextualItem firstItem;
			int expectedSize = 1;
			int actualSize;

			// Act
			ctx = new();
			ctxItemsSubscription = ctx.ConnectToItems().Bind(out items).Subscribe();
			firstItem = new ContextualItem(ctx, "Foo");
			_ = ctx.AddItem.Execute(firstItem).Wait();
			actualSize = items.Count;

			// Assert
			Assert.Equal(expectedSize, actualSize);

			// Cleanup
			ctxItemsSubscription.Dispose();
		}

		[Fact]
		[Trait("TestingMember", "Command_AddItem")]
		public static void Correct_instance_is_added_to_Items_after_first_execution_of_AddItem()
		{
			// Arrange
			EditorContext? ctx;
#pragma warning disable IDE0018 // Inline variable declaration
			ReadOnlyObservableCollection<IContextualItem> items;
#pragma warning restore IDE0018 // Inline variable declaration
			IDisposable ctxItemsSubscription;
			IContextualItem firstItem;
			IContextualItem? expected;
			IContextualItem? actual;

			// Act
			ctx = new();
			ctxItemsSubscription = ctx.ConnectToItems().Bind(out items).Subscribe();
			firstItem = new ContextualItem(ctx, "Foo");
			expected = firstItem;
			_ = ctx.AddItem.Execute(firstItem).Wait();
			actual = items.FirstOrDefault();

			// Assert
			Assert.Equal(expected, actual);

			// Cleanup
			ctxItemsSubscription.Dispose();
		}

		[Fact]
		[Trait("TestingMember", "Command_AddItem")]
		public static void AddItem_with_unique_item_when_other_items_exist_returns_true()
		{
			// Arrange
			EditorContext? ctx;
			IContextualItem firstItem, secondItem;
			bool returnValue;

			// Act
			ctx = new();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			Debug.Assert(
				firstItem != secondItem,
				$"Can't finish unit test; {nameof(firstItem)} and {nameof(secondItem)} are supposed to be two seperate instances."
			);
			_ = ctx.AddItem.Execute(firstItem).Wait();
			returnValue = ctx.AddItem.Execute(secondItem).Wait();

			// Assert
			Assert.True(returnValue);
		}

		[Fact]
		[Trait("TestingMember", "Command_AddItem")]
		public static void Size_of_Items_increments_after_executing_AddItem_with_unique_item_when_other_items_exist()
		{
			// Arrange
			EditorContext? ctx;
#pragma warning disable IDE0018 // Inline variable declaration
			ReadOnlyObservableCollection<IContextualItem> items;
#pragma warning restore IDE0018 // Inline variable declaration
			IDisposable ctxItemsSubscription;
			IContextualItem firstItem, secondItem;
			int expected;
			int actual;

			// Act
			ctx = new();
			ctxItemsSubscription = ctx.ConnectToItems().Bind(out items).Subscribe();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			Debug.Assert(
				firstItem != secondItem,
				$"Can't finish unit test; {nameof(firstItem)} and {nameof(secondItem)} are supposed to be two seperate instances."
			);
			_ = ctx.AddItem.Execute(firstItem).Wait();

			expected = items.Count + 1;
			_ = ctx.AddItem.Execute(secondItem).Wait();
			actual = items.Count;

			// Assert
			Assert.Equal(expected, actual);

			// Cleanup
			ctxItemsSubscription.Dispose();
		}

		[Fact]
		[Trait("TestingMember", "Command_AddItem")]
		public static void Expected_item_exists_in_Items_after_executing_AddItem_with_unique_item_when_other_items_exist()
		{
			// Arrange
			EditorContext? ctx;
#pragma warning disable IDE0018 // Inline variable declaration
			ReadOnlyObservableCollection<IContextualItem> items;
#pragma warning restore IDE0018 // Inline variable declaration
			IDisposable ctxItemsSubscription;
			IContextualItem firstItem, secondItem;
			IContextualItem expected;

			// Act
			ctx = new();
			ctxItemsSubscription = ctx.ConnectToItems().Bind(out items).Subscribe();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			Debug.Assert(
				firstItem != secondItem,
				$"Can't finish unit test; {nameof(firstItem)} and {nameof(secondItem)} are supposed to be two seperate instances."
			);
			_ = ctx.AddItem.Execute(firstItem).Wait();

			expected = secondItem;
			_ = ctx.AddItem.Execute(secondItem).Wait();

			// Assert
			Assert.Contains(expected, items);

			// Cleanup
			ctxItemsSubscription.Dispose();
		}

		[Fact]
		[Trait("TestingMember", "Command_AddItem")]
		public static void AddItem_with_already_existing_item_doesnt_throw()
		{
			// Arrange
			EditorContext? ctx;
			IContextualItem firstItem, secondItem;

			// Act
			ctx = new();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			Debug.Assert(
				firstItem != secondItem,
				$"Can't finish unit test; {nameof(firstItem)} and {nameof(secondItem)} are supposed to be two seperate instances."
			);
			_ = ctx.AddItem.Execute(firstItem).Wait();
			_ = ctx.AddItem.Execute(secondItem).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() =>
				_ = ctx.AddItem
				.Execute(secondItem)
				.Wait()
			);
		}

		[Fact]
		[Trait("TestingMember", "Command_AddItem")]
		public static void AddItem_with_already_existing_item_returns_false()
		{
			// Arrange
			EditorContext? ctx;
			IContextualItem firstItem, secondItem;
			bool returnValue;

			// Act
			ctx = new();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			Debug.Assert(
				firstItem != secondItem,
				$"Can't finish unit test; {nameof(firstItem)} and {nameof(secondItem)} are supposed to be two seperate instances."
			);
			_ = ctx.AddItem.Execute(firstItem).Wait();
			_ = ctx.AddItem.Execute(secondItem).Wait();
			returnValue = ctx.AddItem.Execute(secondItem).Wait();

			// Assert
			Assert.False(returnValue);
		}

		[Fact]
		[Trait("TestingMember", "Command_AddItem")]
		public static void Size_of_Items_is_unchanged_after_executing_AddItem_with_already_existing_item()
		{
			// Arrange
			EditorContext? ctx;
#pragma warning disable IDE0018 // Inline variable declaration
			ReadOnlyObservableCollection<IContextualItem> items;
#pragma warning restore IDE0018 // Inline variable declaration
			IDisposable ctxItemsSubscription;
			IContextualItem firstItem, secondItem;
			int expected;
			int actual;

			// Act
			ctx = new();
			ctxItemsSubscription = ctx.ConnectToItems().Bind(out items).Subscribe();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			Debug.Assert(
				firstItem != secondItem,
				$"Can't finish unit test; {nameof(firstItem)} and {nameof(secondItem)} are supposed to be two seperate instances."
			);
			_ = ctx.AddItem.Execute(firstItem).Wait();
			_ = ctx.AddItem.Execute(secondItem).Wait();

			expected = items.Count;
			_ = ctx.AddItem.Execute(secondItem).Wait();
			actual = items.Count;

			// Assert
			Assert.Equal(expected, actual);

			// Cleanup
			ctxItemsSubscription.Dispose();
		}

		#endregion


		#region RemoveItem

		[Fact]
		[Trait("TestingMember", "Command_RemoveItem")]
		public static void RemoveItem_isnt_null()
		{
			// Arrange
			EditorContext? ctx;
			object? actualValue;

			// Act
			ctx = new();
			actualValue = ctx.RemoveItem;

			// Assert
			Assert.NotNull(actualValue);
		}

		[Fact]
		[Trait("TestingMember", "Command_RemoveItem")]
		public static void RemoveItem_with_missing_item_doesnt_throw()
		{
			// Arrange
			EditorContext? ctx;
			IContextualItem firstItem, secondItem, itemToRemove;

			// Act
			ctx = new();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			itemToRemove = new ContextualItem(ctx, "Missing");
			_ = ctx.AddItem.Execute(firstItem).Wait();
			_ = ctx.AddItem.Execute(secondItem).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() =>
				_ = ctx.RemoveItem
				.Execute(itemToRemove)
				.Wait()
			);
		}

		[Fact]
		[Trait("TestingMember", "Command_RemoveItem")]
		public static void RemoveItem_with_missing_item_returns_false()
		{
			// Arrange
			EditorContext? ctx;
			IContextualItem firstItem, secondItem, itemToRemove;
			bool returnValue;

			// Act
			ctx = new();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			itemToRemove = new ContextualItem(ctx, "Missing");
			_ = ctx.AddItem.Execute(firstItem).Wait();
			_ = ctx.AddItem.Execute(secondItem).Wait();
			returnValue = ctx.RemoveItem.Execute(itemToRemove).Wait();

			// Assert
			Assert.False(returnValue);
		}

		[Fact]
		[Trait("TestingMember", "Command_RemoveItem")]
		public static void Size_of_Items_is_unchanged_after_executing_RemoveItem_with_missing_item()
		{
			// Arrange
			EditorContext? ctx;
#pragma warning disable IDE0018 // Inline variable declaration
			ReadOnlyObservableCollection<IContextualItem> items;
#pragma warning restore IDE0018 // Inline variable declaration
			IDisposable ctxItemsSubscription;
			IContextualItem firstItem, secondItem, itemToRemove;
			int expected;
			int actual;

			// Act
			ctx = new();
			ctxItemsSubscription = ctx.ConnectToItems().Bind(out items).Subscribe();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			itemToRemove = new ContextualItem(ctx, "Missing");
			_ = ctx.AddItem.Execute(firstItem).Wait();
			_ = ctx.AddItem.Execute(secondItem).Wait();

			expected = items.Count;
			_ = ctx.RemoveItem.Execute(itemToRemove).Wait();
			actual = items.Count;

			// Assert
			Assert.Equal(expected, actual);

			// Cleanup
			ctxItemsSubscription.Dispose();
		}

		[Fact]
		[Trait("TestingMember", "Command_RemoveItem")]
		public static void RemoveItem_with_existing_item_doesnt_throw()
		{
			// Arrange
			EditorContext? ctx;
			IContextualItem firstItem, secondItem, itemToRemove;

			// Act
			ctx = new();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			itemToRemove = secondItem;
			_ = ctx.AddItem.Execute(firstItem).Wait();
			_ = ctx.AddItem.Execute(secondItem).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() =>
				_ = ctx.RemoveItem
				.Execute(itemToRemove)
				.Wait()
			);
		}

		[Fact]
		[Trait("TestingMember", "Command_RemoveItem")]
		public static void RemoveItem_with_existing_item_returns_true()
		{
			// Arrange
			EditorContext? ctx;
			IContextualItem firstItem, secondItem, itemToRemove;
			bool returnValue;

			// Act
			ctx = new();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			itemToRemove = secondItem;
			_ = ctx.AddItem.Execute(firstItem).Wait();
			_ = ctx.AddItem.Execute(secondItem).Wait();
			returnValue = ctx.RemoveItem.Execute(itemToRemove).Wait();

			// Assert
			Assert.True(returnValue);
		}

		[Fact]
		[Trait("TestingMember", "Command_RemoveItem")]
		public static void Size_of_Items_decrements_after_executing_RemoveItem_with_existing_item()
		{
			// Arrange
			EditorContext? ctx;
#pragma warning disable IDE0018 // Inline variable declaration
			ReadOnlyObservableCollection<IContextualItem> items;
#pragma warning restore IDE0018 // Inline variable declaration
			IDisposable ctxItemsSubscription;
			IContextualItem firstItem, secondItem, itemToRemove;
			int expected;
			int actual;

			// Act
			ctx = new();
			ctxItemsSubscription = ctx.ConnectToItems().Bind(out items).Subscribe();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			itemToRemove = secondItem;
			_ = ctx.AddItem.Execute(firstItem).Wait();
			_ = ctx.AddItem.Execute(secondItem).Wait();

			expected = items.Count - 1;
			_ = ctx.RemoveItem.Execute(itemToRemove).Wait();
			actual = items.Count;

			// Assert
			Assert.Equal(expected, actual);

			// Cleanup
			ctxItemsSubscription.Dispose();
		}

		[Fact]
		[Trait("TestingMember", "Command_RemoveItem")]
		public static void RemoveItem_with_existing_unfocused_item_doesnt_affect_FocusedItem()
		{
			// Arrange
			EditorContext? ctx;
			IContextualItem firstItem, secondItem, itemToRemove;
			IContextualItem? expected;
			IContextualItem? actual;

			// Act
			ctx = new();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			itemToRemove = secondItem;
			_ = ctx.AddItem.Execute(firstItem).Wait();
			_ = ctx.AddItem.Execute(secondItem).Wait();
			ctx.FocusedItem = firstItem;

			expected = ctx.FocusedItem;
			_ = ctx.RemoveItem.Execute(itemToRemove).Wait();
			actual = ctx.FocusedItem;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Fact]
		[Trait("TestingMember", "Command_RemoveItem")]
		public static void FocusedItem_is_null_after_executing_RemoveItem_with_existing_focused_item()
		{
			// Arrange
			EditorContext? ctx;
			IContextualItem firstItem, secondItem, itemToRemove;
			IContextualItem? result;

			// Act
			ctx = new();
			firstItem = new ContextualItem(ctx, "Foo");
			secondItem = new ContextualItem(ctx, "Bar");
			itemToRemove = secondItem;
			_ = ctx.AddItem.Execute(firstItem).Wait();
			_ = ctx.AddItem.Execute(secondItem).Wait();
			ctx.FocusedItem = itemToRemove;

			_ = ctx.RemoveItem.Execute(itemToRemove).Wait();
			result = ctx.FocusedItem;

			// Assert
			Assert.Null(result);
		}

		#endregion


		#region Items

		[Fact]
		[Trait("TestingMember", "Method_ConnectToItems")]
		public static void Calling_ConnectToItems_doesnt_throw()
		{
			// Arrange
			EditorContext ctx = new();

			// Act / Assert
			Util.AssertCodeDoesNotThrowException(
				() => _ = ctx.ConnectToItems()
			);
		}

		[Fact]
		[Trait("TestingMember", "Method_ConnectToItems")]
		public static void Calling_ConnectToItems_doesnt_return_null()
		{
			// Arrange
			EditorContext ctx;
			object? actual;

			// Act
			ctx = new();
			actual = ctx.ConnectToItems();

			// Assert
			Assert.NotNull(actual);
		}

		#endregion


		#region GetCommand

		[Fact]
		[Trait("TestingMember", "Method_CreateEditorWindow")]
		public static void GetCommand_with_valid_id_before_loading_returns_null()
		{
			// Arrange
			EditorContext? ctx;
			string commandId = _DATA_ValidIdForGetCommand;
			GKCommand<Unit, Unit>? returnValue;

			// Act
			ctx = new();
			returnValue = ctx.GetCommand<Unit, Unit>(commandId);

			// Assert
			Assert.Null(returnValue);
		}

		[Fact]
		[Trait("TestingMember", "Method_CreateEditorWindow")]
		public static void GetCommand_with_invalid_id_before_loading_returns_null()
		{
			// Arrange
			EditorContext? ctx;
			string commandId = _DATA_InvalidIdForGetCommand;
			GKCommand<Unit, Unit>? returnValue;

			// Act
			ctx = new();
			returnValue = ctx.GetCommand<Unit, Unit>(commandId);

			// Assert
			Assert.Null(returnValue);
		}

		[Fact]
		[Trait("TestingMember", "Method_CreateEditorWindow")]
		public static void GetCommand_with_valid_id_after_loading_dosent_return_null()
		{
			// Arrange
			EditorContext? ctx;
			string commandId = _DATA_ValidIdForGetCommand;
			GKCommand<Unit, Unit>? returnValue;

			// Act
			ctx = new();
			ctx.ModuleLoader.LoadModules();
			returnValue = ctx.GetCommand<Unit, Unit>(commandId);

			// Assert
			Assert.NotNull(returnValue);
		}

		[Fact]
		[Trait("TestingMember", "Method_CreateEditorWindow")]
		public static void GetCommand_with_invalid_id_after_loading_returns_null()
		{
			// Arrange
			EditorContext? ctx;
			string commandId = _DATA_InvalidIdForGetCommand;
			GKCommand<Unit, Unit>? returnValue;

			// Act
			ctx = new();
			ctx.ModuleLoader.LoadModules();
			returnValue = ctx.GetCommand<Unit, Unit>(commandId);

			// Assert
			Assert.Null(returnValue);
		}

		[Fact]
		[Trait("TestingMember", "Method_CreateEditorWindow")]
		public static void Executing_command_with_assumed_value_from_GetCommand_does_not_throw()
		{
			// Arrange
			EditorContext? ctx;
			string commandId = _DATA_ValidIdForGetCommand;
			GKCommand<Unit, Unit>? returnValue;

			// Act
			ctx = new();
			ctx.ModuleLoader.LoadModules();
			returnValue = ctx.GetCommand<Unit, Unit>(commandId);

			// Assert
			Util.AssertCodeDoesNotThrowException(() =>
			{
				returnValue!.Command.Execute().Subscribe();
			});
		}

		#endregion
	}
}
