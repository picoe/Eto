using System;
using Eto.Drawing;
using System.Collections.Generic;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Linq;

namespace Eto.Platform.Mac.Drawing
{
	public class FontFamilyHandler : WidgetHandler<object, FontFamily>, IFontFamily
	{
		public string Name { get; set; }
		
		public IEnumerable<FontTypeface> Typefaces
		{
			get { 
				var descriptors = NSFontManager.SharedFontManager.AvailableMembersOfFontFamily (Name);
				return descriptors.Select (r => new FontTypeface(Widget, new FontTypefaceHandler(this, r)));
			}
		}

		public FontFamilyHandler ()
		{
		}

		public FontFamilyHandler (string name)
		{
			this.Name = name;
		}
		
		public void Create (string familyName)
		{
			this.Name = familyName;
		}

		public FontTypeface GetFace(NSFont font)
		{
			var postScriptName = font.FontDescriptor.PostscriptName;
			var faceHandler = Typefaces.Select (r => r.Handler).OfType<FontTypefaceHandler>().FirstOrDefault (r => r.PostScriptName == postScriptName);
			if (faceHandler == null)
				faceHandler = new FontTypefaceHandler(this, font);
			return new FontTypeface(Widget, faceHandler);
		}
	}
}

