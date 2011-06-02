using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class ButtonHandler : GtkControl<Gtk.Button, Button>, IButton
	{
		Gtk.AccelLabel label;

		public ButtonHandler()
		{
			Control = new Gtk.Button();
			label = new Gtk.AccelLabel(string.Empty);
			Control.Add(label);
			Control.Clicked += delegate { Widget.OnClick(EventArgs.Empty); };
			var defaultSize = Button.DefaultSize;
			Control.SizeAllocated += delegate(object o, Gtk.SizeAllocatedArgs args) {
				var size = args.Allocation;
				if (size.Width > 1 || size.Height > 1)
				{
					if (defaultSize.Width > size.Width) size.Width = defaultSize.Width;
					if (defaultSize.Height > size.Height) size.Height = defaultSize.Height;
					if (args.Allocation != size) Control.SetSizeRequest(size.Width, size.Height);
				}
			};
			//Control.SetSizeRequest(Button.DefaultSize.Width, Button.DefaultSize.Height);
		}

		public override string Text
		{
			get { return MnuemonicToString(label.Text); }
			set { 
				label.TextWithMnemonic = StringToMnuemonic(value);
			}
		}
		
	}
}
