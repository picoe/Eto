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
				var control = XamlReader.Load<Panel>(new StringReader(text), null);
				if (control != null)
					controlCreated(control);
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
