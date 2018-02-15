using Eto.Forms;

namespace Eto.Test.Sections.Serialization
{
	[Section("Serialization", "Json")]
	public class JsonReadSection : Panel
	{
		public JsonReadSection()
		{
			Content = new Json.Test();
		}
	}
}

