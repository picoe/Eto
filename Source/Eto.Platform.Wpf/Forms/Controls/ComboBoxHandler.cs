using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class ComboBoxHandler : WpfControl<swc.ComboBox, ComboBox>, IComboBox
	{
		public ComboBoxHandler ()
		{
			Control = new swc.ComboBox ();
			Control.SelectionChanged += delegate {
				Widget.OnSelectedIndexChanged (EventArgs.Empty);
			};

			var template = new sw.DataTemplate (typeof (IListItem));
			var labelFactory = new sw.FrameworkElementFactory (typeof (swc.TextBlock));
			labelFactory.SetBinding (swc.TextBlock.TextProperty, new swd.Binding ("Text"));
			template.VisualTree = labelFactory;
			Control.ItemTemplate = template;
		}

		public void AddRange (IEnumerable<IListItem> collection)
		{
			foreach (var item in collection)
				AddItem (item);
		}

		public void AddItem (IListItem item)
		{
			Control.Items.Add (item);
		}

		public void RemoveItem (IListItem item)
		{
			Control.Items.Remove (item);
		}

		public void RemoveAll ()
		{
			Control.Items.Clear ();
		}

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}
	}
}
