using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Interface.Controls
{
	public class TextAreaSection : Panel
	{
		public TextAreaSection ()
		{
			var layout = new DynamicLayout (this);

			layout.BeginHorizontal ();
			layout.Add (new Label{ Text = "Default" });
			layout.Add (new TextArea{ Text = "Some Text" });
			layout.EndHorizontal ();
			
			layout.BeginHorizontal ();
			layout.Add (new Label{ Text = "Different Size" });
			layout.Add (new TextArea{ Text = "Some Text", Size = new Size (100, 80) });
			layout.EndHorizontal ();
			
			layout.BeginHorizontal ();
			layout.Add (new Label{ Text = "Read Only" });
			layout.Add (new TextArea{ Text = "Read only text", ReadOnly = true, Size = new Size (100, 80) });
			layout.EndHorizontal ();
			
			// growing space at end is blank!
			layout.Add (null);
		}
	}
}

