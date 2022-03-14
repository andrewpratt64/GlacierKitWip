using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Utility.Tree
{
    // TODO
    public interface ITree<TreeNode_t>
    {
        /// <summary>
        /// Allows enumeration over the root of the tree
        /// </summary>
        public IEnumerator<TreeNode_t> RootNodes
        { get; }
    }
}
