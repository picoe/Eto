using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.UnitTests.Handlers
{
	public class Generator : Eto.Generator
	{
		public override bool IsDesktop { get { return true; } }

		public override bool IsMobile { get { return false; } }

		public Generator()
		{
			// Add the handlers in this assembly
			AddTo(this);
		}

		public static void AddTo(Eto.Generator g)
		{
			// Drawing
			g.Add<IBitmap>(() => new TestBitmapHandler());
			g.Add<IFont>(() => new TestFontHandler()); 
			g.Add<IGraphics>(() => new TestGraphicsHandler()); 
			g.Add<IMatrixHandler>(() => new TestMatrixHandler());

			g.Add<IGridView>(() => new TestGridViewHandler());
		}

		public override string ID
		{
			get { return "eto.test"; }
		}
	}
}
