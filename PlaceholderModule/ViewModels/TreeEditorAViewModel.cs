using DynamicData;
using GlacierKitCore.Models;
using GlacierKitCore.ViewModels.EditorWindows;
using PlaceholderModule.DesignTime;
using PlaceholderModule.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace PlaceholderModule.ViewModels
{
	public class TreeEditorAViewModel :
		EditorWindowViewModel,
		IActivatableViewModel
	{
		private readonly ReadOnlyObservableCollection<TreeModel> _trees;


		public readonly struct TreeHeightUnit
		{
			public readonly string Name { get; init; }
			public readonly double Scale { get; init; }
		}


		public static new string DisplayName { get; } = "Tree Editor A";

		public static IEnumerable<TreeHeightUnit> TreeHeightUnits { get; } =
			new List<TreeHeightUnit>()
			{
				new TreeHeightUnit{
					Name = "Meters",
					Scale = 0.1
				},
				new TreeHeightUnit{
					Name = "Centimeters",
					Scale = 10
				},
				new TreeHeightUnit{
					Name = "Feet",
					Scale = 0.328084
				},
				new TreeHeightUnit{
					Name = "Inches",
					Scale = 3.93701
				}
			};


		public ViewModelActivator Activator { get; }
		public EditorContext Ctx { get; }
		public ReadOnlyObservableCollection<TreeModel> Trees => _trees;


		[Reactive]
		public TreeModel? SelectedTree { get; set; }

		[Reactive]
		public TreeHeightUnit SelectedUnit { get; set; }

		[Reactive]
		public double TreeEditorHeightValue { get; set; }


		[ObservableAsProperty]
		public string TreeEditorTitle { get; }

		[ObservableAsProperty]
		public string? TreeEditorEmoji { get; }

		[ObservableAsProperty]
		public double TreeEditorEmojiFontSize { get; }
		[ObservableAsProperty]
		public string? TreeEditorStringifiedHeight { get; }



		public TreeEditorAViewModel() :
			this(null)
		{ }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public TreeEditorAViewModel(EditorContext? ctx)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
			Activator = new();
			Title = DisplayName;
			Ctx = ctx ?? new();
			SelectedUnit = TreeHeightUnits.First(unit => unit.Name == "Meters");
			TreeEditorHeightValue = 0;

			// Bind Trees property
			IObservable<IChangeSet<TreeModel>> itemsChangeSet =
					Ctx.ConnectToItems()
					.Filter(item => item is TreeModel)
					.Transform(item => (TreeModel)item)
					.Bind(out _trees);


			this.WhenActivated(disposables =>
			{
				IObservable<TreeModel?> selectedTreeObservable = this.WhenAnyValue(x => x.SelectedTree);

				// TreeEditorHeightValue reacts to SelectedUnit
				this.WhenAnyValue(x => x.SelectedUnit)
					.Subscribe(unit => TreeEditorHeightValue = (unit.Scale * SelectedTree?.Height) ?? 0)
					.DisposeWith(disposables);

				// TreeEditorHeightValue reacts to SelectedTree's height
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				this.WhenAnyValue
					(
						x => x.SelectedTree,
						x => x.SelectedTree.Height,
						(treeModel, height) => treeModel?.Height
					)
#pragma warning restore CS8602 // Dereference of a possibly null reference.
					.Where(height => height != TreeEditorHeightValue)
					.Subscribe(height => TreeEditorHeightValue = (SelectedUnit.Scale * height) ?? 0)
					.DisposeWith(disposables);

				// SelectedTree's height reacts to TreeEditorHeightValue
				this.WhenAnyValue(x => x.TreeEditorHeightValue)
					.Where(
						heightValue =>
						SelectedTree != null
						&& heightValue != SelectedTree.Height
					)
					.Subscribe(heightValue => SelectedTree!.Height = heightValue / SelectedUnit.Scale)
					.DisposeWith(disposables);

				// Setup TreeEditorTitle
				selectedTreeObservable
					.Select(
						treeModel =>
							treeModel == null
							? "No tree selected"
							: "Tree #" + treeModel!.TreeId.ToString()
					)
					.ToPropertyEx(this, x => x.TreeEditorTitle);

				// Setup TreeEditorEmoji
				selectedTreeObservable
					.Select(
						treeModel =>
							treeModel == null
							? null
							: TreeModel.GetEmojiForTreeType(treeModel.TreeType)
					)
					.ToPropertyEx(this, x => x.TreeEditorEmoji);

				// Setup TreeEditorEmojiFontSize
				this.WhenAnyValue(x => x.SelectedTree)
					.Select(
						treeModel =>
							treeModel == null
							? 0.0
							: treeModel.Height * 10.0
					)
					.ToPropertyEx(this, x => x.TreeEditorEmojiFontSize);

				// Setup TreeEditorStringifiedHeight
				this.WhenAnyValue(x => x.SelectedTree, x => x.SelectedUnit)
					.Select(
						_ =>
							SelectedTree == null
							? null
							: (SelectedTree!.Height * SelectedUnit.Scale).ToString("N3", CultureInfo.InvariantCulture) + " :)"
					)
					.ToPropertyEx(this, x => x.TreeEditorStringifiedHeight);

				// Subscribe to changes in contextual items
				itemsChangeSet
					.Subscribe()
					.DisposeWith(disposables);

				// Bind the selected tree to the app's focused item
				this.WhenAnyValue(x => x.SelectedTree)
					.Subscribe(treeModel => Ctx.FocusedItem = treeModel)
					.DisposeWith(disposables);

				// Bind the app's focused item to the selected tree
				this.WhenAnyValue(x => x.Ctx.FocusedItem)
					.Select(item => item as TreeModel)
					.Where(item => item != SelectedTree)
					.BindTo(this, x => x.SelectedTree)
					.DisposeWith(disposables);
			});
		}
	}
}
