using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", typeof(Icon))]
	public class IconSection : Panel
	{
		public IconSection()
		{
			Content = new TabControl
			{
				Pages =
				{
					new TabPage { Text="Multi-resolution", Content = CreateViews(TestIcons.Logo) },
					new TabPage { Text = ".ico", Content = CreateViews(TestIcons.TestIcon) }
				}
			};
		}

		Control CreateViews(Image image)
		{
			var layout = new DynamicLayout();
			layout.BeginHorizontal();
			layout.Add("ImageView");
			layout.AddAutoSized(new ImageView { Image = image });
			layout.AddAutoSized(new ImageView { Image = image, Size = new Size(64, 64) });
			layout.AddAutoSized(new ImageView { Image = image, Size = new Size(32, 32) });
			layout.EndBeginHorizontal();
			layout.Add("Drawable");
			layout.AddAutoSized(new DrawableImageView { Image = image });
			layout.AddAutoSized(new DrawableImageView { Image = image, MinimumSize = new Size(64, 64), ScaleImage = true });
			layout.AddAutoSized(new DrawableImageView { Image = image, MinimumSize = new Size(32, 32), ScaleImage = true });
			layout.EndBeginHorizontal();
			layout.Add("Button");
			layout.AddAutoSized(new Button { Image = image, Text = "Auto Size" });
			layout.AddAutoSized(new Button { Image = image, Text = "64px Height", Height = 64 });
			layout.AddAutoSized(new Button { Image = image, Text = "32px Height", Height = 32 });
			layout.EndBeginHorizontal();
			layout.EndHorizontal();
			return layout;
		}
	}
}