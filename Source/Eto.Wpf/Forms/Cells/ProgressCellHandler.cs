using Eto.Forms;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;
using sw = System.Windows;

namespace Eto.Wpf.Forms.Cells
{
	public class ProgressCellHandler : CellHandler<ProgressCellHandler.DataGridProgressBarColumn, ProgressCell, ProgressCell.ICallback>, ProgressCell.IHandler
	{
		public int GetValue(object dataItem)
		{
			if (Widget.Binding != null)
				return (int)Widget.Binding.GetValue(dataItem);
			return 0;
		}

		public void SetValue(object dataItem, int value)
		{
			if (Widget.Binding != null)
				Widget.Binding.SetValue(dataItem, value);
		}

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
					    style.Setters.Add((sw.SetterBase) new sw.Setter(sw.UIElement.IsHitTestVisibleProperty, (object) false));
					    style.Setters.Add((sw.SetterBase) new sw.Setter(sw.UIElement.FocusableProperty, (object) false));
					    style.Setters.Add((sw.SetterBase) new sw.Setter(sw.FrameworkElement.HorizontalAlignmentProperty, (object) sw.HorizontalAlignment.Center));
					    style.Setters.Add((sw.SetterBase) new sw.Setter(sw.FrameworkElement.VerticalAlignmentProperty, (object) sw.VerticalAlignment.Top));
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
                var element = GenerateProgressBar(cell);
				    element.DataContextChanged += (sender, e) =>
				    {
					    var grid = sender as swc.Grid;

					    swc.ProgressBar progressBar = null;
					    swc.TextBlock textBlock = null;

					    foreach (var child in grid.Children)
						    if (progressBar != null && textBlock != null)
							    break;
						    else if (child is swc.ProgressBar)
							    progressBar = child as swc.ProgressBar;
						    else if (child is swc.TextBlock)
							    textBlock = child as swc.TextBlock;

					    progressBar.Value = Handler.GetValue(grid.DataContext);
					    textBlock.Text = ((int)progressBar.Value) + "%";

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
			    swc.Grid grid = (cell != null ? cell.Content as swc.Grid : (swc.Grid)null) ?? new swc.Grid();
			    if (grid != null)
			    {
				    swc.ProgressBar progressBar = new swc.ProgressBar();
				    grid.Children.Add(progressBar);

				    swc.TextBlock textBlock = new swc.TextBlock();
				    textBlock.Text = ((int)progressBar.Value) + "%";
				    textBlock.HorizontalAlignment = sw.HorizontalAlignment.Center;
				    textBlock.VerticalAlignment = sw.VerticalAlignment.Center;
				    sw.Style textStyle = new sw.Style(typeof(swc.TextBlock), textBlock.Style);
				    textStyle.Setters.Add(new sw.Setter(swc.TextBlock.ForegroundProperty, (object)sw.SystemColors.ControlTextBrush));
				    textBlock.Style = textStyle;
				    grid.Children.Add(textBlock);
			    }
			    return grid;
		    }
	    }

		public ProgressCellHandler()
		{
			Control = new DataGridProgressBarColumn { Handler = this };
		}
	}
}

