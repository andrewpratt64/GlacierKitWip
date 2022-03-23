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
        public TreeNode_t? ParentNode
        { get; }

        /// <summary>
        /// The child nodes of this node, if any
        /// </summary>
        public IEnumerator<TreeNode_t>? ChildNodes
        { get; }

        /// <summary>
        /// The siblings of this node, if any
        /// </summary>
        public IEnumerator<TreeNode_t>? Siblings
        { get; }
    }
}
