using System;
using Eto.Forms;

namespace Eto.Test.Interface.Sections.Controls
{
	public class XamlSection : Panel
	{
		public XamlSection() {
			
			var layout = new DynamicLayout(this);
			
			layout.Add (new Xaml.Test());
		}
	}
}

