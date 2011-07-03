using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Drawing
{
	public class FontHandler : WidgetHandler<object, Font>, IFont
	{

		public void Apply (System.Windows.Controls.Control control)
		{
			control.FontFamily = new System.Windows.Media.FontFamily (Convert (Family));

			if (Bold) control.FontWeight = System.Windows.FontWeights.Bold;
			else control.FontWeight = System.Windows.FontWeights.Normal;

			if (Italic) control.FontStyle = System.Windows.FontStyles.Italic;
			else control.FontStyle = System.Windows.FontStyles.Normal;

			control.FontSize = Size;
		}

		string Convert (FontFamily family)
		{
			switch (family) {
				case FontFamily.Monospace:
					return "Courier New";
				case FontFamily.Sans:
					return "Verdana";
				case FontFamily.Serif:
					return "Times New Roman";
				default:
					throw new NotSupportedException ();
			}
		}

		public void Create (FontFamily family)
		{
			this.Family = family;
		}

		public FontFamily Family
		{
			get; set;
		}

		public bool Bold
		{
			get; set;
		}

		public bool Italic
		{
			get; set; 
		}

		public float Size
		{
			get; set; 
		}
	}
}
