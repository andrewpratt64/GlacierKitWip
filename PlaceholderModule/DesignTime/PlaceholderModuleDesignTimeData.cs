using System;
using System.Collections.Generic;
using System.Text;
using PlaceholderModule.ViewModels;
using PlaceholderModule.Models;
using GlacierKitCore.Models;
using PlaceholderModule.Services;
using System.Reactive.Linq;
using System.Diagnostics;

namespace PlaceholderModule.DesignTime
{
	public static class PlaceholderModuleDesignTimeData
	{
		public static int NumberOfExtraExampleTreesToMake => 25;


		public static EditorContext ExampleEditorContext { get; }

		public static Forest ExampleForest { get; }

		public static TreeModel ExampleTreeModel { get; }

		public static TreeEditorAViewModel ExampleTreeEditorAViewModel { get; }


		static PlaceholderModuleDesignTimeData()
		{
			ExampleEditorContext = new();
			ExampleForest = new(ExampleEditorContext, "The Woods");
			ExampleTreeEditorAViewModel = new(ExampleEditorContext);

			ExampleEditorContext.AddItem
				.Execute(ExampleForest)
				.Subscribe();

			ExampleTreeModel = ExampleForest.PlantTree
				.Execute(ETreeType.Oak)
				.Wait();

			for (int i = 0; i < NumberOfExtraExampleTreesToMake; i++)
			{
				ETreeType[] treeTypes = Enum.GetValues<ETreeType>();
				_ = ExampleForest.PlantTree
					.Execute(treeTypes[(i + 1) % treeTypes.Length])
					.Wait();
			}
		}
	}
}
