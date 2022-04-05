using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Utility.Tree
{
    /// <summary>
    /// Describes a hierarchy of nodes
    /// </summary>
    public interface ITree
    {
        /// <summary>
		/// Represents the type of a single node
		/// </summary>
        public enum ENodeType
		{
            /// <summary>
            /// Root node; i.e. nodes with no parents
            /// </summary>
            Root = 1,
            /// <summary>
            /// Branch node; i.e. nodes with both parents and children
            /// </summary>
            Branch = 2,
            /// <summary>
            /// Leaf node; i.e. nodes with no children
            /// </summary>
            Leaf = 4
        }

        /// <summary>
        /// Bitflags for addressing nodes based on their type of node
        /// </summary>
        [Flags]
        public enum ENodeSetTypeFlags
        {
            /// <summary>
            /// Excludes all nodes
            /// </summary>
            None = 0,
            /// <summary>
            /// Includes root nodes; i.e. nodes with no parents
            /// </summary>
            RootNodes = 1,
            /// <summary>
            /// Includes branch nodes; i.e. nodes with both parents and children
            /// </summary>
            BranchNodes = 2,
            /// <summary>
            /// Includes leaf nodes; i.e. nodes with no children
            /// </summary>
            LeafNodes = 4,
            /// <summary>
            /// Includes all nodes
            /// </summary>
            All = RootNodes | BranchNodes | LeafNodes,
            /// <summary>
            /// Includes all nodes that are not root nodes
            /// </summary>
            NonRootNodes = All & ~RootNodes,
            /// <summary>
            /// Includes all nodes that are not branch nodes
            /// </summary>
            NonBranchNodes = All & ~BranchNodes,
            /// <summary>
            /// Includes all nodes that are not leaf nodes
            /// </summary>
            NonLeafNodes = All & ~LeafNodes
        }

		/// <summary>
		/// The types of relationships between a tree and a tree node
		/// </summary>
		public enum ETreeAndNodeRelationshipType
		{
			/// <summary>
			/// The tree node uses a different type of tree than the tree being compared
			/// </summary>
			DifferentTypes,
			/// <summary>
			/// The tree node uses the same type of tree than the tree being compared, but does not
			/// belong to the tree being compared
			/// </summary>
			SameTypes,
			/// <summary>
			/// The tree node uses the same type of tree than the tree being compared, and
			/// belongs to the tree being compared
			/// </summary>
			SameTypesAndOwned
		}


        /// <summary>
        /// Determines what nodes may be reparented
        /// </summary>
        public abstract ENodeSetTypeFlags ReparentableNodes
        { get; }

        /// <summary>
        /// Determines what nodes may be deleted
        /// </summary>
        public abstract ENodeSetTypeFlags DeletableNodes
        { get; }

        /// <summary>
        /// If true, only one node may be the root of the tree at a time
        /// </summary>
        public abstract bool IsSingleRootOnly
        { get; }

        /// <summary>
        /// If true, root nodes may be swapped with other nodes
        /// </summary>
        public bool IsReparentingRootAllowed => ReparentableNodes.HasFlag(ENodeSetTypeFlags.All);

        /// <summary>
        /// If true, nodes may be parented to their children
        /// </summary>
        public abstract bool AreCircularDependenciesAllowed
        { get; }

        /// <summary>
        /// If true, a single node may have more than one parent at a time
        /// </summary>
        public abstract bool AreMultipleParentsAllowed
        { get; }

        /// <summary>
        /// If true, each node holds a reference to it's own parent
        /// </summary>
        public abstract bool DoNodesKnowParents
        { get; }

        /// <summary>
        /// If true, each node holds a reference to it's own children
        /// </summary>
        public abstract bool DoNodesKnowChildren
        { get; }

        /// <summary>
        /// If true, each node holds a reference to it's own siblings
        /// </summary>
        public abstract bool DoNodesKnowSiblings
        { get; }
    }


    /// <summary>
    /// Describes a hierarchy of nodes
    /// </summary>
    /// <typeparam name="TTreeNode">The type of each node</typeparam>
    public interface ITree<TTreeNode> : ITree
    {
        /// <summary>
        /// Allows enumeration over the root of the tree
        /// </summary>
        public abstract IEnumerable<TTreeNode> RootNodes
        { get; }

        /// <summary>
        /// Accessor for a node's parent
        /// </summary>
        /// <param name="node">Node who's parent to get</param>
        /// <returns>The node's parent, or null if no parent exists</returns>
        public abstract TTreeNode? GetParentOf(TTreeNode node);

		/// <summary>
		/// Accessor for a node's parents
		/// </summary>
		/// <param name="node">Node who's parents to get</param>
		/// <returns>The node's parent, or null if no parents exist</returns>
		//public abstract IEnumerable<TTreeNode>? GetParentsOf(TTreeNode node);

		/// <summary>
		/// Accessor for a node's children
		/// </summary>
		/// <param name="node">Node who's children to get</param>
		/// <returns>The node's children</returns>
		public abstract IEnumerable<TTreeNode>? GetChildrenOf(TTreeNode node);

        /// <summary>
        /// Test if a given node is part of the tree
        /// </summary>
        /// <param name="node">Node to test</param>
        /// <returns>True if <paramref name="node"/> belongs to this tree, false otherwise</returns>
        public abstract bool DoesNodeBelongToTree(TTreeNode node);

        /// <summary>
        /// Attempts to add a new root node to the tree
        /// </summary>
        /// <param name="node">The new root node to add</param>
        /// <returns>True if <paramref name="node"/> was added to the tree, false otherwise</returns>
        public abstract bool AddRootNodeToTree(TTreeNode node);

		/// <summary>
		/// Attempts to remove a node from the tree
		/// </summary>
		/// <param name="node">The node to delete</param>
		/// <returns>True if node was deleted, false otherwise</returns>
		public abstract bool DeleteNode(TTreeNode node, bool shouldDeleteRecursively);
    }


	/// <summary>
	/// Reactive class that describes a hierarchy of nodes
	/// </summary>
	/// <typeparam name="TTreeNode">The type of each node</typeparam>
	public abstract class ReactiveTree<TTreeNode> :
		ReactiveObject,
		ITree<TTreeNode>
	{
		public abstract IObservable<IChangeSet<TTreeNode>> RootNodesConnect();


		public abstract IEnumerable<TTreeNode> RootNodes { get; }
		public abstract ITree.ENodeSetTypeFlags ReparentableNodes { get; }
		public abstract ITree.ENodeSetTypeFlags DeletableNodes { get; }
		public abstract bool IsSingleRootOnly { get; }
		public abstract bool AreCircularDependenciesAllowed { get; }
		public abstract bool AreMultipleParentsAllowed { get; }
		public abstract bool DoNodesKnowParents { get; }
		public abstract bool DoNodesKnowChildren { get; }
		public abstract bool DoNodesKnowSiblings { get; }

		public abstract bool DeleteNode(TTreeNode node, bool shouldDeleteRecursively = true);
		public abstract bool AddRootNodeToTree(TTreeNode node);
		public abstract bool DoesNodeBelongToTree(TTreeNode node);
		public abstract IEnumerable<TTreeNode>? GetChildrenOf(TTreeNode node);
		public abstract TTreeNode? GetParentOf(TTreeNode node);
		//public abstract IEnumerable<TTreeNode>? GetParentsOf(TTreeNode node);
	}
}
