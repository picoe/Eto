namespace Eto.GirCore.Forms.Menu
{
	public class SeparatorMenuItemHandler : MenuItemHandler<Gtk.Separator, SeparatorMenuItem, SeparatorMenuItem.ICallback>, SeparatorMenuItem.IHandler
	{
		public SeparatorMenuItemHandler()
		{
			Control = Gtk.Separator.New(Gtk.Orientation.Horizontal);
			Control.Visible = true;
		}

		protected override void UpdateDisplay()
		{
		}

		public override string Text
		{
			get => string.Empty;
			set => throw new NotSupportedException();
		}

		public override string ToolTip
		{
			get => string.Empty;
			set => throw new NotSupportedException();
		}

		public override Keys Shortcut
		{
			get => Keys.None;
			set => throw new NotSupportedException();
		}

		public override bool Enabled
		{
			get => false;
			set { }
		}
	}
}
