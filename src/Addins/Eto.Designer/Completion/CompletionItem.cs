using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Text;

namespace Eto.Designer.Completion
{

	public class CompletionItem
	{
		public CompletionType Type { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
	}
	
}
