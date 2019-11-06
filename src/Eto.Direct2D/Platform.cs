using Eto.Drawing;
using Eto.Forms;
using Eto.Direct2D.Drawing;
using Eto.Direct2D.Forms.Controls;
using Eto.Direct2D.Forms.Printing;

namespace Eto.Direct2D
{
	public class Platform : Eto.WinForms.Platform
	{
		public override string ID => "Direct2D";

		public override PlatformFeatures SupportedFeatures =>
			base.SupportedFeatures & ~ PlatformFeatures.DrawableWithTransparentContent;


		public Platform()
		{
			// generator to use for scenarios where direct 2d doesn't work (e.g. printing)
			BasePlatform = new Eto.WinForms.Platform();

			Eto.WinForms.Platform.AddTo(this);

			// This is added after the base class's assembly
			AddTo(this);
		}

		public Eto.Platform BasePlatform
		{
			get;
			set;
		}

		public static new void AddTo(Eto.Platform p)
		{
			// Drawing
			p.Add<Bitmap.IHandler>(() => new BitmapHandler());
			p.Add<FontFamily.IHandler>(() => new FontFamilyHandler());
			p.Add<Font.IHandler>(() => new FontHandler());
			p.Add<Fonts.IHandler>(() => new FontsHandler());
			p.Add<Graphics.IHandler>(() => new GraphicsHandler());
			p.Add<GraphicsPath.IHandler>(() => new GraphicsPathHandler());
			p.Add<Icon.IHandler>(() => new IconHandler());
			p.Add<IconFrame.IHandler>(() => new IconFrameHandler());
			p.Add<IndexedBitmap.IHandler>(() => new IndexedBitmapHandler());
			p.Add<Matrix.IHandler>(() => new MatrixHandler());
			p.Add<Pen.IHandler>(() => new PenHandler());
			p.Add<SolidBrush.IHandler>(() => new SolidBrushHandler());
			p.Add<TextureBrush.IHandler>(() => new TextureBrushHandler());
			p.Add<LinearGradientBrush.IHandler>(() => new LinearGradientBrushHandler());
			p.Add<RadialGradientBrush.IHandler>(() => new RadialGradientBrushHandler());
			p.Add<FormattedText.IHandler>(() => new FormattedTextHandler());

			// Forms.Cells
			p.Add<Drawable.IHandler>(() => new DrawableHandler());

			// Forms.Printing
			p.Add<PrintDocument.IHandler>(() => new PrintDocumentHandler());
		}
	}
}
