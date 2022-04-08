using DynamicData;
using GlacierKitCore.Utility.Tree;
using GlacierKitTestShared;
using GlacierKitTestShared.CommonTestData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using static GlacierKitCore.Utility.CollectionUtils;
using static GlacierKitCoreTest.Tests.Utility.Tree.TreeTestShared;

namespace GlacierKitCoreTest.Tests.Utility.Tree
{
	public class CommonTreeTest
	{
		#region TheoryData

		private static readonly _TYPE_CommonTree_DeleteNodeTheoryData _DATA_DeleteNodeTheoryData
			= new();

		#endregion



		#region Ctor_and_Properties

		[Fact]
		public static void Default_ctor_works()
		{
			Util.AssertDefaultCtorWorks<CommonTree>();
		}

		[Fact]
		public static void Ctor_with_args_doesnt_throw()
		{
			// Arrange
			ITree.ENodeSetTypeFlags reparentableNodes = ITree.ENodeSetTypeFlags.NonRootNodes;
			ITree.ENodeSetTypeFlags deletableNodes = ITree.ENodeSetTypeFlags.BranchNodes;
			bool isSingleRootOnly = false;
			bool areCircularDependenciesAllowed = false;
			bool areMultipleParentsAllowed = true;

			// Assert
			Util.AssertCodeDoesNotThrowException(() =>
			{
				_ = new CommonTree(
					reparentableNodes,
					deletableNodes,
					isSingleRootOnly,
					areCircularDependenciesAllowed,
					areMultipleParentsAllowed
				);
			});
		}

		[Theory]
		[MemberData(nameof(_nodeTypeFlagsTheoryData))]
		public static void ReparentableNodes_is_set_by_ctor(ITree.ENodeSetTypeFlags reparentableNodes)
		{
			// Arrange
			CommonTree tree;
			//ITree.ENodeSetTypeFlags reparentableNodes = ITree.ENodeSetTypeFlags.NonRootNodes;
			ITree.ENodeSetTypeFlags deletableNodes = ITree.ENodeSetTypeFlags.BranchNodes;
			bool isSingleRootOnly = false;
			bool areCircularDependenciesAllowed = false;
			bool areMultipleParentsAllowed = true;

			// Act
			tree = new CommonTree(
				reparentableNodes,
				deletableNodes,
				isSingleRootOnly,
				areCircularDependenciesAllowed,
				areMultipleParentsAllowed
			);

			// Assert
			Assert.Equal(reparentableNodes, tree.ReparentableNodes);
		}

		[Theory]
		[MemberData(nameof(_nodeTypeFlagsTheoryData))]
		public static void DeletableNodes_is_set_by_ctor(ITree.ENodeSetTypeFlags deletableNodes)
		{
			// Arrange
			CommonTree tree;
			ITree.ENodeSetTypeFlags reparentableNodes = ITree.ENodeSetTypeFlags.NonRootNodes;
			//ITree.ENodeSetTypeFlags deletableNodes = ITree.ENodeSetTypeFlags.BranchNodes;
			bool isSingleRootOnly = false;
			bool areCircularDependenciesAllowed = false;
			bool areMultipleParentsAllowed = true;

			// Act
			tree = new CommonTree(
				reparentableNodes,
				deletableNodes,
				isSingleRootOnly,
				areCircularDependenciesAllowed,
				areMultipleParentsAllowed
			);

			// Assert
			Assert.Equal(deletableNodes, tree.DeletableNodes);
		}

		[Theory]
		[MemberData(nameof(_boolTheoryData))]
		public static void IsSingleRootOnly_is_set_by_ctor(bool isSingleRootOnly)
		{
			// Arrange
			CommonTree tree;
			ITree.ENodeSetTypeFlags reparentableNodes = ITree.ENodeSetTypeFlags.NonRootNodes;
			ITree.ENodeSetTypeFlags deletableNodes = ITree.ENodeSetTypeFlags.BranchNodes;
			//bool isSingleRootOnly = false;
			bool areCircularDependenciesAllowed = false;
			bool areMultipleParentsAllowed = true;

			// Act
			tree = new CommonTree(
				reparentableNodes,
				deletableNodes,
				isSingleRootOnly,
				areCircularDependenciesAllowed,
				areMultipleParentsAllowed
			);

			// Assert
			Assert.Equal(isSingleRootOnly, tree.IsSingleRootOnly);
		}

		[Theory]
		[MemberData(nameof(_boolTheoryData))]
		public static void AreCircularDependenciesAllowed_is_set_by_ctor(bool areCircularDependenciesAllowed)
		{
			// Arrange
			CommonTree tree;
			ITree.ENodeSetTypeFlags reparentableNodes = ITree.ENodeSetTypeFlags.NonRootNodes;
			ITree.ENodeSetTypeFlags deletableNodes = ITree.ENodeSetTypeFlags.BranchNodes;
			bool isSingleRootOnly = false;
			//bool areCircularDependenciesAllowed = false;
			bool areMultipleParentsAllowed = true;

			// Act
			tree = new CommonTree(
				reparentableNodes,
				deletableNodes,
				isSingleRootOnly,
				areCircularDependenciesAllowed,
				areMultipleParentsAllowed
			);
			
			// Assert
			Assert.Equal(areCircularDependenciesAllowed, tree.AreCircularDependenciesAllowed);
		}

		[Theory]
		[MemberData(nameof(_boolTheoryData))]
		public static void AreMultipleParentsAllowed_is_set_by_ctor(bool areMultipleParentsAllowed)
		{
			// Arrange
			CommonTree tree;
			ITree.ENodeSetTypeFlags reparentableNodes = ITree.ENodeSetTypeFlags.NonRootNodes;
			ITree.ENodeSetTypeFlags deletableNodes = ITree.ENodeSetTypeFlags.BranchNodes;
			bool isSingleRootOnly = false;
			bool areCircularDependenciesAllowed = false;
			//bool areMultipleParentsAllowed = true;
			
			// Act
			tree = new CommonTree(
				reparentableNodes,
				deletableNodes,
				isSingleRootOnly,
				areCircularDependenciesAllowed,
				areMultipleParentsAllowed
			);

			// Assert
			Assert.Equal(areMultipleParentsAllowed, tree.AreMultipleParentsAllowed);
		}

		[Fact]
		public static void RootNodes_is_not_initially_null()
		{
			// Arrange
			CommonTree tree;

			// Act
			tree = new();

			// Assert
			Assert.NotNull(tree.RootNodes);
		}

		[Fact]
		public static void RootNodes_is_initially_empty()
		{
			// Arrange
			CommonTree tree;

			// Act
			tree = new();

			// Assert
			Assert.Empty(tree.RootNodes);
		}

		#endregion

		#region Methods


		#region AddRootNodeToTree

		[Theory]
		[MemberData(nameof(_DATA_AddRootNodeToTreeTheoryData))]
		public static void Calling_AddRootNodeToTree_returns_expected_value(
			ITree.ETreeAndNodeRelationshipType nodeType,
			bool shouldTreeBeEmptyWhenCalling,
			bool isSingleRootOnly,
			bool expectedReturnValue
		)
		{
			// Arrange
			CommonTree tree;
			ITree.ENodeSetTypeFlags reparentableNodes = ITree.ENodeSetTypeFlags.NonRootNodes;
			ITree.ENodeSetTypeFlags deletableNodes = ITree.ENodeSetTypeFlags.BranchNodes;
			bool areCircularDependenciesAllowed = false;
			bool areMultipleParentsAllowed = true;
			
			CommonNodeForTree nodeToAdd;
			bool actualReturnValue;


			// Act
			tree = new CommonTree(
				reparentableNodes,
				deletableNodes,
				isSingleRootOnly,
				areCircularDependenciesAllowed,
				areMultipleParentsAllowed
			);

			nodeToAdd = CreateNodeToAdd(tree, nodeType, shouldTreeBeEmptyWhenCalling);

			actualReturnValue = tree.AddRootNodeToTree(nodeToAdd);

			// Assert
			Assert.Equal(expectedReturnValue, actualReturnValue);
		}

		[Theory]
		[MemberData(nameof(_DATA_AddRootNodeToTreeTheoryData))]
		public static void Size_of_RootNodes_increments_only_when_calling_AddRootNodeToTree_returns_true(
			ITree.ETreeAndNodeRelationshipType nodeType,
			bool shouldTreeBeEmptyWhenCalling,
			bool isSingleRootOnly,
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
			bool _
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
		)
		{
			// Arrange
			CommonTree tree;
			ITree.ENodeSetTypeFlags reparentableNodes = ITree.ENodeSetTypeFlags.NonRootNodes;
			ITree.ENodeSetTypeFlags deletableNodes = ITree.ENodeSetTypeFlags.BranchNodes;
			bool areCircularDependenciesAllowed = false;
			bool areMultipleParentsAllowed = true;

			CommonNodeForTree nodeToAdd;
			bool returnValueWhenAdding;
			int sizeBeforeAdding;
			int sizeAfterAdding;


			// Act
			tree = new CommonTree(
				reparentableNodes,
				deletableNodes,
				isSingleRootOnly,
				areCircularDependenciesAllowed,
				areMultipleParentsAllowed
			);

			nodeToAdd = CreateNodeToAdd(tree, nodeType, shouldTreeBeEmptyWhenCalling);

			sizeBeforeAdding = tree.RootNodes.Count();
			returnValueWhenAdding = tree.AddRootNodeToTree(nodeToAdd);
			sizeAfterAdding = tree.RootNodes.Count();

			// Assert
			Assert.Equal(
				expected: returnValueWhenAdding
				? sizeBeforeAdding + 1
				: sizeBeforeAdding,
				actual: sizeAfterAdding
			);
		}

		#endregion

		#region DeleteNode

		[Theory]
		[MemberData(nameof(_DATA_DeleteNodeTheoryData))]
		public static void Calling_DeleteNode_returns_expected_value(
			ITree.ENodeType typeToDelete,
			ITree.ETreeAndNodeRelationshipType relationshipOfNodeToDelete,
			ITree.ENodeSetTypeFlags deletableNodes,
			bool shouldDeleteRecursively,
			bool expectedReturnValue
		)
		{
			// Arrange
			CommonTree tree;
			CommonNodeForTree nodeToDelete;
			bool actualReturnValue;


			// Act
			tree = new(deletableNodes: deletableNodes);
			
			CommonNodeForTree ownedNodeWithTypeToDelete = GetNodeToTest(tree, typeToDelete);
			nodeToDelete = relationshipOfNodeToDelete switch
			{
				ITree.ETreeAndNodeRelationshipType.DifferentTypes => new(new()),
				ITree.ETreeAndNodeRelationshipType.SameTypes => new(tree),
				ITree.ETreeAndNodeRelationshipType.SameTypesAndOwned => ownedNodeWithTypeToDelete,
				_ => throw new XunitException("Calling_DeleteNode_returns_expected_value dosen't test all possible ETreeAndNodeRelationshipType values")
			};

			actualReturnValue = tree.DeleteNode(nodeToDelete, shouldDeleteRecursively);


			// Assert
			Assert.Equal(expectedReturnValue, actualReturnValue);
		}

		#endregion

		#region DoesNodeBelongToTree

		[Theory]
		[InlineData(	false,	false,	false	)]
		[InlineData(	false,	true,	false	)]
		[InlineData(	true,	false,	true	)]
		//[InlineData(	true,	true,	????	)] <- Can't be empty and own a node at the same time
		public static void Calling_DoesNodeBelongToTree_returns_expected_value(
			bool shouldUseAlreadyOwnedNode,
			bool shouldTreeBeEmptyWhenCalling,
			bool expectedReturnValue
		)
		{
			// Arrange
			CommonTree tree;
			ITree.ENodeSetTypeFlags reparentableNodes = ITree.ENodeSetTypeFlags.NonRootNodes;
			ITree.ENodeSetTypeFlags deletableNodes = ITree.ENodeSetTypeFlags.BranchNodes;
			bool isSingleRootOnly = false;
			bool areCircularDependenciesAllowed = false;
			bool areMultipleParentsAllowed = true;

			CommonNodeForTree nodeToTest;
			bool actualReturnValue;


			// Act
			tree = new CommonTree(
				reparentableNodes,
				deletableNodes,
				isSingleRootOnly,
				areCircularDependenciesAllowed,
				areMultipleParentsAllowed
			);

			if (!shouldTreeBeEmptyWhenCalling)
			{
				CommonNodeForTree initialNode = new(tree);
				tree.AddRootNodeToTree(initialNode);
				if (shouldUseAlreadyOwnedNode)
					nodeToTest = initialNode;
				else
					nodeToTest = new(tree);
			}
			else
				nodeToTest = new(tree);

			actualReturnValue = tree.DoesNodeBelongToTree(nodeToTest);


			// Assert
			Assert.Equal(expectedReturnValue, actualReturnValue);
		}

		#endregion

		#region GetChildrenOf

		[Theory]
		[InlineData(
			ITree.ENodeType.Root,
			1,
			ECollectionSizeFlags.SingleItem
		)]
		[InlineData(
			ITree.ENodeType.Root,
			3,
			ECollectionSizeFlags.SeveralItems
		)]
		[InlineData(
			ITree.ENodeType.Branch,
			1,
			ECollectionSizeFlags.SingleItem
		)]
		[InlineData(
			ITree.ENodeType.Branch,
			3,
			ECollectionSizeFlags.SeveralItems
		)]
		[InlineData(
			ITree.ENodeType.Leaf,
			1,
			ECollectionSizeFlags.Empty
		)]
		[InlineData(
			ITree.ENodeType.Leaf,
			3,
			ECollectionSizeFlags.Empty
		)]
		public static void Calling_GetChildrenOf_returns_with_expected_size(
			ITree.ENodeType nodeType,
			int nodesPerLevel,
			ECollectionSizeFlags expectedReturnValueFlags
		)
		{
			// Arrange
			CommonTree tree;
			CommonNodeForTree nodeToTest;

			IEnumerable<CommonNodeForTree>? actualReturnValue;
			ECollectionSizeFlags actualReturnValueFlags;


			// Act
			tree = new CommonTree();
			nodeToTest = GetNodeToTest(tree, nodeType, nodesPerLevel);

			actualReturnValue = tree.GetChildrenOf(nodeToTest);
			actualReturnValueFlags = GetCollectionSizeFlagsOf(actualReturnValue?.ToList());


			// Assert
			Assert.Equal(expectedReturnValueFlags, actualReturnValueFlags);
		}

		[Theory]
		[InlineData(
			ITree.ENodeType.Root,
			1,
			ITree.ENodeType.Branch
		)]
		[InlineData(
			ITree.ENodeType.Root,
			3,
			ITree.ENodeType.Branch
		)]
		[InlineData(
			ITree.ENodeType.Branch,
			1,
			ITree.ENodeType.Leaf
		)]
		[InlineData(
			ITree.ENodeType.Branch,
			3,
			ITree.ENodeType.Leaf
		)]
		[InlineData(
			ITree.ENodeType.Leaf,
			1
		)]
		[InlineData(
			ITree.ENodeType.Leaf,
			3
		)]
		public static void Calling_GetChildrenOf_returns_expected_values(
			ITree.ENodeType nodeType,
			int nodesPerLevel,
			ITree.ENodeType? expectedValuesFrom = null
		)
		{
			// Arrange
			CommonTree tree;
			CommonNodeForTree nodeToTest;

			ICollection<CommonNodeForTree> expectedReturnValue;
			IEnumerable<CommonNodeForTree>? actualReturnValue;

			// Act
			tree = new CommonTree();
			nodeToTest = GetMultiNodesToTest(
				tree,
				singleNodeTypeToTest: nodeType,
				levelTypeToTest: expectedValuesFrom ?? ITree.ENodeType.Root,
				nodeLevelToTest: out expectedReturnValue,
				childrenPerLevel: nodesPerLevel
			);
			actualReturnValue = tree.GetChildrenOf(nodeToTest);

			// Assert
			if (expectedValuesFrom == null)
				Assert.Null(actualReturnValue);
			else
				Util.AssertCollectionsHaveSameItems(expectedReturnValue, actualReturnValue!);
		}

		#endregion

		#region GetParentOf

		[Theory]
		[InlineData(
			ITree.ENodeType.Root,
			1
		)]
		[InlineData(
			ITree.ENodeType.Root,
			3
		)]
		[InlineData(
			ITree.ENodeType.Branch,
			1,
			ITree.ENodeType.Root
		)]
		[InlineData(
			ITree.ENodeType.Branch,
			3,
			ITree.ENodeType.Root
		)]
		[InlineData(
			ITree.ENodeType.Leaf,
			1,
			ITree.ENodeType.Branch
		)]
		[InlineData(
			ITree.ENodeType.Leaf,
			3,
			ITree.ENodeType.Branch
		)]
		public static void Calling_GetParentOf_returns_expected_value(
			ITree.ENodeType nodeType,
			int nodesPerLevel,
			ITree.ENodeType? expectedValueFromType = null
		)
		{
			// Arrange
			CommonTree tree;
			CommonNodeForTree nodeToTest;

			ICollection<CommonNodeForTree> expectedReturnValueLevel;
			CommonNodeForTree? actualReturnValue;

			// Act
			tree = new CommonTree();
			nodeToTest = GetMultiNodesToTest(
				tree,
				singleNodeTypeToTest: nodeType,
				levelTypeToTest: expectedValueFromType ?? ITree.ENodeType.Root,
				nodeLevelToTest: out expectedReturnValueLevel,
				childrenPerLevel: nodesPerLevel
			);
			actualReturnValue = tree.GetParentOf(nodeToTest);

			// Assert
			if (expectedValueFromType == null)
				Assert.Null(actualReturnValue);
			else
				Assert.Contains(actualReturnValue, expectedReturnValueLevel);
		}

		#endregion

		#region RootNodesConnect

		[Theory]
		[MemberData(nameof(_DATA_RootNodesConnectTheoryData))]
		public static void Bound_RootNodesConnect_matches_RootNodes_items(
			Action<CommonTree> preConnectAction,
			Action<CommonTree> preBindAction,
			Action<CommonTree> postBindAction
		)
		{
			// Arrange
			CommonTree tree;
			IObservable<IChangeSet<CommonNodeForTree>> actualValue;
			ReadOnlyObservableCollection<CommonNodeForTree> actualBoundValue;

			// Act
			tree = new();
			preConnectAction(tree);
			actualValue = tree.RootNodesConnect();
			preBindAction(tree);
			actualValue.Bind(out actualBoundValue);
			postBindAction(tree);

			// Assert
			Assert.True(tree.RootNodes.SequenceEqual(actualBoundValue));
		}

		#endregion


		#endregion
	}
}
