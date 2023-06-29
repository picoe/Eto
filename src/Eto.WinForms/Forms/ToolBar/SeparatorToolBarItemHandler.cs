namespace Eto.WinForms.Forms.ToolBar
{
	public class SeparatorToolBarItemHandler : ToolItemHandler<swf.ToolStripSeparator, SeparatorToolItem>, SeparatorToolItem.IHandler, IToolBarItemHandler
	{
		public SeparatorToolBarItemHandler()
		{
			Control = new swf.ToolStripSeparator();
		}

		public override void CreateControl(ToolBarHandler handler, int index)
		{
			handler.Control.Items.Insert(index, Control);
		}

		public SeparatorToolItemType Type
		{
			get
			{
				return Control.AutoSize ? SeparatorToolItemType.Divider : SeparatorToolItemType.FlexibleSpace;
			}
			set
			{
				switch (value)
				{
					case SeparatorToolItemType.Divider:
						Control.AutoSize = true;
						break;
					default:
						Control.AutoSize = false;
						break;
				}
			}
		}

		public override bool Enabled
		{
			get { return false; }
			set { throw new NotSupportedException(); }
		}
	}
}
