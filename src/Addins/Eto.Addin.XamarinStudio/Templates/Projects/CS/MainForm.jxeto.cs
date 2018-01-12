using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.$if$($UseXeto$==True)Xaml$endif$$if$($UseJeto$==True)Json$endif$;

namespace ${Namespace}
{	
	public class ${EscapedIdentifier} : Form
	{	
		public ${EscapedIdentifier}()
		{
			$if$($UseXeto$==True)Xaml$endif$$if$($UseJeto$==True)Json$endif$Reader.Load(this);
		}

		protected void HandleClickMe(object sender, EventArgs e)
		{
			MessageBox.Show("I was clicked!");
		}

		protected void HandleQuit(object sender, EventArgs e)
		{
			Application.Instance.Quit();
		}
	}
}
