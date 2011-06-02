using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class SeparatorMenuItemHandler : MenuHandler<Gtk.SeparatorMenuItem, SeparatorMenuItem>, ISeparatorMenuItem
	{

		public SeparatorMenuItemHandler()
		{
			Control = new Gtk.SeparatorMenuItem();
		}

		public override void AddMenu(int index, MenuItem item)
		{
			throw new NotSupportedException("Cannot add items to a separator");
		}

		public override void RemoveMenu(MenuItem item)
		{
			throw new NotSupportedException("Separators do not contain any items to remove");
		}

		public override void Clear()
		{
			throw new NotSupportedException("Separators do not contain any items to remove");
		}
	}
}
