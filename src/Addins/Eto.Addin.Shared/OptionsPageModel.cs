using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Eto.Addin.Shared
{
	public class OptionsPageModel
	{
		public virtual string Title { get; set; }

		public List<Option> Options { get; } = new List<Option>();

		public OptionsPageModel(XElement element)
		{
			Title = (string)element?.Attribute("title");
			Options.AddRange(Option.LoadXml(element));
		}
	}
}
