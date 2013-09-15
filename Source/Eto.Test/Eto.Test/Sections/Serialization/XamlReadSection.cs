#if XAML
using System;
using Eto.Forms;

namespace Eto.Test.Sections.Serialization
{
	public class XamlReadSection : Panel
	{
		public XamlReadSection()
		{
			Content = new Xaml.Test();
		}
	}
}
#endif