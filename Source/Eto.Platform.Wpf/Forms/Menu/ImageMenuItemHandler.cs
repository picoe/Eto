using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using swi = System.Windows.Input;

namespace Eto.Platform.Wpf.Forms.Menu
{
	public class ImageMenuItemHandler : WpfMenuItem<swc.MenuItem, ImageMenuItem>, IImageMenuItem
	{
		public ImageMenuItemHandler ()
		{
			Control = new swc.MenuItem();
			Setup ();
		}

	}
}
