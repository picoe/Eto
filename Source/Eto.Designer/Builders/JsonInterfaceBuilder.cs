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
		public IBuildToken Create(string text, string mainAssembly, IEnumerable<string> references, Action<Forms.Control> controlCreated, Action<Exception> error)
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
				error(ex);
			}
			return null;
		}
	}
}
