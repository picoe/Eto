using System;
using Eto.Drawing;

namespace Eto.GtkSharp.Drawing
{
	public class SystemColorsHandler : SystemColors.IHandler
	{
		readonly Gtk.TextView entry = new Gtk.TextView();
		readonly Gtk.Entry textEntry = new Gtk.Entry();

		public Color ControlText
		{
			get { return textEntry.GetTextColor(GtkStateFlags.Normal); }
		}

		public Color HighlightText
		{
			get { return entry.GetTextColor(GtkStateFlags.Selected); }
		}

		public Color Control
		{
			get { return new Gtk.EventBox().GetBackground(GtkStateFlags.Normal); }
		}

		public Color ControlBackground
		{
			get { return entry.GetBase(); }
		}

		public Color Highlight
		{
			get { return new Gtk.TreeView().GetBackground(GtkStateFlags.Selected); }
		}

		public Color WindowBackground
		{
			get { return new Gtk.Dialog().GetBase(GtkStateFlags.Normal); }
		}

		public Color DisabledText
		{
			get { return entry.GetForeground(GtkStateFlags.Insensitive); }
		}
	}
}

