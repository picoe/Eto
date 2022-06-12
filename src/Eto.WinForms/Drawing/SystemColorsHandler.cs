using Eto.Drawing;
using sd = System.Drawing;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Drawing
{
	public class SystemColorsHandler : SystemColors.IHandler
	{
		public Color ControlBackground =>sd.SystemColors.Window.ToEto();

		public Color Control => sd.SystemColors.Control.ToEto();

		public Color ControlText => sd.SystemColors.ControlText.ToEto();

		public Color HighlightText => sd.SystemColors.HighlightText.ToEto();

		public Color Highlight => sd.SystemColors.Highlight.ToEto();

		public Color WindowBackground => sd.SystemColors.Window.ToEto();

		public Color DisabledText => sd.SystemColors.GrayText.ToEto();

		public Color SelectionText => sd.SystemColors.HighlightText.ToEto();

		public Color Selection => sd.SystemColors.Highlight.ToEto();

		public Color LinkText => sd.SystemColors.Highlight.ToEto();
	}
}
