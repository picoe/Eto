using System;
using System.Drawing;

namespace Eto.Forms.WXWidgets
{
	internal class FormHandler : WXContainer, IForm
	{
		wx.Frame control;
		wx.Panel panel;
		MenuBar menuBar = null;

		public event EventHandler Closed;

		public FormHandler(Widget widget) : base(widget)
		{
			control = new wx.Frame(string.Empty);
			control.EVT_CLOSE(new wx.EventListener(control_Close));
			panel = new wx.Panel(control);
		}

		public override object ControlObject
		{
			get { return control; }
		}

		public override object ContainerObject
		{
			get { return panel; }
		}

		protected void control_Close(object sender, wx.Event e)
		{
			control.Destroy();			
			((Form)Widget).OnClosed(EventArgs.Empty);
			if (Closed != null) Closed(this, EventArgs.Empty);
		}



		public override string Text
		{
			get { return control.Title; }
			set { control.Title = value; }
		}

		#region IForm Members

		public MenuBar Menu
		{
			get { return menuBar; }
			set 
			{
				menuBar = value;
				control.MenuBar = ((MenuBarHandler)menuBar.InnerControl).CreateMenu();
			}
		}

		public void Close()
		{
			control.Destroy();			
		}

		public void Show()
		{
			control.Show(true);
		}
		#endregion
	}
}
