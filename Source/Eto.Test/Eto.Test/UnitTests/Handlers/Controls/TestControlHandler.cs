using System;
using Eto.Forms;

namespace Eto.Test.UnitTests.Handlers.Controls
{

	public class TestControlHandler : TestWidgetHandler, Control.IHandler
	{
		public void Invalidate()
		{
			throw new NotImplementedException();
		}

		public void Invalidate(Eto.Drawing.Rectangle rect)
		{
			throw new NotImplementedException();
		}

		public void SuspendLayout()
		{
			throw new NotImplementedException();
		}

		public void ResumeLayout()
		{
			throw new NotImplementedException();
		}

		public void Focus()
		{
			throw new NotImplementedException();
		}

		public void OnPreLoad(EventArgs e)
		{
		}

		public void OnLoad(EventArgs e)
		{
		}

		public void OnLoadComplete(EventArgs e)
		{
		}

		public void OnUnLoad(EventArgs e)
		{
		}

		public void SetParent(Container parent)
		{
		}

		public void MapPlatformCommand(string systemCommand, Command command)
		{
			throw new NotImplementedException();
		}

		public Eto.Drawing.PointF PointFromScreen(Eto.Drawing.PointF point)
		{
			throw new NotImplementedException();
		}

		public Eto.Drawing.PointF PointToScreen(Eto.Drawing.PointF point)
		{
			throw new NotImplementedException();
		}

		public Eto.Drawing.Color BackgroundColor
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

		public Eto.Drawing.Size Size
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

		public bool Enabled
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

		public bool HasFocus
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool Visible
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

		public System.Collections.Generic.IEnumerable<string> SupportedPlatformCommands
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Eto.Drawing.Point Location
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string ToolTip
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

		public Cursor Cursor
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
	}
}

