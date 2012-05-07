﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public interface IGridHandler
	{
		sw.FrameworkElement SetupCell (IGridColumnHandler column, sw.FrameworkElement defaultContent);
	}

	public interface IGridColumnHandler : IGridColumn
	{
		swc.DataGridColumn Control { get; }
	}


	public class GridColumnHandler : WidgetHandler<swc.DataGridColumn, GridColumn>, IGridColumnHandler, ICellContainerHandler
	{
		Cell dataCell;

		public IGridHandler GridHandler { get; set; }

		public override void Initialize ()
		{
			base.Initialize ();
			DataCell = new TextBoxCell (Widget.Generator);
			Editable = false;
		}

		public string HeaderText
		{
			get { return Control.Header as string; }
			set { Control.Header = value;  }
		}

		public bool Resizable
		{
			get { return Control.CanUserResize; }
			set { Control.CanUserResize = value; }
		}

		public bool Sortable
		{
			get { return Control.CanUserSort; }
			set { Control.CanUserSort = value; }
		}

		public bool AutoSize
		{
			get { return Control.Width.IsAuto; }
			set { Control.Width = value ? new swc.DataGridLength(1, swc.DataGridLengthUnitType.Auto) : new swc.DataGridLength(100); }
		}

		public int Width
		{
			get
			{
				return (int)Control.Width.Value;
			}
			set
			{
				Control.Width = new swc.DataGridLength (value);
			}
		}

		void CopyValues (Cell oldCell)
		{
			if (oldCell == null) return;
			var handler = (ICellHandler)oldCell.Handler;
			var oldControl = handler.Control;
			Control.Header = oldControl.Header;
			Control.CanUserResize = oldControl.CanUserResize;
			Control.CanUserSort = oldControl.CanUserSort;
			Control.Width = oldControl.Width;
			Control.MinWidth = oldControl.MinWidth;
			Control.MaxWidth = oldControl.MaxWidth;
			Control.Visibility = oldControl.Visibility;
			Control.IsReadOnly = oldControl.IsReadOnly;
		}

		public Cell DataCell
		{
			get { return dataCell; }
			set
			{
				var oldCell = dataCell;
				dataCell = value;
				var cellHandler = (ICellHandler)dataCell.Handler;
				cellHandler.ContainerHandler = this;
				Control = cellHandler.Control;
				CopyValues (oldCell);
			}
		}

		public bool Editable
		{
			get { return !Control.IsReadOnly; }
			set { Control.IsReadOnly = !value; }
		}

		public bool Visible
		{
			get { return Control.Visibility == sw.Visibility.Visible; }
			set { Control.Visibility = (value) ? sw.Visibility.Visible : sw.Visibility.Hidden; }
		}

		public void Setup (IGridHandler gridHandler)
		{
			this.GridHandler = gridHandler;
		}

		public sw.FrameworkElement SetupCell (ICellHandler cell, sw.FrameworkElement defaultContent)
		{
			if (this.GridHandler != null)
				return this.GridHandler.SetupCell (this, defaultContent);
			else
				return defaultContent;
		}

		swc.DataGridColumn IGridColumnHandler.Control
		{
			get { return (swc.DataGridColumn)this.Control; }
		}

	}
}
