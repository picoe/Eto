using System;

namespace EmbedGtkInEto
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class MyNativeWidget : Gtk.Bin
	{
		public MyNativeWidget()
		{
			this.Build();
		}
	}
}

