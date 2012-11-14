using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows
{
	public class GroupBoxHandler : WindowsContainer<SWF.GroupBox, GroupBox>, IGroupBox
	{
		SWF.Panel content;

		public GroupBoxHandler()
		{
			Control = new SWF.GroupBox {
				AutoSize = true,
				AutoSizeMode = SWF.AutoSizeMode.GrowAndShrink
			};
			Control.SuspendLayout ();
			content = new SWF.Panel {
				Font = SD.SystemFonts.DefaultFont,
				Dock = SWF.DockStyle.Fill,
				AutoSize = true,
				AutoSizeMode = SWF.AutoSizeMode.GrowAndShrink
			};
			Control.Controls.Add(content);
		}

		public override Size DesiredSize
		{
			get { return Size.Max(Size.Empty, base.DesiredSize + Control.Size.ToEto () - Control.DisplayRectangle.Size.ToEto ()); }
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
