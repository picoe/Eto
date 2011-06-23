using System;
using System.Reflection;
using SD = System.Drawing;
using Eto.Forms;
using System.Linq;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Collections.Generic;

namespace Eto.Platform.Mac
{
	public class ComboBoxHandler : MacControl<NSPopUpButton, ComboBox>, IComboBox
	{
		
		public ComboBoxHandler ()
		{
			Control = new NSPopUpButton ();
			Control.Activated += delegate {
				Widget.OnSelectedIndexChanged (EventArgs.Empty);
			};
			
			/*Control.Changed += delegate {
				Widget.OnSelectedIndexChanged(EventArgs.Empty);
			};
			Control.Editable = false;*/
		}
		
		public void AddRange (IEnumerable<IListItem> collection)
		{
			Control.AddItems (collection.Select (r => r.Text).ToArray ());
		}

		public void AddItem (IListItem item)
		{
			Control.AddItem (item.Text);
		}

		public void RemoveItem (IListItem item)
		{
			Control.RemoveItem (item.Text);
		}

		public int SelectedIndex {
			get	{ return Control.IndexOfSelectedItem; }
			set { Control.SelectItem (value); }
		}

		public void RemoveAll ()
		{
			Control.RemoveAllItems ();
		}

	}
}
