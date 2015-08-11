using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Addin.VisualStudio.Wizards
{
	public class BaseClassPanelWizard : BaseClassWizard
	{
		protected override string TypeName { get { return "panel"; } }

		protected override IEnumerable<BaseClassDefinition> Definitions
		{
			get
			{
				yield return new BaseClassDefinition
				{
					Name = "Panel",
					Description = "An empty panel with your custom content",
					CodeContent = @"
Content = new Label { Text = ""Some Content"" };"
				};

				yield return new BaseClassDefinition
				{
					Name = "Scrollable",
					Description = "An empty panel with content that can be scrolled",
					CodeContent = @"
Content = new Label { Text = ""Some Content"" };"
				};

				yield return new BaseClassDefinition
				{
					Name = "Drawable",
					Description = "A custom drawn panel",
					Methods = @"
protected override void OnPaint(PaintEventArgs e)
{
	// your custom drawing
	e.Graphics.FillRectangle(Colors.Blue, e.ClipRectangle);
}"
				};

				yield return new BaseClassDefinition
				{
					Name = "GroupBox",
					Description = "A box panel with a title",
					CodeContent = @"
Text = ""My group box"";

Content = new Label { Text = ""Some Content"" };"
				};
			}
		}
	}
}
