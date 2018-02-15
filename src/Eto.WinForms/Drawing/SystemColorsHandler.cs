using Eto.Drawing;
using sd = System.Drawing;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Drawing
{
	public class SystemColorsHandler : SystemColors.IHandler
	{
		public Color ControlBackground
		{
			get { return sd.SystemColors.Window.ToEto(); }
		}

		public Color Control
		{
			get { return sd.SystemColors.Control.ToEto(); }
		}

		public Color ControlText
		{
			get { return sd.SystemColors.ControlText.ToEto(); }
		}

		public Color HighlightText
		{
			get { return sd.SystemColors.HighlightText.ToEto(); }
		}

		public Color Highlight
		{
			get { return sd.SystemColors.Highlight.ToEto(); }
		}

		public Color WindowBackground
		{
			get { return sd.SystemColors.Window.ToEto(); }
		}

		public Color DisabledText
		{
			get { return sd.SystemColors.GrayText.ToEto(); }
		}
	}
}
