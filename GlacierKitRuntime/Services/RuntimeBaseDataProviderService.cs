using GlacierKitCore.Attributes;
using GlacierKitCore.Attributes.DataProviders;
using GlacierKitCore.Commands;
using GlacierKitCore.Models;
using GlacierKitCore.Services;
using GlacierKitCore.ViewModels.Common;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Text;

namespace GlacierKitRuntime.Services
{
    public class RuntimeBaseDataProviderService : GKDataProviderService
	{
		#region Commands

		#endregion


		#region Menu_items
#pragma warning disable CA1822 // Mark members as static

		#region Root_menu_item_groups

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo File_MenuItemGroupInfo => new
		(
			title: "File",
			path: new string[] { "file" },
			command: null,
			order: int.MinValue
		);

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo Edit_MenuItemGroupInfo => new
		(
			title: "Edit",
			path: new string[] { "edit" },
			command: null,
			order: int.MinValue + 1
		);

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo View_MenuItemGroupInfo => new
		(
			title: "View",
			path: new string[] { "view" },
			command: null,
			order: int.MinValue + 2
		);

		#endregion


		#region Menu_item_subgroups

		[ExposeAsMainMenuItem]
		public MainMenuItemSetupInfo View_EditorWindows_MenuItemGroupInfo => new
		(
			title: "Editor windows",
			path: new string[] { "view", "editorWindows" },
			command: null
		);

		#endregion

#pragma warning restore CA1822 // Mark members as static
		#endregion


		#region Constructor

		public RuntimeBaseDataProviderService(EditorContext ctx) :
			base(ctx)
		{ }

		#endregion
	}
}
