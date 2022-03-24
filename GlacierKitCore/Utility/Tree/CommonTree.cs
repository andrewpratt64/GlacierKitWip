using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Utility.Tree
{
    /// <summary>
    /// Describes a hierarchy of nodes. Some implementation is provided for common use.
    /// </summary>
    /// <typeparam name="TreeNode_t">The type of each node</typeparam>
    public class CommonTree<TreeNode_t> : Tree<TreeNode_t>
    {
        public override bool DoNodesKnowParents => true;

        public override bool DoNodesKnowChildren => true;

        public override bool DoNodesKnowSiblings => false;

        public override IEnumerator<TreeNode_t> RootNodes => throw new NotImplementedException();

        public override ITree.ENodeTypeFlags ReparentableNodes => throw new NotImplementedException();

        public override ITree.ENodeTypeFlags DeletableNodes => throw new NotImplementedException();

        public override bool IsSingleRootOnly => throw new NotImplementedException();

        public override bool AreCircularDependenciesAllowed => throw new NotImplementedException();

        public override bool AreMultipleParentsAllowed => throw new NotImplementedException();

        public override bool DoesNodeBelongToTree(TreeNode_t node)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator<TreeNode_t>? GetChildrenOf(TreeNode_t node)
        {
            throw new NotImplementedException();
        }

        public override TreeNode_t? GetParentOf(TreeNode_t node)
        {
            throw new NotImplementedException();
        }


        public CommonTree(
            IEnumerable<TreeNode_t>? rootNodes = null,
            ITree.ENodeTypeFlags reparentableNodes = ITree.ENodeTypeFlags.All,
            ITree.ENodeTypeFlags deletableNodes = ITree.ENodeTypeFlags.NonRootNodes,
            bool isSingleRootOnly = true,
            bool areCircularDependenciesAllowed = false,
            bool areMultipleParentsAllowed = false
        )
        {
            throw new NotImplementedException();
        }
    }
}
