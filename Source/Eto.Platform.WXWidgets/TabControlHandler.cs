using System;
using System.Collections;
using System.Drawing;

namespace Eto.Forms.WXWidgets
{
	internal class TabControlHandler : WXControl, ITabControl
	{
		private wx.Notebook control = null;
		private ArrayList tabs = new ArrayList();

		public TabControlHandler(Widget widget) : base(widget)
		{
		}


		public override object ControlObject
		{
			get { return control; }
		}

		public override string Text
		{
			get { return control.Name; }
			set { control.Name = value; }
		}

		public int SelectedIndex
		{
			get { return control.Selection; }
			set { control.Selection = value; }
		}

		public override object CreateControl(Control parent, object container)
		{
			control = new wx.Notebook((wx.Window)container);
			control.ID = ((WXGenerator)Generator).GetNextButtonID();
			control.EVT_NOTEBOOK_PAGE_CHANGED(control.ID, new wx.EventListener(control_PageChanged));

			foreach (TabPage tp in tabs)
			{
				AddTab(tp);
			}  

			return control;
		}


		public void AddTab(TabPage page)
		{
			if (control != null)
			{
				((WXControl)page.InnerControl).CreateControl((Container)Widget);
				control.AddPage((wx.Window)page.ControlObject, page.Text);
			}
			else
			{
				tabs.Add(page);
			}
		}

		public void RemoveTab(TabPage page)
		{
			if (control != null)
			{
				for (int i=0; i<control.PageCount; i++)
				{
					wx.Window window = control.GetPage(i);
					if (window == (wx.Window)page.ControlObject)
					{
						control.RemovePage(i);
						break;
					}
				}
			}
			else
			{
				tabs.Remove(page);
			}
		}

		private void control_PageChanged(object sender, wx.Event e)
		{
			((TabControl)Widget).OnSelectedIndexChanged(EventArgs.Empty);
			e.Skip(true);
		}
	}
}
