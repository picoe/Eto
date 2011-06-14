using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Interface.Controls
{
	public class ButtonSection : Panel
	{
		public ButtonSection ()
		{
			var layout = new PixelLayout(this);
			
			layout.Add (new Button{ Text = "Hello", 
				Size = new Size(100, 100) 
			}, 0, 0);
		}
	}
}

