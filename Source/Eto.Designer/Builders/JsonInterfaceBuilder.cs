using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Serialization.Json;
using Eto.Forms;
using System.IO;

namespace Eto.Designer.Builders
{
	public class JsonInterfaceBuilder : IInterfaceBuilder
	{
		public void Create(string text, Action<Forms.Control> controlCreated, Action<string> error)
		{
			try
			{
				using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text ?? ""), false))
				{
					stream.Position = 0;
					var control = JsonReader.Load<Panel>(stream);
					if (control != null)
						controlCreated(control);
				}
			}
			catch (Exception ex)
			{
				error(ex.ToString());
			}
		}

		public string GetSample()
		{
			return @"{
	$type: ""Scrollable"",
	Content: {
		$type: ""TableLayout"",
		Padding : ""10"",
		Spacing: ""5, 5"",
		Rows: [
			{
				Spacing: ""5, 5"",
				Rows: [
					[ { $type: ""Label"", Text: ""TextBox"" }, { $type: ""TextBox"" } ],
					[ { $type: ""Label"", Text: ""TextArea"" }, { $type: ""TextArea"" } ],
					[ { }, { $type: ""CheckBox"", Text: ""Some check box"" } ],
					[ { }, { $type: ""Slider"" } ]
				]
			},
			{
				Spacing: ""5, 5"",
				Rows: [
					[ null, { $type: ""Button"", Text: ""Cancel"" }, { $type: ""Button"", Text: ""Apply"" } ]
				]
			},
			null	
		]
	}
}";
		}
	}
}
