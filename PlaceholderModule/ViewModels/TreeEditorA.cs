using GlacierKitCore.Models;
using GlacierKitCore.ViewModels.EditorWindows;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PlaceholderModule.ViewModels
{
	public class TreeEditorA : EditorWindowViewModel
	{
		public static new string DisplayName => "Tree Editor A";


		public EditorContext Ctx { get; }
		public ReadOnlyObservableCollection<TreeModel> Trees { get; }


		public TreeEditorA(EditorContext? ctx)
		{
			Title = DisplayName;
			Ctx = ctx ?? new();
			
			Ctx.Items.
		}
	}
}
