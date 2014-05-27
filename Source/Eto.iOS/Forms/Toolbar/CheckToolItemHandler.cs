using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using sd = System.Drawing;

namespace Eto.iOS.Forms.Toolbar
{

	public class CheckToolItemHandler : ToolItemHandler<UIBarButtonItem, CheckToolItem>, CheckToolItem.IHandler
	{
		bool isChecked;
		ToolBarHandler toolbarHandler;

		public bool Checked
		{
			get { return isChecked; }
			set
			{
				isChecked = value;
#if TODO				
				if (isChecked && Control != null && toolbarHandler != null && toolbarHandler.Control != null)
					toolbarHandler.Control.SelectedItemIdentifier = Identifier;
#endif
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			Selectable = true;
		}

		public override void ControlAdded(ToolBarHandler toolbar)
		{
			base.ControlAdded(toolbar);
			toolbarHandler = toolbar;
#if TODO
			if (isChecked)
				toolbar.Control.SelectedItemIdentifier = Identifier;
#endif
		}

		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}
	}
	
}
