using Avalonia.Data;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Models
{
	/// <summary>
	/// A single node within a <see cref="Tree{TNodeValue}"/>
	/// </summary>
	/// <typeparam name="TNodeValue">The type of data this node represents</typeparam>
	/// <remarks>This is used instead of DynamicData's <see cref="DynamicData.Node{TObject, TKey}"/> to restrict how nodes are constructed and to avoid re-generating trees</remarks>
	public class TreeNode<TNodeValue> : ReactiveObject
	{
		#region Private_fields

		private readonly SourceList<TreeNode<TNodeValue>> _childNodes;

		#endregion


		#region Public_methods

		/// <summary>
		/// Connect to and observe changes of this node's direct children
		/// </summary>
		/// <returns>An observable that emits the change set of child nodes</returns>
		public IObservable<IChangeSet<TreeNode<TNodeValue>>> ConnectToChildNodes()
		{
			return _childNodes.Connect();
		}


		/// <summary>
		/// Test if this node is a direct or indirect child of another node
		/// </summary>
		/// <param name="node">Node to test if this is a child of or not</param>
		/// <returns>True if this node is a child of <paramref name="node"/>, false otherwise.</returns>
		public bool IsChildOf(TreeNode<TNodeValue> node)
		{
			// Return false if this node was passed; a node can't be a child of itself
			if (node == this)
				return false;

			// Walk up the tree until a root node is passed
			TreeNode<TNodeValue>? nextNode = this;
			do
			{
				nextNode = nextNode.Parent;
				// Short-circuit and return true if one of the parent nodes are the one we're looking for
				if (nextNode == node)
					return true;
			}
			while (nextNode != null);

			// If a root node was passed, we aren't a child
			return false;
			
		}

		#endregion


		#region Public_properties

		/// <summary>
		/// The tree this node is a part of
		/// </summary>
		public Tree<TNodeValue> ContainingTree { get; }

		/// <summary>
		/// The value this node represents
		/// </summary>
		[Reactive]
		public TNodeValue Value { get; set; }

		/// <summary>
		/// This node's parent, or null if this is a root node
		/// </summary>
		[Reactive]
		public TreeNode<TNodeValue>? Parent { get; set; }

		/// <summary>
		/// The node that this node would like to reparent to, if any
		/// </summary>
		/// <remarks>
		/// Whether or not this node may be reparented to it's desired node depends on <seealso cref="CanReparent"/>.
		/// <br/>
		/// <list type="table">
		///		<listheader>
		///			<term>If this property is...</term>
		///			<description>Then...</description>
		///		</listheader>
		///		<item>
		///				<term>Empty</term>
		///				<description>This node does not wish to be reparented</description>
		///		</item>
		///		<item>
		///				<term>Not empty with a null value</term>
		///				<description>This node wishes to become a root node</description>
		///		</item>
		///		<item>
		///				<term>Not empty with a non-null value</term>
		///				<description>This node wishes to become a child node of another node</description>
		///		</item>
		/// </list>
		/// </remarks>
		[Reactive]
		public ReactiveOptional<TreeNode<TNodeValue>?> DesiredParent { get; set; }

		#endregion


		#region Public_commands

		/// <summary>
		/// Creates a new child node
		/// <list type="bullet">
		///		<item>
		///				<term>Input</term>
		///				<description>The value of the node to create</description>
		///		</item>
		///		<item>
		///				<term>Output</term>
		///				<description>The new child node</description>
		///		</item>
		/// </list>
		/// </summary>
		/// <remarks></remarks>
		public ReactiveCommand<TNodeValue, TreeNode<TNodeValue>> AddChild
		{ get; }

		/// <summary>
		/// 
		/// <list type="bullet">
		///		<item>
		///				<term>Input</term>
		///				<description>True to delete the children of this node recursively as well, false to move all children up to this node's depth instead</description>
		///		</item>
		///		<item>
		///				<term>Output</term>
		///				<description>(none)</description>
		///		</item>
		/// </list>
		/// </summary>
		/// <remarks></remarks>
		public ReactiveCommand<bool, Unit> Delete
		{ get; }

		/// <summary>
		/// 
		/// <list type="bullet">
		///		<item>
		///				<term>Input</term>
		///				<description>True to delete the children of this node recursively as well, false to move all children up to this node's depth instead</description>
		///		</item>
		///		<item>
		///				<term>Output</term>
		///				<description>(none)</description>
		///		</item>
		/// </list>
		/// </summary>
		/// <remarks></remarks>
		public ReactiveCommand<Unit, Unit> Reparent
		{ get; }

		#endregion


		#region Public_OAPHs

		/// <summary>
		/// True if this is a root node, false otherwise
		/// </summary>
		[ObservableAsProperty]
		public bool IsRootNode{ get; }

		/// <summary>
		/// True if this node may reparent to the node stated by <see cref="DesiredParent"/>, false otherwise
		/// </summary>
		[ObservableAsProperty]
		public bool CanReparent { get; }

		#endregion


		#region Constructor

		/// <summary>
		/// Creates a new instance of the <see cref="TreeNode{TNodeValue}"/> class
		/// </summary>
		/// <param name="containingTree"><inheritdoc cref="ContainingTree"/></param>
		/// <param name="nodeValue"><inheritdoc cref="Value"/></param>
		/// <param name="parent"><inheritdoc cref="Parent"/></param>
		internal TreeNode(Tree<TNodeValue> containingTree, TNodeValue nodeValue, TreeNode<TNodeValue>? parent)
		{
			// Init fields
			_childNodes = new();

			// Init properties
			ContainingTree = containingTree;
			Value = nodeValue;
			Parent = parent;
			DesiredParent = ReactiveOptional<TreeNode<TNodeValue>?>.MakeEmpty();


			#region Init_OAPHs

			// Have CanReparent react to changes in DesiredParent
			IObservable<bool> canReparentObservable = this
				.WhenAnyValue(x => x.DesiredParent.LastValue)
				.Select(_ => CanReparentNow())
				.ObserveOn(RxApp.MainThreadScheduler);
			canReparentObservable.ToPropertyEx(this, x => x.CanReparent, initialValue: false, scheduler: RxApp.MainThreadScheduler);

			// Have IsRootNode react to changes in Parent
			//	This is a root node when it's "Parent" property is set to null
			this.WhenAnyValue(x => x.Parent)
				.ObserveOn(RxApp.MainThreadScheduler)
				.Select(x => x == null)
				.ToPropertyEx(this, x => x.IsRootNode);

			#endregion


			#region Init_commands

			AddChild = ReactiveCommand.Create<TNodeValue, TreeNode<TNodeValue>>(
				execute: nodeValue =>
				{
					// Create a new node as a child of this node
					TreeNode<TNodeValue> newChildNode = new(
						containingTree: ContainingTree,
						nodeValue: nodeValue,
						parent: this
					);
					// Remember the child
					_childNodes.Add(newChildNode);
					// Return the child
					return newChildNode;
				}
			);

			Delete = ReactiveCommand.Create<bool, Unit>(
				execute: shouldDeleteRecursively =>
				{
					// Delete all direct children if this is a recursive delete
					if (shouldDeleteRecursively)
					{
						foreach (TreeNode<TNodeValue> childNode in _childNodes.Items)
						{
							childNode.Delete.Execute(true).Wait();
						}
					}
					// Move all direct children up a depth level if this is not a recursive delete
					else
					{
						foreach (TreeNode<TNodeValue> childNode in _childNodes.Items)
						{
							childNode.DesiredParent.LastValue = Parent;
							childNode.Reparent.Execute().Wait();
						}
					}

					// Unregister node from tree
					ContainingTree._nodes.Remove(this);

					return Unit.Default;
				}
			);

			Reparent = ReactiveCommand.Create<Unit, Unit>(
				execute: nodeValue =>
				{
					// Change our parent to our desired parent
					Parent = DesiredParent.LastValue;

					return Unit.Default;
				},
				canExecute: canReparentObservable
			);

			#endregion

			// Register node to containing tree
			ContainingTree._nodes.Add(this);
		}

		#endregion

		private bool CanReparentNow()
		{
			// Node can reparent to it's desired parent when the desired parent...
			return
				// ...has a value,
				DesiredParent.HasValue
				// is not this node,
				&& DesiredParent.LastValue != this
				// is not this node's direct parent,
				&& DesiredParent.LastValue != Parent
				// and isn't a child of this node
				&& ((!DesiredParent.LastValue?.IsChildOf(this)) ?? true)
			;
		}
	}
}
