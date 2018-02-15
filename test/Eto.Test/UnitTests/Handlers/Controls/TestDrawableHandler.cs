using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Test.UnitTests.Handlers.Controls
{
	public class TestDrawableHandler : TestPanelHandler, Drawable.IHandler
	{
		new Drawable.ICallback Callback { get { return (Drawable.ICallback)base.Callback; } }
		new Drawable Widget { get { return (Drawable)base.Widget; } }

		public bool SupportsCreateGraphics
		{
			get { throw new NotImplementedException(); }
		}

		public void Create()
		{
		}

		public void Create(bool largeCanvas)
		{
		}

		public override void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			var graphics = new Graphics(new Drawing.TestGraphicsHandler(Widget));
			var e = new PaintEventArgs(graphics, rect);
			Callback.OnPaint(Widget, e);
		}

		public void Update(Rectangle region)
		{
			Invalidate(region, false);
		}

		public bool CanFocus
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Graphics CreateGraphics()
		{
			throw new NotImplementedException();
		}
	}
}
