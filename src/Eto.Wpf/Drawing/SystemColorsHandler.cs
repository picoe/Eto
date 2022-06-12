using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sw = System.Windows;
using swm = System.Windows.Media;

namespace Eto.Wpf.Drawing
{
	public class SystemColorsHandler : SystemColors.IHandler
	{
		public Color ControlBackground => sw.SystemColors.WindowColor.ToEto();

		public Color Control => sw.SystemColors.ControlColor.ToEto();

		public Color ControlText => sw.SystemColors.ControlTextColor.ToEto();

		public Color HighlightText => sw.SystemColors.HighlightTextColor.ToEto();

		public Color Highlight => sw.SystemColors.HighlightColor.ToEto();

		public Color WindowBackground => sw.SystemColors.WindowColor.ToEto();

		public Color DisabledText => sw.SystemColors.GrayTextColor.ToEto();

		public Color SelectionText => sw.SystemColors.HighlightTextColor.ToEto();

		public Color Selection => sw.SystemColors.HighlightColor.ToEto();

		public Color LinkText => sw.SystemColors.HighlightColor.ToEto();
	}
}
