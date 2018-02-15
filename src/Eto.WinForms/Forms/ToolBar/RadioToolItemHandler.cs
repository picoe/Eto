using System;
using System.Linq;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms.ToolBar
{
	public class RadioToolItemHandler : ToolItemHandler<SWF.ToolStripButton, RadioToolItem>, RadioToolItem.IHandler
	{
		public RadioToolItemHandler()
		{
			Control = new SWF.ToolStripButton();
			Control.Tag = this;
			Control.Click += control_Click;
		}

		void control_Click(object sender, EventArgs e)
		{
			var parent = Control.GetCurrentParent();
			if (parent != null)
			{
				foreach (var button in parent.Items.OfType<SWF.ToolStripButton>().Select(r => r.Tag).OfType<RadioToolItemHandler>().Where(r => r != this))
				{
					button.Checked = false;
				}
			}
			Control.Checked = true;
			Widget.OnClick(EventArgs.Empty);
		}

		public bool Checked
		{
			get { return Control.Checked; }
			set { Control.Checked = value; }
		}

		public override void CreateControl(ToolBarHandler handler, int index)
		{
			handler.Control.Items.Insert(index, Control);
		}

		public override bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
	}
}
