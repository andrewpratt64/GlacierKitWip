using Avalonia.Data;
using DynamicData;
using GlacierKitCore.Models;
using GlacierKitTestShared;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.Models
{
	public class TreeNodeTest
	{
		#region Theory_data
#pragma warning disable IDE1006 // Naming Styles

		private class _TYPE_DeleteTheoryData : TheoryData<Func<Tree<object>>, bool>
		{
			public _TYPE_DeleteTheoryData()
			{
				Add(() => new SingleRootTree<object>(), false);
				Add(() => new SingleRootTree<object>(), true);
				Add(() => new MultiRootTree<object>(), false);
				Add(() => new MultiRootTree<object>(), true);
			}
		}


		private class _TYPE_Value_test_TheoryData : TheoryData<Func<Tree<object?>>, object?>
		{
			private void AddWithValue(object? v)
			{
				Add(() => new SingleRootTree<object?>(), v);
				Add(() => new MultiRootTree<object?>(), v);
			}

			public _TYPE_Value_test_TheoryData()
			{

				AddWithValue(null);
				AddWithValue(string.Empty);
				AddWithValue(GeneralUseData.TinyString);
				AddWithValue(GeneralUseData.LongString);
				AddWithValue(GeneralUseData.SmallPositiveInt);
				AddWithValue(GeneralUseData.LargeNegativeInt);
			}
		}


		internal readonly _TYPE_TreeTheoryData<object> _DATA_TreeTheoryData = new();
		private readonly _TYPE_DeleteTheoryData _DATA_DeleteTheoryData = new();
		private readonly _TYPE_Value_test_TheoryData _DATA_Value_test_TheoryData = new();

#pragma warning restore IDE1006 // Naming Styles
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
		[MemberData(nameof(_DATA_DeleteTheoryData))]
		public static void _DATA_DeleteTheoryData_Provides_unique_instances_on_each_invoke(Func<Tree<object>> treeSource, bool _)
		{
			TheoryData_provides_unique_instances_on_each_invoke(treeSource);
		}

		[Theory]
		[MemberData(nameof(_DATA_Value_test_TheoryData))]
		public static void _DATA_Value_test_TheoryData_Provides_unique_instances_on_each_invoke(Func<Tree<object?>> treeSource, object? _)
		{
			TheoryData_provides_unique_instances_on_each_invoke(treeSource);
		}

#pragma warning restore xUnit1026 // Theory methods should use all of their parameters

		#endregion


		#region ContainingTree

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_Value_test_TheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Parent_is_null_after_becoming_a_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? actualValue;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent = null;

			if (node.CanReparent)
				_ = node.Reparent.Execute().Wait();
			else
			{
				Assert.True(node.Parent != null, "Can't finish test, something is likely wrong with CanReparent or this test's implementation. (CanReparent is false while Parent is null)");
				node.Parent!.Delete.Execute(false);
			}

			actualValue = node.Parent;

			// Assert
			Assert.Null(actualValue);
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Parent_is_not_null_after_becoming_a_non_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object>? actualValue;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();

			Assert.True(node.CanReparent, "Can't finish test, something is likely wrong with CanReparent or this test's implementation. (CanReparent is false)");
			_ = node.Reparent.Execute().Wait();

			actualValue = node.Parent;

			// Assert
			Assert.NotNull(actualValue);
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = lowestBranchNode;
			node.Reparent.Execute();
			Assert.Equal(node.Value, reactor);
			node.DesiredParent = rootNode;
			node.Reparent.Execute();
			Assert.Equal(node.Value, reactor);
			node.DesiredParent = leafNode;
			node.Reparent.Execute();
			Assert.Equal(node.Value, reactor);
			node.DesiredParent = lowerLeafNode;
			node.Reparent.Execute();
			Assert.Equal(node.Value, reactor);
			node.DesiredParent = firstBranchNode;
			node.Reparent.Execute();
			Assert.Equal(node.Value, reactor);
			node.DesiredParent = null;
			node.Reparent.Execute();
			Assert.Equal(node.Value, reactor);
			node.DesiredParent = lowerBranchNode;
			node.Reparent.Execute();
			Assert.Equal(node.Value, reactor);

			disposable.Dispose();
		}

		#endregion


		#region DesiredParent

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void DesiredParent_is_initally_empty(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			Optional<TreeNode<object>?> actualValue;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			actualValue = node.DesiredParent;

			// Assert
			Assert.False(actualValue.HasValue);
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void DesiredParent_is_empty_after_executing_Reparent(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			TreeNode<object> nodeToReparentTo;
			Optional<TreeNode<object>?> actualValue;

			// Act
			tree = treeSource();
			nodeToReparentTo = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node = nodeToReparentTo
				.AddChild.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();

			node.DesiredParent = nodeToReparentTo;
			node.Reparent.Execute().Wait();

			actualValue = node.DesiredParent;

			// Assert
			Assert.False(actualValue.HasValue);
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Changes_to_DesiredParent_may_be_reacted_to(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			Optional<TreeNode<object>?> reactor = null;
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
			Assert.Equal(node.Parent, reactor);
			node.DesiredParent = root;
			Assert.Equal(node.Parent, reactor);
			node.DesiredParent = Optional<TreeNode<object>?>.Empty;
			Assert.Equal(node.Parent, reactor);
			node.DesiredParent = otherNode;
			Assert.Equal(node.Parent, reactor);
			node.DesiredParent = null;
			Assert.Equal(node.Parent, reactor);

			disposable.Dispose();
		}

		#endregion


		#region AddChild

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void AddChild_CanExecute_is_true_for_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			bool actualValue;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			actualValue = node.AddChild.CanExecute.Wait();

			// Assert
			Assert.True(actualValue);
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void AddChild_CanExecute_is_true_for_non_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			bool actualValue;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			actualValue = node.AddChild.CanExecute.Wait();

			// Assert
			Assert.True(actualValue);
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Delete_CanExecute_is_true_for_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			bool actualValue;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			actualValue = node.Delete.CanExecute.Wait();

			// Assert
			Assert.True(actualValue);
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Delete_CanExecute_is_true_for_non_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;
			bool actualValue;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			actualValue = node.Delete.CanExecute.Wait();

			// Assert
			Assert.True(actualValue);
		}

		[Theory]
		[MemberData(nameof(_DATA_DeleteTheoryData))]
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
		[MemberData(nameof(_DATA_DeleteTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		// TODO: Test **BOTH** TreeNode.CanReparent and TreeNode.Reparent.CanExecute. They should == each other.

		#region Reparent_from_root

		#region Reparent_from_root_to_child

		#region Reparent_from_root_to_child_->_direct_child

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_root_to_child_->_grandchild
		// NOTE: "Grandchild" refers to an indirect child, i.e. a direct child of a direct child

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;

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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_root_to_child_->_another_root

		[Fact]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_root_node_whos_DesiredParent_is_another_root(/*Impossible in a SingleRootTree*/)
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

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
			node.DesiredParent = desiredParent;

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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_parent_->_non_root_direct_parent

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;

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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.True(actualCanReparentValue);
			Assert.True(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_parent_->_non_root_grandparent

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.True(actualCanReparentValue);
			Assert.True(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;

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
			node.DesiredParent = desiredParent;
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
			node.DesiredParent = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_parent_->_non_root_uncle

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.True(actualCanReparentValue);
			Assert.True(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;

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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_child_->_grandchild
		// NOTE: "Grandchild" refers to an indirect child, i.e. a direct child of a direct child

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_child_->_nephew
		// NOTE: "Nephew" refers to a direct child of a sibling

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.True(actualCanReparentValue);
			Assert.True(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;

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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.False(actualCanReparentValue);
			Assert.False(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_misc_->_a_sibling

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.True(actualCanReparentValue);
			Assert.True(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => node.Reparent.Execute().Wait()
			);
		}

		#endregion

		#region Reparent_from_non_root_to_misc_->_a_cousin
		// NOTE: "Cousin" refers to a direct child of a sibling of a direct parent

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;
			actualCanReparentValue = node.CanReparent;
			IDisposable disposable = node.Reparent.CanExecute.Subscribe(x => actualReparent_CanExecuteValue = x);

			// Assert
			Assert.True(actualCanReparentValue);
			Assert.True(actualReparent_CanExecuteValue);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
			node.DesiredParent = desiredParent;

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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
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
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void IsRootNode_is_true_for_a_node_after_becoming_a_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait()
				.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent = null;

			if (node.CanReparent)
				node.Reparent.Execute().Wait();
			else
			{
				Assert.True(node.Parent != null, "Can't finish test, something is likely wrong with CanReparent or this test's implementation. (CanReparent is false while Parent is null)");
				node.Parent!.Delete.Execute(false);
			}

			// Assert
			Assert.True(node.IsRootNode);
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void IsRootNode_is_false_for_node_after_becoming_a_non_root_node(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object> node;

			// Act
			tree = treeSource();
			node = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();
			node.DesiredParent = tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait();

			Assert.True(node.CanReparent, "Can't finish test, something is likely wrong with CanReparent or this test's implementation. (CanReparent is false)");
			node.Reparent.Execute().Wait();

			// Assert
			Assert.True(node.IsRootNode);
		}

		#endregion
	}
}
