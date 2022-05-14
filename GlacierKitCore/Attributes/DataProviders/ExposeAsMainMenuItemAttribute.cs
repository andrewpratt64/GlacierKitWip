﻿using GlacierKitCore.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Attributes.DataProviders
{
	/// <summary>
	/// Marks a GK command to be used as a menu item in the main meu bar
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ExposeAsMainMenuItemAttribute : Attribute
	{}


	/// <summary>
	/// Contains data related to initializing a main menu item
	/// </summary>
	public class MainMenuItemSetupInfo
	{
		/// <summary>
		/// The print-friendly name of this item
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// The ids of the parents to create the item in, where the
		/// first is a root item and the last is the menu item itself.
		/// </summary>
		public IEnumerable<string> Path { get; }

		/// <summary>
		/// The command to execute when the menu item is clicked, or
		/// null if the menu item should be a group
		/// </summary>
		public IGKCommand? Command { get; }

		public MainMenuItemSetupInfo(string title, IEnumerable<string> path, IGKCommand? command)
		{
			Debug.Assert(path.Any(), $"Parameter {nameof(path)} must have at least one id");

			Title = title;
			Path = path;
			Command = command;
		}

	}
}