using System;

using Eto.Forms;

using SWUC = System.Web.UI.WebControls;
using SWUH = System.Web.UI.HtmlControls;
using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	public class MenuItemHandler : MenuHandler, IMenuItem
	{
		
		public String ShortcutText
		{
			get
			{
				return null;
			}
			set
			{
				// TODO
			}
		}
		
		public bool Enabled
		{
			get
			{
				// TODO
				return false;
			}
			set
			{
				// TODO
			}
		}
		
		SWUC.WebControl control = null;
		
		public MenuItemHandler(Widget widget) : base(widget)
		{
			control = new SWUC.WebControl(SWU.HtmlTextWriterTag.Div);
			//control.Click += new EventHandler(control_Click);
		}
		
		public override object ControlObject
		{
			get { return control; }
		}
		
		/*
		private void control_Click(object sender, EventArgs e)
		{
			((MenuItem)Widget).OnClick(e);
		}
		*/
		
		#region IMenuItem Members
		
		public string Text
		{
			get
			{
				return string.Empty;
			}
			set	{  }
		}
		
		#endregion
	}
}

