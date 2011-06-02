using System;
using System.Reflection;
using SD = System.Drawing;
using Eto.Forms;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class ComboBoxHandler : MacControl<UIButton, ComboBox>, IComboBox
	{
		
		public ComboBoxHandler()
		{
			Control = new NSPopUpButton();
			Control.Activated += delegate {
				Widget.OnSelectedIndexChanged(EventArgs.Empty);
			};
			
			/*Control.Changed += delegate {
				Widget.OnSelectedIndexChanged(EventArgs.Empty);
			};
			Control.Editable = false;*/
		}

		public void AddItem(object item)
		{
			Control.AddItem(Convert.ToString(item));
		}

		public void RemoveItem(object item)
		{
			Control.RemoveItem(Convert.ToString(item));
		}

		public int SelectedIndex
		{
			get	{ return Control.IndexOfSelectedItem; }
			set { Control.SelectItem(value); }
		}

		public void RemoveAll()
		{
			Control.RemoveAllItems();
		}

	}
}
