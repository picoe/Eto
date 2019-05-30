using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", typeof(Icon))]
	public class IconSection : Panel
	{
		Panel mainContent;
		bool _use288DPI;
		public bool Use288DPI
		{
			get => _use288DPI;
			set
			{
				if (_use288DPI != value)
				{
					_use288DPI = value;
					CreateContent();
				}
			}
		}

		public IconSection()
		{
			mainContent = new Panel();

			var use288DPIImage = new CheckBox { Text = "Use 288 DPI image" };
			use288DPIImage.CheckedBinding.Bind(this, c => c.Use288DPI);

			var layout = new DynamicLayout();
			layout.BeginCentered();
			layout.Add(use288DPIImage);
			layout.EndCentered();
			layout.Add(mainContent);

			CreateContent();

			Content = layout;
		}

		void CreateContent()
		{
			var logo = Use288DPI ? TestIcons.Logo288 : TestIcons.Logo;

			mainContent.Content = new TabControl
			{
				Pages =
				{
					new TabPage { Text="Multi-resolution", Content = CreateViews(logo) },
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