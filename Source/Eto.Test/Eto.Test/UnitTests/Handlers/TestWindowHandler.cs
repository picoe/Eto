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

		public Rectangle RestoreBounds
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
}
