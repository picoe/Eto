using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(ImageView))]
	public class ImageViewSection : Panel
	{
		public ImageViewSection()
		{
			var tabs = new TabControl();

			tabs.Pages.Add(FixedSize());
			tabs.Pages.Add(ScaledSize());

			Content = tabs;
		}

		TabPage FixedSize()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };
			AddHeaders(layout);

			layout.AddRow(new Label { Text = "Auto Sized" }, AutoSized(GetBitmap()), AutoSized(GetIcon()));
			layout.AddRow(new Label { Text = "Small Size" }, SmallSize(GetBitmap()), SmallSize(GetIcon()));
			layout.AddRow(new Label { Text = "Large Size" }, LargeSize(GetBitmap()), LargeSize(GetIcon()));

			var page = new TabPage { Text = "Fixed Size" };
			page.Content = new Scrollable
			{
				Border = BorderType.Bezel,
				ExpandContentWidth = false,
				ExpandContentHeight = false,
				Content = layout
			};
			return page;
		}

		TabPage ScaledSize()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };
			AddHeaders(layout);

			layout.AddRow(new Label { Text = "Scaled Size" }, Scaled(GetBitmap()), Scaled(GetIcon()));

			var page = new TabPage { Text = "Scaled Size" };
			page.Content = new Scrollable { Border = BorderType.None, Content = layout };
			return page;
		}

		static void AddHeaders(DynamicLayout layout)
		{
			layout.BeginHorizontal();
			layout.Add(null, xscale: false);
			layout.Add(new Label { Text = "Bitmap", TextAlignment = TextAlignment.Center }, xscale: true);
			layout.Add(new Label { Text = "Icon", TextAlignment = TextAlignment.Center }, xscale: true);
			layout.EndHorizontal();
		}

		Icon GetIcon()
		{
			return TestIcons.Logo;
		}

		Bitmap GetBitmap()
		{
			return TestIcons.TestImage;
		}

		Control AutoSized(Image image)
		{
			return TableLayout.AutoSized(new ImageView { Image = image }, centered: true);
		}

		Control LargeSize(Image image)
		{
			return new ImageView { Image = image, Size = new Size(200, 200) };
		}

		Control SmallSize(Image image)
		{
			return new ImageView { Image = image, Size = new Size(64, 64) };
		}

		Control Scaled(Image image)
		{
			return new ImageView { Image = image };
		}
	}
}
