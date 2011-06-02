using System;

using Eto.Forms;
using Eto.Drawing;

using SWUC = System.Web.UI.WebControls;
using SWUH = System.Web.UI.HtmlControls;
using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	public abstract class WebControl : IControl
	{
		
		public Color BackgroundColor
		{
			get { return Color.Black; }
			set {  }
		}
		
		public void Invalidate()
		{
		}
		
		public void Invalidate(Rectangle rect)
		{
		}
		
		public Graphics CreateGraphics()
		{
			return null;
		}
		
		public void SuspendLayout()
		{
		}
		
		public void ResumeLayout()
		{
		}
		
		public void Focus()
		{
		}
		
		public bool HasFocus
		{
			get { return false; }
		}
		
		public bool Visible
		{
			get { return this.WebControlObject.Visible; }
			set { WebControlObject.Visible = value; }
		}
		
		public void SetWidget(Widget widget)
		{
			// TODO: Implement this method
		}
		
		public virtual bool HandleEvent(String handler)
		{
			// TODO: Implement this method
			return false;
		}
		
		public IAsyncResult BeginInvoke(Delegate method, Object[] args)
		{
			return null;
		}
		
		public Object EndInvoke(IAsyncResult result)
		{
			return null;
		}
		
		public Object Invoke(Delegate method, Object[] args)
		{
			return null;
		}
		
		public bool InvokeRequired
		{
			get { return false; }
		}
		
		private Widget widget;
		private Size size = new Size(0,0);
		
		public WebControl(Widget widget)
		{
			this.widget = widget;
		}
		
		public Widget Widget
		{
			get { return widget; }
		}

		public Control Control
		{
			get { return (Control)widget; }
		}
		
		public void Initialize()
		{
		}
		
		public SWUC.WebControl WebControlObject
		{
			get { return (SWUC.WebControl)ControlObject; }
		}
		
		public SWUH.HtmlControl HtmlControl
		{
			get { return (SWUH.HtmlControl)ControlObject; }
		}
		
		
		#region IControl Members
		
		public string Id
		{
			get { return ((SWU.Control)ControlObject).ID; }
			set { ((SWU.Control)ControlObject).ID = value; }
		}
		
		public virtual string Text
		{
			get { return string.Empty; }
			set { }
		}
		
		public bool Enabled
		{
			get { return WebControlObject.Enabled; }
			set { WebControlObject.Enabled = value; }
		}
		
		
		public virtual Size Size
		{
			get { return size; }
			set
			{
				size = value;
				SWUC.WebControl control = WebControlObject;
				control.Style["WIDTH"] = (Control.Width > 0) ? Control.Width + "px" : null;
				control.Style["HEIGHT"] = (Control.Height > 0) ? Control.Height + "px" : null;
			}
		}
		
		public virtual Size ClientSize
		{
			set { Size = value; }
			get { return Size; }
		}
		
		#endregion
		
		#region IWidgetBase Members
		
		public abstract object ControlObject
		{ get; }
		
		#endregion
	}
}

