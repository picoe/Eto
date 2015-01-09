using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using System.Collections.Generic;


namespace Eto.Designer.Builders
{
	public class CSharpInterfaceBuilder : CodeInterfaceBuilder
	{
		protected override CodeDomProvider CreateCodeProvider()
		{
			var options = new Dictionary<string, string> { { "CompilerVersion", "v4.0" } };
			return new CSharpCodeProvider(options);
		}

		protected override void SetParameters(CompilerParameters parameters)
		{
			base.SetParameters(parameters);
			/* Needed for PCL version of Eto.dll
			 */
			string facadeDir = null;
			if (EtoEnvironment.Platform.IsWindows)
			{
				var programFiles = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
				facadeDir = programFiles + @"\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\Facades";
				if (!Directory.Exists(facadeDir))
					facadeDir = Environment.GetEnvironmentVariable("ProgramFiles") + @"\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\Facades";
			}
			else if (EtoEnvironment.Platform.IsMac)
				facadeDir = @"/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5/Facades";

			if (Directory.Exists(facadeDir))
			{
				foreach (var file in Directory.GetFiles(facadeDir))
				{
					parameters.ReferencedAssemblies.Add(file);
				}
			}
			/**/
		}

		public override string GetSample()
		{
			return @"using Eto.Forms;
using Eto.Drawing;

class MyPanel : Scrollable
{
	public MyPanel()
	{
		Content = new TableLayout
		{
			Padding = new Padding(10),
			Spacing = new Size(5, 5),
			Rows =
			{
				new TableLayout
				{
					Spacing = new Size(5, 5),
					Rows =
					{
						new TableRow(new Label { Text = ""TextBox"" }, new TextBox()),
						new TableRow(new Label { Text = ""TextArea"" }, new TextArea()),
						new TableRow(new TableCell(), new CheckBox { Text = ""Some check box"" }),
						new TableRow(new TableCell(), new Slider { SnapToTick = true }),
					}
				},
				new TableLayout
				{
					Spacing = new Size(5, 5),
					Rows =
					{
						new TableRow(
							null,
							new Button { Text = ""Cancel"" }.With(r => r.Click += (s, e) => MessageBox.Show(""Cancel!"")),
							new Button { Text = ""Apply"" }.With(r => r.Click += (s, e) => MessageBox.Show(""Apply!""))
						)
					}
				},
				null
			}
		};
	}
}";
		}
	}
}

