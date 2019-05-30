using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Serialization.Xaml;
using Eto.Forms;
using System.IO;
using System.Reflection;

namespace Eto.Designer.Builders
{
	public class XamlInterfaceBuilder : IInterfaceBuilder
	{
		public IBuildToken Create(string text, string mainAssembly, IEnumerable<string> references, Action<Control> controlCreated, Action<Exception> error)
		{
			var oldDesignMode = XamlReader.DesignMode;
			XamlReader.DesignMode = true;
			try
			{
				using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text ?? ""), false))
				{
					stream.Position = 0;
					var control = XamlReader.Load<Panel>(stream);
					if (control != null)
						controlCreated(control);
				}
			}
			catch (Exception ex)
			{
				error(ex);
			}
			finally
			{
				XamlReader.DesignMode = oldDesignMode;
			}
			return null;
		}
	}
}
