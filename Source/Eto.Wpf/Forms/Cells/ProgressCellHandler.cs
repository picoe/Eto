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

			private sw.Style textStyle;
			private sw.Style TextStyle
			{
				get
				{
					if (textStyle == null)
					{
						var textBlock = new swc.TextBlock();
						textStyle = new sw.Style(typeof(swc.TextBlock), textBlock.Style);
						textStyle.Setters.Add(new sw.Setter(swc.TextBlock.ForegroundProperty, (object)sw.SystemColors.ControlTextBrush));
						textStyle.Seal();
					}
					return textStyle;
				}
			}

			protected override sw.FrameworkElement GenerateElement(swc.DataGridCell cell, object dataItem)
			{
				var element = GenerateProgressBar(cell);
				element.DataContextChanged += (sender, e) =>
				{
					swc.Grid grid = sender as swc.Grid;

					swc.ProgressBar progressBar = null;
					swc.TextBlock percentageText = null;

					foreach (var child in grid.Children)
					{
						// If the progress bar and the percentage text are found, break.
						if (progressBar != null && percentageText != null)
							break;
						else if (child is swc.ProgressBar)
							progressBar = child as swc.ProgressBar;
						else if (child is swc.TextBlock)
							percentageText = child as swc.TextBlock;
					}

					// Get the value
					float? value = Handler.GetValue(grid.DataContext);

					// If the value is -1
					if(!value.HasValue)
					{
						// Hide the bar and text
						progressBar.Visibility = sw.Visibility.Hidden;
						percentageText.Visibility = sw.Visibility.Hidden;
					}
					else
					{
						// Hide the bar and text
						progressBar.Visibility = sw.Visibility.Visible;
						percentageText.Visibility = sw.Visibility.Visible;
					}

					// Set the value of the progress bar and make sure that it is above 0.
					progressBar.Value = value.HasValue ? (double)value : 0;
					// Set the text on the percentage text text block
					percentageText.Text = (int)(progressBar.Value * 100f) + "%";

					Handler.FormatCell(grid, cell, dataItem);
				};
				return Handler.SetupCell(element);
			}

			protected override sw.FrameworkElement GenerateEditingElement(swc.DataGridCell cell, object dataItem)
			{
				return GenerateProgressBar(cell);
			}

			private swc.Grid GenerateProgressBar(swc.DataGridCell cell)
			{
				swc.Grid grid = (cell != null ? cell.Content as swc.Grid : null) ?? new swc.Grid();

				// Add a progress bar to the grid
				swc.ProgressBar progressBar = new swc.ProgressBar { Value = 0, Minimum = 0, Maximum = 1 };
				grid.Children.Add(progressBar);

				// Add a text block that shows the progress percentage to the grid
				swc.TextBlock textBlock = new swc.TextBlock { Text = (int)progressBar.Value + "%", HorizontalAlignment = sw.HorizontalAlignment.Center,
					VerticalAlignment = sw.VerticalAlignment.Center, Style = TextStyle };
				grid.Children.Add(textBlock);

				return grid;
			}
		}
	}
}

