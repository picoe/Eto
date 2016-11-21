using Eto.Addin.Shared;
using Eto.Forms;
using Microsoft.VisualStudio.Shell.Interop;
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

namespace Eto.Addin.VisualStudio.Wizards
{
	static class Helpers
	{
		static Helpers()
		{
			EtoInitializer.Initialize();
		}

		public static Window MainWindow
		{
			get
			{
				var uiShell = Services.GetService<IVsUIShell>();
				IntPtr hwnd;
				uiShell.GetDialogOwnerHwnd(out hwnd);
				return new Form(new Eto.Wpf.Forms.HwndFormHandler(hwnd));
			}
		}

		public static XNamespace WizardNamespace { get; } = XNamespace.Get("http://schemas.microsoft.com/developer/vstemplate/2005");

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
            if (string.IsNullOrEmpty(value))
                return null;
			return propertyRegex.Replace(value, match => replacementsDictionary[match.Value]);
		}

		const string SupportedParametersPrefix = "$IsSupportedParameter.";
		const string ParametersPrefix = "$passthrough:";

		public static bool IsSupportedParameter(this Dictionary<string, string> replacementsDictionary, string name)
		{
			name = SupportedParametersPrefix + name + "$";
            string value;
			if (replacementsDictionary.TryGetValue(name, out value)
				|| replacementsDictionary.TryGetValue("$root." + name.TrimStart('$'), out value))
			{
				bool boolValue;
				if (bool.TryParse(value, out boolValue))
					return boolValue;
			}
			return false;
		}
		public static void SetSupportedParameter(this Dictionary<string, string> replacementsDictionary, string name, bool value)
		{
			replacementsDictionary[SupportedParametersPrefix + name + "$"] = value.ToString();
		}

		public static string GetParameter(this Dictionary<string, string> replacementsDictionary, string name)
		{
			string value;
			if (
				replacementsDictionary.TryGetValue("$" + name + "$", out value)
				|| replacementsDictionary.TryGetValue("$root." + name + "$", out value)
				|| replacementsDictionary.TryGetValue(ParametersPrefix + name + "$", out value)
				|| replacementsDictionary.TryGetValue("$root." + (ParametersPrefix + name).TrimStart('$') + "$", out value)
				)
				return value;
			return null;
		}
		public static void SetParameter(this Dictionary<string, string> replacementsDictionary, string propertyName, string value)
		{
			replacementsDictionary[ParametersPrefix + propertyName.Trim('$') + "$"] = value;
		}

		public static bool ToBool(this string value, bool defaultValue = false)
		{
			bool result;
			if (bool.TryParse(value, out result))
				return result;
			return defaultValue;
        }

		public static bool MatchesCondition(this Dictionary<string, string> replacementsDictionary, string conditionString)
		{
			if (string.IsNullOrEmpty(conditionString))
				return true;
			var conditions = conditionString.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var entry in conditions)
			{
				var condition = entry;
				bool matchValue = true;
				if (condition.StartsWith("!"))
				{
					condition = condition.Substring(1);
					matchValue = false;
				}
				var value = replacementsDictionary.GetParameter(condition).ToBool();

				if (matchValue != value)
					return false;
			}
			return true;
		}

		public static IEnumerable<ReplacementItem> FindMatchedItems(this IEnumerable<ReplacementGroup> groups, Dictionary<string, string> replacementsDictionary)
		{
			foreach (var group in groups.Where(r => replacementsDictionary.MatchesCondition(r.Condition)))
			{
				foreach (var item in group.Replacements.Where(r => replacementsDictionary.MatchesCondition(r.Condition)))
				{
					yield return item;
				}
			}
		}

		public static void SetMatchedItems(this IEnumerable<ReplacementGroup> groups, Dictionary<string, string> replacementsDictionary)
		{
			foreach (var replacement in groups.FindMatchedItems(replacementsDictionary))
			{
				var content = replacement.Content;
				if (replacement.ReplaceParameters)
				{
					foreach (var replacementItem in replacementsDictionary)
					{
						content = content.Replace(replacementItem.Key, replacementItem.Value);
					}
				}
				replacementsDictionary[replacement.Name] = content;
			}
		}
	}
}
