using System;
using Eto.Forms;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.ToolBar
{
	public class RadioToolItemHandler : ToolItemHandler<NSToolbarItem, RadioToolItem>, RadioToolItem.IHandler
	{
		bool isChecked;
		ToolBarHandler toolbarHandler;

		protected override MacToolBarItemStyle DefaultStyle
		{
			get { return MacToolBarItemStyle.Default; }
		}

		public bool Checked
		{
			get { return isChecked; }
			set { 
				isChecked = value;
				if (isChecked && Control != null && toolbarHandler != null && toolbarHandler.Control != null)
					toolbarHandler.Control.SelectedItemIdentifier = Identifier;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
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
			if (toolbarHandler != null && toolbarHandler.Control != null)
			{
				isChecked = toolbarHandler.Control.SelectedItemIdentifier == Identifier;
			}
			Widget.OnClick(EventArgs.Empty);
		}
	}
}
