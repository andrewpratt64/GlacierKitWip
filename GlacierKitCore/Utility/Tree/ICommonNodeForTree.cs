using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Utility.Tree
{
    public interface ICommonNodeForTree<Tree_t, TreeNode_t> : INodeForTree<Tree_t, TreeNode_t>
        where Tree_t : ITree
    {

        public new IEnumerator<TreeNode_t>? Siblings => throw new NotImplementedException();

        public new bool CanReparentTo(TreeNode_t node)
        {
            throw new NotImplementedException();
        }

        public new bool IsDescendantOf(TreeNode_t node)
        {
            throw new NotImplementedException();
        }

        public new bool TryReparentTo(TreeNode_t node)
        {
            throw new NotImplementedException();
        }
    }
}
