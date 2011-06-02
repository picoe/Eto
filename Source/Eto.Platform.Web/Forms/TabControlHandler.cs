using System;
using System.Collections;

using Eto.Forms;

using SWUC = System.Web.UI.WebControls;
using SWUH = System.Web.UI.HtmlControls;
using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	public class TabControlHandler : WebControl, ITabControl
	{
		private SWUC.Table table;
		//private WebControls.TabStrip control;
		private string text = string.Empty;
		private SWUC.PlaceHolder tabArea;
		private ArrayList tabs;
		
		public TabControlHandler(Widget widget) : base(widget)
		{
			SWUC.TableRow tr;
			SWUC.TableCell tc;
			tabs = new ArrayList();
			table = new SWUC.Table();
			
			tr = new SWUC.TableRow();
			tc = new SWUC.TableCell();
			tc.VerticalAlign = SWUC.VerticalAlign.Top;
			tr.Height = new SWUC.Unit("1");
			//control = new WebControls.TabStrip();
			//control.SelectedIndexChanged += new EventHandler(control_SelectedIndexChanged);
			//tc.Controls.Add(control);
			
			tr.Cells.Add(tc);
			table.Rows.Add(tr);
			
			tr = new SWUC.TableRow();
			tc = new SWUC.TableCell();
			tc.VerticalAlign = SWUC.VerticalAlign.Top;
			
			tabArea = new SWUC.PlaceHolder();
			
			tc.Controls.Add(tabArea);
			tr.Cells.Add(tc);
			table.Rows.Add(tr);
			
			table.Load += new EventHandler(table_Load);
		}
		
		
		public override object ControlObject
		{
			get
			{ return table; }
		}
		
		public override string Text
		{
			get
			{ return text; }
			set
			{ text = value; }
		}
		
		public int SelectedIndex
		{
			get { return 0; }
			set { }
		}
		
		public void AddTab(TabPage page)
		{
			//control.Items.Add((WebControls.TabItem)page.ControlObject);
			tabs.Add(page);
		}
		
		public void RemoveTab(TabPage page)
		{
			//tabs.RemoveAt(control.Items.IndexOf((WebControls.TabItem)page.ControlObject));
			//control.Items.Remove((WebControls.TabItem)page.ControlObject);
		}
		
		/*
		private void control_SelectedIndexChanged(object sender, EventArgs e)
		{
			((TabControl)Widget).OnSelectedIndexChanged(e);
			tabArea.Controls.Clear();
			TabPage page = (TabPage)tabs[SelectedIndex];
			SWU.Control control = (SWU.Control)page.ContainerObject;
			tabArea.Controls.Add(control);
		}
		*/
		
		private void table_Load(object sender, EventArgs e)
		{
			// called when the tab control is loaded
			if (tabs.Count > SelectedIndex)
			{
				tabArea.Controls.Clear();
				TabPage page = (TabPage)tabs[SelectedIndex];
				SWU.Control control = (SWU.Control)page.ContainerObject;
				tabArea.Controls.Add(control);
			}
		}
	}
}

