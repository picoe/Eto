using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Layouts.PixelLayoutSection
{
	[Section("FlowLayout", "Test")]
	public class FlowLayoutSection : Scrollable
	{
		public FlowLayoutSection()
		{
			var layout = new FlowLayout();

			TextBox tb;
			layout.Contents.Add(new Label { Text = "Hello", Margin = new Padding(10) });
			layout.Contents.Add(tb = new TextBox { Text = "There", Margin = new Padding(20) });
			layout.Contents.Add(new TextArea { Text = "There" });
			layout.Contents.Add(new CheckBox { Text = "Curtis" });
			layout.Contents.Add(new Button { Text = "Grow" }.With(r => r.Click += (sender, e) => {
				var size = tb.Size;
				size.Width += 100;
				tb.Size = size;
			}));
			layout.Contents.Add(new Label { Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum." });


			var stack = new StackLayout
			{
				Orientation = Forms.Orientation.Horizontal,
				Items = {
					"Hello", "There"
				}
			};

			layout.Contents.Add(stack);
			layout.Contents.Add(new Label { Text = "After" });
			/*for (int i = 0; i < 200; i++)
			{
				layout.Contents.Add(new TextBox { Text = "Hrm " + i });
			}*/
			Content = layout;
		}
	}
}
