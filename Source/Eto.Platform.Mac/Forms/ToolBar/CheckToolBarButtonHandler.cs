using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{

	public class CheckToolBarButtonHandler : ToolBarItemHandler<NSToolbarItem, CheckToolBarButton>, ICheckToolBarButton
	{
		bool isChecked;
		ToolBarHandler toolbarHandler;

		public bool Checked
		{
			get { return isChecked; }
			set { 
				isChecked = value;
				if (isChecked && Control != null && toolbarHandler != null && toolbarHandler.Control != null) 
					toolbarHandler.Control.SelectedItemIdentifier = this.Identifier;
			}
		}

		public override void Initialize ()
		{
			base.Initialize ();
			Selectable = true;
		}

		public override void ControlAdded (ToolBarHandler toolbar)
		{
			base.ControlAdded (toolbar);
			this.toolbarHandler = toolbar;
			if (isChecked) toolbar.Control.SelectedItemIdentifier = this.Identifier;
		}
		
		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}
	}
}
