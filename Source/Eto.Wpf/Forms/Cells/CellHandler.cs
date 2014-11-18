using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Wpf.Forms.Cells
{
	public interface ICellContainerHandler
	{
		sw.FrameworkElement SetupCell(ICellHandler cell, sw.FrameworkElement defaultContent);
		void FormatCell(ICellHandler cell, sw.FrameworkElement element, swc.DataGridCell datacell, object dataItem);
	}

	public interface ICellHandler : Cell.IHandler
	{
		ICellContainerHandler ContainerHandler { get; set; }
		swc.DataGridColumn Control { get; }
	}

	public abstract class CellHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, ICellHandler
		where TControl : swc.DataGridColumn
		where TWidget : Cell
	{
		public ICellContainerHandler ContainerHandler { get; set; }

		swc.DataGridColumn ICellHandler.Control
		{
			get { return Control; }
		}

		public void FormatCell(sw.FrameworkElement element, swc.DataGridCell cell, object dataItem)
		{
			ContainerHandler.FormatCell(this, element, cell, dataItem);
		}

		public sw.FrameworkElement SetupCell(sw.FrameworkElement defaultContent)
		{
			return ContainerHandler != null ? ContainerHandler.SetupCell(this, defaultContent) : defaultContent;
		}
	}
}