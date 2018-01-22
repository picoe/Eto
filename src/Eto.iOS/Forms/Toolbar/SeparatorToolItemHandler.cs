using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using ObjCRuntime;
using UIKit;
using sd = System.Drawing;

namespace Eto.iOS.Forms.Toolbar
{
	public class SeparatorToolItemHandler : WidgetHandler<UIBarButtonItem, SeparatorToolItem>, SeparatorToolItem.IHandler, IToolBarBaseItemHandler
	{
		public SeparatorToolItemHandler()
		{
			Type = SeparatorToolItemType.Divider;
			Control = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace);
			Control.Width = 10;
		}

		public bool Selectable
		{
			get { return false; }
		}

		public SeparatorToolItemType Type { get; set; }

		public void ControlAdded(ToolBarHandler toolbar)
		{
		}

		public void CreateFromCommand(Command command)
		{
		}

		public string Text
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public string ToolTip
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public Eto.Drawing.Image Image
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public bool Enabled
		{
			get { return false; }
			set { throw new NotSupportedException(); }
		}

		public void OnLoad(EventArgs e)
		{
		}

		public void OnPreLoad(EventArgs e)
		{
		}

		public void OnUnLoad(EventArgs e)
		{
		}
	}
}
