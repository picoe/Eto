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
		public Color ControlBackground
		{
			get { return sw.SystemColors.WindowColor.ToEto(); }
		}

		public Color Control
		{
			get { return sw.SystemColors.ControlColor.ToEto(); }
		}

		public Color ControlText
		{
			get { return sw.SystemColors.ControlTextColor.ToEto(); }
		}

		public Color HighlightText
		{
			get { return sw.SystemColors.HighlightTextColor.ToEto(); }
		}

		public Color Highlight
		{
			get { return sw.SystemColors.HighlightColor.ToEto(); }
		}

		public Color WindowBackground
		{
			get { return sw.SystemColors.WindowColor.ToEto(); }
		}

		public Color DisabledText
		{
			get { return sw.SystemColors.GrayTextColor.ToEto(); }
		}
	}
}
