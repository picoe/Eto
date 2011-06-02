using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class GroupBoxHandler : WindowsContainer<SWF.GroupBox, GroupBox>, IGroupBox
	{
		SWF.Control container;

		public GroupBoxHandler()
		{
			Control = new SWF.GroupBox();
			container = new SWF.Control();
			//container.DockPadding.Top = 16;
			//container.DockPadding.Left = 8;
			container.Dock = SWF.DockStyle.Fill;
			Control.Controls.Add(container);
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
