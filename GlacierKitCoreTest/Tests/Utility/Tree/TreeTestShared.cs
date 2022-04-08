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
	internal class TreeTestShared
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
					EParamFlags.UnrelatedNode   | EParamFlags.MultiRoot     | EParamFlags.AddToEmptyTree        | EParamFlags.ExpectFalse,
					EParamFlags.UnrelatedNode   | EParamFlags.MultiRoot     | EParamFlags.AddToPopulatedTree    | EParamFlags.ExpectFalse,

					EParamFlags.UnrelatedNode   | EParamFlags.SingleRoot    | EParamFlags.AddToEmptyTree        | EParamFlags.ExpectFalse,
					EParamFlags.UnrelatedNode   | EParamFlags.SingleRoot    | EParamFlags.AddToPopulatedTree    | EParamFlags.ExpectFalse,

					EParamFlags.RelatedNode     | EParamFlags.MultiRoot     | EParamFlags.AddToEmptyTree        | EParamFlags.ExpectTrue,
					EParamFlags.RelatedNode     | EParamFlags.MultiRoot     | EParamFlags.AddToPopulatedTree    | EParamFlags.ExpectTrue,

					EParamFlags.RelatedNode     | EParamFlags.SingleRoot    | EParamFlags.AddToEmptyTree        | EParamFlags.ExpectTrue,
					EParamFlags.RelatedNode     | EParamFlags.SingleRoot    | EParamFlags.AddToPopulatedTree    | EParamFlags.ExpectFalse,

					EParamFlags.OwnedNode       | EParamFlags.MultiRoot     | EParamFlags.AddToEmptyTree        | EParamFlags.ImpossibleCall,
					EParamFlags.OwnedNode       | EParamFlags.MultiRoot     | EParamFlags.AddToPopulatedTree    | EParamFlags.ExpectFalse,

					EParamFlags.OwnedNode       | EParamFlags.SingleRoot    | EParamFlags.AddToEmptyTree        | EParamFlags.ImpossibleCall,
					EParamFlags.OwnedNode       | EParamFlags.SingleRoot    | EParamFlags.AddToPopulatedTree    | EParamFlags.ExpectFalse
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
			{ }

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
				foreach (CommonNodeForTree rootNode in tree.RootNodes)
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
				Add(DoNothing, DoNothing, DoNothing);
				Add(AddOneRoot, DoNothing, DoNothing);
				Add(DoNothing, AddOneRoot, DoNothing);
				Add(DoNothing, DoNothing, AddOneRoot);
				Add(AddSeveralRoots, AddOneLeaf, RemoveOneRoot);
				Add(AddOneRoot, DoNothing, AddAndRemoveSeveralA);
				Add(AddAndRemoveSeveralA, AddAndRemoveSeveralA, AddAndRemoveSeveralA);
				Add(AddAndRemoveSeveralB, RemoveOneRoot, DoNothing);
			}
		}

		public static _TYPE_RootNodesConnectTheoryData _DATA_RootNodesConnectTheoryData = new();

		#endregion

		#endregion


		#region Tree_creation_utils

		internal static CommonNodeForTree CreateNodeToAdd(
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


		internal static void CreateCommonTree(
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

		internal static ICollection<CommonNodeForTree> GetNodesToTest(
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

		internal static CommonNodeForTree GetMultiNodesToTest(
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

		internal static CommonNodeForTree GetNodeToTest(
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

		internal static ICollection<CommonNodeForTree> GetNodesToTest(
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

		internal static CommonNodeForTree GetNodeToTest(
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

		internal static CommonNodeForTree GetMultiNodesToTest(
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
	}
}
