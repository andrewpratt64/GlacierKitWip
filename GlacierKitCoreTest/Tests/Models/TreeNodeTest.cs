using GlacierKitCore.Models;
using GlacierKitTestShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.Models
{
	public class TreeNodeTest
	{
		#region Theory_data
#pragma warning disable IDE1006 // Naming Styles

		private class _TYPE_DeleteTheoryData : TheoryData<Tree<object>, bool>
		{
			public _TYPE_DeleteTheoryData()
			{
				Add(new SingleRootTree<object>(), false);
				Add(new SingleRootTree<object>(), true);
				Add(new MultiRootTree<object>(), false);
				Add(new MultiRootTree<object>(), true);
			}
		}

		internal readonly _TYPE_TreeTheoryData<object> _DATA_TreeTheoryData = new();
		private readonly _TYPE_DeleteTheoryData _DATA_DeleteTheoryData = new();

#pragma warning restore IDE1006 // Naming Styles
		#endregion


		#region ContainingTree

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void ContainingTree_matches_tree_it_was_created_from(Tree<object> tree)
		{
			throw new NotImplementedException();
		}

		#endregion


		#region Value

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Value_matches_given_parameter(Tree<object?> tree, object? valueToUse)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Changes_to_Value_may_be_reacted_to(Tree<object?> tree, object? initialValue, object? newValue)
		{
			throw new NotImplementedException();
		}

		#endregion


		#region Parent

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Parent_is_initially_null_for_root_nodes(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Parent_is_initially_not_null_for_non_root_nodes(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Parent_is_null_after_becoming_a_root_node(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Parent_is_not_null_after_becoming_a_non_root_node(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Changes_to_Parent_may_be_reacted_to(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion


		#region DesiredParent

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void DesiredParent_is_initally_empty(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void DesiredParent_is_empty_after_executing_Reparent(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Changes_to_DesiredParent_may_be_reacted_to(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion


		#region AddChild

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void AddChild_CanExecute_is_true_for_root_node(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void AddChild_CanExecute_is_true_for_non_root_node(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_AddChild_on_root_node_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_AddChild_on_non_root_node_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_AddChild_on_root_node_returns_non_null_value(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_AddChild_on_non_root_node_returns_non_null_value(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion


		#region Delete

		[Theory]
		[MemberData(nameof(_DATA_DeleteTheoryData))]
		public static void Delete_CanExecute_is_true_for_root_node(Tree<object?> tree, bool shouldDeleteRecursively)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_DeleteTheoryData))]
		public static void Delete_CanExecute_is_true_for_non_root_node(Tree<object?> tree, bool shouldDeleteRecursively)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_DeleteTheoryData))]
		public static void Executing_Delete_on_root_node_doesnt_throw(Tree<object?> tree, bool shouldDeleteRecursively)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_DeleteTheoryData))]
		public static void Executing_Delete_on_non_root_node_doesnt_throw(Tree<object?> tree, bool shouldDeleteRecursively)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Delete_with_false_moves_direct_children_up_one_level(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Delete_with_false_moves_indirect_children_up_one_level(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Delete_with_true_deletes_all_direct_children(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Delete_with_true_deletes_all_indirect_children(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion


		#region Reparent/CanReparent
		// TODO: Test **BOTH** TreeNode.CanReparent and TreeNode.Reparent.CanExecute. They should == each other.

		#region Reparent_from_root

		#region Reparent_from_root_to_child

		#region Reparent_from_root_to_child_->_direct_child

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_root_node_whos_DesiredParent_is_a_direct_child(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_root_node_whos_DesiredParent_is_a_direct_child_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Reparent_from_root_to_child_->_grandchild
		// NOTE: "Grandchild" refers to an indirect child, i.e. a direct child of a direct child

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_root_node_whos_DesiredParent_is_a_grandchild(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_root_node_whos_DesiredParent_is_a_grandchild_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#endregion

		#region Reparent_from_root_to_non_child

		#region Reparent_from_root_to_non_child_->_empty
		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_root_node_whos_DesiredParent_is_empty(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_root_node_whos_DesiredParent_is_empty_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Reparent_from_root_to_child_->_itself

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_root_node_whos_DesiredParent_is_itself(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_root_node_whos_DesiredParent_is_itself_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Reparent_from_root_to_child_->_another_root

		[Fact]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_root_node_whos_DesiredParent_is_another_root(/*Impossible in a SingleRootTree*/)
		{
			throw new NotImplementedException();
		}

		[Fact]
		public static void Executing_Reparent_on_root_node_whos_DesiredParent_is_another_root_doesnt_throw(/*Impossible in a SingleRootTree*/)
		{
			throw new NotImplementedException();
		}

		#endregion

		#endregion

		#endregion


		#region Reparent_from_non_root

		#region Reparent_from_non_root_to_parent

		#region Reparent_from_non_root_to_parent_->_direct_parent

		#region Reparent_from_non_root_to_parent_->_root_direct_parent

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_non_root_node_whos_DesiredParent_is_a_root_direct_parent(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_root_direct_parent_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Reparent_from_non_root_to_parent_->_non_root_direct_parent

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_non_root_node_whos_DesiredParent_is_a_non_root_direct_parent(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_non_root_direct_parent_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#endregion

		#region Reparent_from_non_root_to_parent_->_grandparent
		// NOTE: "Grandparent" refers to an indirect parent, i.e. a direct parent of a direct parent

		#region Reparent_from_non_root_to_parent_->_root_grandparent

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_true_for_non_root_node_whos_DesiredParent_is_a_root_grandparent(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_root_grandparent_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Reparent_from_non_root_to_parent_->_non_root_grandparent

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_true_for_non_root_node_whos_DesiredParent_is_a_non_root_grandparent(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_non_root_grandparent_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#endregion

		#region Reparent_from_non_root_to_parent_->_uncle
		// NOTE: "Uncle" refers to a sibling of a direct parent

		#region Reparent_from_non_root_to_parent_->_root_uncle

		[Fact]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_true_for_non_root_node_whos_DesiredParent_is_a_root_uncle(/*Impossible in a SingleRootTree*/)
		{
			throw new NotImplementedException();
		}

		[Fact]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_root_uncle_doesnt_throw(/*Impossible in a SingleRootTree*/)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Reparent_from_non_root_to_parent_->_non_root_uncle

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_true_for_non_root_node_whos_DesiredParent_is_a_non_root_uncle(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_non_root_uncle_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#endregion

		#endregion


		#region Reparent_from_non_root_to_child

		#region Reparent_from_non_root_to_child_->_direct_child

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_non_root_node_whos_DesiredParent_is_a_direct_child(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_direct_child_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Reparent_from_non_root_to_child_->_grandchild
		// NOTE: "Grandchild" refers to an indirect child, i.e. a direct child of a direct child

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_non_root_node_whos_DesiredParent_is_a_grandchild(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_grandchild_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Reparent_from_non_root_to_child_->_nephew
		// NOTE: "Nephew" refers to a direct child of a sibling

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_non_root_node_whos_DesiredParent_is_a_nephew(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_nephew_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#endregion


		#region Reparent_from_non_root_misc

		#region Reparent_from_non_root_to_misc_->_empty

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_non_root_node_whos_DesiredParent_is_empty(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_empty_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Reparent_from_non_root_to_misc_->_itself

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_false_for_non_root_node_whos_DesiredParent_is_itself(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_itself_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Reparent_from_non_root_to_misc_->_a_sibling

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_true_for_non_root_node_whos_DesiredParent_is_a_sibling(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_sibling_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Reparent_from_non_root_to_misc_->_a_cousin
		// NOTE: "Cousin" refers to a direct child of a sibling of a direct parent

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Both_CanReparent_and_Reparent_CanExecute_are_true_for_non_root_node_whos_DesiredParent_is_a_cousin(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void Executing_Reparent_on_non_root_node_whos_DesiredParent_is_a_cousin_doesnt_throw(Tree<object?> tree)
		{
			throw new NotImplementedException();
		}

		#endregion

		#endregion

		#endregion


		#endregion


		#region IsRootNode

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void IsRootNode_is_true_for_a_node_created_as_a_root_node(Tree<object> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void IsRootNode_is_false_for_a_node_created_as_a_non_root_node(Tree<object> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void IsRootNode_is_true_for_a_node_after_becoming_a_root_node(Tree<object> tree)
		{
			throw new NotImplementedException();
		}

		[Theory]
		[MemberData(nameof(_DATA_TreeTheoryData))]
		public static void IsRootNode_is_false_for_node_after_becoming_a_non_root_node(Tree<object> tree)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
