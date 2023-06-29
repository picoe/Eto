namespace Eto.WinForms.Forms.Menu
{
	public class SeparatorMenuItemHandler : MenuHandler<swf.ToolStripSeparator, SeparatorMenuItem, SeparatorMenuItem.ICallback>, SeparatorMenuItem.IHandler
	{
		
		public SeparatorMenuItemHandler()
		{
			Control = new swf.ToolStripSeparator();
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

		public Keys Shortcut
		{
			get { return Keys.None; }
			set { throw new NotSupportedException(); }
		}

		public bool Enabled
		{
			get { return false; }
			set { throw new NotSupportedException(); }
		}

		public bool Visible
		{
			get => Control.Visible;
			set => Control.Visible = value;
		}
	}
}
