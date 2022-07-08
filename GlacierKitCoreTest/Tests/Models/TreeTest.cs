using DynamicData;
using GlacierKitCore.Models;
using GlacierKitTestShared;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using ReactiveUI.Testing;
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
	#region Shared_theory_data

	public class TreeTheoryData<TNodeValue> : TheoryData<Func<Tree<TNodeValue>>>
	{
		public TreeTheoryData()
		{
			Add(() => new SingleRootTree<TNodeValue>());
			Add(() => new MultiRootTree<TNodeValue>());
		}
	}

	#endregion


	[SuppressMessage("Style", "IDE0018:Variable declaration can be inlined", Justification = "Test code often intentionally seperates declaration (Arrange) and assignment (Act)")]
	public class TreeTest
	{
		#region Theory_data

		private struct SingleRootNodeChange
		{
			public int TimesToAddRootNode;
			public int TimesToRemoveRootNode;

			public SingleRootNodeChange(int timesToAddRootNode, int timesToRemoveRootNode)
			{
				TimesToAddRootNode = timesToAddRootNode;
				TimesToRemoveRootNode = timesToRemoveRootNode;
			}
		}

		public static readonly TreeTheoryData<object> TreeTheoryData = new();

		#endregion


		#region Theory_data_tests

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
#pragma warning disable IDE1006 // Naming Styles
		public static void TreeTheoryData_Provides_unique_instances_on_each_invoke(Func<Tree<object>> treeSource)
#pragma warning restore IDE1006 // Naming Styles
		{
			// Arrange
			List<Tree<object>> returnedValues = new();
			int timesToInvoke = 10;
			int expectedDistinctReturnedValues = timesToInvoke;
			int actualDistinctReturnedValues;

			// Act
			for (int i = 0; i < timesToInvoke; i++)
				returnedValues.Add(treeSource());
			actualDistinctReturnedValues = returnedValues.Distinct().Count();

			// Assert
			Assert.Equal(expectedDistinctReturnedValues, actualDistinctReturnedValues);
		}

		#endregion


		#region Tree
		
		#region CreateRootNode

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		[Trait("TreeClass", "Tree")]
		public static void CreateRootNode_CanExecute_is_true_only_when_CanAddRootNode_is_true(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			TreeNode<object>? rootNode;
			List<(bool CreateRootNode_CanExecute, bool CanAddRootNode)> values;
			bool CreateRootNode_CanExecuteCurrentValue = default;
			IDisposable disposable;
			Action updateValues;

			// Act
			tree = treeSource();
			values = new();
			disposable = tree.CreateRootNode.CanExecute
				.Subscribe(x => CreateRootNode_CanExecuteCurrentValue = x);

#pragma warning disable IDE0037 // Use inferred member name
			updateValues = () => values.Add((
				CreateRootNode_CanExecute: CreateRootNode_CanExecuteCurrentValue,
				CanAddRootNode: tree.CanAddRootNode
			));
#pragma warning restore IDE0037 // Use inferred member name

			updateValues();
			rootNode = tree.CreateRootNode.Execute(GeneralUseData.TinyString).Wait();
			updateValues();
			rootNode.Delete.Execute(false).Wait();
			updateValues();

			disposable.Dispose();

			// Assert
#pragma warning disable IDE0008 // Use explicit type
			foreach (var (CreateRootNode_CanExecute, CanAddRootNode) in values)
#pragma warning restore IDE0008 // Use explicit type
			{
				Assert.Equal(CreateRootNode_CanExecute, CanAddRootNode);
			}
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		[Trait("TreeClass", "Tree")]
		public static void Executing_CreateRootNode_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;

			// Act
			tree = treeSource();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => tree.CreateRootNode
				.Execute(GeneralUseData.SmallString)
				.Wait()
			);
		}

		#endregion


		#region ConnectToNodes

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		[Trait("TreeClass", "Tree")]
		public static void Calling_ConnectToNodes_doesnt_throw(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;

			// Act
			tree = treeSource();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => tree.ConnectToNodes()
			);
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		[Trait("TreeClass", "Tree")]
		public static void ConnectToNodes_doesnt_return_null(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;

			// Act
			tree = treeSource();

			// Assert
			Assert.NotNull(tree.ConnectToNodes());
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		[Trait("TreeClass", "Tree")]
		public static void ConnectToNodes_return_value_contains_pre_existing_nodes(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			List<TreeNode<object>> preExistingNodes = new();
			IObservable<IChangeSet<TreeNode<object>>> returnValue;
			ReadOnlyObservableCollection<TreeNode<object>> returnValueAsCollection;
			IDisposable disposable;

			// Act
			tree = treeSource();

			// Add a single root node
			preExistingNodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());
			// Add another two root nodes, if possible
			if (tree.CanAddRootNode)
			{
				preExistingNodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());
				preExistingNodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());
			}

			// Temporarily remember the first root node
			TreeNode<object> temporaryRootNode = preExistingNodes[0];
			// Create a child of that root node and remember it temporarily
			TreeNode<object> temporaryBranchNode = temporaryRootNode
				.AddChild.Execute(GeneralUseData.SmallString).Wait();
			// Create a child of that node as well and remember it temporarily
			TreeNode<object> temporaryLeafNode = temporaryBranchNode
				.AddChild.Execute(GeneralUseData.SmallString).Wait();
			// Add extra children to the temporarily remembered nodes
			preExistingNodes.Add(temporaryRootNode.AddChild.Execute(GeneralUseData.SmallString).Wait());
			preExistingNodes.Add(temporaryBranchNode.AddChild.Execute(GeneralUseData.SmallString).Wait());
			// Add the temporarily remembered nodes to preExistingNodes
			//	(excluding temporaryRootNode, since it should already be added)
			preExistingNodes.Add(temporaryBranchNode);
			preExistingNodes.Add(temporaryLeafNode);

			returnValue = tree.ConnectToNodes();
			disposable = returnValue.Bind(out returnValueAsCollection).Subscribe();

			// Assert
			Util.AssertCollectionsHaveSameItems(preExistingNodes, returnValueAsCollection);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		[Trait("TreeClass", "Tree")]
		public static void ConnectToNodes_return_value_reflects_future_added_nodes(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			List<TreeNode<object>> nodes = new();
			IObservable<IChangeSet<TreeNode<object>>> returnValue;
			ReadOnlyObservableCollection<TreeNode<object>> returnValueAsCollection;
			IDisposable disposable;

			// Act
			tree = treeSource();

			// Add a single root node
			nodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());
			// Add another two root nodes, if possible
			if (tree.CanAddRootNode)
			{
				nodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());
				nodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());
			}

			// Temporarily remember the first root node
			TreeNode<object> temporaryRootNode = nodes[0];
			// Create a child of that root node and remember it temporarily
			TreeNode<object> temporaryBranchNode = temporaryRootNode
				.AddChild.Execute(GeneralUseData.SmallString).Wait();

			// Bind ConnectToNodes()
			returnValue = tree.ConnectToNodes();
			disposable = returnValue.Bind(out returnValueAsCollection).Subscribe();



			// Create a child of the node mentioned before the binding and remember it temporarily
			TreeNode<object> temporaryLeafNode = temporaryBranchNode
				.AddChild.Execute(GeneralUseData.SmallString).Wait();
			// Add extra children to the temporarily remembered nodes
			nodes.Add(temporaryRootNode.AddChild.Execute(GeneralUseData.SmallString).Wait());
			nodes.Add(temporaryBranchNode.AddChild.Execute(GeneralUseData.SmallString).Wait());
			// Add the temporarily remembered nodes to the nodes List
			//	(excluding temporaryRootNode, since it should already be added)
			nodes.Add(temporaryBranchNode);
			nodes.Add(temporaryLeafNode);

			// Add one more root node, if possible
			if (tree.CanAddRootNode)
				nodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());

			// Assert
			Util.AssertCollectionsHaveSameItems(nodes, returnValueAsCollection);

			// Cleanup
			disposable.Dispose();
		}

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		[Trait("TreeClass", "Tree")]
		public static void ConnectToNodes_return_value_reflects_future_removed_nodes(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			List<TreeNode<object>> nodes = new();
			IObservable<IChangeSet<TreeNode<object>>> returnValue;
			ReadOnlyObservableCollection<TreeNode<object>> returnValueAsCollection;
			IDisposable disposable;

			// Act
			tree = treeSource();

			// Add a single root node
			nodes.Add(tree.CreateRootNode.Execute("Root").Wait());
			// Add another two root nodes, if possible.
			//	Temporarily remember one of them for later if added
			TreeNode<object>? temporaryOtherRootNode = null;
			if (tree.CanAddRootNode)
			{
				temporaryOtherRootNode = tree.CreateRootNode.Execute("Another root").Wait();
				nodes.Add(temporaryOtherRootNode);
				nodes.Add(tree.CreateRootNode.Execute("Yet another root").Wait());
			}

			// Temporarily remember the first root node
			TreeNode<object> temporaryRootNode = nodes[0];
			// Create two children of that root node and remember both temporarily
			TreeNode<object> temporaryBranchNode = temporaryRootNode
				.AddChild.Execute("Branch").Wait();
			TreeNode<object> temporaryOtherBranchNode = temporaryRootNode
				.AddChild.Execute("Another branch").Wait();
			// Create a child of that node as well and remember it temporarily
			TreeNode<object> temporaryLeafNode = temporaryBranchNode
				.AddChild.Execute("Leaf").Wait();
			// Add extra children to the temporarily remembered nodes
			nodes.Add(temporaryRootNode.AddChild.Execute("Some node").Wait());
			nodes.Add(temporaryOtherBranchNode.AddChild.Execute("A child of another branch").Wait());
			// Add the temporarily remembered nodes to the nodes List
			//	(excluding temporaryRootNode, since it should already be added)
			nodes.Add(temporaryBranchNode);
			nodes.Add(temporaryLeafNode);

			// Bind ConnectToNodes()
			returnValue = tree.ConnectToNodes();
			disposable = returnValue.Bind(out returnValueAsCollection).Subscribe();

			// Delete some of the previously remembered nodes
			nodes.Remove(temporaryLeafNode);
			_ = temporaryLeafNode.Delete.Execute(true).Wait();
			nodes.Remove(temporaryBranchNode);
			_ = temporaryBranchNode.Delete.Execute(true).Wait();
			nodes.Remove(temporaryOtherBranchNode);
			temporaryOtherBranchNode.Delete.Execute(false).Wait();
			if (temporaryOtherRootNode != null)
			{
				nodes.Remove(temporaryOtherRootNode);
				temporaryOtherRootNode.Delete.Execute(false).Wait();
			}

			// Assert
			Util.AssertCollectionsHaveSameItems(nodes, returnValueAsCollection);

			// Cleanup
			disposable.Dispose();
		}

		#endregion


		#region ShouldRecursivelyNotifyNodesOfReparenting

		[Theory]
		[MemberData(nameof(TreeTheoryData))]
		[Trait("TreeClass", "Tree")]
		[Trait("TestingMember", "Property_ShouldRecursivelyNotifyNodesOfReparenting")]
		public static void Changes_to_ShouldRecursivelyNotifyNodesOfReparenting_may_be_reacted_to(Func<Tree<object>> treeSource)
		{
			// Arrange
			Tree<object> tree;
			bool? reactor = null;
			IDisposable disposable;

			// Act
			tree = treeSource();
			disposable = tree
				.WhenAnyValue(x => x.ShouldRecursivelyNotifyNodesOfReparenting)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(x => reactor = x);

			// Act / Assert
			Assert.Equal(tree.ShouldRecursivelyNotifyNodesOfReparenting, reactor);
			tree.ShouldRecursivelyNotifyNodesOfReparenting = false;
			Assert.Equal(tree.ShouldRecursivelyNotifyNodesOfReparenting, reactor);
			tree.ShouldRecursivelyNotifyNodesOfReparenting = true;
			Assert.Equal(tree.ShouldRecursivelyNotifyNodesOfReparenting, reactor);
			tree.ShouldRecursivelyNotifyNodesOfReparenting = false;
			Assert.Equal(tree.ShouldRecursivelyNotifyNodesOfReparenting, reactor);
			tree.ShouldRecursivelyNotifyNodesOfReparenting = false;
			Assert.Equal(tree.ShouldRecursivelyNotifyNodesOfReparenting, reactor);
			tree.ShouldRecursivelyNotifyNodesOfReparenting = true;
			Assert.Equal(tree.ShouldRecursivelyNotifyNodesOfReparenting, reactor);

			// Cleanup
			disposable.Dispose();
		}

		#endregion

	#endregion


	#region SingleRootTree

		#region Constructor

		[Fact]
		[Trait("TreeClass", "SingleRootTree")]
		public static void SingleRootTree_Default_ctor_works()
		{
			Util.AssertDefaultCtorWorks<SingleRootTree<object>>();
		}

		#endregion


		#region RootNode

		[Fact]
		[Trait("TreeClass", "SingleRootTree")]
		public static void SingleRootTree_RootNode_is_initially_null()
		{
			// Arrange
			SingleRootTree<object> tree;

			// Act
			tree = new();

			// Assert
			Assert.Null(tree.RootNode);
		}

		[Fact]
		[Trait("TreeClass", "SingleRootTree")]
		public static void SingleRootTree_RootNode_is_not_null_after_creating_new_root_node()
		{
			// Arrange
			SingleRootTree<object> tree;

			// Act
			tree = new();
			tree.CreateRootNode.Execute(GeneralUseData.SmallString).Wait();

			// Assert
			Assert.NotNull(tree.RootNode);
		}

		[Fact]
		[Trait("TreeClass", "SingleRootTree")]
		public static void SingleRootTree_RootNode_is_null_after_removing_root_node()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				SingleRootTree<object> tree;
				TreeNode<object> rootNode;
				TreeNode<object>? actualValue;

				// Act
				tree = new();
				rootNode = tree.CreateRootNode.Execute(GeneralUseData.SmallString).Wait();
				rootNode.Delete.Execute(false).Wait();
				scheduler.AdvanceBy(2);
				actualValue = tree.RootNode;

				// Assert
				Assert.Null(tree.RootNode);
			});
		}

		[Fact]
		[Trait("TreeClass", "SingleRootTree")]
		public static void SingleRootTree_Changes_to_RootNode_may_be_reacted_to()
		{
			// Arrange
			int timesToCreateAndDeleteRootNode = 5;
			SingleRootTree<object> tree;
			TreeNode<object> rootNode;
			TreeNode<object>? reactor = default;
			IDisposable disposable;

			// Act
			tree = new();

			disposable = tree
				.WhenAnyValue(x => x.RootNode)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(x => reactor = x);

			// Act/Assert
			for (int i = 0; i < timesToCreateAndDeleteRootNode; i++)
			{
				Assert.Equal(tree.RootNode, reactor);
				rootNode = tree.CreateRootNode.Execute(GeneralUseData.SmallString).Wait();
				Assert.Equal(tree.RootNode, reactor);
				rootNode.Delete.Execute(false).Wait();
			}
			disposable.Dispose();

			// Assert
			Assert.Equal(tree.RootNode, reactor);
		}

		[Fact]
		[Trait("TreeClass", "SingleRootTree")]
		public static void SingleRootTree_RootNode_unaffected_after_executing_AddChild_on_non_root_node()
		{
			// Arrange
			SingleRootTree<object> tree;
			TreeNode<object>? rootNodeBefore;
			TreeNode<object>? rootNodeAfter;
			TreeNode<object> rootNode;
			TreeNode<object> nonRootNode;

			// Act
			tree = new();
			rootNode = tree.CreateRootNode.Execute(GeneralUseData.SmallString).Wait();
			nonRootNode = rootNode.AddChild.Execute(GeneralUseData.TinyString).Wait();

			rootNodeBefore = tree.RootNode;
			nonRootNode.AddChild.Execute(GeneralUseData.OneCharString).Wait();
			rootNodeAfter = tree.RootNode;

			// Assert
			Assert.Equal(rootNodeBefore, rootNodeAfter);
		}

		#endregion


		#region CanAddRootNode

		[Fact]
		[Trait("TreeClass", "SingleRootTree")]
		public static void SingleRootTree_CanAddRootNode_is_initially_true()
		{
			// Arrange
			SingleRootTree<object> tree;

			// Act
			tree = new();

			// Assert
			Assert.True(tree.CanAddRootNode);
		}

		[Fact]
		[Trait("TreeClass", "SingleRootTree")]
		public static void SingleRootTree_CanAddRootNode_is_false_after_root_node_is_added()
		{
			// Arrange
			SingleRootTree<object> tree;

			// Act
			tree = new();
			tree.CreateRootNode.Execute(GeneralUseData.SmallString).Wait();

			// Assert
			Assert.False(tree.CanAddRootNode);
		}

		[Fact]
		[Trait("TreeClass", "SingleRootTree")]
		public static void SingleRootTree_CanAddRootNode_is_true_after_root_node_is_deleted()
		{
			new TestScheduler().With(scheduler =>
			{
				// Arrange
				SingleRootTree<object> tree;
				TreeNode<object> rootNode;
				bool actualValue;

				// Act
				tree = new();
				rootNode = tree.CreateRootNode.Execute(GeneralUseData.SmallString).Wait();
				rootNode.Delete.Execute(false).Wait();
				scheduler.AdvanceBy(2);
				actualValue = tree.CanAddRootNode;

				// Assert
				Assert.True(actualValue);
			});
		}

		[Fact]
		[Trait("TreeClass", "SingleRootTree")]
		public static void SingleRootTree_Changes_to_CanAddRootNode_may_be_reacted_to()
		{
			// Arrange
			int timesToCreateAndDeleteRootNode = 5;
			SingleRootTree<object> tree;
			TreeNode<object> rootNode;
			bool? reactor = null;
			IDisposable disposable;

			// Act
			tree = new();

			disposable = tree
				.WhenAnyValue(x => x.CanAddRootNode)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(x => reactor = x);

			// Act/Assert
			for (int i = 0; i < timesToCreateAndDeleteRootNode; i++)
			{
				Assert.Equal(tree.CanAddRootNode, reactor);
				rootNode = tree.CreateRootNode.Execute(GeneralUseData.SmallString).Wait();
				Assert.Equal(tree.CanAddRootNode, reactor);
				rootNode.Delete.Execute(false).Wait();
			}
			disposable.Dispose();

			// Assert
			Assert.Equal(tree.CanAddRootNode, reactor);
		}

		#endregion

		#endregion


	#region MultiRootTree

		#region Constructor

		[Fact]
		[Trait("TreeClass", "MultiRootTree")]
		public static void MultiRootTree_Default_ctor_works()
		{
			Util.AssertDefaultCtorWorks<MultiRootTree<object>>();
		}

		#endregion


		#region CanAddRootNode

		[Fact]
		[Trait("TreeClass", "MultiRootTree")]
		public static void MultiRootTree_CanAddRootNode_is_initially_true()
		{
			// Arrange
			MultiRootTree<object> tree;

			// Act
			tree = new();

			// Assert
			Assert.True(tree.CanAddRootNode);
		}

		[Fact]
		[Trait("TreeClass", "MultiRootTree")]
		public static void MultiRootTree_CanAddRootNode_is_true_with_one_existing_root_node()
		{
			// Arrange
			MultiRootTree<object> tree;

			// Act
			tree = new();
			tree.CreateRootNode.Execute(GeneralUseData.SmallString).Wait();

			// Assert
			Assert.True(tree.CanAddRootNode);
		}

		[Fact]
		[Trait("TreeClass", "MultiRootTree")]
		public static void MultiRootTree_CanAddRootNode_is_true_with_several_existing_root_nodes()
		{
			// Arrange
			MultiRootTree<object> tree;
			int rootNodesToCreate = 5;

			// Act
			tree = new();
			for (int i = 0; i < rootNodesToCreate; i++)
				tree.CreateRootNode.Execute(GeneralUseData.SmallString).Wait();

			// Assert
			Assert.True(tree.CanAddRootNode);
		}

		[Theory]
		[Trait("TreeClass", "MultiRootTree")]
		[InlineData(1, 1)]
		[InlineData(5, 1)]
		[InlineData(5, 3)]
		[InlineData(5, 4)]
		[InlineData(5, 5)]
		public static void MultiRootTree_CanAddRootNode_is_true_after_root_nodes_are_deleted(int rootNodesToCreate, int rootNodesToDelete)
		{
			Assert.True(rootNodesToCreate >= rootNodesToDelete);

			// Arrange
			MultiRootTree<object> tree;
			List<TreeNode<object>> rootNodes = new();

			// Act
			tree = new();
			for (int i = 0; i < rootNodesToCreate; i++)
				rootNodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallString).Wait());
			for (int i = 0; i < rootNodesToDelete; i++)
			{
				TreeNode<object> rootNodeToDelete = rootNodes.Last();
				rootNodes.Remove(rootNodeToDelete);
				rootNodeToDelete.Delete.Execute(false).Wait();
			}

				// Assert
				Assert.True(tree.CanAddRootNode);
		}

		[Fact]
		[Trait("TreeClass", "MultiRootTree")]
		public static void MultiRootTree_Changes_to_CanAddRootNode_may_be_reacted_to()
		{
			// Arrange
			List<SingleRootNodeChange> changes = new()
			{
							// 0
				new(1, 1),	// +1 -1 = 0
				new(3, 1),	// +3 -1 = 2
				new(3, 4),	// +3 -4 = 1
				new(2, 3)	// +2 -3 = 0
			};
			MultiRootTree<object> tree;
			List<TreeNode<object>> rootNodes;
			bool? reactor = null;
			IDisposable disposable;

			// Act
			tree = new();
			rootNodes = new();

			disposable = tree
				.WhenAnyValue(x => x.CanAddRootNode)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(x => reactor = x);

			// Act/Assert
			foreach (SingleRootNodeChange change in changes)
			{
				for (int i = 0; i < change.TimesToAddRootNode; i++)
				{
					Assert.Equal(tree.CanAddRootNode, reactor);
					rootNodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallString).Wait());
				}
				for (int i = 0; i < change.TimesToRemoveRootNode; i++)
				{
					Assert.Equal(tree.CanAddRootNode, reactor);
					TreeNode<object> rootNodeToDelete = rootNodes.Last();
					rootNodes.Remove(rootNodeToDelete);
					rootNodeToDelete.Delete.Execute(false).Wait();
				}
			}
			disposable.Dispose();

			// Assert
			Assert.Equal(tree.CanAddRootNode, reactor);
		}

		#endregion


		#region ConnectToRootNodes

		[Fact]
		[Trait("TreeClass", "MultiRootTree")]
		public static void MultiRootTree_Calling_ConnectToRootNodes_doesnt_throw()
		{
			// Arrange
			MultiRootTree<object> tree;

			// Act
			tree = new();

			// Assert
			Util.AssertCodeDoesNotThrowException(
				() => tree.ConnectToRootNodes()
			);
		}

		[Fact]
		[Trait("TreeClass", "MultiRootTree")]
		public static void MultiRootTree_ConnectToRootNodes_doesnt_return_null()
		{
			// Arrange
			MultiRootTree<object> tree;

			// Act
			tree = new();

			// Assert
			Assert.NotNull(tree.ConnectToRootNodes());
		}

		[Theory]
		[Trait("TreeClass", "MultiRootTree")]
		[InlineData(1)]
		[InlineData(5)]
		public static void MultiRootTree_ConnectToRootNodes_return_value_contains_pre_existing_root_nodes(int rootNodesToCreate)
		{
			Assert.True(rootNodesToCreate > 0);

			// Arrange
			MultiRootTree<object> tree;
			List<TreeNode<object>> preExistingRootNodes = new();
			IObservable<IChangeSet<TreeNode<object>>> returnValue;
			ReadOnlyObservableCollection<TreeNode<object>> returnValueAsCollection;
			IDisposable disposable;

			// Act
			tree = new();
			for (int i = 0; i < rootNodesToCreate; i++)
				preExistingRootNodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());

			returnValue = tree.ConnectToRootNodes();
			disposable = returnValue.Bind(out returnValueAsCollection).Subscribe();

			// Assert
			Util.AssertCollectionsHaveSameItems(preExistingRootNodes, returnValueAsCollection);

			// Cleanup
			disposable.Dispose();
		}

		[Fact]
		[Trait("TreeClass", "MultiRootTree")]
		public static void MultiRootTree_ConnectToRootNodes_return_value_reflects_future_added_root_nodes()
		{
			// Arrange
			MultiRootTree<object> tree;
			List<TreeNode<object>> rootNodes = new();
			IObservable<IChangeSet<TreeNode<object>>> returnValue;
			ReadOnlyObservableCollection<TreeNode<object>> returnValueAsCollection;
			IDisposable disposable;

			// Act
			tree = new();
			rootNodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());

#pragma warning disable IDE0059 // Unnecessary assignment of a value
			returnValue = tree.ConnectToRootNodes();
#pragma warning restore IDE0059 // Unnecessary assignment of a value

			rootNodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());
			rootNodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());

			returnValue = tree.ConnectToRootNodes();
			disposable = returnValue.Bind(out returnValueAsCollection).Subscribe();

			// Assert
			Util.AssertCollectionsHaveSameItems(rootNodes, returnValueAsCollection);

			// Cleanup
			disposable.Dispose();
		}

		[Fact]
		[Trait("TreeClass", "MultiRootTree")]
		public static void MultiRootTree_ConnectToRootNodes_return_value_reflects_future_removed_root_nodes()  
		{
			// Arrange
			MultiRootTree<object> tree;
			List<TreeNode<object>> rootNodes = new();
			IObservable<IChangeSet<TreeNode<object>>> returnValue;
			ReadOnlyObservableCollection<TreeNode<object>> returnValueAsCollection;
			IDisposable disposable;

			// Act
			tree = new();
			rootNodes.Add(tree.CreateRootNode.Execute("Root #1").Wait());
			rootNodes.Add(tree.CreateRootNode.Execute("Root #2").Wait());
			rootNodes.Add(tree.CreateRootNode.Execute("Root #3").Wait());
			rootNodes.Add(tree.CreateRootNode.Execute("Root #4").Wait());
			rootNodes.Add(tree.CreateRootNode.Execute("Root #5").Wait());

			returnValue = tree.ConnectToRootNodes();
			disposable = returnValue.Bind(out returnValueAsCollection).Subscribe();

			for (int i = 0; i < 2; i++)
			{
				TreeNode<object> rootNodeToDelete = rootNodes.Last();
				rootNodes.Remove(rootNodeToDelete);
				_ = rootNodeToDelete.Delete.Execute(false).Wait();
			}

			// Assert
			Util.AssertCollectionsHaveSameItems(rootNodes, returnValueAsCollection);

			// Cleanup
			disposable.Dispose();
		}

		[Fact]
		[Trait("TreeClass", "MultiRootTree")]
		public static void MultiRootTree_ConnectToRootNodes_return_value_unaffected_after_executing_AddChild_on_root_node()
		{
			// Arrange
			MultiRootTree<object> tree;
			List<TreeNode<object>> rootNodes = new();
			IObservable<IChangeSet<TreeNode<object>>> returnValue;
			ReadOnlyObservableCollection<TreeNode<object>> returnValueAsCollection;
			int returnValueAsCollectionCountBefore;
			int returnValueAsCollectionCountAfter;
			int expectedReturnValueAsCollectionCountDelta = 0;
			int actualReturnValueAsCollectionCountDelta;
			IDisposable disposable;

			// Act
			tree = new();
			rootNodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());
			rootNodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());

			returnValue = tree.ConnectToRootNodes();
			disposable = returnValue.Bind(out returnValueAsCollection).Subscribe();

			returnValueAsCollectionCountBefore = returnValueAsCollection.Count;
			_ = rootNodes.Last().AddChild.Execute(GeneralUseData.SmallInt).Wait();
			returnValueAsCollectionCountAfter = returnValueAsCollection.Count;

			actualReturnValueAsCollectionCountDelta =
				returnValueAsCollectionCountAfter - returnValueAsCollectionCountBefore;

			// Assert
			Assert.Equal(expectedReturnValueAsCollectionCountDelta, actualReturnValueAsCollectionCountDelta);

			// Cleanup
			disposable.Dispose();
		}

		[Fact]
		[Trait("TreeClass", "MultiRootTree")]
		public static void MultiRootTree_ConnectToRootNodes_return_value_unaffected_after_executing_AddChild_on_non_root_node()
		{
			// Arrange
			MultiRootTree<object> tree;
			List<TreeNode<object>> rootNodes = new();
			TreeNode<object> nodeToAddChildTo;
			IObservable<IChangeSet<TreeNode<object>>> returnValue;
			ReadOnlyObservableCollection<TreeNode<object>> returnValueAsCollection;
			int returnValueAsCollectionCountBefore;
			int returnValueAsCollectionCountAfter;
			int expectedReturnValueAsCollectionCountDelta = 0;
			int actualReturnValueAsCollectionCountDelta;
			IDisposable disposable;

			// Act
			tree = new();
			rootNodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());
			rootNodes.Add(tree.CreateRootNode.Execute(GeneralUseData.SmallInt).Wait());
			nodeToAddChildTo = rootNodes.Last().AddChild.Execute(GeneralUseData.SmallInt).Wait();

			returnValue = tree.ConnectToRootNodes();
			disposable = returnValue.Bind(out returnValueAsCollection).Subscribe();

			returnValueAsCollectionCountBefore = returnValueAsCollection.Count;
			nodeToAddChildTo.AddChild.Execute(GeneralUseData.SmallInt).Wait();
			returnValueAsCollectionCountAfter = returnValueAsCollection.Count;

			actualReturnValueAsCollectionCountDelta =
				returnValueAsCollectionCountAfter - returnValueAsCollectionCountBefore;

			// Assert
			Assert.Equal(expectedReturnValueAsCollectionCountDelta, actualReturnValueAsCollectionCountDelta);

			// Cleanup
			disposable.Dispose();
		}

		#endregion

		#endregion
	}
}
