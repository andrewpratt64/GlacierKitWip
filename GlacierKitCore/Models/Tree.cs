using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Models
{
	/// <summary>
	/// Describes a hierarchy of data
	/// </summary>
	/// <typeparam name="TNodeValue">The type of data each node represents</typeparam>
	public abstract class Tree<TNodeValue> : ReactiveObject
	{
		#region Non_public_fields_and_properties

		// NOTE: This field is instantiated and owned by the tree, but all manipulations comes from tree nodes
		internal readonly SourceList<TreeNode<TNodeValue>> _nodes;

		#endregion


		#region Public_properties

		/// <summary>
		/// True if a new root node may be created, false otherwise
		/// </summary>
		public abstract bool CanAddRootNode { get; }

		#endregion


		#region Public_methods

		/// <summary>
		/// Connect to and observe changes of a flat view of all of the tree's nodes
		/// </summary>
		/// <returns>An observable that emits the change set of nodes</returns>
		public IObservable<IChangeSet<TreeNode<TNodeValue>>> ConnectToNodes()
		{
			return _nodes.Connect();
		}

		#endregion


		#region Public_commands

		/// <summary>
		/// Creates a new root node
		/// <list type="bullet">
		///		<item>
		///				<term>Input</term>
		///				<description>The value of the node to create</description>
		///		</item>
		///		<item>
		///				<term>Output</term>
		///				<description>The new root node</description>
		///		</item>
		/// </list>
		/// </summary>
		/// <remarks></remarks>
		public abstract ReactiveCommand<TNodeValue, TreeNode<TNodeValue>> CreateRootNode
		{ get; }

		#endregion


		#region Constructor

		/// <summary>
		/// Creates a new instance of the <see cref="Tree"/> class
		/// </summary>
		protected Tree()
		{
			_nodes = new();
		}

		#endregion
	}


	/// <summary>
	/// Describes a <see cref="Tree{TNodeValue}"/> that dosen't support more than one root node
	/// </summary>
	/// <typeparam name="TNodeValue"><inheritdoc cref="Tree{TNodeValue}"/></typeparam>
	public class SingleRootTree<TNodeValue> : Tree<TNodeValue>
	{
		#region Private_fields

		private IObservable<bool> _canCreateRootNodeObservable;
		private ObservableAsPropertyHelper<bool> _canCreateRootNodePropertyHelper;
		private IDisposable? _rootNodeDeleteCommandSubscription;


		#endregion


		#region Public_properties

		/// <summary>
		/// The root node of the tree, or null if the tree is empty
		/// </summary>
		[Reactive]
		public TreeNode<TNodeValue>? RootNode { get; internal set; }

		#endregion


		#region Public_override_properties

		/*[ObservableAsProperty]
		public override bool CanAddRootNode { get; }*/
		public override bool CanAddRootNode => _canCreateRootNodePropertyHelper.Value;

		#endregion


		#region Public_override_commands

		public override ReactiveCommand<TNodeValue, TreeNode<TNodeValue>> CreateRootNode
		{ get; }

		#endregion


		#region Constructor

		/// <summary>
		/// <inheritdoc cref="Tree{TNodeValue}.Tree"/>
		/// </summary>
		public SingleRootTree() :
			base()
		{
			// Root node may be added only when no root node already exists
			_canCreateRootNodeObservable = this.WhenAnyValue(x => x.RootNode)
				.Select(x => x == null);
			_canCreateRootNodePropertyHelper = _canCreateRootNodeObservable
				.ToProperty(
					source: this,
					property: x => x.CanAddRootNode,
					scheduler: RxApp.MainThreadScheduler
				);
			//_canCreateRootNodeObservable.ToPropertyEx(this, x => x.CanAddRootNode);

			// Update the subscription to the root node's delete command when the root node is added or removed
			_canCreateRootNodeObservable
				.Subscribe(x => UpdateRootNodeDeleteCommandSubscription(x));

			CreateRootNode = ReactiveCommand.Create<TNodeValue, TreeNode<TNodeValue>>(
				execute: nodeValue =>
				{
					TreeNode<TNodeValue> newRootNode = new(
						containingTree: this,
						nodeValue: nodeValue,
						parent: null
					);
					RootNode = newRootNode;

					return newRootNode;
				},
				canExecute: _canCreateRootNodeObservable
			);
		}

		#endregion


		#region Private_methods

		private void UpdateRootNodeDeleteCommandSubscription(bool didRootNodeJustBecomeNull)
		{
			if (didRootNodeJustBecomeNull)
			{
				// Dispose the subscription if the root node was just deleted
				_rootNodeDeleteCommandSubscription?.Dispose();
				_rootNodeDeleteCommandSubscription = null;
			}
			else
			{
				// Subscribe if the root node was just added
				Debug.Assert(_rootNodeDeleteCommandSubscription == null);
				// When the delete command executes, set RootNode to null
				_rootNodeDeleteCommandSubscription = RootNode!.Delete.IsExecuting
					.Where(x => x)
					.Subscribe(_ => RootNode = null);
			}
		}

		#endregion
	}


	/// <summary>
	/// Describes a <see cref="Tree{TNodeValue}"/> that supports any number of root nodes
	/// </summary>
	/// <typeparam name="TNodeValue"><inheritdoc cref="Tree{TNodeValue}"/></typeparam>
	public class MultiRootTree<TNodeValue> : Tree<TNodeValue>
	{
		#region Non_public_fields

		internal readonly SourceList<TreeNode<TNodeValue>> _rootNodes;
		private IObservable<bool> _canCreateRootNodeObservable;
		private ObservableAsPropertyHelper<bool> _canCreateRootNodePropertyHelper;
		private IDictionary<TreeNode<TNodeValue>, IDisposable> _rootNodeDeleteCommandSubscriptions;

		#endregion


		#region Public_override_properties

		/*[ObservableAsProperty]
		public override bool CanAddRootNode { get; }*/
		public override bool CanAddRootNode => _canCreateRootNodePropertyHelper.Value;

		#endregion


		#region Public_override_commands

		public override ReactiveCommand<TNodeValue, TreeNode<TNodeValue>> CreateRootNode
		{ get; }

		#endregion


		#region Public_methods

		/// <summary>
		/// Connect to and observe changes of all of the tree's root nodes
		/// </summary>
		/// <returns>An observable that emits the change set of root nodes</returns>
		public IObservable<IChangeSet<TreeNode<TNodeValue>>> ConnectToRootNodes()
		{
			return _rootNodes.Connect();
		}

		#endregion


		#region Constructor

		/// <summary>
		/// <inheritdoc cref="Tree{TNodeValue}.Tree"/>
		/// </summary>
		public MultiRootTree() : base()
		{
			_rootNodes = new();
			_rootNodeDeleteCommandSubscriptions = new Dictionary<TreeNode<TNodeValue>, IDisposable>();

			// Adding root nodes is always allowed
			// Root node may be added only when no root node already exists
			_canCreateRootNodeObservable = Observable.Return(true);
			_canCreateRootNodePropertyHelper = _canCreateRootNodeObservable
				.ToProperty(
					source: this,
					property: x => x.CanAddRootNode,
					scheduler: RxApp.MainThreadScheduler
				);

			// When a new root node is added, subscribe to it's delete command
			_rootNodes
				.Connect()
				.OnItemAdded(x => SubscribeToRootNodeDeleteCommand(x))
				.Subscribe();


			CreateRootNode = ReactiveCommand.Create<TNodeValue, TreeNode<TNodeValue>>(
				execute: nodeValue =>
				{
					TreeNode<TNodeValue> newRootNode = new(
						containingTree: this,
						nodeValue: nodeValue,
						parent: null
					);
					_rootNodes.Add(newRootNode);
					return newRootNode;
				}
			);
		}

		#endregion


		#region Private_methods

		private void SubscribeToRootNodeDeleteCommand(TreeNode<TNodeValue> newRootNode)
		{
			// Remember the new root node, and unsubscribe when it's delete command executes
			Debug.Assert(!_rootNodeDeleteCommandSubscriptions.ContainsKey(newRootNode));
			_rootNodeDeleteCommandSubscriptions[newRootNode] = newRootNode.Delete.IsExecuting
				.Where(x => x)
				.Subscribe(_ => UnsubscribeFromRootNodeDeleteCommand(newRootNode));
		}

		private void UnsubscribeFromRootNodeDeleteCommand(TreeNode<TNodeValue> newRootNode)
		{
			_rootNodeDeleteCommandSubscriptions[newRootNode].Dispose();
			_rootNodeDeleteCommandSubscriptions.Remove(newRootNode);
			_rootNodes.Remove(newRootNode);
		}

		#endregion
	}
}
