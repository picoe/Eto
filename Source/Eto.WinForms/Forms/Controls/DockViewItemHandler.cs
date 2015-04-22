using System;
using System.Collections.ObjectModel;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using sd = System.Drawing;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class DockViewItemHandler : WindowsControl<swf.Control, DockViewItem, DockViewItem.ICallback>, DockViewItem.IHandler
	{
		public DockViewItemHandler()
		{
			this.Control = new swf.Control
			{
				Visible = false
			};
		}

		protected override void Initialize()
		{
			base.Initialize();
		}
	}
}
