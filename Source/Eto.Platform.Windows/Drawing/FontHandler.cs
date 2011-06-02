using System;
using System.Collections.Generic;
using System.Text;
using Eto.Drawing;
using SD = System.Drawing;
using SWF = System.Windows.Forms;

namespace Eto.Platform.Windows.Drawing
{
	public class FontHandler : WidgetHandler<SD.Font, Font>, IFont
	{
		FontFamily family;
		bool bold;
		bool italic;
		float size;

		public override SD.Font Control {
			get {
				if (base.Control == null) 
				{
					SD.FontStyle style = SD.FontStyle.Regular;
					if (bold) style |= SD.FontStyle.Bold;
					if (italic) style |= SD.FontStyle.Italic;
					Control = new SD.Font(Generator.Convert(family), size, style);
				}
				return base.Control;
			}
			protected set {
				base.Control = value;
			}
		}
		
		#region IFont Members

		public void Create(FontFamily family)
		{
			this.family = family;
		}

		public float Size
		{
			get { return size; }
			set { size = value; Reset(); }
		}

		public bool Bold
		{
			get { return bold; }
			set { bold = value; Reset(); }
		}

		private void Reset()
		{
			if (Control != null)
			{
				Control.Dispose();
				Control = null;
			}
		}

		public bool Italic
		{
			get { return italic; }
			set { italic = value; Reset(); }
		}

		#endregion


	}
}
