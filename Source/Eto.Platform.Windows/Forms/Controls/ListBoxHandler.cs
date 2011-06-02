using System;
using System.Reflection;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.Windows
{
	public class ListBoxHandler : WindowsControl<SWF.ListBox, ListBox>, IListBox
	{

		public ListBoxHandler()
		{
			Control = new SWF.ListBox();
			Control.SelectedIndexChanged += control_SelectedIndexChanged;
			Control.IntegralHeight = false;
			Control.DoubleClick += control_DoubleClick;
			Control.KeyDown += new System.Windows.Forms.KeyEventHandler(control_KeyDown);
		}

		#region IListControl Members

		public void AddRange (IEnumerable<object> collection)
		{
			this.Control.SuspendLayout();
			this.Control.Items.AddRange(collection.ToArray());
			this.Control.ResumeLayout();
		}

		public void AddItem(object item)
		{
			Control.Items.Add(item);
		}

		public void RemoveItem(object item)
		{
			Control.Items.Remove(item);
		}

		public int SelectedIndex
		{
			get	{ return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}

		public void RemoveAll()
		{
			Control.Items.Clear();
		}

		#endregion

		private void control_SelectedIndexChanged(object sender, EventArgs e)
		{
			Widget.OnSelectedIndexChanged(e);
		}

		private void control_DoubleClick(object sender, EventArgs e)
		{
			Widget.OnActivated(EventArgs.Empty);
		}

		private void control_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == SWF.Keys.Return)
			{
				Widget.OnActivated(EventArgs.Empty);
				e.Handled = true;
			}
		}
	}
}
