using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.UnitTests.Handlers
{
	public class TestPlatform : Eto.Platform
	{
		public override bool IsDesktop { get { return true; } }

		public override bool IsMobile { get { return false; } }

		public TestPlatform()
		{
			// Add the handlers in this assembly
			AddTo(this);
		}

		public static void AddTo(Eto.Platform p)
		{
			// Drawing
			p.Add<IBitmap>(() => new TestBitmapHandler());
			p.Add<IFont>(() => new TestFontHandler()); 
			p.Add<IGraphics>(() => new TestGraphicsHandler()); 
			p.Add<IMatrixHandler>(() => new TestMatrixHandler());

			p.Add<IGridView>(() => new TestGridViewHandler());
		}

		public override string ID
		{
			get { return "eto.test"; }
		}
	}
}
