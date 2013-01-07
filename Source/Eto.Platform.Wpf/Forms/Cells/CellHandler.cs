using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public interface ICellContainerHandler
	{
		sw.FrameworkElement SetupCell (ICellHandler cell, sw.FrameworkElement defaultContent);
		void FormatCell (ICellHandler cell, sw.FrameworkElement element, swc.DataGridCell datacell, object dataItem);
	}

	public interface ICellHandler : ICell
	{
		ICellContainerHandler ContainerHandler { get; set; }
		swc.DataGridColumn Control { get; }
	}

	public abstract class CellHandler<T,W> : WidgetHandler<T, W>, ICellHandler
		where T: swc.DataGridColumn
		where W: Cell
	{
		public ICellContainerHandler ContainerHandler { get; set; }

		swc.DataGridColumn ICellHandler.Control
		{
			get { return Control; }
		}

		public void FormatCell (sw.FrameworkElement element, swc.DataGridCell cell, object dataItem)
		{
			ContainerHandler.FormatCell (this, element, cell, dataItem);
		}

		public sw.FrameworkElement SetupCell (sw.FrameworkElement defaultContent)
		{
			if (ContainerHandler != null)
				return ContainerHandler.SetupCell (this, defaultContent);
			else
				return defaultContent;
		}
	}
}