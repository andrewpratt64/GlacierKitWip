using Avalonia.Data;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		private SourceList<TreeNode<TNodeValue>> _childNodes;

		#endregion


		#region Public_methods

		/// <summary>
		/// Connect to and observe changes of this node's direct children
		/// </summary>
		/// <returns>An observable that emits the change set of child nodes</returns>
		public IObservable<IChangeSet<TreeNode<TNodeValue>>> ConnectToChildNodes()
		{
			throw new NotImplementedException();
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
		public Optional<TreeNode<TNodeValue>?> DesiredParent { get; set; }

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
			_childNodes = new();

			ContainingTree = containingTree;
			Value = nodeValue;
			Parent = parent;

			DesiredParent = Optional<TreeNode<TNodeValue>?>.Empty;

			AddChild = ReactiveCommand.Create<TNodeValue, TreeNode<TNodeValue>>(
				execute: nodeValue =>
				{
					throw new NotImplementedException();
				}
				//canExecute: TODO
			);

			Delete = ReactiveCommand.Create<bool, Unit>(
				execute: nodeValue =>
				{
					return Unit.Default;
					//throw new NotImplementedException();
				}
				//canExecute: TODO
			);

			Reparent = ReactiveCommand.Create<Unit, Unit>(
				execute: nodeValue =>
				{
					throw new NotImplementedException();
				}
				//canExecute: TODO
			);
		}

		#endregion
	}
}
