using System;

using Eto.Forms;
using Eto.Drawing;

using SWUC = System.Web.UI.WebControls;
using SWUH = System.Web.UI.HtmlControls;
using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	public class FormHandler : WebContainer, IForm
	{
		
		public Icon Icon
		{
			get { return null; }
			set { }
		}
		
		public void AddToolbar(ToolBar toolBar)
		{
			// TODO: Implement this method
		}
		
		public void RemoveToolbar(ToolBar toolBar)
		{
			// TODO: Implement this method
		}
		
		public void ClearToolbars()
		{
			// TODO: Implement this method
		}
		
		
		public FormHandler(Widget widget) : base(widget)
		{
			//control = new SWUC.Panel();
			//control.Style["position"] = "relative";
			
			//control = new WebControls.ControlHolder();
			//control.Load += new EventHandler(control_Load);
		}
		
		public override object ControlObject
		{
			get { return null; }
		}
		
		public override object ContainerObject
		{
			get { return null; }
		}
		
		
		public override string Text
		{
			get { return string.Empty; }
			set {  }
		}
		
		public override Size Size
		{
			get { return new Size(0, 0); }
			set {  }
		}
		
		public bool Resizable
		{
			get { return false; }
			set { }
		}

		#region IForm Members
		
		public MenuBar Menu
		{
			get
			{
				// TODO:  Add FormHandler.Menu getter implementation
				return null;
			}
			set
			{
				// TODO:  Add FormHandler.Menu setter implementation
			}
		}
		
		public void Show()
		{
			// create form in control holder?
		}
		
		public void Close()
		{
			// destroy form from control holder??
		}
		
		#endregion
	}
}

