using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using Eto.IO;
using Eto.Platform.Direct2D.Drawing;
using Eto.Platform.Direct2D.Forms.Controls;
using Eto.Platform.Direct2D.Forms.Printing;

namespace Eto.Platform.Direct2D
{
    public class Generator : Eto.Platform.Windows.Generator
    {
        public override string ID
        {
            get { return Generators.Direct2D; }
        }

        public Generator()
        {
			// generator to use for scenarios where direct 2d doesn't work (e.g. printing)
			BaseGenerator = new Eto.Platform.Windows.Generator();

			Eto.Platform.Windows.Generator.AddTo(this);

            // This is added after the base class's assembly
			AddTo(this);
        }

		public Eto.Generator BaseGenerator
		{
			get;
			set;
		}

		public static new void AddTo(Eto.Generator g)
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
