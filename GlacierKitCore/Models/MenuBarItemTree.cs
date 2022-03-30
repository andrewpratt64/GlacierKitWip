using GlacierKitCore.Utility.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// TODO
/*
namespace GlacierKitCore.Models
{
    /// <summary>
    /// Describes the layout of menu items in a menu bar
    /// </summary>
    public class MenuBarItemTree : Tree<MenuBarItemNode>
    {
        private List<MenuBarItemNode> nodes;


        public override ITree.EReparentingRule ReparentingRule => throw new NotImplementedException();

        public override bool IsSingleRootOnly => throw new NotImplementedException();

        public override bool AreCircularDependenciesAllowed => throw new NotImplementedException();

        public override bool AreMultipleParentsAllowed => throw new NotImplementedException();

        public override bool DoNodesKnowParents => throw new NotImplementedException();

        public override bool DoNodesKnowChildren => throw new NotImplementedException();

        public override bool DoNodesKnowSiblings => throw new NotImplementedException();

        public override IEnumerator<MenuBarItemNode> RootNodes => throw new NotImplementedException();

        public override bool DoesNodeBelongToTree(MenuBarItemNode node)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator<MenuBarItemNode>? GetChildrenOf(MenuBarItemNode node)
        {
            throw new NotImplementedException();
        }

        public override MenuBarItemNode? GetParentOf(MenuBarItemNode node)
        {
            throw new NotImplementedException();
        }


        public MenuBarItemTree()
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// A single item within a menu
    /// </summary>
    public abstract class MenuBarItemNode : INodeForTree<MenuBarItemTree, MenuBarItemNode>
    {
        public MenuBarItemTree Tree => throw new NotImplementedException();

        public MenuBarItemNode? ParentNode => throw new NotImplementedException();

        public IEnumerator<MenuBarItemNode>? ChildNodes => throw new NotImplementedException();

        public IEnumerator<MenuBarItemNode>? Siblings => throw new NotImplementedException();

        public bool CanReparentTo(MenuBarItemNode node)
        {
            throw new NotImplementedException();
        }

        public bool IsDescendantOf(MenuBarItemNode node)
        {
            throw new NotImplementedException();
        }

        public bool TryReparentTo(MenuBarItemNode node)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// A single item within a menu that has text
    /// </summary>
    public class MenuBarLabeledItemNode : MenuBarItemNode
    {
        /// <summary>
        /// Text content of the menu item
        /// </summary>
        public string Label
        { get; set; }


        public MenuBarLabeledItemNode(string label)
        {
            Label = label;
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// A seperator within a menu
    /// </summary>
    public class MenuBarSeperatorItemNode : MenuBarItemNode
    {
        public MenuBarSeperatorItemNode()
        {
            throw new NotImplementedException();
        }
    }
}
	*/