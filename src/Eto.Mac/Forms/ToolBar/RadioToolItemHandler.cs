using System;
using Eto.Forms;
using System.Linq;

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

		public override bool Selectable => true;

		public override void ControlAdded (ToolBarHandler toolbar)
		{
			base.ControlAdded (toolbar);
			toolbarHandler = toolbar;
			if (isChecked)
				toolbar.Control.SelectedItemIdentifier = Identifier;
		}
		
		public override void InvokeButton()
		{
			var wasChecked = isChecked;
			if (toolbarHandler != null && toolbarHandler.Control != null)
			{
				isChecked = toolbarHandler.Control.SelectedItemIdentifier == Identifier;
				foreach (var radioHandler in toolbarHandler.Widget.Items.Select(r => r.Handler).OfType<RadioToolItemHandler>())
				{
					if (!ReferenceEquals(this, radioHandler) && radioHandler.isChecked)
					{
						radioHandler.isChecked = false;
						radioHandler.Widget.OnCheckedChanged(EventArgs.Empty);
					}
				}
			}
			Widget.OnClick(EventArgs.Empty);
			if (wasChecked != isChecked)
				Widget.OnCheckedChanged(EventArgs.Empty);
		}
	}
}
