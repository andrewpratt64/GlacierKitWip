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
        /// If true, only one node may be the root of the tree at a time
        /// </summary>
        public abstract bool IsSingleRootOnly
        { get; }

        /// <summary>
        /// If true, root nodes may be swapped with other nodes
        /// </summary>
        public abstract bool IsReparentingRootAllowed
        { get; }

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
    /// <typeparam name="TreeNode_t">The type of each node</typeparam>
    public abstract class Tree<TreeNode_t> : ITree
    {
        public abstract bool IsSingleRootOnly { get; }
        public abstract bool IsReparentingRootAllowed { get; }
        public abstract bool AreCircularDependenciesAllowed { get; }
        public abstract bool AreMultipleParentsAllowed { get; }
        public abstract bool DoNodesKnowParents { get; }
        public abstract bool DoNodesKnowChildren { get; }
        public abstract bool DoNodesKnowSiblings { get; }


        /// <summary>
        /// Allows enumeration over the root of the tree
        /// </summary>
        public abstract IEnumerator<TreeNode_t> RootNodes
        { get; }

        /// <summary>
        /// Accessor for a node's parent
        /// </summary>
        /// <param name="node">Node who's parent to get</param>
        /// <returns>The node's parent, or null if no parent exists</returns>
        public abstract TreeNode_t? GetParentOf(TreeNode_t node);

        /// <summary>
        /// Accessor for a node's children
        /// </summary>
        /// <param name="node">Node who's children to get</param>
        /// <returns>The node's children</returns>
        public abstract IEnumerator<TreeNode_t>? GetChildrenOf(TreeNode_t node);

        /// <summary>
        /// Test if a given node is part of the tree
        /// </summary>
        /// <param name="node">Node to test</param>
        /// <returns>True if <paramref name="node"/> belongs to this tree, false otherwise</returns>
        public abstract bool DoesNodeBelongToTree(TreeNode_t node);
    }
}
