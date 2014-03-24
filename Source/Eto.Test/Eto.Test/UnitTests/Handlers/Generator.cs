using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests.Handlers;

namespace Eto.Test.Handlers
{
	public class Generator : Eto.Generator
	{
		public Generator()
		{
			// Add the handlers in this assembly
			AddTo(this);
		}

		public static void AddTo(Eto.Generator g)
		{
			g.Add<IGridView>(() => new TestGridViewHandler());
			g.Add<IMatrixHandler>(() => new TestMatrixHandler());
		}

		public override string ID
		{
			get { return "eto.test"; }
		}
	}
}
