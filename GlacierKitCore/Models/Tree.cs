using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
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
		#region Non_public_fields

		internal readonly SourceList<TreeNode<TNodeValue>> _nodes;

		#endregion


		#region Public_properties

		/// <summary>
		/// True if a new root node may be created, false otherwise
		/// </summary>
		[Reactive]
		public abstract bool CanAddRootNode { get; protected set; }

		#endregion


		#region Public_methods

		/// <summary>
		/// Connect to and observe changes of a flat view of all of the tree's nodes
		/// </summary>
		/// <returns>An observable that emits the change set of nodes</returns>
		public IObservable<IChangeSet<TreeNode<TNodeValue>>> ConnectToNodes()
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();

			//_nodes = new();
		}

		#endregion
	}


	/// <summary>
	/// Describes a <see cref="Tree{TNodeValue}"/> that dosen't support more than one root node
	/// </summary>
	/// <typeparam name="TNodeValue"><inheritdoc cref="Tree{TNodeValue}"/></typeparam>
	public class SingleRootTree<TNodeValue> : Tree<TNodeValue>
	{
		#region Public_properties

		/// <summary>
		/// The root node of the tree, or null if the tree is empty
		/// </summary>
		public TreeNode<TNodeValue>? RootNode { get; internal set; }

		#endregion


		#region Public_override_properties

		[Reactive]
		public override bool CanAddRootNode {get; protected set; }

		#endregion


		#region Public_override_commands

		public override ReactiveCommand<TNodeValue, TreeNode<TNodeValue>> CreateRootNode
		{ get; }

		#endregion


		#region Constructor

		/// <summary>
		/// <inheritdoc cref="Tree{TNodeValue}.Tree"/>
		/// </summary>
		public SingleRootTree() : base()
		{
			throw new NotImplementedException();
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

		#endregion


		#region Public_override_properties

		public override bool CanAddRootNode { get; protected set; }

		#endregion


		#region Public_override_commands

		public override ReactiveCommand<TNodeValue, TreeNode<TNodeValue>> CreateRootNode
		{ get; }

		#endregion


		#region Constructor

		/// <summary>
		/// <inheritdoc cref="Tree{TNodeValue}.Tree"/>
		/// </summary>
		public MultiRootTree() : base()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
