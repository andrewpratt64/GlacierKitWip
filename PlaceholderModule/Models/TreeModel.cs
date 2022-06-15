using GlacierKitCore.Models;
using PlaceholderModule.Models;
using PlaceholderModule.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlaceholderModule.Models
{
	public enum ETreeType
	{
		Oak,
		Spruce,
		Palm,
		Broccoli
	}


	public class TreeModel :
		ReactiveObject,
		IContextualItem
	{
		private static readonly Random _rng = new();
		private static ulong _nextId = 1U;


		/// <summary>
		/// The forest containing the tree
		/// </summary>
		public ForestModel ContainingForest { get; }

		public ulong TreeId { get; }

		/// <summary>
		/// The type of the tree
		/// </summary>
		[Reactive]
		public ETreeType TreeType { get; set; }

		/// <summary>
		/// The tree's current height in decimeters
		/// </summary>
		[Reactive]
		public double Height { get; set; }

		/// <summary>
		/// The moment in time the tree was planted
		/// </summary>
		[Reactive]
		public DateTime PlantedTime { get; set; }


		public TreeModel(ForestModel forest, ETreeType treeType)
		{
			ContainingForest = forest;
			TreeId = _nextId++;
			TreeType = treeType;
			Height = _rng.Next(1, 4) + (0.5d * (float)_rng.NextDouble() - 0.5d);
			PlantedTime = DateTime.Now;
		}


		public static string GetEmojiForTreeType(ETreeType treeType)
		{
			return treeType switch
			{
				ETreeType.Oak => "🌳",
				ETreeType.Spruce => "🌲",
				ETreeType.Palm => "🌴",
				ETreeType.Broccoli => "🥦",
				_ => "?",
			};
		}
	}
}
