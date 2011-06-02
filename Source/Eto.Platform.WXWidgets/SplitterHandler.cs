using System;
using System.Drawing;

namespace Eto.Forms.WXWidgets
{
	internal class SplitterHandler : WXControl, ISplitter
	{
		Control panel1 = null;
		Control panel2 = null;
		wx.SplitterWindow control = null;
		int pos = 0;
		private SplitterOrientation orientation = SplitterOrientation.Horizontal;

		public SplitterHandler(Widget widget) : base(widget)
		{
			
		}

		public override object ControlObject
		{
			get { return control; }
		}

		public Control Panel1
		{
			get { return panel1; }
			set 
			{
				panel1 = value; 
			}
		}

		public Control Panel2
		{
			get { return panel2; }
			set 
			{
				panel2 = value;
			}
		}

		public int Position
		{
			get
			{
				return (control == null) ? pos : control.SashPosition;
			}
			set
			{
				if (control == null) pos = value; else control.SashPosition = value;
			}
		}

		public SplitterOrientation Orientation
		{
			get
			{
				if (control == null) return orientation;
				else if (control.SplitMode == wx.SplitMode.wxSPLIT_HORIZONTAL)
					return SplitterOrientation.Horizontal;
				else if (control.SplitMode == wx.SplitMode.wxSPLIT_VERTICAL)
					return SplitterOrientation.Vertical;
				return orientation;
			}
			set
			{
				if (control == null) orientation = value;
				else
				{
					switch (value)
					{
						default:
						case SplitterOrientation.Horizontal:
							control.SplitVertically((wx.Window)panel1.ControlObject, (wx.Window)panel2.ControlObject, pos);
							break;
						case SplitterOrientation.Vertical:
							control.SplitHorizontally((wx.Window)panel1.ControlObject, (wx.Window)panel2.ControlObject, pos);
							break;
					}
				}
			}
		}



		public override object CreateControl(Control parent, object container)
		{
			control = new wx.SplitterWindow((wx.Window)container, ((WXGenerator)Generator).GetNextButtonID(), Location, Size, 0, string.Empty);
			if (panel2 == null || panel1 == null) throw new Exception("must set both panels of a splitter before adding to a window");

			 ((WXControl)panel1.InnerControl).CreateControl((Control)this.Widget, control);
			((WXControl)panel2.InnerControl).CreateControl((Control)this.Widget, control);
			switch (orientation)
			{
				default:
				case SplitterOrientation.Horizontal:
					control.SplitVertically((wx.Window)panel1.ControlObject, (wx.Window)panel2.ControlObject, pos);
					break;
				case SplitterOrientation.Vertical:
					control.SplitHorizontally((wx.Window)panel1.ControlObject, (wx.Window)panel2.ControlObject, pos);
					break;
			}
			return control;
		}


	}
}
