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


		public readonly struct TreeNumericalUnit
		{
			public readonly string Name { get; init; }
			public readonly double Scale { get; init; }
		}


		public static new string DisplayName { get; } = "Tree Editor A";

		public static IEnumerable<TreeNumericalUnit> TreeHeightUnits { get; } =
			new List<TreeNumericalUnit>()
			{
				new TreeNumericalUnit{
					Name = "Meters",
					Scale = 0.1
				},
				new TreeNumericalUnit{
					Name = "Centimeters",
					Scale = 10
				},
				new TreeNumericalUnit{
					Name = "Feet",
					Scale = 0.328084
				},
				new TreeNumericalUnit{
					Name = "Inches",
					Scale = 3.93701
				}
			}
		;

		public static IEnumerable<TreeNumericalUnit> TreeAgeUnits { get; } =
			new List<TreeNumericalUnit>()
			{
				new TreeNumericalUnit{
					Name = "Days",
					Scale = 1
				},
				new TreeNumericalUnit{
					Name = "Weeks",
					Scale = 7
				},
				new TreeNumericalUnit{
					Name = "Years",
					Scale = 365.25
				},
				new TreeNumericalUnit{
					Name = "Decades",
					Scale = 3652.5
				}
			}
		;


		public ViewModelActivator Activator { get; }
		public ReadOnlyObservableCollection<TreeModel> Trees => _trees;


		[Reactive]
		public TreeModel? SelectedTree { get; set; }

		[Reactive]
		public TreeNumericalUnit SelectedHeightUnit { get; set; }

		[Reactive]
		public TreeNumericalUnit SelectedAgeUnit { get; set; }

		[Reactive]
		public double TreeEditorHeightValue { get; set; }

		[Reactive]
		public double TreeEditorAgeValue { get; set; }


		[ObservableAsProperty]
		public string TreeEditorTitle { get; }

		[ObservableAsProperty]
		public string? TreeEditorEmoji { get; }

		[ObservableAsProperty]
		public double TreeEditorEmojiFontSize { get; }



		public TreeEditorAViewModel() :
			this(new())
		{ }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public TreeEditorAViewModel(EditorContext ctx) :
			base(ctx)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
			Activator = new();
			Title = DisplayName;
			SelectedHeightUnit = TreeHeightUnits.First(unit => unit.Name == "Meters");
			SelectedAgeUnit = TreeAgeUnits.First(unit => unit.Name == "Years");
			TreeEditorHeightValue = 0;
			TreeEditorAgeValue = 0;

			// Bind Trees property
			IObservable<IChangeSet<TreeModel>> itemsChangeSet =
					Ctx.ConnectToItems()
					.Filter(item => item is TreeModel)
					.Transform(item => (TreeModel)item)
					.Bind(out _trees);


			this.WhenActivated(disposables =>
			{
				IObservable<TreeModel?> selectedTreeObservable = this.WhenAnyValue(x => x.SelectedTree);

				// TreeEditorHeightValue reacts to SelectedHeightUnit
				this.WhenAnyValue(x => x.SelectedHeightUnit)
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
					.Subscribe(height => TreeEditorHeightValue = (SelectedHeightUnit.Scale * height) ?? 0)
					.DisposeWith(disposables);

				// SelectedTree's height reacts to TreeEditorHeightValue
				this.WhenAnyValue(x => x.TreeEditorHeightValue)
					.Select(heightValue => heightValue / SelectedHeightUnit.Scale)
					.Where(
						heightValue =>
						SelectedTree != null
						&& heightValue != SelectedTree.Height
					)
					.Subscribe(heightValue => SelectedTree!.Height = heightValue)
					.DisposeWith(disposables);

				// TreeEditorAgeValue reacts to SelectedAgeUnit
				this.WhenAnyValue(x => x.SelectedAgeUnit)
					.Where(_ => SelectedTree != null)
					.Subscribe(unit => TreeEditorAgeValue = CalculateTimeSinceDateWithUnit(SelectedTree!.PlantedTime, unit))
					.DisposeWith(disposables);

				// TreeEditorAgeValue reacts to the time SelectedTree was planted
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				this.WhenAnyValue
					(
						x => x.SelectedTree,
						x => x.SelectedTree.PlantedTime,
						(treeModel, height) => treeModel?.PlantedTime
					)
#pragma warning restore CS8602 // Dereference of a possibly null reference.
					.WhereNotNull()
					.Select(plantedTime => CalculateTimeSinceDateWithUnit((DateTime)plantedTime!, SelectedAgeUnit))
					.Where(age => age != TreeEditorHeightValue)
					.Subscribe(age => TreeEditorAgeValue = age)
					.DisposeWith(disposables);

				// The time SelectedTree was planted reacts to TreeEditorAgeValue
				this.WhenAnyValue(x => x.TreeEditorAgeValue)
					.Select(ageValue => CalculateDateFromTimeOffsetWithUnit(ageValue, SelectedAgeUnit))
					.Where(
						plantedTime =>
						SelectedTree != null
						&& plantedTime != SelectedTree.PlantedTime
					)
					.Subscribe(plantedTime => SelectedTree!.PlantedTime = plantedTime)
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


		private static double CalculateTimeSinceDateWithUnit(DateTime date, TreeNumericalUnit unit)
		{
			return unit.Scale * DateTime.Now.Subtract(date).TotalDays;
		}

		private static DateTime CalculateDateFromTimeOffsetWithUnit(double timeOffset, TreeNumericalUnit unit)
		{
			return DateTime.Now.AddDays(-timeOffset * unit.Scale);
		}
	}
}
