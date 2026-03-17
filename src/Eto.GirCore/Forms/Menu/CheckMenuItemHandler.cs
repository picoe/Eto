namespace Eto.GirCore.Forms.Menu
{
	public class CheckMenuItemHandler : MenuItemHandler<Gtk.CheckButton, CheckMenuItem, CheckMenuItem.ICallback>, CheckMenuItem.IHandler
	{
		int suppressClick;

		public CheckMenuItemHandler()
		{
			Control = Gtk.CheckButton.New();
			Control.UseUnderline = true;
			Control.OnToggled += (sender, e) =>
			{
				Callback.OnCheckedChanged(Widget, EventArgs.Empty);
				if (suppressClick == 0)
				{
					ParentMenu?.PrepareForChildActivation();
					Callback.OnClick(Widget, EventArgs.Empty);
					ParentMenu?.CloseHierarchy();
				}
			};
		}

		protected override void UpdateDisplay()
		{
			Control.Label = GirMenuHelper.ToMnemonic(Text);
			Control.UseUnderline = true;
		}

		public bool Checked
		{
			get => Control.Active;
			set
			{
				suppressClick++;
				Control.Active = value;
				suppressClick--;
			}
		}
	}
}
