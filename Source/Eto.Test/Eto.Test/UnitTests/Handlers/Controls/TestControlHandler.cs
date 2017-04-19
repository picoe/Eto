using System;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Test.UnitTests.Handlers.Controls
{
	interface IControlHandler
	{
		void OnShown();
		Size GetPreferredSize();
		void SetBounds(Rectangle rect);
	}

	public class TestControlHandler : TestWidgetHandler, Control.IHandler, IControlHandler
	{
		new Control.ICallback Callback { get { return (Control.ICallback)base.Callback; } }
		new Control Widget { get { return (Control)base.Widget; } }

		protected bool AutoSize { get; set; }

		public TestControlHandler()
		{
			AutoSize = true;
		}

		public virtual void Invalidate(bool invalidateChildren)
		{
			Invalidate(new Rectangle(Point.Empty, Size), invalidateChildren);
		}

		public virtual void Invalidate(Rectangle rect, bool invalidateChildren)
		{
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

		public virtual void OnPreLoad(EventArgs e)
		{
		}

		public virtual void OnLoad(EventArgs e)
		{
		}

		public virtual void OnLoadComplete(EventArgs e)
		{
		}

		public virtual void OnUnLoad(EventArgs e)
		{
		}

		public void SetParent(Container parent)
		{
		}

		public void MapPlatformCommand(string systemCommand, Command command)
		{
			throw new NotImplementedException();
		}

		public PointF PointFromScreen(PointF point)
		{
			throw new NotImplementedException();
		}

		public PointF PointToScreen(PointF point)
		{
			throw new NotImplementedException();
		}

		public Color BackgroundColor
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

		Size? size;
		Size desiredSize;
		public virtual Size Size
		{
			get { return size ?? desiredSize; }
			set
			{
				if (Widget.Loaded)
					size = value;
				desiredSize = value;
				AutoSize = value.Width == -1 && value.Height == -1;
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

		public Point Location { get; set; }

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

		public int TabIndex
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

		public IEnumerable<Control> VisualControls
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual void OnShown()
		{
			Invalidate(false);
		}

		public virtual Size GetPreferredSize()
		{
			return desiredSize;
		}

		public virtual void SetBounds(Rectangle rect)
		{
			size = rect.Size;
			Location = rect.Location;
		}
	}
}

