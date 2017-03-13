using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using Eto.Wpf.Drawing;
using Eto.Drawing;
using System.Collections.Generic;
using System;

namespace Eto.Wpf.Forms.Cells
{
	public class CustomCellHandler : CellHandler<swc.DataGridColumn, CustomCell, CustomCell.ICallback>, CustomCell.IHandler
	{
		public static int ImageSize = 16;

		object GetValue(object dataItem)
		{
			return dataItem;
		}

		public class WpfCellEventArgs : CellEventArgs
		{
			public WpfCellEventArgs(int row, object item, CellStates cellState)
				: base(row, item, cellState)
			{
			}

			public void SetSelected(bool selected)
			{ 
				IsSelected = selected;
				CellTextColor = selected ? SystemColors.HighlightText : SystemColors.ControlText;
			}

			public void SetDataContext(object dataContext)
			{
				Item = dataContext;
			}
		}

		public class EtoBorder : swc.Border
		{
			public WpfCellEventArgs Args { get; set; }

			public Control Control { get; set; }

			public Column Column { get; set; }

			public string Identifier { get; set; }

			protected override void OnRender(swm.DrawingContext dc)
			{
				var handler = Column.Handler;
			}
		}

		public class Column : swc.DataGridColumn
		{
			public CustomCellHandler Handler { get; set; }
			static string defaultId = Guid.NewGuid().ToString();
			Dictionary<string, Stack<Control>> cellCache = new Dictionary<string,Stack<Control>>();

			Stack<Control> GetCached(string id)
			{
				id = id ?? defaultId;
				Stack<Control> cachedList;
				if (!cellCache.TryGetValue(id, out cachedList))
				{
					cachedList = new Stack<Control>();
					cellCache.Add(id, cachedList);
				}
				return cachedList;
			}

			EtoBorder Create(swc.DataGridCell cell)
			{
				var control = cell.Content as EtoBorder;
				if (control == null)
				{
					control = new EtoBorder { Column = this };
					control.Args = new WpfCellEventArgs(-1, null, CellStates.None);
					control.Unloaded += (sender, e) =>
					{
						var ctl = sender as EtoBorder;
						ctl.Control?.DetachNative();
					};
					control.Loaded += (sender, e) =>
					{
						var ctl = sender as EtoBorder;
						ctl.Control?.AttachNative();
					};
					control.DataContextChanged += (sender, e) =>
					{
						var ctl = sender as EtoBorder;
						ctl.Args.SetSelected(cell.IsSelected);
						ctl.Args.SetDataContext(ctl.DataContext);
						var id = Handler.Callback.OnGetIdentifier(Handler.Widget, ctl.Args);
						var child = ctl.Control;
						if (id != ctl.Identifier || child == null)
						{
							Stack<Control> cache;
							if (child != null)
							{
								// store old child into cache
								cache = GetCached(ctl.Identifier);
								cache.Push(child);
							}
							// get new from cache or create if none created yet
							cache = GetCached(id);
							if (cache.Count > 0)
								child = cache.Pop();
							else
								child = Handler.Callback.OnCreateCell(Handler.Widget, ctl.Args);
							ctl.Control = child;
							var handler = child.GetWpfFrameworkElement();
							if (handler != null)
								handler.SetScale(true, true);
							ctl.Child = child.ToNative(ctl.IsLoaded);
						}
						Handler.Callback.OnConfigureCell(Handler.Widget, ctl.Args, child);

						Handler.FormatCell(ctl, cell, ctl.DataContext);
						ctl.InvalidateVisual();
					};
					cell.Selected += (sender, e) =>
					{
						control.Args.SetSelected(cell.IsSelected);
						Handler.Callback.OnConfigureCell(Handler.Widget, control.Args, control.Control);
					};
					cell.Unselected += (sender, e) =>
					{
						control.Args.SetSelected(cell.IsSelected);
						Handler.Callback.OnConfigureCell(Handler.Widget, control.Args, control.Control);
					};
				}
				return control;
			}

			protected override sw.FrameworkElement GenerateElement(swc.DataGridCell cell, object dataItem)
			{
				return Handler.SetupCell(Create(cell));
			}

			protected override sw.FrameworkElement GenerateEditingElement(swc.DataGridCell cell, object dataItem)
			{
				return Handler.SetupCell(Create(cell));
			}
		}

		public CustomCellHandler()
		{
			Control = new Column { Handler = this };

		}
	}
}