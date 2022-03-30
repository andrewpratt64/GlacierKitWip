using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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
    public class CommonNodeForTree :
		ReactiveObject,
		INodeForTree<CommonTree>
    {
        private IEnumerable<CommonNodeForTree> _children;
        private IEnumerable<CommonNodeForTree> _parents;


		[Reactive]
        public CommonTree Tree
		{ get; internal set; }

        public INodeForTree<CommonTree>? ParentNode => _parents.FirstOrDefault();

        public IEnumerable<INodeForTree<CommonTree>>? ParentNodes => _parents;

        public IEnumerable<INodeForTree<CommonTree>>? ChildNodes => _children;

        public IEnumerable<INodeForTree<CommonTree>>? Siblings => throw new NotImplementedException();

        public bool CanReparentTo(INodeForTree<CommonTree> node)
        {
            throw new NotImplementedException();
        }

        public bool IsDescendantOf(INodeForTree<CommonTree> node)
        {
            throw new NotImplementedException();
        }

        public bool TryReparentTo(INodeForTree<CommonTree> node)
        {
            throw new NotImplementedException();
        }


        public CommonNodeForTree(CommonTree tree)
        {
			Tree = tree;
			_children = new List<CommonNodeForTree>();
			_parents = new List<CommonNodeForTree>();
        }


		public IObservable<IChangeSet<CommonNodeForTree>> ParentsConnect()
		{
			throw new NotImplementedException();
		}
		public IObservable<IChangeSet<CommonNodeForTree>> ChildrenConnect()
		{
			throw new NotImplementedException();
		}
	}
}
