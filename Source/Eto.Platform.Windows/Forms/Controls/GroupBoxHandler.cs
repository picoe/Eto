using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class GroupBoxHandler : WindowsContainer<SWF.GroupBox, GroupBox>, IGroupBox
	{
		SWF.Panel content;

		public GroupBoxHandler()
		{
			Control = new SWF.GroupBox();
			Control.SuspendLayout ();
			content = new SWF.Panel();
			content.Font = SD.SystemFonts.DefaultFont;
			//container.DockPadding.Top = 16;
			//container.DockPadding.Left = 8;
			Control.AutoSize = true;
			Control.AutoSizeMode = SWF.AutoSizeMode.GrowAndShrink;
			content.Dock = SWF.DockStyle.Fill;
			content.AutoSize = true;
			content.AutoSizeMode = SWF.AutoSizeMode.GrowAndShrink;
			Control.Controls.Add(content);
		}

		public override Eto.Drawing.Size DesiredSize
		{
			get
			{
				return base.DesiredSize + Control.Size.ToEto () - Control.ClientSize.ToEto ();
			}
		}

		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			Control.ResumeLayout ();
		}

		public override SWF.Control ContentContainer
		{
			get { return content; }
		}


		public override string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}
	}
}
