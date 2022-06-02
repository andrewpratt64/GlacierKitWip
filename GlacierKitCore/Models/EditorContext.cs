using Dock.Model.Controls;
using DynamicData;
using GlacierKitCore.Attributes.DataProviders;
using GlacierKitCore.Commands;
using GlacierKitCore.Services;
using GlacierKitCore.ViewModels.Common;
using GlacierKitCore.ViewModels.EditorWindows;
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
    public class EditorContext : ReactiveObject
    {
		#region Private_fields

		private readonly SourceList<IContextualItem> _items;

		#endregion


		#region Public_properties

		/// <summary>
		/// The module loader instance of this context
		/// </summary>
		public GKModuleLoaderService ModuleLoader { get; }

		/// <summary>
		/// The main menu bar of the editor
		/// </summary>
		public MenuBarViewModel MainMenuBar { get; }
		
		/// <summary>
		/// The item currently focused by the user
		/// </summary>
		[Reactive]
		public IContextualItem? FocusedItem { get; set; }

		#endregion


		#region Public_commands

		/// <summary>
		/// Creates a new editor window of a given type
		/// </summary>
		public ReactiveCommand<Type?, Type?> CreateEditorWindow { get; }

		/// <summary>
		/// Adds a new item to the context
		/// </summary>
		public ReactiveCommand<IContextualItem, bool> AddItem { get; }

		/// <summary>
		/// Removes an item from the context
		/// </summary>
		public ReactiveCommand<IContextualItem, bool> RemoveItem { get; }

		#endregion


		#region Constructor

		public EditorContext()
        {
			_items = new();

			ModuleLoader = new(this);
			MainMenuBar = new();

			// Load the main menu items into the editor context after modules have finished loading
			this.WhenAnyValue(x => x.ModuleLoader.State)
				.Where(x => x == GKModuleLoaderService.ELoaderState.Loaded)
				.Subscribe(_ => LoadMainMenuItems());

            CreateEditorWindow = ReactiveCommand.Create((Type? windowType) =>
            {
                // Return null for any type that isn't a concrete subclass of EditorWindowViewModel
                if ((windowType?.IsSubclassOf(typeof(EditorWindowViewModel)) ?? false) && !windowType!.IsAbstract )
                    return windowType;
                return null;
            });

			AddItem = ReactiveCommand.Create((IContextualItem item) =>
			{
				if (_items.Items.Contains(item))
					return false;

				_items.Add(item);
				return true;
			});

			RemoveItem = ReactiveCommand.Create((IContextualItem item) =>
			{
				if (FocusedItem == item)
					FocusedItem = null;
				
				return _items.Remove(item);
			});
		}

		#endregion


		#region Public_methods


		/// <summary>
		/// Connect to and observe changes of the items currently relative to this context
		/// </summary>
		/// <returns>An observable that emits the change set of contextual items</returns>
		public IObservable<IChangeSet<IContextualItem>> ConnectToItems()
		{
			return _items.Connect();
		}

		public GKCommand<TParam, TResult>? GetCommand<TParam, TResult>(string id)
        {
            return (GKCommand<TParam, TResult>?)
                ModuleLoader.GKCommands.FirstOrDefault(cmd =>
                       cmd.GKCommandId == id
                    && cmd.TParamValue == typeof(TParam)
                    && cmd.TResultValue== typeof(TResult)
                );
        }

		#endregion


		#region Private_methods

		/// <summary>
		/// Populates the main menu bar
		/// </summary>
		private void LoadMainMenuItems()
		{
			// Assert modules have been loaded
			Debug.Assert(
				ModuleLoader.State == GKModuleLoaderService.ELoaderState.Loaded,
				"Can't load main menu items before loading modules"
			);

			// Create a collection to hold the setup info of items that
			//	are waiting for their parent nodes to be be created.
			//	This is a dictionary where the key is the full path of the node that
			//	must first be created before the item may be created as a string with
			//	each id seperated by periods and the value is the setup infos of the items
			//	awaiting creation.
			Dictionary<string, ICollection<MainMenuItemSetupInfo>> setupInfosAwaitingCreation = new();

			// Subscribe to the root menu item tree nodes
			IDisposable rootNodeSubscription = MainMenuBar.ItemTree.ConnectToRootNodes()
				.Bind(out ReadOnlyObservableCollection<TreeNode<MenuBarItemViewModel>> rootNodes)
				.Subscribe();

			// Iterate over each item setup info instance
			foreach (MainMenuItemSetupInfo setupInfo in ModuleLoader.MainMenuBarItemsSetupInfo)
			{
				LoadMainMenuItem(setupInfo, setupInfosAwaitingCreation, rootNodes);
			}

			// Unsubscribe from the root menu item tree nodes
			rootNodeSubscription.Dispose();
		}


		/// <summary>
		/// Attempts to add a single menu item to the main menu bar
		/// </summary>
		/// <param name="setupInfo">Info for the item to add</param>
		/// <param name="setupInfosAwaitingCreation">A collection holding items that cannot be created
		/// until their parent nodes have been created</param>
		/// <param name="rootNodes">The root nodes of the main menu</param>
		/// <returns>The tree node created for the menu item, or null if none was created</returns>
		/// <remarks>Intended to be called from <see cref="LoadMainMenuItems"/></remarks>
		private TreeNode<MenuBarItemViewModel>? LoadMainMenuItem(
			MainMenuItemSetupInfo setupInfo,
			IDictionary<string, ICollection<MainMenuItemSetupInfo>> setupInfosAwaitingCreation,
			ReadOnlyObservableCollection<TreeNode<MenuBarItemViewModel>> rootNodes)
		{
			// Assert the setup info has at least one id
			Debug.Assert(
				setupInfo.Path.Any(),
				$"Main menu item titled, \"{setupInfo.Title}\" is missing an id (i.e. it's {nameof(MainMenuItemSetupInfo.Path)} is empty)"
			);

			// Get the id of the root node to create the item with
			string rootId = setupInfo.Path.First();

			// Attempt to get the root node from the setup info's path
			TreeNode<MenuBarItemViewModel>? rootNode = rootNodes.FirstOrDefault(x => x.Value.Id == rootId);

			// If the root node does NOT exist...
			if (rootNode == null)
			{
				// ...and the item to create is at the root of the menu, then create it
				if (setupInfo.Path.Count() == 1)
					return CreateMainMenuItemNode(setupInfo, null, rootId, setupInfosAwaitingCreation);
				// ...and the item to create is NOT at the root of the menu, then delay creation
				else
				{
					DelayMainMenuItemNodeCreation(setupInfo, rootId, setupInfosAwaitingCreation);
					return null;
				}
			}
			// If the root node exists...
			else
			{
				// ...then assert the item to create isn't at the root of the menu, since that would
				//	mean multiple nodes with the same id were defined
				Debug.Assert(
					setupInfo.Path.Count() > 1,
					$"Multiple main menu root nodes with id \"{rootId}\" were defined"
				);

				// Declare a variable to hold the previously processed node
				TreeNode<MenuBarItemViewModel> previousNode = rootNode!;

				// Iterate over the rest of the ids in the setup info's path
				for (int i = 1; i < setupInfo.Path.Count(); i++)
				{
					// Get the value of the id
					string id = setupInfo.Path.ElementAt(i);

					// Subscribe to the children of the previously processed node
					IDisposable childNodeSubscription = previousNode.ConnectToChildNodes()
						.Bind(out ReadOnlyObservableCollection<TreeNode<MenuBarItemViewModel>> childrenOfPreviousNode)
						.Subscribe();

					// Get the node that each id corresponds to, if any
					TreeNode<MenuBarItemViewModel>? node = childrenOfPreviousNode
						.FirstOrDefault(x => x.Value.Id == id);

					// Unsubscribe from the children of the previously processed node
					childNodeSubscription.Dispose();

					// If the node dosen't exist...
					if (node == null)
					{
						// ...calculate the (dot-seperated) id of the path from the root to the missing node
						string fullId = string.Join(
							'.',
							setupInfo.Path.SkipLast(setupInfo.Path.Count() - 1 - i)
						);

						// If the missing node corresponds to the node to create, then create it
						if (i == setupInfo.Path.Count() - 1)
						{
							return CreateMainMenuItemNode(setupInfo, previousNode, fullId, setupInfosAwaitingCreation);
						}
						// If the missing node does NOT correspond to the node to create
						//	then delay creation
						else
						{
							DelayMainMenuItemNodeCreation(setupInfo, fullId, setupInfosAwaitingCreation);
							return null;
						}
					}

					// Remeber this node for the next iteration
					previousNode = node;
				}

				// If the node already exists, throw an error
				throw new Exception($"Multiple main menu nodes with id \"{string.Join('.', setupInfo.Path)}\" were defined");
			}
		}


		/// <summary>
		/// Creates a new node for the main menu bar
		/// </summary>
		/// <param name="setupInfo">Info for the item to add</param>
		/// <param name="parentNode">The parent of the node to create, or null if a root node should be created</param>
		/// <param name="fullId">The id path from the root node to <paramref name="newNode"/></param>
		/// <param name="setupInfosAwaitingCreation"></param>
		/// <returns>The new node</returns>
		/// <remarks>Unlike <see cref="LoadMainMenuItem"/>, this function assumes that all required parent nodes have already been created.</remarks>
		private TreeNode<MenuBarItemViewModel> CreateMainMenuItemNode(
			MainMenuItemSetupInfo setupInfo,
			TreeNode<MenuBarItemViewModel>? parentNode,
			string fullId,
			IDictionary<string, ICollection<MainMenuItemSetupInfo>> setupInfosAwaitingCreation
		)
		{
			// Create the tree node for the main menu item
			TreeNode<MenuBarItemViewModel> node;
			if (parentNode == null)
			{
				node = MainMenuBar.ItemTree.CreateRootNode
					.Execute(new(setupInfo.Path.Last(), setupInfo.Title, setupInfo.Command))
					.Wait();
			}
			else
			{
				node = parentNode.AddChild
					.Execute(new(setupInfo.Path.Last(), setupInfo.Title, setupInfo.Command))
					.Wait();
			}

			// Create any nodes that were waiting for the current node to be created, if any
			if (setupInfosAwaitingCreation.ContainsKey(fullId))
			{
				foreach (MainMenuItemSetupInfo childSetupInfo in setupInfosAwaitingCreation[fullId])
				{
					CreateMainMenuItemNode(
						setupInfo: childSetupInfo,
						parentNode: node,
						fullId: fullId + '.' + childSetupInfo.Path.Last(),
						setupInfosAwaitingCreation: setupInfosAwaitingCreation
					);
				}

				// Clear nodes from setupInfosAwaitingCreation
				setupInfosAwaitingCreation.Remove(fullId);
			}

			// Return the new node
			return node;
		}


		/// <summary>
		/// Delays the creation of a node in the main menu bar until all of it's parents have been created
		/// </summary>
		/// <param name="setupInfo">Info for the item to delay creation</param>
		/// <param name="awaitingId">The full id of the parent the item is waiting on</param>
		/// <param name="setupInfosAwaitingCreation"></param>
		private static void DelayMainMenuItemNodeCreation(
			MainMenuItemSetupInfo setupInfo,
			string awaitingId,
			IDictionary<string, ICollection<MainMenuItemSetupInfo>> setupInfosAwaitingCreation
		)
		{
			// If there are already other items waiting on the parent, add the node to the collection
			if (setupInfosAwaitingCreation.TryGetValue(awaitingId, out ICollection<MainMenuItemSetupInfo>? awaitingItems))
				awaitingItems.Add(setupInfo);
			// If there is nothing already waiting on the given parent, create a new entry in setupInfosAwaitingCreation
			else
				setupInfosAwaitingCreation.Add(awaitingId, new List<MainMenuItemSetupInfo> { setupInfo });
		}

		#endregion
	}
}
