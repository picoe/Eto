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
	public class TestFormHandler : TestWindowHandler, Form.IHandler
	{
		new Form.ICallback Callback { get { return (Form.ICallback)base.Callback; } }
		new Form Widget { get { return (Form)base.Widget; } }

		public void Show()
		{
			Callback.OnShown(Widget, EventArgs.Empty);
			OnShown();
		}

		public bool ShowActivated { get; set; }

		public bool CanFocus { get; set; }
	}
}
