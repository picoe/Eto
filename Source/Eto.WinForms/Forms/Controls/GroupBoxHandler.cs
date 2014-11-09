using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class GroupBoxHandler : WindowsPanel<swf.GroupBox, GroupBox, GroupBox.ICallback>, GroupBox.IHandler
	{
		readonly swf.Panel content;

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
				ForeColor = sd.SystemColors.ControlText,
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink
			};
			Control.Controls.Add(content);
		}


		protected override Size ContentPadding
		{
			get { return Size.Max(Size.Empty, Control.Size.ToEto() - Control.DisplayRectangle.Size.ToEto()); }
		}

		public override swf.Control ContainerContentControl
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
