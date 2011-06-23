using System;
using System.Reflection;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.Windows
{
	public class ComboBoxHandler : WindowsControl<SWF.ComboBox, ComboBox>, IComboBox
	{

		public ComboBoxHandler()
		{
			Control = new SWF.ComboBox();
			this.Control.DropDownStyle = SWF.ComboBoxStyle.DropDownList;
			this.Control.ValueMember = "Key";
			this.Control.DisplayMember = "Text";
			Control.SelectedIndexChanged += delegate {
				Widget.OnSelectedIndexChanged(EventArgs.Empty);
			};
		}


		#region IListControl Members
		
		public void AddRange (IEnumerable<IListItem> collection)
		{
			this.Control.SuspendLayout();
			this.Control.Items.AddRange(collection.ToArray());
			this.Control.ResumeLayout();
		}
		

		public void AddItem(IListItem item)
		{
			Control.Items.Add(item);
		}

		public void RemoveItem(IListItem item)
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

	}
}
