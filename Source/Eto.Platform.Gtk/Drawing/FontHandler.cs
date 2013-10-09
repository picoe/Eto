using System;
using Eto.Drawing;
using System.Linq;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class FontHandler : WidgetHandler<Pango.FontDescription, Font>, IFont
	{
		FontFamily family;
		FontTypeface face;
		FontStyle? style;

		public FontHandler ()
		{
		}

		public FontHandler (Gtk.Widget widget)
			: this (widget.Style.FontDescription)
		{
		}

		public FontHandler (Pango.FontDescription fontDescription)
		{
			Control = fontDescription;
		}

		public FontHandler (string fontName)
		{
			Control = Pango.FontDescription.FromString (fontName);
		}

		public void Create (SystemFont systemFont, float? size)
		{
			switch (systemFont) {
			case SystemFont.Default:
				Control = Gtk.Window.DefaultStyle.FontDescription;
				break;
			case SystemFont.Bold:
				Control = new Pango.FontDescription();
				Control.Merge (Gtk.Window.DefaultStyle.FontDescription, true);
				Control.Weight = Pango.Weight.Bold;
				break;
			case SystemFont.TitleBar:
				Control = Gtk.Window.DefaultStyle.FontDescription;
				break;
			case SystemFont.ToolTip:
				Control = Gtk.Label.DefaultStyle.FontDescription;
				break;
			case SystemFont.Label:
				Control = Gtk.Label.DefaultStyle.FontDescription;
				break;
			case SystemFont.MenuBar:
				Control = Gtk.MenuBar.DefaultStyle.FontDescription;
				break;
			case SystemFont.Menu:
				Control = Gtk.Menu.DefaultStyle.FontDescription;
				break;
			case SystemFont.Message:
				Control = Gtk.MessageDialog.DefaultStyle.FontDescription;
				break;
			case SystemFont.Palette:
				Control = Gtk.Dialog.DefaultStyle.FontDescription;
				break;
			case SystemFont.StatusBar:
				Control = Gtk.Statusbar.DefaultStyle.FontDescription;
				break;
			default:
				throw new NotSupportedException();
			}
			if (size != null) {
				var old = Control;
				Control = new Pango.FontDescription();
				Control.Merge (old, true);
				Control.Size = (int)(size * Pango.Scale.PangoScale);
			}
		}
		
		public void Create (FontFamily family, float size, FontStyle style)
		{
			this.family = family;
			Control = new Pango.FontDescription();
			SetStyle (style);
			Size = size;

			Control.Family = family.Name;
		}

		public void Create (FontTypeface face, float size)
		{
			this.face = face;
			Control = ((FontTypefaceHandler)face.Handler).Control.Describe();
			Size = size;
		}

		void SetStyle (FontStyle style)
		{
			if ((style & FontStyle.Bold) != 0) 
				Control.Weight = Pango.Weight.Bold;
			if ((style & FontStyle.Italic) != 0) 
				Control.Style = Pango.Style.Italic;
		}

		public float Size
		{
			get { return (float)(Control.Size / Pango.Scale.PangoScale); }
			private set { Control.Size = (int)(value * Pango.Scale.PangoScale); }
		}
		
		public FontStyle FontStyle
		{
			get {
				if (style == null) {
					style = FontStyle.None;
					if (Control.Weight == Pango.Weight.Bold)
						style |= FontStyle.Bold;
					if (Control.Style == Pango.Style.Italic)
						style |= FontStyle.Italic;
				}
				return style.Value;
			}
		}

		public string FamilyName
		{
			get { return Control.Family; }
		}

		public FontFamily Family
		{
			get {
				if (family == null) {
					family = new FontFamily (Widget.Generator, Control.Family);
				}
				return family;
			}
		}

		public FontTypeface Typeface
		{
			get {
				if (face == null) {
					face = Family.Typefaces.FirstOrDefault(r => r.FontStyle == this.FontStyle);
				}
				return face;
			}
		}

		Pango.FontMetrics metrics;

		public Pango.FontMetrics Metrics
		{
			get {
				if (metrics == null)
					metrics = FontsHandler.Context.GetMetrics (Control, Pango.Language.Default);
				return metrics;
			}
		}

		public float Ascent
		{
			get { return (float)Metrics.Ascent / (float)Pango.Scale.PangoScale; }
		}
		
		public float Descent
		{
			get { return (float)Metrics.Descent / (float)Pango.Scale.PangoScale; }
		}

		float? lineHeight;
		public float LineHeight
		{
			get {
				if (lineHeight == null) {
					using (var layout = new Pango.Layout(FontsHandler.Context)) {
						layout.FontDescription = Control;
						layout.SetText ("X");
						Pango.Rectangle ink, logical;
						layout.GetExtents (out ink, out logical);
						lineHeight = (float)logical.Height / (float)Pango.Scale.PangoScale;
					}
				}
				return lineHeight ?? 0f;
			}
		}

		public float Baseline
		{
			get { return Ascent; }
		}

		float? leading;
		public float Leading
		{
			get {
				if (leading == null) {
					using (var layout = new Pango.Layout(FontsHandler.Context)) {
						layout.FontDescription = Control;
						layout.SetText ("X");
						Pango.Rectangle ink, logical;
						layout.GetExtents (out ink, out logical);
						leading = (float)(ink.Y - logical.Y) / (float)Pango.Scale.PangoScale;
					}
				}
				return leading ?? 0f;
			}
		}

		float? xheight;
		public float XHeight
		{
			get {
				if (xheight == null) {
					using (var layout = new Pango.Layout(FontsHandler.Context)) {
						layout.FontDescription = Control;
						layout.SetText ("x");
						layout.Spacing = 0;
						layout.Alignment = Pango.Alignment.Left;
						layout.Width = int.MaxValue;
						Pango.Rectangle ink, logical;
						layout.GetExtents (out ink, out logical);
						xheight = (float)ink.Height / (float)Pango.Scale.PangoScale;
					}
				}
				return xheight ?? 0f;
			}
		}
	}
}
