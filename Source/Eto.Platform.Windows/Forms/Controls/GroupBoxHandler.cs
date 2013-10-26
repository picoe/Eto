using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows
{
	public class GroupBoxHandler : WindowsDockContainer<swf.GroupBox, GroupBox>, IGroupBox
	{
		swf.Panel content;

		public GroupBoxHandler()
		{
			Control = new swf.GroupBox
			{
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink
			};
			content = new swf.Panel
			{
				Font = sd.SystemFonts.DefaultFont,
				Dock = swf.DockStyle.Fill,
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink
			};
			Control.Controls.Add(content);
		}

		public override swf.Control ContainerContentControl
		{
			get { return content; }
		}

		public override Size DesiredSize
		{
			get { return Size.Max(Size.Empty, base.DesiredSize + Control.Size.ToEto() - Control.DisplayRectangle.Size.ToEto()); }
		}

		public override string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}
	}
}
