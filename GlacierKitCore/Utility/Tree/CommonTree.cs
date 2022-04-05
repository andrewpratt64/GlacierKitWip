using DynamicData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Utility.Tree
{
	/// <summary>
	/// Describes a hierarchy of nodes. Some implementation is provided for common use.
	/// </summary>
	public class CommonTree : ReactiveTree<CommonNodeForTree>
    {
		private SourceList<CommonNodeForTree> _nodes;
		private SourceList<CommonNodeForTree> _rootNodes;


        public override bool DoNodesKnowParents => true;

        public override bool DoNodesKnowChildren => true;

        public override bool DoNodesKnowSiblings => false;

		public override IEnumerable<CommonNodeForTree> RootNodes =>
			_rootNodes.Items;

		public override ITree.ENodeSetTypeFlags ReparentableNodes
		{ get; }

        public override ITree.ENodeSetTypeFlags DeletableNodes
		{ get; }

        public override bool IsSingleRootOnly
		{ get; }

		public override bool AreCircularDependenciesAllowed
		{ get; }

		public override bool AreMultipleParentsAllowed
		{ get; }


		private void AddRootNodeUnsafe(CommonNodeForTree node)
		{
			_nodes.Add(node);
			_rootNodes.Add(node);
		}


		/// <summary>
		/// Test if a node can be added as a root node or not
		/// </summary>
		/// <param name="node">Node to test</param>
		/// <returns>True if <paramref name="node"/> can NOT be added as a root node, false otherwise</returns>
		private bool CannotAddRootNode(CommonNodeForTree node)
		{
			return
				// Prevent adding nodes from other tree types
				node.Tree != this
				// Prevent adding root nodes if the tree only allows one root at a time
				//	and a root already exists
				|| (IsSingleRootOnly && _rootNodes.Count > 0)
				// Prevent adding nodes that already belong to the tree
				|| DoesNodeBelongToTree(node);
		}



		public override bool AddRootNodeToTree(CommonNodeForTree node)
        {
			// Return false if the node can't be added
			if (CannotAddRootNode(node))
				return false;

			// Add the node
			AddRootNodeUnsafe(node); 
			// Node was added, return true
			return true;
        }

        public override bool DoesNodeBelongToTree(CommonNodeForTree node)
        {
            return _nodes.Items.Contains(node);
        }


		public override bool DeleteNode(CommonNodeForTree node, bool shouldDeleteRecursively = true)
		{
			throw new NotImplementedException();
		}


		public override IEnumerable<CommonNodeForTree>? GetChildrenOf(CommonNodeForTree node)
        {
            throw new NotImplementedException();
        }

        public override CommonNodeForTree? GetParentOf(CommonNodeForTree node)
        {
            throw new NotImplementedException();
        }

		/*public override IEnumerable<CommonNodeForTree>? GetParentsOf(CommonNodeForTree node)
		{
			throw new NotImplementedException();
		}*/

		public override IObservable<IChangeSet<CommonNodeForTree>> RootNodesConnect()
		{
			throw new NotImplementedException();
		}


		public CommonTree() : this(default)
        { }

        public CommonTree(
            ITree.ENodeSetTypeFlags reparentableNodes = ITree.ENodeSetTypeFlags.All,
            ITree.ENodeSetTypeFlags deletableNodes = ITree.ENodeSetTypeFlags.NonRootNodes,
            bool isSingleRootOnly = true,
            bool areCircularDependenciesAllowed = false,
            bool areMultipleParentsAllowed = false
        )
        {
			_nodes = new();
			_rootNodes = new();

			ReparentableNodes = reparentableNodes;
			DeletableNodes = deletableNodes;
			IsSingleRootOnly = isSingleRootOnly;
			AreCircularDependenciesAllowed = areCircularDependenciesAllowed;
			AreMultipleParentsAllowed = areMultipleParentsAllowed;
        }
    }
}
