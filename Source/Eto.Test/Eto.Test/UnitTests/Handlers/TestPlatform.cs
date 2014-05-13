using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests.Handlers.Controls;
using Eto.Test.UnitTests.Handlers.Layout;

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
			p.Add<Bitmap.IHandler>(() => new TestBitmapHandler());
			p.Add<Font.IHandler>(() => new TestFontHandler()); 
			p.Add<Graphics.IHandler>(() => new TestGraphicsHandler()); 
			p.Add<Matrix.IHandler>(() => new TestMatrixHandler());

			p.Add<GridView.IHandler>(() => new TestGridViewHandler());
			p.Add<Label.IHandler>(() => new TestLabelHandler());
			p.Add<TextBox.IHandler>(() => new TestTextBoxHandler());

			p.Add<TableLayout.IHandler>(() => new TestTableLayoutHandler());
		}

		public override string ID
		{
			get { return "eto.test"; }
		}
	}
}
