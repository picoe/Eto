using System;
using Eto.Forms;

namespace Eto.Test.Interface.Controls
{
	public class XamlSection : Panel
	{
		public XamlSection() {
			
			var layout = new DynamicLayout(this);
			
			var obj = XamlReader.Load<Control>(Resources.GetResource ("Eto.Test.Interface.Xaml.test.xaml"));
			
			layout.Add (obj);
		}
	}
}

