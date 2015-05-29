using Eto.Forms;
using System.Collections.Generic;
using Eto.Test.Sections.Controls;
using Eto.Test.Sections.Drawing;
using Eto.Test.Sections;
using System.Linq;
using System;
using System.Reflection;

namespace Eto.Test
{
	[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class SectionAttribute : System.Attribute
	{
		public string Category { get; private set; }

		public string Name { get; private set; }

		public Type Requires { get; set; }

		public SectionAttribute(string category, string name)
		{
			this.Category = category;
			this.Name = name;
		}

		public SectionAttribute(string category, Type controlType, string name = null)
		{
			this.Category = category;
			this.Name = name ?? controlType.Name;
			this.Requires = controlType;
		}
	}

	public static class TestSections
	{
		public static IEnumerable<Section> Get(IEnumerable<Assembly> testAssemblies)
		{
			var categories = new Dictionary<string, Section>();
			foreach (var asm in testAssemblies)
			{
				foreach (var type in asm.ExportedTypes)
				{
					#if PCL
					var section = type.GetTypeInfo().GetCustomAttribute<SectionAttribute>(false);
					#else
					var section = type.GetCustomAttribute<SectionAttribute>(false);
					#endif
					if (section != null)
					{
						if (section.Requires != null && !Platform.Instance.Supports(section.Requires))
							continue;

						Section category;
						if (!categories.TryGetValue(section.Category, out category))
							categories.Add(section.Category, category = new Section { Text = section.Category });

						var testType = type;
						category.Add(new SectionItem { Creator = () => Activator.CreateInstance(testType) as Control, Text = section.Name });
					}
				}
			}
			foreach (var category in categories.Values)
				category.Sort((x, y) => string.Compare(x.Text, y.Text, StringComparison.CurrentCultureIgnoreCase));
			return categories.Values.OrderBy(r => r.Text);
		}
	}
}
