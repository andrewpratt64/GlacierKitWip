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

namespace GlacierKitCoreTest.Tests.Utility.Tree
{
	public class CommonTreeTest
	{
		#region TheoryData

		public static BoolTheoryData _boolTheoryData = new();
		public static EnumTheoryData<ITree.ENodeType> _nodeTypeTheoryData = new();
		public static EnumTheoryData<ITree.ENodeSetTypeFlags> _nodeTypeFlagsTheoryData = new();
		public static EnumTheoryData<ITree.ETreeAndNodeRelationshipType> _treeAndNodeRelationshipTypeTheoryData = new();

		#region AddRootNodeToTreeTheoryData

		public class _TYPE_AddRootNodeToTreeTheoryData : TheoryData<ITree.ETreeAndNodeRelationshipType, bool, bool, bool>
		{
			[Flags]
			public enum EParamFlags
			{
				// Tree-node relationship type
				UnrelatedNode = 1,
				RelatedNode = 2,
				OwnedNode = 4,
				// Tree root rules
				MultiRoot = 8,
				SingleRoot = 16,
				// Tree state when adding new root node
				AddToEmptyTree = 32,
				AddToPopulatedTree = 64,
				// Expected return value
				ExpectTrue = 128,
				ExpectFalse = 256,
				ImpossibleCall = 0
			}

			internal _TYPE_AddRootNodeToTreeTheoryData(IEnumerable<EParamFlags> data)
			{
				foreach (EParamFlags rowFlags in data)
				{
					Debug.Assert(
						((rowFlags & EParamFlags.UnrelatedNode) ^ (rowFlags & EParamFlags.RelatedNode) ^ (rowFlags & EParamFlags.OwnedNode))
						!= 0
					);
					Debug.Assert(
						((rowFlags & EParamFlags.MultiRoot) ^ (rowFlags & EParamFlags.SingleRoot))
						!= 0
					);
					Debug.Assert(
						((rowFlags & EParamFlags.AddToEmptyTree) ^ (rowFlags & EParamFlags.AddToPopulatedTree))
						!= 0
					);
					Debug.Assert(
						(rowFlags & EParamFlags.ExpectTrue & EParamFlags.ExpectFalse)
						== 0
					);

					if (!(rowFlags.Equals(EParamFlags.ExpectTrue) || rowFlags.HasFlag(EParamFlags.ExpectFalse)))
						continue;

					ITree.ETreeAndNodeRelationshipType relationshipType = ITree.ETreeAndNodeRelationshipType.DifferentTypes;
					if (rowFlags.HasFlag(EParamFlags.RelatedNode))
						relationshipType = ITree.ETreeAndNodeRelationshipType.SameTypes;
					else if (rowFlags.HasFlag(EParamFlags.OwnedNode))
						relationshipType = ITree.ETreeAndNodeRelationshipType.SameTypesAndOwned;

					Add(
						relationshipType,
						rowFlags.HasFlag(EParamFlags.AddToEmptyTree),
						rowFlags.HasFlag(EParamFlags.SingleRoot),
						rowFlags.HasFlag(EParamFlags.ExpectTrue)
					);
				}
			}


			internal _TYPE_AddRootNodeToTreeTheoryData()
				: this(new List<EParamFlags>()
				{
					EParamFlags.UnrelatedNode	| EParamFlags.MultiRoot		| EParamFlags.AddToEmptyTree		| EParamFlags.ExpectFalse,
					EParamFlags.UnrelatedNode	| EParamFlags.MultiRoot		| EParamFlags.AddToPopulatedTree	| EParamFlags.ExpectFalse,

					EParamFlags.UnrelatedNode	| EParamFlags.SingleRoot	| EParamFlags.AddToEmptyTree		| EParamFlags.ExpectFalse,
					EParamFlags.UnrelatedNode	| EParamFlags.SingleRoot	| EParamFlags.AddToPopulatedTree	| EParamFlags.ExpectFalse,

					EParamFlags.RelatedNode		| EParamFlags.MultiRoot		| EParamFlags.AddToEmptyTree		| EParamFlags.ExpectTrue,
					EParamFlags.RelatedNode		| EParamFlags.MultiRoot		| EParamFlags.AddToPopulatedTree	| EParamFlags.ExpectTrue,

					EParamFlags.RelatedNode		| EParamFlags.SingleRoot	| EParamFlags.AddToEmptyTree		| EParamFlags.ExpectTrue,
					EParamFlags.RelatedNode		| EParamFlags.SingleRoot	| EParamFlags.AddToPopulatedTree	| EParamFlags.ExpectFalse,

					EParamFlags.OwnedNode		| EParamFlags.MultiRoot		| EParamFlags.AddToEmptyTree		| EParamFlags.ImpossibleCall,
					EParamFlags.OwnedNode		| EParamFlags.MultiRoot		| EParamFlags.AddToPopulatedTree	| EParamFlags.ExpectFalse,

					EParamFlags.OwnedNode		| EParamFlags.SingleRoot	| EParamFlags.AddToEmptyTree		| EParamFlags.ImpossibleCall,
					EParamFlags.OwnedNode		| EParamFlags.SingleRoot	| EParamFlags.AddToPopulatedTree	| EParamFlags.ExpectFalse
				})
			{ }
		}
		public static _TYPE_AddRootNodeToTreeTheoryData _DATA_AddRootNodeToTreeTheoryData = new();

		#endregion

		#region RootNodesConnectTheoryData

		
		public class _TYPE_RootNodesConnectTheoryData : TheoryData<Action<CommonTree>, Action<CommonTree>, Action<CommonTree>>
		{
			/// <param name="preConnectAction">Before tree.RootNodesConnect()</param>
			/// <param name="preBindAction">After tree.RootNodesConnect(), before tree.RootNodesConnect().Bind()</param>
			/// <param name="postBindAction">After tree.RootNodesConnect().Bind()</param>
			private new void Add(
				Action<CommonTree> preConnectAction,
				Action<CommonTree> preBindAction,
				Action<CommonTree> postBindAction
			)
			{
				base.Add(preConnectAction, preBindAction, postBindAction);
			}


			private static void DoNothing(CommonTree _)
			{}

			private static void AddOneRoot(CommonTree tree)
			{
				tree.AddRootNodeToTree(new(tree));
			}

			private static void RemoveOneRoot(CommonTree tree)
			{
				Debug.Assert(tree.RootNodes.Any(), "Can't run RemoveOneRoot if no root nodes exist");
				tree.DeleteNode(tree.RootNodes.First());
			}

			private static void AddSeveralRoots(CommonTree tree)
			{
				tree.AddRootNodeToTree(new(tree));
				tree.AddRootNodeToTree(new(tree));
				tree.AddRootNodeToTree(new(tree));
			}

			private static void RemoveSeveralRoots(CommonTree tree)
			{
				Debug.Assert(tree.RootNodes.Any(), "Can't run RemoveSeveralRoots if no root nodes exist");

				tree.DeleteNode(tree.RootNodes.First());
				if (tree.RootNodes.Any())
					tree.DeleteNode(tree.RootNodes.First());
				if (tree.RootNodes.Any())
					tree.DeleteNode(tree.RootNodes.First());
			}

			private static void AddOneBranch(CommonTree tree)
			{
				Debug.Assert(tree.RootNodes.Any(), "Can't run AddOneBranch if no root nodes exist");
				new CommonNodeForTree(tree).TryReparentTo(tree.RootNodes.First());
			}

			private static void RemoveOneBranch(CommonTree tree)
			{
				Debug.Assert(tree.RootNodes.Any(), "Can't run RemoveOneBranch if no root nodes (and therefore no branch nodes) exist");
				foreach(CommonNodeForTree rootNode in tree.RootNodes)
				{
					IEnumerable<CommonNodeForTree>? children = tree.GetChildrenOf(rootNode);
					if (children?.Any() ?? false)
					{
						tree.DeleteNode(children.First());
						return;
					}
				}
				throw new XunitException("Can't run RemoveOneBranch if no branch nodes exist");
			}

			private static void AddOneLeaf(CommonTree tree)
			{
				Debug.Assert(tree.RootNodes.Any(), "Can't run AddOneLeaf if no root nodes exist");
				CommonNodeForTree parent = tree.RootNodes.First();
				CommonNodeForTree? child = parent;
				do
				{
					parent = child;
					child = tree.GetChildrenOf(parent)?.FirstOrDefault();
				}
				while (child != null);

				new CommonNodeForTree(tree).TryReparentTo(parent);
			}

			private static void RemoveOneLeaf(CommonTree tree)
			{
				Debug.Assert(tree.RootNodes.Any(), "Can't run RemoveOneLeaf if no root nodes exist");
				CommonNodeForTree parent = tree.RootNodes.First();
				CommonNodeForTree? child = parent;
				do
				{
					parent = child;
					child = tree.GetChildrenOf(parent)?.FirstOrDefault();
				}
				while (child != null);

				tree.DeleteNode(parent);
			}

			private static void AddAndRemoveSeveralA(CommonTree tree)
			{
				AddSeveralRoots(tree);
				RemoveOneRoot(tree);
				AddOneLeaf(tree);
				AddOneBranch(tree);
			}

			private static void AddAndRemoveSeveralB(CommonTree tree)
			{
				AddOneRoot(tree);
				RemoveOneRoot(tree);
				AddOneRoot(tree);
				AddOneLeaf(tree);
				AddOneRoot(tree);
				AddOneLeaf(tree);
				AddOneLeaf(tree);
				RemoveOneLeaf(tree);
				AddOneLeaf(tree);
				AddOneBranch(tree);
			}


			public _TYPE_RootNodesConnectTheoryData()
			{
				Add(	DoNothing,				DoNothing,				DoNothing);
				Add(	AddOneRoot,				DoNothing,				DoNothing);
				Add(	DoNothing,				AddOneRoot,				DoNothing);
				Add(	DoNothing,				DoNothing,				AddOneRoot);
				Add(	AddSeveralRoots,		AddOneLeaf,				RemoveOneRoot);
				Add(	AddOneRoot,				DoNothing,				AddAndRemoveSeveralA);
				Add(	AddAndRemoveSeveralA,	AddAndRemoveSeveralA,	AddAndRemoveSeveralA);
				Add(	AddAndRemoveSeveralB,	RemoveOneRoot,			DoNothing);
			}
		}

		public static _TYPE_RootNodesConnectTheoryData _DATA_RootNodesConnectTheoryData = new();

		#endregion

		#region DeleteNodeTheoryData

		public class _TYPE_DeleteNodeTheoryData : TheoryData<ITree.ENodeType, ITree.ETreeAndNodeRelationshipType, ITree.ENodeSetTypeFlags, bool, bool>
		{
			private static bool GetExpectedReturnValue(
				ITree.ENodeType typeToDelete,
				ITree.ETreeAndNodeRelationshipType relationshipOfNodeToDelete,
				ITree.ENodeSetTypeFlags deletableNodes,
				bool /*shouldDeleteRecursively*/_)
			{
				// shouldDeleteRecursively is discarded since it shouldn't effect the result
				return
					// Can only delete nodes we own
					relationshipOfNodeToDelete == ITree.ETreeAndNodeRelationshipType.SameTypesAndOwned
					// Can only delete deletable nodes
					&& (
						  (typeToDelete == ITree.ENodeType.Root		&& (!deletableNodes.HasFlag(ITree.ENodeSetTypeFlags.RootNodes	)))
						||(typeToDelete == ITree.ENodeType.Branch	&& (!deletableNodes.HasFlag(ITree.ENodeSetTypeFlags.BranchNodes	)))
						||(typeToDelete == ITree.ENodeType.Leaf		&& (!deletableNodes.HasFlag(ITree.ENodeSetTypeFlags.LeafNodes	)))
					)
				;
			}

			private static IEnumerable<object> FlattenTheoryData(IEnumerable<object[]> theoryData)
			{
				return
				from row in theoryData
				from item in row
				select item;
			}


			public _TYPE_DeleteNodeTheoryData()
			{
				// Sorry for the ugly nested code here :)
				foreach (object typeToDelete in FlattenTheoryData(_nodeTypeTheoryData))
				{
				foreach (object relationshipOfNodeToDelete in FlattenTheoryData(_treeAndNodeRelationshipTypeTheoryData))
				{
				foreach (object deletableNodes in FlattenTheoryData(_nodeTypeFlagsTheoryData))
				{
				foreach (object shouldDeleteRecursivelyRow in FlattenTheoryData(_boolTheoryData))
				{
					Add(
						(ITree.ENodeType)typeToDelete,
						(ITree.ETreeAndNodeRelationshipType)relationshipOfNodeToDelete,
						(ITree.ENodeSetTypeFlags)deletableNodes,
						(bool)shouldDeleteRecursivelyRow,
						GetExpectedReturnValue(
							(ITree.ENodeType)typeToDelete,
							(ITree.ETreeAndNodeRelationshipType)relationshipOfNodeToDelete,
							(ITree.ENodeSetTypeFlags)deletableNodes,
							(bool)shouldDeleteRecursivelyRow
						)
					);
				} // shouldDeleteRecursivelyRow
				} // deletableNodes
				} // relationshipOfNodeToDelete
				} // typeToDelete
			}
		}

		public static readonly _TYPE_DeleteNodeTheoryData _DATA_DeleteNodeTheoryData = new();

		#endregion


		#endregion

		#region Tree_creation_utils

		private static CommonNodeForTree CreateNodeToAdd(
			CommonTree tree,
			ITree.ETreeAndNodeRelationshipType nodeType,
			bool shouldTreeBeEmptyWhenCalling)
		{
			CommonNodeForTree? initialNode = null;
			CommonNodeForTree nodeToAdd;

			if (!shouldTreeBeEmptyWhenCalling)
			{
				initialNode = new(tree);
				tree.AddRootNodeToTree(initialNode);
			}

			if (nodeType == ITree.ETreeAndNodeRelationshipType.DifferentTypes)
				nodeToAdd = new(new CommonTree());
			else if (nodeType == ITree.ETreeAndNodeRelationshipType.SameTypes)
				nodeToAdd = new(tree);
			else
			{
				Debug.Assert(initialNode != null);
				nodeToAdd = initialNode;
			}

			return nodeToAdd;
		}


		private static void CreateCommonTree(
			CommonTree tree,
			out ICollection<CommonNodeForTree> rootNodes,
			out ICollection<CommonNodeForTree> branchNodes,
			out ICollection<CommonNodeForTree> leafNodes,
			int childrenPerLevel = 1
		)
		{
			Debug.Assert(childrenPerLevel > 0);

			rootNodes = new List<CommonNodeForTree>();
			branchNodes = new List<CommonNodeForTree>();
			leafNodes = new List<CommonNodeForTree>();

			for (int rootIndex = 1; rootIndex < (tree.IsSingleRootOnly ? 1 : childrenPerLevel); rootIndex++)
			{
				CommonNodeForTree root = new(tree);
				rootNodes.Add(root);
				if (!tree.AddRootNodeToTree(root))
					throw new XunitException("An unexpected error occurred inside GetNodeToTest while adding a root node");

				for (int branchIndex = 1; branchIndex < childrenPerLevel; branchIndex++)
				{
					CommonNodeForTree branch = new(tree);
					branchNodes.Add(branch);
					if (!branch.TryReparentTo(root))
						throw new XunitException("An unexpected error occurred inside GetNodeToTest while reparenting a branch node");

					for (int leafIndex = 1; leafIndex < childrenPerLevel; leafIndex++)
					{
						CommonNodeForTree leaf = new(tree);
						leafNodes.Add(leaf);
						if (!leaf.TryReparentTo(branch))
							throw new XunitException("An unexpected error occurred inside GetNodeToTest while reparenting a leaf node");
					}
				}
			}
		}

		private static ICollection<CommonNodeForTree> GetNodesToTest(
			CommonTree tree,
			ITree.ENodeType nodeTypeToTest,
			out ICollection<CommonNodeForTree> rootNodes,
			out ICollection<CommonNodeForTree> branchNodes,
			out ICollection<CommonNodeForTree> leafNodes,
			int childrenPerLevel = 1
		)
		{
			CreateCommonTree(
				tree,
				out rootNodes,
				out branchNodes,
				out leafNodes,
				childrenPerLevel
			);

			return nodeTypeToTest switch
			{
				ITree.ENodeType.Root => rootNodes,
				ITree.ENodeType.Branch => branchNodes,
				ITree.ENodeType.Leaf => leafNodes,
				_ => throw new XunitException("If this is ran, then not all possible values accounted for in GetNodeToTest()!"),
			};
		}

		private static CommonNodeForTree GetMultiNodesToTest(
			CommonTree tree,
			ITree.ENodeType singleNodeTypeToTest,
			ITree.ENodeType levelTypeToTest,
			out ICollection<CommonNodeForTree> rootNodes,
			out ICollection<CommonNodeForTree> branchNodes,
			out ICollection<CommonNodeForTree> leafNodes,
			out ICollection<CommonNodeForTree> nodeLevelToTest,
			int childrenPerLevel = 1
		)
		{
			nodeLevelToTest = GetNodesToTest(
				tree,
				singleNodeTypeToTest,
				out rootNodes,
				out branchNodes,
				out leafNodes,
				childrenPerLevel
			);

			return levelTypeToTest switch
			{
				ITree.ENodeType.Root => rootNodes.First(),
				ITree.ENodeType.Branch => branchNodes.First(),
				ITree.ENodeType.Leaf => leafNodes.First(),
				_ => throw new XunitException("If this is ran, then not all possible values accounted for in GetMultiNodesToTest()!"),
			};
		}

		private static CommonNodeForTree GetNodeToTest(
			CommonTree tree,
			ITree.ENodeType nodeTypeToTest,
			out ICollection<CommonNodeForTree> rootNodes,
			out ICollection<CommonNodeForTree> branchNodes,
			out ICollection<CommonNodeForTree> leafNodes,
			int childrenPerLevel = 1
		)
		{
			return GetNodesToTest(
				tree,
				nodeTypeToTest,
				out rootNodes,
				out branchNodes,
				out leafNodes,
				childrenPerLevel
			).First();
		}

		private static ICollection<CommonNodeForTree> GetNodesToTest(
			CommonTree tree,
			ITree.ENodeType nodeTypeToTest,
			int childrenPerLevel = 1
		)
		{
			return GetNodesToTest(
				tree: tree,
				nodeTypeToTest: nodeTypeToTest,
				rootNodes: out ICollection<CommonNodeForTree> _,
				branchNodes: out ICollection<CommonNodeForTree> _,
				leafNodes: out ICollection<CommonNodeForTree> _,
				childrenPerLevel: childrenPerLevel
			);
		}

		private static CommonNodeForTree GetNodeToTest(
			CommonTree tree,
			ITree.ENodeType nodeTypeToTest,
			int childrenPerLevel = 1
		)
		{
			return GetNodeToTest(
				tree: tree,
				nodeTypeToTest: nodeTypeToTest,
				rootNodes: out ICollection<CommonNodeForTree> _,
				branchNodes: out ICollection<CommonNodeForTree> _,
				leafNodes: out ICollection<CommonNodeForTree> _,
				childrenPerLevel: childrenPerLevel
			);
		}

		private static CommonNodeForTree GetMultiNodesToTest(
			CommonTree tree,
			ITree.ENodeType singleNodeTypeToTest,
			ITree.ENodeType levelTypeToTest,
			out ICollection<CommonNodeForTree> nodeLevelToTest,
			int childrenPerLevel = 1
		)
		{
			nodeLevelToTest = GetNodesToTest(
				tree,
				singleNodeTypeToTest,
				rootNodes: out ICollection<CommonNodeForTree> rootNodes,
				branchNodes: out ICollection<CommonNodeForTree> branchNodes,
				leafNodes: out ICollection<CommonNodeForTree> leafNodes,
				childrenPerLevel
			);

			return levelTypeToTest switch
			{
				ITree.ENodeType.Root => rootNodes.First(),
				ITree.ENodeType.Branch => branchNodes.First(),
				ITree.ENodeType.Leaf => leafNodes.First(),
				_ => throw new XunitException("If this is ran, then not all possible values accounted for in GetMultiNodesToTest()!"),
			};
		}

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
