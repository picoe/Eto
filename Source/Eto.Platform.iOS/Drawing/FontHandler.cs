using System;
using Eto.Drawing;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Drawing
{
	public class FontHandler : WidgetHandler<UIFont, Font>, IFont, IDisposable
	{
		FontFamily family;
		float size;
		bool bold;
		bool italic;
		UIFont font;
		
		public UIFont GetFont()
		{
			if (font != null) return font;
			
			string familyString;
			switch (family)
			{
			case FontFamily.Monospace: familyString = "Courier New"; break; 
			default:
			case FontFamily.Sans: familyString = "Helvetica"; break; 
			case FontFamily.Serif: familyString = "Times"; break; 
			}
			//NSFontTraitMask traits = (Bold) ? NSFontTraitMask.Bold : NSFontTraitMask.Unbold;
			//traits |= (Italic) ? NSFontTraitMask.Italic : NSFontTraitMask.Unitalic;
			font = UIFont.FromName(familyString, Size);
			//if (font == null || font.Handle == IntPtr.Zero) throw new Exception(string.Format("Could not allocate font with family {0}, traits {1}, size {2}", familyString, traits, Size));
			return font;
		}

		void Reset()
		{
			if (font == null) return;
			
			font.Dispose();
			font = null; 
		}

		public void Create(FontFamily family)
		{
			this.family = family;
			font = null;
		}

		public float Size
		{
			get { return size; }
			set {
				if (size != value)
				{
					size = value;
					Reset();
				}
			}
		}

		public bool Bold
		{
			get { return bold; }
			set
			{
				if (bold != value)
				{
					bold = value;
					Reset();
				}
			}
		}

		public bool Italic
		{
			get { return italic; }
			set
			{
				if (italic != value)
				{
					italic = value;
					Reset();
				}
			}
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (disposing) {
				if (font != null) { font.Dispose(); font = null; }
			}
		}


	}
}
