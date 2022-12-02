using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
    public class BitmapTests : TestBase
    {
		[TestCase(null)]
		[TestCase("Blue")]
		[TestCase("White")]
		[TestCase("Black")]
		[ManualTest]
        public void TemplateImagesShouldDrawCorrectly(string color)
		{
			ManualForm("Both images should be the same in both light and dark mode",
			form => {
				
				Color? backgroundColor = color != null ? Color.Parse(color) : (Color?)null;
				var bmp = new Bitmap(200, 200, PixelFormat.Format32bppRgba);
				var img = bmp.ControlObject as NSImage;
				img.Template = true;
				using (var g = new Graphics(bmp))
				{
					g.PixelOffsetMode = PixelOffsetMode.Half;
					g.FillRectangle(new Color(Colors.Black, .5f), 39, 39, 120, 120);

					g.DrawRectangle(Colors.Red, 4.5f, 4.5f, 191, 191);
					g.FillRectangle(Colors.Red, 49, 49, 100, 100);


					g.DrawRectangle(Colors.Black, 0.5f, 0.5f, 199, 199);
					g.FillRectangle(Colors.Black, 59, 59, 80, 80);

					var path = new GraphicsPath();
					g.DrawPolygon(Colors.Black, new PointF(100, 20), new PointF(180, 180), new PointF(20, 180));

				}
				
				var drawable = new Drawable { Size = bmp.Size };
				drawable.Paint += (sender, e) => {
					if (backgroundColor != null)
						e.Graphics.Clear(backgroundColor.Value);
					e.Graphics.DrawImage(bmp, 0, 0);
					// e.Graphics.DrawImage(bmp, 50, 50, 100, 100);
					// e.Graphics.DrawImage(bmp, new RectangleF(99, 99, 100, 100), new Rectangle(0, 0, 200, 200));
				};
				
				var imageView = new ImageView();
				imageView.Image = bmp;
				if (backgroundColor != null)
					imageView.BackgroundColor = backgroundColor.Value;
				
				var content = new DynamicLayout();
				content.AddRow(drawable, imageView, null);
				content.AddSpace();
				return content;	
			}
			);
		}
    }
}