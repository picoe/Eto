using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Interface.Controls
{
	public class TextAreaSection : Panel
	{
		public TextAreaSection ()
		{
			var layout = new TableLayout(this, 2, 4);
			
			layout.Add (new Label{ Text = "Default" }, 0, 0);
			layout.Add (new TextArea{ Text = "Some Text" }, 1, 0);

			layout.Add (new Label{ Text = "Different Size" }, 0, 1);
			layout.Add (new TextArea{ Text = "Some Text", Size = new Size(100, 80) }, 1, 1);
			
			layout.Add (new Label{ Text = "Read Only" }, 0, 2);
			layout.Add (new TextArea{ Text = "Read only text", ReadOnly = true, Size = new Size(100, 80) }, 1, 2);
			
			
		}
	}
}

