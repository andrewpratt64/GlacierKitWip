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
    /// <typeparam name="TTree">The type of the tree this is a node for</typeparam>
    public interface INodeForTree<TTree, TTreeNode>
        where TTree : ITree
    {
        /// <summary>
        /// The type of tree this is a node for
        /// </summary>
        public abstract TTree Tree
        { get; }

        /// <summary>
        /// The parent node of this node, if any
        /// </summary>
        public abstract TTreeNode? ParentNode
        { get; }

        /// <summary>
        /// The parent nodes of this node, if any
        /// </summary>
        /*public abstract IEnumerable<TTreeNode>? ParentNodes
        { get; }*/

        /// <summary>
        /// The child nodes of this node, if any
        /// </summary>
        public abstract IEnumerable<TTreeNode>? ChildNodes
        { get; }

        /// <summary>
        /// The siblings of this node, if any
        /// </summary>
        public abstract IEnumerable<TTreeNode>? Siblings
        { get; }


        /// <summary>
        /// Test if this node is a descendant of a given node
        /// </summary>
        /// <param name="node">Node to test if this is a descendant of</param>
        /// <returns>True if the node is a direct or indirect child of <paramref name="node"/>, false otherwise</returns>
        public abstract bool IsDescendantOf(TTreeNode node);


        /// <summary>
        /// Test if this node may be reparented to a given node
        /// </summary>
        /// <param name="node">Node to test if this can become a child of</param>
        /// <returns>True if the node may be reparented to <paramref name="node"/>, false otherwise</returns>
        public abstract bool CanReparentTo(TTreeNode node);


        /// <summary>
        /// Attempt to reparent to a given node
        /// </summary>
        /// <param name="node">Node to reparent to</param>
        /// <returns>True if the node was reparented to <paramref name="node"/>, false otherwise</returns>
        public abstract bool TryReparentTo(TTreeNode node);
    }


    public interface INodeForTree<TTree> : INodeForTree<TTree, INodeForTree<TTree>>
        where TTree : ITree
    { }
}
