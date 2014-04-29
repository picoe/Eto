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

		public static new void AddTo(Eto.Platform g)
		{
			// Drawing
			g.Add<IBitmap>(() => new BitmapHandler());
			g.Add<IFontFamily>(() => new FontFamilyHandler());
			g.Add<IFont>(() => new FontHandler());
			g.Add<IFonts>(() => new FontsHandler());
			g.Add<IGraphics>(() => new GraphicsHandler());
			g.Add<IGraphicsPathHandler>(() => new GraphicsPathHandler());
			g.Add<IIcon>(() => new IconHandler());
			g.Add<IIndexedBitmap>(() => new IndexedBitmapHandler());
			g.Add<IMatrixHandler>(() => new MatrixHandler());
			g.Add<IPen>(() => new PenHandler());
			g.Add<ISolidBrush>(() => new SolidBrushHandler());
			g.Add<ITextureBrush>(() => new TextureBrushHandler());
			g.Add<ILinearGradientBrush>(() => new LinearGradientBrushHandler());

			// Forms.Cells
			g.Add<IDrawable>(() => new DrawableHandler());

			// Forms.Printing
			g.Add<IPrintDocument>(() => new PrintDocumentHandler());
		}
    }
}
