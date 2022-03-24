using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Utility.Tree
{
    /// <summary>
    /// Describes a type that is usable as a node for a tree
    /// </summary>
    /// <typeparam name="Tree_t">The type of the tree this is a node for</typeparam>
    /// <typeparam name="TreeNode_t">The type of each node</typeparam>
    public interface INodeForTree<Tree_t, TreeNode_t>
        where Tree_t : ITree
    {
        /// <summary>
        /// The type of tree this is a node for
        /// </summary>
        public abstract Tree_t Tree
        { get; }

        /// <summary>
        /// The parent node of this node, if any
        /// </summary>
        public abstract TreeNode_t? ParentNode
        { get; }

        /// <summary>
        /// The parent nodes of this node, if any
        /// </summary>
        public abstract IEnumerable<TreeNode_t>? ParentNodes
        { get; }

        /// <summary>
        /// The child nodes of this node, if any
        /// </summary>
        public abstract IEnumerator<TreeNode_t>? ChildNodes
        { get; }

        /// <summary>
        /// The siblings of this node, if any
        /// </summary>
        public abstract IEnumerator<TreeNode_t>? Siblings
        { get; }


        /// <summary>
        /// Test if this node is a descendant of a given node
        /// </summary>
        /// <param name="node">Node to test if this is a descendant of</param>
        /// <returns>True if the node is a direct or indirect child of <paramref name="node"/>, false otherwise</returns>
        public abstract bool IsDescendantOf(TreeNode_t node);


        /// <summary>
        /// Test if this node may be reparented to a given node
        /// </summary>
        /// <param name="node">Node to test if this can become a child of</param>
        /// <returns>True if the node may be reparented to <paramref name="node"/>, false otherwise</returns>
        public abstract bool CanReparentTo(TreeNode_t node);


        /// <summary>
        /// Attempt to reparent to a given node
        /// </summary>
        /// <param name="node">Node to reparent to</param>
        /// <returns>True if the node was reparented to <paramref name="node"/>, false otherwise</returns>
        public abstract bool TryReparentTo(TreeNode_t node);
    }
}
