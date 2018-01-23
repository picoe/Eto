using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using Eto.Wpf.Drawing;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Cells
{
	public class DrawableCellHandler : CellHandler<swc.DataGridColumn, DrawableCell, DrawableCell.ICallback>, DrawableCell.IHandler
	{
		public static int ImageSize = 16;

		object GetValue(object dataItem)
		{
			return dataItem;
		}

		public class EtoCanvas : swc.Canvas
		{
			public Column Column { get; set; }

			public bool IsSelected { get; set; }

			protected override void OnRender(swm.DrawingContext dc)
			{
				var handler = Column.Handler;
				var graphics = new Graphics(new GraphicsHandler(this, dc, new sw.Rect(RenderSize), new RectangleF(RenderSize.ToEto()), false));
				var state = IsSelected ? CellStates.Selected : CellStates.None;
#pragma warning disable 618
				var args = new DrawableCellPaintEventArgs(graphics, new Rectangle(RenderSize.ToEtoSize()), state, DataContext);
#pragma warning restore 618
				handler.Callback.OnPaint(handler.Widget, args);
			}
		}

		public class Column : swc.DataGridColumn
		{
			public DrawableCellHandler Handler { get; set; }

			EtoCanvas Create(swc.DataGridCell cell)
			{
				var control = cell.Content as EtoCanvas;
				if (control == null)
				{
					control = new EtoCanvas { Column = this };
					control.DataContextChanged += (sender, e) =>
					{
						var ctl = sender as EtoCanvas;
						ctl.IsSelected = cell.IsSelected;
						Handler.FormatCell(ctl, cell, ctl.DataContext);
						ctl.InvalidateVisual();
					};
					cell.Selected += (sender, e) =>
					{
						control.IsSelected = cell.IsSelected;
						control.InvalidateVisual();
					};
					cell.Unselected += (sender, e) =>
					{
						control.IsSelected = cell.IsSelected;
						control.InvalidateVisual();
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

		public DrawableCellHandler()
		{
			Control = new Column { Handler = this };
		}
	}
}
