using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Interface.Controls
{
	public class NumericUpDownSection : Panel
	{
		public NumericUpDownSection ()
		{
			var layout = new DynamicLayout (this);

			layout.BeginHorizontal ();
			layout.Add (new Label{ Text = "Default" });
			layout.Add (new NumericUpDown{  });
			layout.EndHorizontal ();
			
			layout.BeginHorizontal ();
			layout.Add (new Label{ Text = "Set Min/Max" });
			layout.Add (new NumericUpDown{ Value = 24,  MinValue = 20, MaxValue = 2000 });
			layout.EndHorizontal ();
			
			
			// growing space at end is blank!
			layout.Add (null);
		}
	}
}

