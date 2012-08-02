using System;
using Eto.Forms;

namespace Eto.Test.iOS
{
	public class MainForm : Form
	{
		Navigation nav;
		Splitter splitter;

		public MainForm ()
		{
			nav = new Navigation ();

			if (Splitter.Supported) {
				// ipad and tablets usually support this
				splitter = new Splitter();

				splitter.Panel1 = nav;
				splitter.Panel2 = new Panel();
				this.AddDockedControl (splitter);
			} else {
				// show list directly for smartphones
				this.AddDockedControl (nav);
			}

			SetContent();
		}

		void SetContent ()
		{
			var list = new ListBox ();
			list.SelectedIndexChanged += HandleListItemSelected;
			for (int i = 0; i<1000; i++) {
				list.Items.Add ("Item " + i);
			}
			nav.Push (list, "Test");
		}

		void HandleListItemSelected (object sender, EventArgs e)
		{
			var list = sender as ListBox;
			MessageBox.Show (this, string.Format ("You selected {0}!", list.SelectedValue.Text));
		}
	}
}

