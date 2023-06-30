namespace Eto.WinForms.Forms.ToolBar
{
	public class DropDownToolItemHandler : ToolItemHandler<swf.ToolStripDropDownButton, DropDownToolItem>, DropDownToolItem.IHandler
	{
		bool openedHandled;

		public DropDownToolItemHandler()
		{
			Control = new swf.ToolStripDropDownButton();
			Control.Tag = this;
			Control.Click += control_Click;
		}
		
		void HandleDropDownOpened(object sender, EventArgs e)
		{
			foreach (var item in Widget.Items)
			{
				var callback = ((ICallbackSource)item).Callback as MenuItem.ICallback;
				if (callback != null)
					callback.OnValidate(item, e);
			}
		}
		
		void control_Click(object sender, EventArgs e)
		{
			Widget.OnClick(EventArgs.Empty);
		}

		public override bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		/// <summary>
		/// Gets or sets whether the drop arrow is shown on the button.
		/// </summary>
		public bool ShowDropArrow
		{
			get { return Control.ShowDropDownArrow; }
			set { Control.ShowDropDownArrow = value; }
		}

		public override void CreateControl(ToolBarHandler handler, int index)
		{
			handler.Control.Items.Insert(index, Control);
		}
		
		public void AddMenu(int index, MenuItem item)
		{
			Control.DropDownItems.Insert(index, (swf.ToolStripItem)item.ControlObject);
			if (!openedHandled)
			{
				Control.DropDownOpening += HandleDropDownOpened;
				openedHandled = true;
			}
		}

		public void RemoveMenu(MenuItem item)
		{
			Control.DropDownItems.Remove((swf.ToolStripItem)item.ControlObject);
		}

		public void Clear()
		{
			Control.DropDownItems.Clear();
		}
	}
}
