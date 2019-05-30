using System;
using Eto.Drawing;

namespace Eto.GtkSharp.Drawing
{
	public class SystemColorsHandler : SystemColors.IHandler
	{
		readonly Gtk.TextView entry = new Gtk.TextView();
		readonly Gtk.Entry textEntry = new Gtk.Entry();
		readonly Gtk.EventBox eventBox = new Gtk.EventBox();
		readonly Gtk.Dialog dialog = new Gtk.Dialog();
		readonly Gtk.TreeView treeView = new Gtk.TreeView();

		public Color ControlText => textEntry.GetTextColor(GtkStateFlags.Normal);

		public Color HighlightText => entry.GetTextColor(GtkStateFlags.Selected);

		public Color Control => eventBox.GetBackground(GtkStateFlags.Normal);

		public Color ControlBackground => entry.GetBase();

		public Color Highlight => treeView.GetBackground(GtkStateFlags.Selected);

		public Color WindowBackground => dialog.GetBase(GtkStateFlags.Normal);

		public Color DisabledText => entry.GetForeground(GtkStateFlags.Insensitive);

		public Color SelectionText => HighlightText;

		public Color Selection => Highlight;

		public Color LinkText => Highlight;
	}
}

