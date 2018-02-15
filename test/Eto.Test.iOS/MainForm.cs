using System;
using Eto.Forms;

namespace Eto.Test.iOS
{
	public class MainForm : Form
	{
		Navigation nav;
		Splitter splitter;

		public MainForm()
		{
			nav = new Navigation();

			if (Splitter.IsSupported)
			{
				// ipad and tablets usually support this
				splitter = new Splitter();

				splitter.Panel1 = nav;
				splitter.Panel2 = new Panel();
				Content = splitter;
			}
			else
			{
				// show list directly for smartphones
				Content = nav;
			}

			SetContent();
		}

		void SetContent()
		{
			var list = new ListBox();
			list.SelectedIndexChanged += HandleListItemSelected;
			for (int i = 0; i < 1000; i++)
			{
				list.Items.Add("Item " + i);
			}
			nav.Push(list, "Test");
		}

		void HandleListItemSelected(object sender, EventArgs e)
		{
			var list = sender as ListBox;
			MessageBox.Show(this, string.Format("You selected {0}!", ((ListItem)list.SelectedValue).Text));
		}
	}
}

