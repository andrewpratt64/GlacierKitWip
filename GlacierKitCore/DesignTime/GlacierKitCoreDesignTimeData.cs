﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Styling;
using GlacierKitCore.Models;
using GlacierKitCore.ViewModels.Debug;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.DesignTime
{
	public enum Fake_ECarKind
	{
		GoKart,
		Sedan,
		Minivan,
		Truck,
		MonsterTruck
	}


	public static class GlacierKitCoreDesignTimeData
	{
		#region Public_properties

		public static EditorContext ExampleEditorContext { get; }

		public static StyleClassesViewerViewModel ExampleStyleClassesViewerViewModel { get; }
		public static StyleClassesViewerViewModel ButtonStyleClassesViewerViewModel { get; }


		#region Fake_properties_for_a_car

		public static bool FakeProperties_Car_HasTrunk { get; set; }
		public static bool? FakeProperties_Car_HasBackupCamera { get; set; }
		public static bool? FakeProperties_Car_HasSeatHeaters { get; set; }
		public static int FakeProperties_Car_MilesPerGallon { get; set; } = 30;
		public static int FakeProperties_Car_TotalMilesDriven { get; set; }
		public static string? FakeProperties_Car_Name { get; set; }
		public static float FakeProperties_Car_Length { get; set; } = 450;
		public static float FakeProperties_Car_Width { get; set; } = 100;
		public static Fake_ECarKind FakeProperties_Car_Kind { get; set; }

		#endregion

		#endregion



		static GlacierKitCoreDesignTimeData()
		{
			ExampleEditorContext = new();

			FakeProperties_Car_HasTrunk = true;

			#region ExampleStyleClassesViewerViewModel

			ExampleStyleClassesViewerViewModel = new(
				new string[]
				{
					":disabled",
					"wide",
					"tall",
					"red",
					"green",
					"blue",
					"bold",
					"highlight"
				}
			);

			TextBlock exampleStyleClassesViewerControl = new()
			{
				Text = "Hello, world!",
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Stretch
			};

			Style exampleStyleClassesViewerControlWideStyle = new(selector => selector.OfType<TextBlock>().Class("wide"));
			exampleStyleClassesViewerControlWideStyle.Setters.Add(new Setter(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
			exampleStyleClassesViewerControl.Styles.Add(exampleStyleClassesViewerControlWideStyle);

			Style exampleStyleClassesViewerControlTallStyle = new(selector => selector.OfType<TextBlock>().Class("tall"));
			exampleStyleClassesViewerControlTallStyle.Setters.Add(new Setter(Layoutable.HeightProperty, 100.0d));
			exampleStyleClassesViewerControl.Styles.Add(exampleStyleClassesViewerControlTallStyle);

			Style exampleStyleClassesViewerControlRedStyle = new(selector => selector.OfType<TextBlock>().Class("red"));
			exampleStyleClassesViewerControlRedStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.Red));
			exampleStyleClassesViewerControl.Styles.Add(exampleStyleClassesViewerControlRedStyle);

			Style exampleStyleClassesViewerControlGreenStyle = new(selector => selector.OfType<TextBlock>().Class("green"));
			exampleStyleClassesViewerControlGreenStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.Green));
			exampleStyleClassesViewerControl.Styles.Add(exampleStyleClassesViewerControlGreenStyle);

			Style exampleStyleClassesViewerControlBlueStyle = new(selector => selector.OfType<TextBlock>().Class("blue"));
			exampleStyleClassesViewerControlBlueStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.Blue));
			exampleStyleClassesViewerControl.Styles.Add(exampleStyleClassesViewerControlBlueStyle);

			Style exampleStyleClassesViewerControlBoldStyle = new(selector => selector.OfType<TextBlock>().Class("bold"));
			exampleStyleClassesViewerControlBoldStyle.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeight.Bold));
			exampleStyleClassesViewerControl.Styles.Add(exampleStyleClassesViewerControlBoldStyle);

			Style exampleStyleClassesViewerControlHighlightStyle = new(selector => selector.OfType<TextBlock>().Class("highlight"));
			exampleStyleClassesViewerControlHighlightStyle.Setters.Add(new Setter(TextBlock.BackgroundProperty, Brushes.Yellow));
			exampleStyleClassesViewerControl.Styles.Add(exampleStyleClassesViewerControlHighlightStyle);

			ExampleStyleClassesViewerViewModel.PreviewControl = exampleStyleClassesViewerControl;
			ExampleStyleClassesViewerViewModel.PreviewControlNormalBackground = Brushes.White;
			ExampleStyleClassesViewerViewModel.PreviewControlInvertedBackground = Brushes.Black;

			#endregion

			ButtonStyleClassesViewerViewModel = new(
				new string[]
				{
					":focus",
					":disabled",
					":pressed",
					"accent",
					"noBgButton",
					"noDefaultBgButton",
					"noDisabledBgButton",
					"iconButton"
				}
			);

			ButtonStyleClassesViewerViewModel.PreviewControl = new Button()
			{
				Content = "Hello, world!"
			};

			ButtonStyleClassesViewerViewModel.PreviewControlNormalBackground = Brushes.White;
			ButtonStyleClassesViewerViewModel.PreviewControlInvertedBackground = Brushes.Black;
		}
	}
}
