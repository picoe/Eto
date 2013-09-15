using System;
using Eto.Forms;

namespace Eto.Test.Sections.Serialization
{
	public class JsonReadSection : Panel
	{
		public JsonReadSection()
		{
			Content = new Json.Test();
		}
	}
}

