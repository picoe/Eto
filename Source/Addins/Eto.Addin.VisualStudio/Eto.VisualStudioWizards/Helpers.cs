using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Eto.VisualStudioWizards
{
	static class Helpers
	{
		public static Window MainWindow
		{
			get
			{
				return WpfHelpers.ToEtoWindow(Process.GetCurrentProcess().MainWindowHandle);
			}
		}

		public static XNamespace WizardNamespace { get; private set; }

		static Helpers()
		{
			WizardNamespace = XNamespace.Get("http://schemas.microsoft.com/developer/vstemplate/2005");
		}

		public static XDocument LoadWizardXml(Dictionary<string, string> replacementsDictionary)
		{
			if (replacementsDictionary.ContainsKey("$wizarddata$"))
			{
				var wizardData = "<root>" + replacementsDictionary["$wizarddata$"] + "</root>";
				return XDocument.Load(new StringReader(wizardData));
			}
			return XDocument.Load(new StringReader("<root></root>"));
		}
		
		static readonly Regex propertyRegex = new Regex(@"\$[\w\.]+\$", RegexOptions.Compiled);

		internal static string ReplaceProperties(string value, Dictionary<string, string> replacementsDictionary)
		{
			return propertyRegex.Replace(value, match => replacementsDictionary[match.Value]);
		}
	}
}
