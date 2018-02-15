using System;
using Eto.Forms;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;
using sw = System.Windows;

namespace Eto.Wpf.Forms.Cells
{
	public class ProgressCellHandler : CellHandler<ProgressCellHandler.DataGridProgressBarColumn, ProgressCell, ProgressCell.ICallback>, ProgressCell.IHandler
	{
		public ProgressCellHandler()
		{
			Control = new DataGridProgressBarColumn { Handler = this };
		}

		public void SetValue(object dataItem, float? value)
		{
			if (Widget.Binding != null)
			{
				if (value.HasValue)
					value = value < 0f ? 0f : value > 1f ? 1f : value;

				Widget.Binding.SetValue(dataItem, value);
			}
		}

		public float? GetValue(object dataItem)
		{
			if (Widget.Binding != null)
			{
				float? progress = Widget.Binding.GetValue(dataItem);
				if (progress.HasValue)
					progress = progress < 0f ? 0f : progress > 1f ? 1f : progress;
				return progress;
			}
			return (float?)null;
		}


		// The progress bar column class
		public class DataGridProgressBarColumn : swc.DataGridBoundColumn
		{
			private static sw.Style defaultElementStyle;
			public static sw.Style DefaultElementStyle
			{
				get
				{
					if (DataGridProgressBarColumn.defaultElementStyle == null)
					{
						sw.Style style = new sw.Style(typeof (swc.ProgressBar));
						style.Setters.Add(new sw.Setter(sw.UIElement.IsHitTestVisibleProperty, (object) false));
						style.Setters.Add(new sw.Setter(sw.UIElement.FocusableProperty, (object) false));
						style.Setters.Add(new sw.Setter(sw.FrameworkElement.HorizontalAlignmentProperty, (object) sw.HorizontalAlignment.Center));
						style.Setters.Add(new sw.Setter(sw.FrameworkElement.VerticalAlignmentProperty, (object) sw.VerticalAlignment.Top));
						style.Seal();
						DataGridProgressBarColumn.defaultElementStyle = style;
					}
					return DataGridProgressBarColumn.defaultElementStyle;
				}
			}

			static DataGridProgressBarColumn()
			{
				swc.DataGridBoundColumn.ElementStyleProperty.OverrideMetadata(typeof(DataGridProgressBarColumn), new sw.FrameworkPropertyMetadata(DataGridProgressBarColumn.DefaultElementStyle));
			}

			public ProgressCellHandler Handler { get; set; }

			protected override sw.FrameworkElement GenerateElement(swc.DataGridCell cell, object dataItem)
			{
				var element = GenerateProgressBar(cell, dataItem);
				return Handler.SetupCell(element);
			}

			protected override sw.FrameworkElement GenerateEditingElement(swc.DataGridCell cell, object dataItem)
			{
				return GenerateProgressBar(cell, dataItem);
			}

			private swc.Grid GenerateProgressBar(swc.DataGridCell cell, object dataItem)
			{
				swc.Grid element = cell?.Content as swc.Grid ?? new swc.Grid();
				cell.Foreground = sw.SystemColors.ControlTextBrush;

				// Add a progress bar to the grid
				var progressBar = new swc.ProgressBar { Value = 0, Minimum = 0, Maximum = 1 };
				element.Children.Add(progressBar);

				// Add a text block that shows the progress percentage to the grid
				swc.TextBlock textBlock = new swc.TextBlock
				{
					Text = (int)progressBar.Value + "%",
					HorizontalAlignment = sw.HorizontalAlignment.Center,
					VerticalAlignment = sw.VerticalAlignment.Center
				};
				element.Children.Add(textBlock);

				SetValue(cell, element, progressBar, textBlock, dataItem);

				element.DataContextChanged += (sender, e) =>
				{
					var grid = (swc.Grid)sender;

					var bar = grid.FindChild<swc.ProgressBar>();
					var text = grid.FindChild<swc.TextBlock>();

					SetValue(cell, grid, bar, text, grid.DataContext);
				};

				return element;
			}

			void SetValue(swc.DataGridCell cell, swc.Grid grid, swc.ProgressBar bar, swc.TextBlock text, object dataItem)
			{
				// Get the value
				float? value = Handler.GetValue(dataItem);

				// If the value is -1
				if (!value.HasValue)
				{
					// Hide the bar and text
					bar.Visibility = sw.Visibility.Hidden;
					text.Visibility = sw.Visibility.Hidden;
				}
				else
				{
					// Hide the bar and text
					bar.Visibility = sw.Visibility.Visible;
					text.Visibility = sw.Visibility.Visible;
				}

				// Set the value of the progress bar and make sure that it is above 0.
				bar.Value = value ?? 0;
				// Set the text on the percentage text text block
				text.Text = (int)(bar.Value * 100f) + "%";

				Handler.FormatCell(grid, cell, dataItem);
			}
		}
	}
}

