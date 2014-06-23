using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Drawing;
using System.ComponentModel;

namespace Eto.Test.UnitTests.Handlers
{
	interface IControlHandler
	{
		void OnShown();
		Size GetPreferredSize();
		void SetBounds(Rectangle rect);
	}

	public abstract class TestContainerHandler : Controls.TestControlHandler, Eto.Forms.Container.IHandler
	{
		public virtual Size ClientSize
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public bool RecurseToChildren
		{
			get { return true; }
		}
	}

	public class TestPanelHandler : TestContainerHandler, Panel.IHandler
	{
		Control content;
		public Control Content
		{
			get { return content; }
			set
			{
				content = value;
				SetContentSize();
			}
		}

		public override void OnShown()
		{
			if (Content != null)
			{
				var handler = Content.Handler as IControlHandler;
				if (handler != null)
					handler.OnShown();
			}
			base.OnShown();
		}

		public override Size ClientSize
		{
			get { return Size; }
			set { Size = value; }
		}

		void SetContentSize()
		{
			if (Content != null)
			{
				var handler = Content.Handler as IControlHandler;
				if (handler != null)
				{
					if (AutoSize)
						ClientSize = handler.GetPreferredSize();
					else
						handler.SetBounds(new Rectangle(ClientSize));
				}
			}
		}

		public override Size Size
		{
			get { return base.Size; }
			set
			{
				base.Size = value;
				SetContentSize();
			}
		}

		public Padding Padding
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public Size MinimumSize
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public ContextMenu ContextMenu
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}

	public class TestWindowHandler : TestPanelHandler, Window.IHandler
	{
		new Window.ICallback Callback { get { return (Window.ICallback)base.Callback; } }
		new Window Widget { get { return (Window)base.Widget; } }

		public ToolBar ToolBar
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public void Close()
		{
			var e = new CancelEventArgs();
			Callback.OnClosing(Widget, e);
			if (!e.Cancel)
				Callback.OnClosed(Widget, e);
		}

		public new Point Location
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public double Opacity
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public string Title
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public Screen Screen
		{
			get { throw new NotImplementedException(); }
		}

		public MenuBar Menu
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public Icon Icon
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public bool Resizable
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public bool Maximizable
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public bool Minimizable
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public bool ShowInTaskbar
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public bool Topmost
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public WindowState WindowState
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public Rectangle? RestoreBounds
		{
			get { throw new NotImplementedException(); }
		}

		public WindowStyle WindowStyle
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public void BringToFront()
		{
			throw new NotImplementedException();
		}

		public void SendToBack()
		{
			throw new NotImplementedException();
		}
	}

	public class TestFormHandler : TestWindowHandler, Form.IHandler
	{
		new Form.ICallback Callback { get { return (Form.ICallback)base.Callback; } }
		new Form Widget { get { return (Form)base.Widget; } }

		public void Show()
		{
			Callback.OnShown(Widget, EventArgs.Empty);
			OnShown();
		}
	}
}
