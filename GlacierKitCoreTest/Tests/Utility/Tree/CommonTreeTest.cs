using GlacierKitCore.Utility.Tree;
using GlacierKitTestShared;
using GlacierKitTestShared.CommonTestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.Utility.Tree
{
	public class CommonTreeTest
	{
#pragma warning disable CA2211 // Non-constant fields should not be visible
		
		public static BoolTheoryData _boolTheoryData = new();
		public static EnumTheoryData<ITree.ENodeSetTypeFlags> _nodeTypeFlagsTheoryData = new();

#pragma warning restore CA2211 // Non-constant fields should not be visible



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

		[Theory]
		[InlineData(	false,	false,	false,	true	)]
		[InlineData(	false,	false,	true,	true	)]
		[InlineData(	false,	true,	false,	false	)]
		//[InlineData(	false,	true,	true,	????	)] <- Can't be empty and own a node at the same time
		[InlineData(	true,	false,	false,	false	)]
		[InlineData(	true,	false,	true,	true	)]
		[InlineData(	true,	true,	false,	false	)]
		//[InlineData(	true,	true,	true,	????	)] <- Can't be empty and own a node at the same time
		public static void Calling_AddRootNodeToTree_returns_expected_value(
			bool isSingleRootOnly,
			bool shouldUseAlreadyOwnedNode,
			bool shouldTreeBeEmptyWhenCalling,
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

			if (!shouldTreeBeEmptyWhenCalling)
			{
				CommonNodeForTree initialNode = new(tree);
				tree.AddRootNodeToTree(initialNode);
				if (shouldUseAlreadyOwnedNode)
					nodeToAdd = initialNode;
				else
					nodeToAdd = new(tree);
			}
			else
				nodeToAdd = new(tree);

			actualReturnValue = tree.AddRootNodeToTree(nodeToAdd);

			// Assert
			Assert.Equal(expectedReturnValue, actualReturnValue);
		}

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

		/*[Theory]
		[InlineData(	false,	ITree.ENodeType.Root,	true	)]
		[InlineData(	false,	ITree.ENodeType.Branch,	true	)]
		[InlineData(	false,	ITree.ENodeType.Leaf,	true	)]
		[InlineData(	true,	ITree.ENodeType.Root,	?	)]
		[InlineData(	true,	ITree.ENodeType.Branch,	?	)]
		[InlineData(	true,	ITree.ENodeType.Leaf,   ?	)]
		public static void Calling_GetChildrenOf_returns_expected_value(
			bool shouldUseAlreadyOwnedNode,
			ITree.ENodeType nodeType,
			bool expectNullReturnValue
		)*/
	}
}
