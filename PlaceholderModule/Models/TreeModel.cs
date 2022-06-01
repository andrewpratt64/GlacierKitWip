using GlacierKitCore.Models;
using PlaceholderModule.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
	public enum ETreeType
	{
		Oak,
		Spruce,
		Birch,
		Chestnut
	}


	public class TreeModel :
		ReactiveObject,
		IContextualItem
	{
		private static readonly Random _rng = new();


		/// <summary>
		/// The forest containing the tree
		/// </summary>
		public Forest ContainingForest { get; }

		/// <summary>
		/// The type of the tree
		/// </summary>
		[Reactive]
		public ETreeType TreeType { get; set; }

		/// <summary>
		/// The tree's current height in decimeters
		/// </summary>
		[Reactive]
		public float Height { get; set; }

		/// <summary>
		/// The moment in time the tree was planted
		/// </summary>
		[Reactive]
		public DateTime PlantedTime { get; set; }


		public TreeModel(Forest forest, ETreeType treeType)
		{
			ContainingForest = forest;
			TreeType = treeType;
			Height = _rng.Next(1, 4) + (0.5f * (float)_rng.NextDouble() - 0.5f);
			PlantedTime = DateTime.Now;
		}
	}
}
