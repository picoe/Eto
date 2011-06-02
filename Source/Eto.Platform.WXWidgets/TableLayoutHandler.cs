using System;
using System.Drawing;
using SWF = System.Windows.Forms;

namespace Eto.Forms.WXWidgets
{
	internal class TableLayoutHandler : WXLayout, ITableLayout
	{
		//SWF.TableLayoutPanel control;
		SWF.Control control;

		public TableLayoutHandler(Widget widget, int cols, int rows) : base(widget)
		{
		/*	control = new SWF.TableLayoutPanel();
			control.Dock = SWF.DockStyle.Fill;
			control.RowCount = rows;
			control.ColumnCount = cols;*/
		}

		public override object ControlObject
		{
			get { return control; }
		}


		public override void AddChild(Control child)
		{
			//control.Controls.Add((SWF.Control)child.ControlObject, child.Left, child.Top);
		}

		public override void RemoveChild(Control child)
		{
			control.Controls.Remove((SWF.Control)child.ControlObject);
		}

		#region IPositionalLayout Members

		public void SetLocation(Control child, int x, int y)
		{
			//control.SetRow((SWF.Control)child.ControlObject, y);
			//control.SetColumn((SWF.Control)child.ControlObject, x);
		}
		#endregion

	}
}
