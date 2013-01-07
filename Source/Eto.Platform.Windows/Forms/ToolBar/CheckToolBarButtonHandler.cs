using System;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{

	public class CheckToolBarButtonHandler : ToolBarItemHandler<SWF.ToolStripButton, CheckToolBarButton>, ICheckToolBarButton
	{
		SWF.ToolStripButton control;

		public CheckToolBarButtonHandler()
		{
			control = new SWF.ToolStripButton();
			control.Tag = this;
			control.Click += control_Click;
			Control = control;
		}

		void control_Click(object sender, EventArgs e)
		{
			((CheckToolBarButton)Widget).OnClick(EventArgs.Empty);
		}

		#region ICheckToolBarButton Members

		public bool Checked
		{
			get { return control.Checked; }
			set { control.Checked = value; }
		}

		#endregion

		public override void CreateControl(ToolBarHandler handler)
		{
			handler.Control.Items.Add(control);
		}


		public override void InvokeButton()
		{
			((CheckToolBarButton)Widget).OnClick(EventArgs.Empty);
		}
		
		public override bool Enabled
		{
			get { return control.Enabled; }
			set { control.Enabled = value; }
		}
		

	}


}
