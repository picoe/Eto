using System;
using System.Drawing;
using System.Collections;

namespace Eto.Forms.WXWidgets
{
	public abstract class WXControl : IControl
	{
		private Widget widget;
		private Size size = new Size(0,0);
		private Point position = new Point(0,0);
		private bool enabled = true;

		public WXControl(Widget widget)
		{
			this.widget = widget;
		}

		public Widget Widget
		{
			get { return widget; }
		}

		public Generator Generator
		{
			get { return widget.Generator; }
		}

		protected wx.Window WXControlObject
		{
			get { return (wx.Window)widget.ControlObject; }
		}

		public object CreateControl(Container parent)
		{
			object o = CreateControl(parent, parent.ContainerObject);
			WXControlObject.Enabled = enabled;
			/*
			switch (dock)
			{
				case DockStyle.Fill:
					sizer = new wx.BoxSizer(wx.Orientation.wxHORIZONTAL);
					sizer.Add(WXControlObject, 1, wx.Stretch.wxEXPAND);
					sizer.Fit(WXControlObject.Parent);
					WXControlObject.Parent.Sizer = sizer;
					break;
			}
			*/
			return o;
		}

		public virtual object CreateControl(Control parent, object container)
		{
			return widget.ControlObject;
		}


		#region IControl Members

		public void Initialize()
		{
		
		}

		public bool Enabled
		{
			get { return (WXControlObject == null) ? enabled : WXControlObject.Enabled; }
			set { if (WXControlObject == null) enabled = value; else WXControlObject.Enabled = value; }
		}

		public Size ClientSize
		{
			get { return (WXControlObject == null) ? size : WXControlObject.ClientSize; }
		}


		public System.Drawing.Size Size
		{
			get { return (WXControlObject == null) ? size : WXControlObject.Size; }
			set { if (WXControlObject == null) size = value; else WXControlObject.Size = value; } 
		}

		public System.Drawing.Point Location
		{
			get { return (WXControlObject == null) ? position : WXControlObject.Position; }
			set { if (WXControlObject == null) position = value; else WXControlObject.Position = value; } 
		}

		public virtual string Text { get { return string.Empty; } set { } } 

		public void Invalidate()
		{
		}

		#endregion

		#region IWidget Members

		public abstract object ControlObject { get; }

		#endregion
	}
}
