using Avalonia.Data;
using DynamicData;
using GlacierKitCore.Models;
using GlacierKitCore.Utility;
using GlacierKitTestShared;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.Models
{
	[SuppressMessage("Style", "IDE0018:Variable declaration can be inlined", Justification = "Test code often intentionally seperates declaration (Arrange) and assignment (Act)")]
	public class TreeNodeTest
	{
		#region Private_utility_methods

		private static bool IsNodesPathToRootNotEqualTo(TreeNode<object> node, IEnumerable<TreeNode<object>> compareTo)
		{
			if (node.PathToRoot.Count() != compareTo.Count())
				return true;

			for (int i = 0; i < compareTo.Count(); i++)
			{
				if (node.PathToRoot.ElementAt(i) != compareTo.ElementAt(i))
					return true;
			}
			return false;
		}

		private static bool DidPathToRootChangeWhenReparenting(Tree<object> tree)
		{

			// Create nodes and get their initial values for PathToRoot
			TreeNode<object> root = tree.CreateRootNode.Execute("Root").Wait();
			IEnumerable<TreeNode<object>> rootValueBefore = root.PathToRoot;

			TreeNode<object> initialParent = root.AddChild.Execute("Initial parent").Wait();
			IEnumerable<TreeNode<object>> initialParentValueBefore = initialParent.PathToRoot;

			TreeNode<object> nodeToReparent = initialParent.AddChild.Execute("Node").Wait();
			IEnumerable<TreeNode<object>> nodeToReparentValueBefore = nodeToReparent.PathToRoot;

			TreeNode<object> nodeToReparentTo = root.AddChild.Execute("Future Parent").Wait();
			IEnumerable<TreeNode<object>> nodeToReparentToValueBefore = nodeToReparentTo.PathToRoot;

			// Reparent a node
			nodeToReparent.DesiredParent.LastValue = nodeToReparentTo;
			nodeToReparent.Reparent.Execute().Wait();

			// Compare the new PathToRoot values with the old ones and return true if any changed
			return
				   IsNodesPathToRootNotEqualTo(root, rootValueBefore)
				|| IsNodesPathToRootNotEqualTo(initialParent, initialParentValueBefore)
				|| IsNodesPathToRootNotEqualTo(nodeToReparent, nodeToReparentValueBefore)
				|| IsNodesPathToRootNotEqualTo(nodeToReparentTo, nodeToReparentToValueBefore);
		}

		#endregion


		#region Theory_data

		public class DeleteTheoryData : TheoryData<Func<Tree<object>>, bool>
		{
			public DeleteTheoryData()
			{
				Add(() => new SingleRootTree<object>(), false);
				Add(() => new SingleRootTree<object>(), true);
				Add(() => new MultiRootTree<object>(), false);
				Add(() => new MultiRootTree<object>(), true);
			}
		}


		public class Value_test_TheoryData : TheoryData<Func<Tree<object?>>, object?>
		{
			private void AddWithValue(object? v)
			{
				Add(() => new SingleRootTree<object?>(), v);
				Add(() => new MultiRootTree<object?>(), v);
			}

			public Value_test_TheoryData()
			{

				AddWithValue(null);
				AddWithValue(string.Empty);
				AddWithValue(GeneralUseData.TinyString);
				AddWithValue(GeneralUseData.LongString);
				AddWithValue(GeneralUseData.SmallPositiveInt);
				AddWithValue(GeneralUseData.LargeNegativeInt);
			}
		}


		public static readonly TreeTheoryData<object> TreeTheoryData = new();
		public static readonly DeleteTheoryData DeleteTheoryDataValue = new();
		public static readonly Value_test_TheoryData Value_test_TheoryDataValue = new();

		#endregion


		#region Theory_data_tests


		private static void TheoryData_provides_unique_instances_on_each_invoke(Func<object> treeSource)
		{
			// Arrange
			List<object> returnedValues = new();
			int timesToInvoke = 3;
			int expectedDistinctReturnedValues = timesToInvoke;
			int actualDistinctReturnedValues;

			// Act
			for (int i = 0; i < timesToInvoke; i++)
				returnedValues.Add(treeSource());
			actualDistinctReturnedValues = returnedValues.Distinct().Count();

			// Assert
			Assert.Equal(expectedDistinctReturnedValues, actualDistinctReturnedValues);
		}

#pragma warning disable xUnit1026 // Theory methods should use all of their parameters

		[Theory]
		[MemberData(nameof(DeleteTheoryDataValue))]
		public static void DeleteTheoryData_Provides_unique_instances_on_each_invoke(Func<Tree<object>> treeSource, bool _)
		{
			TheoryData_provides_unique_instances_on_each_invoke(treeSource);
		}

		[Theory]
		[MemberData(nameof(Value_test_TheoryDataValue))]
		public static void Value_test_TheoryData_Provides_unique_instances_on_each_invoke(Func<Tree<object?>> treeSource, object? _)
		{
			TheoryData_provides_unique_instances_on_each_invoke(treeSource);
		}

#pragma warning restore xUnit1026 // Theory methods should use all of their parameters

		#endregion


		#region ConnectToChildNodes

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Calling_ConnectToChildNodes_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.ConnectToChildNodes()
			);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void ConnectToChildNodes_doesnt_return_null(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Assert.NotNull(node.ConnectToChildNodes());
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void ConnectToChildNodes_return_value_contains_pre_existing_nodes(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			List<TreeNode<object>> preExistingNodes = new();
			IObservable<IChangeSet<TreeNode<object>>> returnValue;
			ReadOnlyObservableCollection<TreeNode<object>> returnValueAsCollection;
			IDisposable disposable;
			const int childNodesToCreate = 3;
			Assert.True(childNodesToCreate > 0);

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			preExistingNodes.Add(node);
			for (int i = 0; i < childNodesToCreate; i++)
				preExistingNodes.Add(node.AddChild.Execute(GeneralUseData.SmallInt).Wait());

			returnValue = tree.ConnectToNodes();
			disposable = returnValue.Bind(out returnValueAsCollection).Subscribe();

			// Assert
			Util.AssertCollectionsHaveSameItems(preExistingNodes, returnValueAsCollection);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void ConnectToChildNodes_return_value_reflects_future_added_nodes(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			List<TreeNode<object>> nodes = new();
			IObservable<IChangeSet<TreeNode<object>>> returnValue;
			ReadOnlyObservableCollection<TreeNode<object>> returnValueAsCollection;
			IDisposable disposable;
			const int childNodesToCreateBeforeBinding = 3;
			const int childNodesToCreateAfterBinding = 2;
			Assert.True(childNodesToCreateBeforeBinding > 0);
			Assert.True(childNodesToCreateAfterBinding > 0);

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			for (int i = 0; i < childNodesToCreateBeforeBinding; i++)
				nodes.Add(node.AddChild.Execute(GeneralUseData.SmallInt).Wait());

			returnValue = node.ConnectToChildNodes();
			disposable = returnValue.Bind(out returnValueAsCollection).Subscribe();

			for (int i = 0; i < childNodesToCreateAfterBinding; i++)
				nodes.Add(node.AddChild.Execute(GeneralUseData.SmallInt).Wait());

			// Assert
			Util.AssertCollectionsHaveSameItems(nodes, returnValueAsCollection);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void ConnectToChildNodes_return_value_reflects_future_removed_nodes(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			List<TreeNode<object>> nodes = new();
			IObservable<IChangeSet<TreeNode<object>>> returnValue;
			ReadOnlyObservableCollection<TreeNode<object>> returnValueAsCollection;
			IDisposable disposable;
			const int childNodesToCreateBeforeBinding = 3;
			const int childNodesToRemoveAfterBinding = 2;
			Assert.True(childNodesToCreateBeforeBinding > 0);
			Assert.True(childNodesToRemoveAfterBinding > 0);
			Assert.True(childNodesToRemoveAfterBinding <= childNodesToCreateBeforeBinding);

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			for (int i = 0; i < childNodesToCreateBeforeBinding; i++)
				nodes.Add(node.AddChild.Execute(GeneralUseData.SmallInt).Wait());

			returnValue = node.ConnectToChildNodes();
			disposable = returnValue.Bind(out returnValueAsCollection).Subscribe();

			for (int i = 0; i < childNodesToRemoveAfterBinding; i++)
			{
				TreeNode<object> nodeToDelete = nodes.Last();
				nodes.Remove(nodeToDelete);
				nodeToDelete.Delete.Execute(false).Wait();
			}

			// Assert
			Util.AssertCollectionsHaveSameItems(nodes, returnValueAsCollection);

			// Cleanup
			disposable.Dispose();
		}

		#endregion


		#region IsChildOf

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Calling_IsChildOf_with_direct_parent_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> nodeParam;

			// Act
			tree = treeSource();
			TreeNode<object> root = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			nodeParam = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node = nodeParam.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.IsChildOf(nodeParam)
			);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Calling_IsChildOf_with_direct_parent_returns_true(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> nodeParam;

			// Act
			tree = treeSource();
			TreeNode<object> root = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			nodeParam = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node = nodeParam.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Assert.True(node.IsChildOf(nodeParam));
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Calling_IsChildOf_with_indirect_parent_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> nodeParam;

			// Act
			tree = treeSource();
			TreeNode<object> root = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			nodeParam = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node = nodeParam.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.IsChildOf(nodeParam)
			);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Calling_IsChildOf_with_indirect_parent_returns_true(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> nodeParam;

			// Act
			tree = treeSource();
			TreeNode<object> root = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			nodeParam = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node = nodeParam.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Assert.True(node.IsChildOf(nodeParam));
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Calling_IsChildOf_with_child_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> nodeParam;

			// Act
			tree = treeSource();
			TreeNode<object> root = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			nodeParam = node.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.IsChildOf(nodeParam)
			);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Calling_IsChildOf_with_child_returns_false(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> nodeParam;

			// Act
			tree = treeSource();
			TreeNode<object> root = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			nodeParam = node.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Assert.False(node.IsChildOf(nodeParam));
		}

		[Fact]
		public static void Calling_IsChildOf_with_root_sibling_doesnt_throw()
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> nodeParam;

			// Act
			tree = new MultiRootTree<object>();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			nodeParam = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.IsChildOf(nodeParam)
			);
		}

		[Fact]
		public static void Calling_IsChildOf_with_root_sibling_returns_false()
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> nodeParam;

			// Act
			tree = new MultiRootTree<object>();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			nodeParam = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Assert.False(node.IsChildOf(nodeParam));
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Calling_IsChildOf_with_non_root_sibling_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> nodeParam;

			// Act
			tree = treeSource();
			TreeNode<object> root = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			nodeParam = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.IsChildOf(nodeParam)
			);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Calling_IsChildOf_with_non_root_sibling_returns_false(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> nodeParam;

			// Act
			tree = treeSource();
			TreeNode<object> root = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			nodeParam = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Assert.False(node.IsChildOf(nodeParam));
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Calling_IsChildOf_with_itself_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> nodeParam;

			// Act
			tree = treeSource();
			TreeNode<object> root = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			nodeParam = node;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.IsChildOf(nodeParam)
			);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Calling_IsChildOf_with_itself_returns_false(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> nodeParam;

			// Act
			tree = treeSource();
			TreeNode<object> root = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			nodeParam = node;

			// Assert
			Assert.False(node.IsChildOf(nodeParam));
		}

		#endregion


		#region ContainingTree

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void ContainingTree_matches_tree_it_was_created_from(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			Tree<object> actualValue;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			actualValue = node.ContainingTree;

			// Assert
			Assert.Equal(tree, actualValue);
		}

		#endregion


		#region Value

		[Theory]
		[MemberData(nameof(Value_test_TheoryDataValue))]
		public static void Value_matches_given_parameter(Func<Tree<object?>> treeSource, object? valueToUse)
		{
			// Arrange
			Tree<object?> tree;
			TreeNode<object?> node;
			Optional<object?> actualValue;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(valueToUse).Wait();
			actualValue = node.Value;

			// Assert
			Assert.True(actualValue.HasValue);
			Assert.Equal(valueToUse, actualValue.Value);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Changes_to_Value_may_be_reacted_to(Func<Tree<object>> treeSource)
		{
			// Arrange
			object initialValue = 20.2f;
			List<object> valuesToChangeTo = new()
			{
				0,
				string.Empty,
				GeneralUseData.TinyString,
				GeneralUseData.LargePositiveInt,
				true,
				GeneralUseData.SmallString,
				GeneralUseData.SmallNegativeInt,
				new int[] { 1, 2, 3 },
				false
			};
			Tree<object> tree;
			TreeNode<object> node;
			object? reactor = null;
			IDisposable disposable;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(initialValue).Wait();

			disposable = node
				.WhenAnyValue(x => x.Value)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(x => reactor = x);

			// Act/Assert
			foreach (object nextValue in valuesToChangeTo)
			{
				Assert.Equal(node.Value, reactor);
				node.Value = nextValue;
			}
			disposable.Dispose();

			// Assert
			Assert.Equal(node.Value, reactor);
		}

		#endregion


		#region Parent

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Parent_is_initially_null_for_root_nodes(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? actualValue;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			actualValue = node.Parent;

			// Assert
			Assert.Null(actualValue);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Parent_is_initially_not_null_for_non_root_nodes(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? actualValue;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			actualValue = node.Parent;

			// Assert
			Assert.NotNull(actualValue);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Parent_is_null_after_becoming_a_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? actualValue;

			// Act
			tree = treeSource();
			TreeNode<object> rootNode = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = rootNode.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			node.Parent!.Delete.Execute(false).Wait();

			actualValue = node.Parent;

			// Assert
			Assert.Null(actualValue);
		}

		[Fact]
		public static void Parent_is_not_null_after_becoming_a_non_root_node()
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? actualValue;

			// Act
			tree = new MultiRootTree<object>();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();

			Assert.True(node.CanReparent, "Can't finish test, something is likely wrong with CanReparent or this test's implementation. (CanReparent is false)");
			_ = node.Reparent.Execute().Wait();

			actualValue = node.Parent;

			// Assert
			Assert.NotNull(actualValue);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Changes_to_Parent_may_be_reacted_to(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? reactor = null;
			IDisposable disposable;

			// Act
			tree = treeSource();
			TreeNode<object> rootNode = tree.CreateRootNode.Execute(GeneralUseData.TinyString).Wait();
			TreeNode<object> leafNode = rootNode.AddChild.Execute(GeneralUseData.TinyString).Wait();
			TreeNode<object> firstBranchNode = rootNode.AddChild.Execute(GeneralUseData.TinyString).Wait();
			TreeNode<object> secondBranchNode = rootNode.AddChild.Execute(GeneralUseData.TinyString).Wait();
			TreeNode<object> lowerLeafNode = secondBranchNode.AddChild.Execute(GeneralUseData.TinyString).Wait();
			TreeNode<object> lowerBranchNode = secondBranchNode.AddChild.Execute(GeneralUseData.TinyString).Wait();
			TreeNode<object> lowestBranchNode = lowerBranchNode.AddChild.Execute(GeneralUseData.TinyString).Wait();
			node = firstBranchNode.AddChild.Execute(GeneralUseData.TinyString).Wait();


			disposable = node
				.WhenAnyValue(x => x.Parent)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(x => reactor = x);

			// Act/Assert
			// This just reparents the node a bunch of times to make sure the binding has the correct value
			Assert.Equal(node.Parent, reactor);
			node.DesiredParent.LastValue = lowestBranchNode;
			node.Reparent.Execute();
			Assert.Equal(node.Value, reactor!.Value);
			node.DesiredParent.LastValue = rootNode;
			node.Reparent.Execute();
			Assert.Equal(node.Value, reactor!.Value);
			node.DesiredParent.LastValue = leafNode;
			node.Reparent.Execute();
			Assert.Equal(node.Value, reactor!.Value);
			node.DesiredParent.LastValue = lowerLeafNode;
			node.Reparent.Execute();
			Assert.Equal(node.Value, reactor!.Value);
			node.DesiredParent.LastValue = firstBranchNode;
			node.Reparent.Execute();
			Assert.Equal(node.Value, reactor!.Value);
			node.DesiredParent.LastValue = null;
			node.Reparent.Execute();
			Assert.Equal(node.Value, reactor!.Value);
			node.DesiredParent.LastValue = lowerBranchNode;
			node.Reparent.Execute();
			Assert.Equal(node.Value, reactor!.Value);

			disposable.Dispose();
		}

		#endregion


		#region DesiredParent

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void DesiredParent_is_initally_empty(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			ReactiveOptional<TreeNode<object>?> actualValue;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			actualValue = node.DesiredParent;

			// Assert
			Assert.False(actualValue.HasValue);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void DesiredParent_is_empty_after_executing_Reparent(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> nodeToReparentTo;
			ReactiveOptional<TreeNode<object>?> actualValue;

			// Act
			tree = treeSource();
			nodeToReparentTo = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = nodeToReparentTo
				.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			node.DesiredParent.LastValue = nodeToReparentTo;
			node.Reparent.Execute().Wait();

			actualValue = node.DesiredParent;

			// Assert
			Assert.False(actualValue.HasValue);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Changes_to_DesiredParent_may_be_reacted_to(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			ReactiveOptional<TreeNode<object>?> reactor = ReactiveOptional<TreeNode<object>?>.MakeEmpty();
			IDisposable disposable;

			// Act
			tree = treeSource();
			TreeNode<object> root = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			TreeNode<object> otherNode = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			disposable = node
				.WhenAnyValue(x => x.DesiredParent)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(x => reactor = x);


			// Act/Assert
			Assert.False(node.DesiredParent.HasValue);
			
			node.DesiredParent.LastValue = root;
			Assert.True(node.DesiredParent.HasValue);
			Assert.Equal(node.DesiredParent.LastValue, reactor.LastValue);
			
			node.DesiredParent = ReactiveOptional<TreeNode<object>?>.MakeEmpty();
			Assert.False(node.DesiredParent.HasValue);

			node.DesiredParent.LastValue = otherNode;
			Assert.True(node.DesiredParent.HasValue);
			Assert.Equal(node.DesiredParent.LastValue, reactor.LastValue);
			
			node.DesiredParent.LastValue = null;
			Assert.True(node.DesiredParent.HasValue);
			Assert.Equal(node.DesiredParent.LastValue, reactor.LastValue);


			disposable.Dispose();
		}

		#endregion


		#region AddChild

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void AddChild_CanExecute_is_true_for_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			bool? actualValue = null;
			IDisposable disposable;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			disposable = node.AddChild.CanExecute.Subscribe(x => actualValue = x);

			// Assert
			Assert.True(actualValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void AddChild_CanExecute_is_true_for_non_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			bool? actualValue = null;
			IDisposable disposable;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			disposable = node.AddChild.CanExecute.Subscribe(x => actualValue = x);

			// Assert
			Assert.True(actualValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_AddChild_on_root_node_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.AddChild.Execute(GeneralUseData.SmallInt).Wait()
			);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_AddChild_on_non_root_node_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.AddChild.Execute(GeneralUseData.SmallInt).Wait()
			);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_AddChild_on_root_node_returns_non_null_value(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? returnValue;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			returnValue = node.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Assert.NotNull(returnValue);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_AddChild_on_non_root_node_returns_non_null_value(Func<Tree<object>> treeSource)
		{
			// NOTE: This technically requires AddChild to be working to be able to test but it's probably not a big deal

			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? returnValue;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			returnValue = node.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Assert.NotNull(returnValue);
		}

		#endregion


		#region Delete

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Delete_CanExecute_is_true_for_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			bool? actualValue = null;
			IDisposable disposable;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			disposable = node.Delete.CanExecute.Subscribe(x => actualValue = x);

			// Assert
			Assert.True(actualValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Delete_CanExecute_is_true_for_non_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			bool? actualValue = null;
			IDisposable disposable;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			disposable = node.Delete.CanExecute.Subscribe(x => actualValue = x);

			// Assert
			Assert.True(actualValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(DeleteTheoryDataValue))]
		public static void Executing_Delete_on_root_node_doesnt_throw(Func<Tree<object>> treeSource, bool shouldDeleteRecursively)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Delete.Execute(shouldDeleteRecursively).Wait()
			);
		}

		[Theory]
		[MemberData(nameof(DeleteTheoryDataValue))]
		public static void Executing_Delete_on_non_root_node_doesnt_throw(Func<Tree<object>> treeSource, bool shouldDeleteRecursively)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Delete.Execute(shouldDeleteRecursively).Wait()
			);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Delete_with_false_moves_direct_children_up_one_level(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> parentNode;
			TreeNode<object>[] childNodes;
			const int numberOfChildNodesToCreate = 3;
			Assert.True(numberOfChildNodesToCreate > 0);

			// Act
			tree = treeSource();
			parentNode = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = parentNode.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			childNodes = new TreeNode<object>[numberOfChildNodesToCreate];
			for (int i = 0; i < numberOfChildNodesToCreate; i++)
				childNodes[i] = node.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			node.Delete.Execute(false).Wait();

			// Assert
			Assert.True(childNodes.All(childNode => childNode.Parent == parentNode));
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Delete_with_false_moves_indirect_children_up_one_level(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> parentNode;
			TreeNode<object>[,] childNodes;
			const int numberOfChildNodesToCreatePerDepthLevel = 3;
			const int numberOfDepthLevelsToCreate = 3;
			Assert.True(numberOfChildNodesToCreatePerDepthLevel > 0);
			Assert.True(numberOfDepthLevelsToCreate > 1);

			// Act
			tree = treeSource();
			parentNode = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = parentNode.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			childNodes = new TreeNode<object>[numberOfChildNodesToCreatePerDepthLevel, numberOfDepthLevelsToCreate];
			for (int childIndex = 0; childIndex < numberOfChildNodesToCreatePerDepthLevel; childIndex++)
			{
				childNodes[0, childIndex] = node.AddChild.Execute(GeneralUseData.SmallInt).Wait();
				for (int depthLvl = 1; depthLvl < numberOfDepthLevelsToCreate; depthLvl++)
				{
					childNodes[depthLvl, childIndex] = childNodes[depthLvl - 1, childIndex].AddChild.Execute(GeneralUseData.SmallInt).Wait();
				}
			}

			node.Delete.Execute(false).Wait();

			// Assert
			for (int childIndex = 0; childIndex < numberOfChildNodesToCreatePerDepthLevel; childIndex++)
			{
				Assert.Equal(parentNode, childNodes[0, childIndex].Parent);
				for (int depthLvl = 1; depthLvl < numberOfDepthLevelsToCreate; depthLvl++)
				{
					Assert.Equal(childNodes[depthLvl - 1, childIndex], childNodes[depthLvl, childIndex].Parent);
				}
			}
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Delete_with_true_deletes_all_direct_children(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> parentNode;
			TreeNode<object>[] childNodes;
			const int numberOfChildNodesToCreate = 3;
			IDisposable disposable;
			ReadOnlyObservableCollection<TreeNode<object>> childNodesOfParentNode;
			Assert.True(numberOfChildNodesToCreate > 0);

			// Act
			tree = treeSource();
			parentNode = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = parentNode.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			childNodes = new TreeNode<object>[numberOfChildNodesToCreate];
			for (int i = 0; i < numberOfChildNodesToCreate; i++)
				childNodes[i] = node.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			node.Delete.Execute(true).Wait();

			disposable = parentNode.ConnectToChildNodes()
				.Bind(out childNodesOfParentNode)
				.Subscribe();

			// Assert
			Assert.Empty(childNodesOfParentNode);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Delete_with_true_deletes_all_indirect_children(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> parentNode;
			TreeNode<object>[,] childNodes;
			const int numberOfChildNodesToCreatePerDepthLevel = 3;
			const int numberOfDepthLevelsToCreate = 3;
			IDisposable disposable;
			ReadOnlyObservableCollection<TreeNode<object>> childNodesOfParentNode;
			Assert.True(numberOfChildNodesToCreatePerDepthLevel > 0);
			Assert.True(numberOfDepthLevelsToCreate > 1);

			// Act
			tree = treeSource();
			parentNode = tree.CreateRootNode.Execute("Parent").Wait();
			node = parentNode.AddChild.Execute("Node").Wait();
			childNodes = new TreeNode<object>[numberOfChildNodesToCreatePerDepthLevel, numberOfDepthLevelsToCreate];
			for (int childIndex = 0; childIndex < numberOfChildNodesToCreatePerDepthLevel; childIndex++)
			{
				childNodes[0, childIndex] = node.AddChild.Execute($"Child #{childIndex} at lvl 0").Wait();
				for (int depthLvl = 1; depthLvl < numberOfDepthLevelsToCreate; depthLvl++)
				{
					childNodes[depthLvl, childIndex] = childNodes[depthLvl - 1, childIndex].AddChild
						.Execute($"Child #{childIndex} at lvl {depthLvl}").Wait();
				}
			}

			node.Delete.Execute(true).Wait();

			disposable = parentNode.ConnectToChildNodes()
				.Bind(out childNodesOfParentNode)
				.Subscribe();

			// Assert
			Assert.Empty(childNodesOfParentNode);

			// Cleanup
			disposable.Dispose();
		}

		#endregion


		#region Reparent/CanReparent

		#region Reparent_from_root

		#region Reparent_from_root_to_child

		#region Reparent_from_root_to_child_->_direct_child

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_root_node_whos_DesiredParent_is_a_direct_child(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = node.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_root_node_whos_DesiredParent_is_a_direct_child_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = node.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_root_to_child_->_grandchild
		// NOTE: "Grandchild" refers to an indirect child, i.e. a direct child of a direct child

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_root_node_whos_DesiredParent_is_a_grandchild(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = node.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_root_node_whos_DesiredParent_is_a_grandchild_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = node.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#endregion

		#region Reparent_from_root_to_non_child

		#region Reparent_from_root_to_non_child_->_empty
		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_root_node_whos_DesiredParent_is_empty(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			//TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_root_node_whos_DesiredParent_is_empty_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_root_to_child_->_itself

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_root_node_whos_DesiredParent_is_itself(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = node;
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_root_node_whos_DesiredParent_is_itself_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = node;
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_root_to_child_->_another_root

		[Fact]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_true_for_root_node_whos_DesiredParent_is_another_root(/*Impossible in a SingleRootTree*/)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = new MultiRootTree<object>();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.True(actualCanReparentValue);
			Assert.True(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Fact]
		public static void Executing_Reparent_on_root_node_whos_DesiredParent_is_another_root_doesnt_throw(/*Impossible in a SingleRootTree*/)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = new MultiRootTree<object>();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#endregion

		#endregion


		#region Reparent_from_non_root

		#region Reparent_from_non_root_to_parent

		#region Reparent_from_non_root_to_parent_->_direct_parent

		#region Reparent_from_non_root_to_parent_->_root_direct_parent

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_non_root_node_whos_DesiredParent_is_a_root_direct_parent(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			desiredParent = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = desiredParent.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_root_direct_parent_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = treeSource();
			desiredParent = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = desiredParent.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_parent_->_non_root_direct_parent

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_non_root_node_whos_DesiredParent_is_a_non_root_direct_parent(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			desiredParent = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node = desiredParent.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_non_root_direct_parent_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = treeSource();
			desiredParent = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node = desiredParent.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#endregion

		#region Reparent_from_non_root_to_parent_->_grandparent
		// NOTE: "Grandparent" refers to an indirect parent, i.e. a direct parent of a direct parent

		#region Reparent_from_non_root_to_parent_->_root_grandparent

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_true_for_non_root_node_whos_DesiredParent_is_a_root_grandparent(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			desiredParent = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = desiredParent.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.True(actualCanReparentValue);
			Assert.True(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_root_grandparent_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = treeSource();
			desiredParent = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = desiredParent.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_parent_->_non_root_grandparent

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_true_for_non_root_node_whos_DesiredParent_is_a_non_root_grandparent(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			desiredParent = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node = desiredParent.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.True(actualCanReparentValue);
			Assert.True(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_non_root_grandparent_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = treeSource();
			desiredParent = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node = desiredParent.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#endregion

		#region Reparent_from_non_root_to_parent_->_uncle
		// NOTE: "Uncle" refers to a sibling of a direct parent

		#region Reparent_from_non_root_to_parent_->_root_uncle

		[Fact]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_true_for_non_root_node_whos_DesiredParent_is_a_root_uncle(/*Impossible in a SingleRootTree*/)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = new MultiRootTree<object>();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.True(actualCanReparentValue);
			Assert.True(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Fact]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_root_uncle_doesnt_throw(/*Impossible in a SingleRootTree*/)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = new MultiRootTree<object>();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_parent_->_non_root_uncle

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_true_for_non_root_node_whos_DesiredParent_is_a_non_root_uncle(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			TreeNode<object> root = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = root.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.True(actualCanReparentValue);
			Assert.True(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_non_root_uncle_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = treeSource();
			TreeNode<object> root = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = root.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = root.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#endregion

		#endregion


		#region Reparent_from_non_root_to_child

		#region Reparent_from_non_root_to_child_->_direct_child

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_non_root_node_whos_DesiredParent_is_a_direct_child(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = node.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_direct_child_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = node.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_child_->_grandchild
		// NOTE: "Grandchild" refers to an indirect child, i.e. a direct child of a direct child

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_non_root_node_whos_DesiredParent_is_a_grandchild(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = node.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_grandchild_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = node.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_child_->_nephew
		// NOTE: "Nephew" refers to a direct child of a sibling

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_true_for_non_root_node_whos_DesiredParent_is_a_nephew(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			TreeNode<object> rootNode = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = rootNode.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = rootNode.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.True(actualCanReparentValue);
			Assert.True(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_nephew_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = treeSource();
			TreeNode<object> rootNode = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = rootNode.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = rootNode.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#endregion


		#region Reparent_from_non_root_misc

		#region Reparent_from_non_root_to_misc_->_empty

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_non_root_node_whos_DesiredParent_is_empty(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			//TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_empty_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_misc_->_itself

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_non_root_node_whos_DesiredParent_is_itself(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = node;
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_itself_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = node;
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_misc_->_a_sibling

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_true_for_non_root_node_whos_DesiredParent_is_a_sibling(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			TreeNode<object> rootNode = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = rootNode.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = rootNode.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.True(actualCanReparentValue);
			Assert.True(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_sibling_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = treeSource();
			TreeNode<object> rootNode = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = rootNode.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = rootNode.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_misc_->_a_cousin
		// NOTE: "Cousin" refers to a direct child of a sibling of a direct parent

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_true_for_non_root_node_whos_DesiredParent_is_a_cousin(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;
			bool actualCanReparentValue;
			bool? actualReparent_CanExecuteValue = null;

			// Act
			tree = treeSource();
			TreeNode<object> rootNode = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = rootNode.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = rootNode.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.True(actualCanReparentValue);
			Assert.True(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_cousin_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? desiredParent;

			// Act
			tree = treeSource();
			TreeNode<object> rootNode = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = rootNode.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			desiredParent = rootNode.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#endregion

		#endregion


		#endregion


		#region IsRootNode

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void IsRootNode_is_true_for_a_node_created_as_a_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Assert.True(node.IsRootNode);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void IsRootNode_is_false_for_a_node_created_as_a_non_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			// Assert
			Assert.False(node.IsRootNode);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void IsRootNode_is_true_for_a_node_after_becoming_a_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = null;

			if (node.CanReparent)
				node.Reparent.Execute().Wait();
			else
			{
				Assert.True(
					node.Parent != null,
					"Can't finish test, something is likely wrong with CanReparent or this test's implementation. (CanReparent is false while Parent is null)"
				);
				node.Parent!.Delete.Execute(false).Wait();
			}

			// Assert
			Assert.True(node.IsRootNode);
		}

		[Fact]
		public static void IsRootNode_is_false_for_node_after_becoming_a_non_root_node()
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;

			// Act
			tree = new MultiRootTree<object>();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent.LastValue = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();

			Assert.True(node.CanReparent, "Can't finish test, something is likely wrong with CanReparent or this test's implementation. (CanReparent is false)");
			node.Reparent.Execute().Wait();

			// Assert
			Assert.False(node.IsRootNode);
		}

		#endregion


		#region PathToRoot

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void PathToRoot_has_expected_value_for_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			IEnumerable<TreeNode<object>> expectedValue;
			IEnumerable<TreeNode<object>> actualValue;

			// Act
			tree = treeSource();
			tree.ShouldRecursivelyNotifyNodesOfReparenting = true;
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			expectedValue = new List<TreeNode<object>>() { node };
			actualValue = node.PathToRoot;

			// Assert
			Util.AssertCollectionsHaveSameItems(expectedValue, actualValue);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void PathToRoot_has_expected_value_for_direct_child_of_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			IEnumerable<TreeNode<object>> expectedValue;
			IEnumerable<TreeNode<object>> actualValue;

			// Act
			tree = treeSource();
			tree.ShouldRecursivelyNotifyNodesOfReparenting = true;
			TreeNode<object> root = tree.CreateRootNode.Execute("Root").Wait();
			node = root.AddChild.Execute("Node").Wait();
			expectedValue = new List<TreeNode<object>>() { node, root };
			actualValue = node.PathToRoot;

			// Assert
			Util.AssertCollectionsHaveSameItems(expectedValue, actualValue);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void PathToRoot_has_expected_value_for_indirect_child_of_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			IEnumerable<TreeNode<object>> expectedValue;
			IEnumerable<TreeNode<object>> actualValue;

			// Act
			tree = treeSource();
			tree.ShouldRecursivelyNotifyNodesOfReparenting = true;
			TreeNode<object> root = tree.CreateRootNode.Execute("Root").Wait();
			TreeNode<object>[] branches = new TreeNode<object>[3];
			branches[0] = root.AddChild.Execute("Branch 0/2").Wait();
			branches[1] = branches[0].AddChild.Execute("Branch 1/2").Wait();
			branches[2] = branches[1].AddChild.Execute("Branch 2/2").Wait();
			node = branches[2].AddChild.Execute("Node").Wait();
			expectedValue = new List<TreeNode<object>>() { node, branches[2], branches[1], branches[0], root };
			actualValue = node.PathToRoot;

			// Assert
			Util.AssertCollectionsHaveSameItems(expectedValue, actualValue);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void PathToRoot_has_expected_value_for_indirect_child_of_root_node_when_unrelated_nodes_exist(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			IEnumerable<TreeNode<object>> expectedValue;
			IEnumerable<TreeNode<object>> actualValue;

			// Act
			tree = treeSource();
			tree.ShouldRecursivelyNotifyNodesOfReparenting = true;
			TreeNode<object> root = tree.CreateRootNode.Execute("Root").Wait();
			TreeNode<object>[] branches = new TreeNode<object>[3];
			root.AddChild.Execute("Unrelated child leaf of root").Wait();
			TreeNode<object> tmp = root.AddChild.Execute("Unrelated child branch of root").Wait();
			tmp.AddChild.Execute("Unrelated grandchild of root 0/1").Wait();
			tmp.AddChild.Execute("Unrelated grandchild of root 1/1").Wait();
			branches[0] = root.AddChild.Execute("Branch 0/2").Wait();
			branches[0].AddChild.Execute("Unrelated child leaf of branch 0/2").Wait();
			branches[1] = branches[0].AddChild.Execute("Branch 1/2").Wait();
			branches[2] = branches[1].AddChild.Execute("Branch 2/2").Wait();
			node = branches[2].AddChild.Execute("Node").Wait();
			branches[2].AddChild.Execute("Unrelated child leaf 0/1 of branch 2/2").Wait();
			branches[2].AddChild.Execute("Unrelated child leaf 1/1 of branch 2/2").Wait();
			expectedValue = new List<TreeNode<object>>() { node, branches[2], branches[1], branches[0], root };
			actualValue = node.PathToRoot;

			// Assert
			Util.AssertCollectionsHaveSameItems(expectedValue, actualValue);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void PathToRoot_is_unchanged_after_reparenting_when_ShouldRecursivelyNotifyNodesOfReparenting_is_false(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			bool shouldRecursivelyNotifyNodesOfReparenting = false;

			// Act
			tree = treeSource();
			tree.ShouldRecursivelyNotifyNodesOfReparenting = shouldRecursivelyNotifyNodesOfReparenting;

			// Assert
			Assert.False(DidPathToRootChangeWhenReparenting(tree));
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		public static void PathToRoot_changes_after_reparenting_when_ShouldRecursivelyNotifyNodesOfReparenting_is_true(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			bool shouldRecursivelyNotifyNodesOfReparenting = true;

			// Act
			tree = treeSource();
			tree.ShouldRecursivelyNotifyNodesOfReparenting = shouldRecursivelyNotifyNodesOfReparenting;

			// Assert
			Assert.True(DidPathToRootChangeWhenReparenting(tree));
		}

		#endregion
	}
}
