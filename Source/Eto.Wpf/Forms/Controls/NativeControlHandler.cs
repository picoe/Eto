using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sw = System.Windows;

namespace Eto.Wpf.Forms.Controls
{
	public class NativeControlHandler : WpfFrameworkElement<sw.FrameworkElement, Control, Control.ICallback>
	{
		public NativeControlHandler(sw.FrameworkElement nativeControl)
		{
			Control = nativeControl;
		}

		public override Eto.Drawing.Color BackgroundColor
		{
			get
			{
				throw new NotSupportedException("You cannot get this property for native controls");
			}
			set
			{
				throw new NotSupportedException("You cannot set this property for native controls");
			}
		}
	}
}
