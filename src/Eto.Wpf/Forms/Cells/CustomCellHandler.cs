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

			public void SetSelected(swc.DataGridCell cell)
			{
				var row = cell.GetVisualParent<swc.DataGrid>();
				var selected = cell.IsSelected;
				IsSelected = selected;
				var focused = row?.IsKeyboardFocusWithin == true;
				CellTextColor = selected && focused ? SystemColors.HighlightText : SystemColors.ControlText;
			}

			public void SetDataContext(object dataContext)
			{
				Item = dataContext;
			}
		}

		public class EtoBorder : swc.Border
		{
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

			static sw.DependencyProperty dpSelectedHookedUp = sw.DependencyProperty.Register("SelectedHandled", typeof(bool), typeof(sw.FrameworkElement));

			EtoBorder Create(swc.DataGridCell cell)
			{
				var control = cell.Content as EtoBorder;
				if (control == null)
				{
					control = new EtoBorder { Column = this };
					control.Unloaded += HandleControlUnloaded;
					control.Loaded += HandleControlLoaded;
					control.DataContextChanged += HandleControlDataContextChanged;
					if (!Equals(cell.GetValue(dpSelectedHookedUp), true))
					{
						cell.SetValue(dpSelectedHookedUp, true);
						cell.Selected += HandleCellSelectedChanged;
						cell.Unselected += HandleCellSelectedChanged;
					}
					var grid = cell.GetVisualParent<swc.DataGrid>();
					if (grid != null && !Equals(grid.GetValue(dpSelectedHookedUp), true))
					{
						grid.SetValue(dpSelectedHookedUp, true);
						grid.IsKeyboardFocusWithinChanged += HandleRowFocusChanged;
					}
				}
				return control;
			}

			static void HandleRowFocusChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
			{
				var grid = sender as swc.DataGrid;
				if (grid == null)
					return;
				foreach (var item in grid.SelectedItems)
				{
					var row = grid.ItemContainerGenerator.ContainerFromItem(item) as swc.DataGridRow;
					if (row == null)
						continue;
					foreach (var ctl in row.FindVisualChildren<swc.DataGridCell>())
					{
						HandleCellSelectedChanged(ctl, null);
					}
				}
			}

			static readonly object CellEventArgs_Key = new object();

			static void HandleControlDataContextChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
			{
				var ctl = sender as EtoBorder;
				var cell = ctl?.GetParent<swc.DataGridCell>();
				var col = cell?.Column as Column;
				var handler = col?.Handler;
				if (handler == null)
					return;
				var args = new WpfCellEventArgs(-1, ctl.DataContext, CellStates.None);
				args.SetSelected(cell);
				var id = handler.Callback.OnGetIdentifier(handler.Widget, args);
				var child = ctl.Control;
				if (id != ctl.Identifier || child == null)
				{
					Stack<Control> cache;
					if (child != null)
					{
						// store old child into cache
						cache = col.GetCached(ctl.Identifier);
						cache.Push(child);
					}
					// get new from cache or create if none created yet
					cache = col.GetCached(id);
					if (cache.Count > 0)
					{
						child = cache.Pop();
						if (child.Properties.ContainsKey(CellEventArgs_Key))
						{
							args = child.Properties.Get<WpfCellEventArgs>(CellEventArgs_Key);
							args.SetSelected(cell);
							args.SetDataContext(ctl.DataContext);
						}
						else
							child.Properties.Set(CellEventArgs_Key, args);
					}
					else
					{
						child = handler.Callback.OnCreateCell(handler.Widget, args);
						child.GetWpfFrameworkElement()?.SetScale(true, true);
						child.Properties.Set(CellEventArgs_Key, args);
					}
					ctl.Control = child;
					ctl.Identifier = id;
					ctl.Child = child.ToNative(ctl.IsLoaded);
				}
				else
				{
					if (child.Properties.ContainsKey(CellEventArgs_Key))
					{
						args = child.Properties.Get<WpfCellEventArgs>(CellEventArgs_Key);
						args.SetSelected(cell);
						args.SetDataContext(ctl.DataContext);
					}
					else
						child.Properties.Set(CellEventArgs_Key, args);
				}
				handler.Callback.OnConfigureCell(handler.Widget, args, child);

				handler.FormatCell(ctl, cell, ctl.DataContext);
				ctl.InvalidateVisual();
			}

			static void HandleControlLoaded(object sender, sw.RoutedEventArgs e)
			{
				// WPF's loaded event is called more than once, e.g. when on a tab that is not initially visible.
				var wpfctl = sender as EtoBorder;
				var ctl = wpfctl.Control;
				if (ctl != null && !ctl.Loaded)
					ctl.AttachNative();
			}

			static void HandleControlUnloaded(object sender, sw.RoutedEventArgs e)
			{
				var wpfctl = sender as EtoBorder;
				var ctl = wpfctl.Control;
				if (ctl != null && ctl.Loaded)
					ctl.DetachNative();
			}

			static void HandleCellSelectedChanged(object sender, sw.RoutedEventArgs e)
			{
				var cell = sender as swc.DataGridCell;
				var col = cell?.Column as Column;
				if (cell?.Content is EtoBorder ctl)
				{
					var args = ctl.Control?.Properties.Get<WpfCellEventArgs>(CellEventArgs_Key);
					if (args != null)
					{
						args.SetSelected(cell);
						col.Handler.Callback.OnConfigureCell(col.Handler.Widget, args, ctl.Control);
					}
				}
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