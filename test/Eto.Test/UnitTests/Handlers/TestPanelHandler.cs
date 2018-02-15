using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Drawing;
using System.ComponentModel;
using Eto.Test.UnitTests.Handlers.Controls;

namespace Eto.Test.UnitTests.Handlers
{
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
}
