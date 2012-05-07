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

		public sw.FrameworkElement SetupCell (sw.FrameworkElement defaultContent)
		{
			if (ContainerHandler != null)
				return ContainerHandler.SetupCell (this, defaultContent);
			else
				return defaultContent;
		}
	}
}
