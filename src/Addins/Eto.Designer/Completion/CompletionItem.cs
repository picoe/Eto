using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Text;

namespace Eto.Designer.Completion
{
	[Flags]
	public enum CompletionBehavior
	{
		None = 0,
		ChildProperty = 1 << 0
	}

	public class CompletionItem
	{
		public CompletionType Type { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string Suffix { get; set; }

		public CompletionBehavior Behavior { get; set; }
	}
	
}
