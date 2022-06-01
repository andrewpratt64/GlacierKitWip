﻿using DynamicData;
using GlacierKitCore.Models;
using Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Text;

namespace PlaceholderModule.Services
{
	public class Forest : IContextualItem
	{
		private readonly SourceList<TreeModel> _trees;
		private readonly ReadOnlyObservableCollection<TreeModel> _viewOfTrees;


		public EditorContext Ctx { get; }
		
		public ReadOnlyObservableCollection<TreeModel> Trees => _viewOfTrees;


		public ReactiveCommand<ETreeType, TreeModel> PlantTree { get; }
		public ReactiveCommand<TreeModel, Unit> ChopTree { get; }


		public Forest(EditorContext? ctx)
		{
			_trees = new();

			Ctx = ctx ?? new();

			PlantTree = ReactiveCommand.Create<ETreeType, TreeModel>(
				treeType =>
				{
					TreeModel newTree = new(this, treeType);
					_trees.Add(newTree);
					Ctx.AddItem.Execute(newTree).Subscribe();
					return newTree;
				}
			);

			ChopTree = ReactiveCommand.Create<TreeModel, Unit>(
				tree =>
				{
					Ctx.RemoveItem.Execute(tree).Subscribe();
					_trees.Remove(tree);

					return Unit.Default;
				}
			);

			_trees.Connect().Bind(out _viewOfTrees);
		}
	}
}