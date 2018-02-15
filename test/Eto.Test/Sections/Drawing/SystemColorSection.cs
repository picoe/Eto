using System;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;
using System.Reflection;
using System.Collections.Generic;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", typeof(SystemColors))]
	public class SystemColorSection : Panel
	{
		public SystemColorSection()
		{
			var layout = new StackLayout
			{
				Spacing = 10,
				HorizontalContentAlignment = HorizontalAlignment.Stretch
			};

			var type = typeof(SystemColors);

			var properties = type.GetRuntimeProperties();

			var skip = new List<PropertyInfo>();
			var colorProperties = properties.Where(r => r.PropertyType == typeof(Color)).OrderBy(r => r.Name).ToList();
			foreach (var property in colorProperties)
			{
				if (skip.Contains(property))
					continue;
				var color = (Color)property.GetValue(null);
				var label = new Label { Text = property.Name };
				var panel = new Panel
				{ 
					Content = label,
					Padding = new Padding(10),
				};

				bool isTextColor = property.Name.EndsWith("Text");

				if (isTextColor)
					label.TextColor = color;
				else
				{
					panel.BackgroundColor = color;
					var textProp = colorProperties.FirstOrDefault(r => r.Name == property.Name + "Text");
					if (textProp != null)
					{
						label.TextColor = (Color)textProp.GetValue(null);
						label.Text += " && " + textProp.Name;
						skip.Add(textProp);
					}
					else if (color.ToHSB().B < 0.5)
						label.TextColor = Colors.White;
				}

				layout.Items.Add(panel);
			}

			Content = new Scrollable { Content = TableLayout.AutoSized(layout, centered: true) };

		}
	}
}

