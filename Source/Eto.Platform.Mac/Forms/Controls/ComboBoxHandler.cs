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
		
		public void AddRange (IEnumerable<object> collection)
		{
			Control.AddItems (collection.Select (r => Convert.ToString (r)).ToArray ());
		}

		public void AddItem (object item)
		{
			Control.AddItem (Convert.ToString (item));
		}

		public void RemoveItem (object item)
		{
			Control.RemoveItem (Convert.ToString (item));
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
