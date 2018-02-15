using Eto.Forms;

namespace Eto.Test.Sections.Serialization
{
	[Section("Serialization", "Xaml")]
	public class XamlReadSection : Panel
	{
		public XamlReadSection()
		{
			Content = new Xaml.Test();
		}
	}
}