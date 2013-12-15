using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{

	public class CheckToolBarButtonHandler : ToolBarItemHandler<NSToolbarItem, CheckToolItem>, ICheckToolItem
	{
		bool isChecked;
		ToolBarHandler toolbarHandler;

		public bool Checked
		{
			get { return isChecked; }
			set { 
				isChecked = value;
				if (isChecked && Control != null && toolbarHandler != null && toolbarHandler.Control != null)
					toolbarHandler.Control.SelectedItemIdentifier = Identifier;
			}
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			Selectable = true;
		}

		public override void ControlAdded (ToolBarHandler toolbar)
		{
			base.ControlAdded (toolbar);
			toolbarHandler = toolbar;
			if (isChecked)
				toolbar.Control.SelectedItemIdentifier = Identifier;
		}
		
		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
		}
	}
}
