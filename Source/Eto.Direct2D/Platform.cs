using Eto.Drawing;
using Eto.Forms;
using Eto.Direct2D.Drawing;
using Eto.Direct2D.Forms.Controls;
using Eto.Direct2D.Forms.Printing;

namespace Eto.Direct2D
{
    public class Platform : Eto.WinForms.Platform
    {
        public override string ID
        {
            get { return Platforms.Direct2D; }
        }

        public Platform()
        {
			// generator to use for scenarios where direct 2d doesn't work (e.g. printing)
			BaseGenerator = new Eto.WinForms.Platform();

			Eto.WinForms.Platform.AddTo(this);

            // This is added after the base class's assembly
			AddTo(this);
        }

		public Eto.Platform BaseGenerator
		{
			get;
			set;
		}

		public static new void AddTo(Eto.Platform p)
		{
			// Drawing
			p.Add<IBitmap>(() => new BitmapHandler());
			p.Add<IFontFamily>(() => new FontFamilyHandler());
			p.Add<IFont>(() => new FontHandler());
			p.Add<IFonts>(() => new FontsHandler());
			p.Add<IGraphics>(() => new GraphicsHandler());
			p.Add<IGraphicsPathHandler>(() => new GraphicsPathHandler());
			p.Add<IIcon>(() => new IconHandler());
			p.Add<IIndexedBitmap>(() => new IndexedBitmapHandler());
			p.Add<IMatrixHandler>(() => new MatrixHandler());
			p.Add<IPen>(() => new PenHandler());
			p.Add<ISolidBrush>(() => new SolidBrushHandler());
			p.Add<ITextureBrush>(() => new TextureBrushHandler());
			p.Add<ILinearGradientBrush>(() => new LinearGradientBrushHandler());

			// Forms.Cells
			p.Add<IDrawable>(() => new DrawableHandler());

			// Forms.Printing
			p.Add<IPrintDocument>(() => new PrintDocumentHandler());
		}
    }
}
