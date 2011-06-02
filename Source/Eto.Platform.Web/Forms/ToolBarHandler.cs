using System;

using Eto.Forms;
using Eto.Drawing;

using SWU = System.Web.UI;
using SWUC = System.Web.UI.WebControls;

namespace Eto.Platform.Web.Forms
{
	public class ToolBarHandler : WebControl, IToolBar
	{
		SWUC.WebControl control;
		
		public ToolBarHandler(Widget widget)
		: base(widget)
		{
			control = new Eto.Web.NamedControl(SWU.HtmlTextWriterTag.Div);
			control.CssClass = "ToolBar";
		}
		
		public void AddButton(ToolBarItem button)
		{
			control.Controls.Add(button.ControlObject as SWU.Control);
		}
		
		public void RemoveButton(ToolBarItem button)
		{
			control.Controls.Remove(button.ControlObject as SWU.Control);
		}
		
		public void Clear()
		{
			control.Controls.Clear();
		}
		
		public ToolBarTextAlign TextAlign
		{
			get { return ToolBarTextAlign.Right; }
			set { }
		}
		
		public ToolBarDock Dock
		{
			get { return ToolBarDock.Top; }
			set { }
		}
		
		public override Object ControlObject
		{
			get { return control; }
		}
	}
}
