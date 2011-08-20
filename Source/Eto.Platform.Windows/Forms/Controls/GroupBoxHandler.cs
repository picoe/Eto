using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class GroupBoxHandler : WindowsContainer<SWF.GroupBox, GroupBox>, IGroupBox
	{
		SWF.Panel container;

		public GroupBoxHandler()
		{
			Control = new SWF.GroupBox();
			Control.SuspendLayout ();
			container = new SWF.Panel();
			//container.DockPadding.Top = 16;
			//container.DockPadding.Left = 8;
			Control.AutoSize = true;
			Control.AutoSizeMode = SWF.AutoSizeMode.GrowAndShrink;
			container.Dock = SWF.DockStyle.Fill;
			container.AutoSize = true;
			container.AutoSizeMode = SWF.AutoSizeMode.GrowAndShrink;
			Control.Controls.Add(container);
		}

		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			Control.ResumeLayout ();
		}

		public override object ContainerObject
		{
			get { return container; }
		}


		public override string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}
	}
}
