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
		private static readonly ETreeType[] _treeTypes = Enum.GetValues<ETreeType>();


		public static int NumberOfExtraTreesInExampleForest { get; } = 25;
		public static IEnumerable<string> ExtraForestNames { get; } = new List<string>()
		{
			"Fuchsia ForestModel",
			"Jade Jungle",
			"Red Redwoods",
			"White Woods"
		};
		public static int TreesPerExtraForest { get; } = 7;


		public static EditorContext ExampleEditorContext { get; }

		public static ForestModel ExampleForest { get; }

		public static TreeModel ExampleTreeModel { get; }

		public static ForestEditorViewModel ExampleForestEditorViewModel { get; }

		public static TreeEditorAViewModel ExampleTreeEditorAViewModel { get; }


		static PlaceholderModuleDesignTimeData()
		{
			ExampleEditorContext = new();

			ExampleEditorContext.ModuleLoader.LoadModules();
			
			ExampleForest = new(ExampleEditorContext, "Example ForestModel Instance");
			ExampleForestEditorViewModel = new(ExampleEditorContext);
			ExampleTreeEditorAViewModel = new(ExampleEditorContext);
			
			ExampleEditorContext.AddItem
				.Execute(ExampleForest)
				.Subscribe();

			ExampleTreeModel = ExampleForest.PlantTree
				.Execute(ETreeType.Oak)
				.Wait();

			for (int i = 0; i < NumberOfExtraTreesInExampleForest; i++)
			{
				PlantNextExampleTree(ExampleForest, i + 1);
			}

			int treeTypeOffset = 2;
			foreach (string forestName in ExtraForestNames)
			{
				ForestModel forest = new(ExampleEditorContext, forestName);
				
				ExampleEditorContext.AddItem
					.Execute(forest)
					.Subscribe();

				for (int treeNum = 0; treeNum < TreesPerExtraForest; treeNum++)
					PlantNextExampleTree(forest, ++treeTypeOffset);
			}
		}


		private static void PlantNextExampleTree(ForestModel forest, int type)
		{
			_ = forest.PlantTree
				.Execute(_treeTypes[type % _treeTypes.Length])
				.Wait();
		}
	}
}
