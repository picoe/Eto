using System;
using System.Globalization;
using Eto.Forms;
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
				throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "You cannot get this property for native controls"));
			}
			set
			{
				throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "You cannot set this property for native controls"));
			}
		}
	}
}
