using System;
using Eto.Drawing;
using Eto.Forms;
using sd = System.Drawing;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Forms.ToolBar
{
	public class ToolBarViewHandler : WindowsControl<swf.ToolStrip, ToolBarView, ToolBarView.ICallback>, ToolBarView.IHandler
	{
		Control content;
		DockPosition dock = DockPosition.None;
		static readonly object minimumSizeKey = new object();

		public ToolBarViewHandler()
		{
			this.Control = new swf.ToolStrip
			{
				Font = sd.SystemFonts.DefaultFont,
				Dock = swf.DockStyle.Fill,
				ForeColor = sd.SystemColors.ControlText,
				AutoSize = true
			};
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		public Control Content
		{
			get { return content; }
			set
			{
				if (Widget.Loaded)
					SuspendLayout();

				if (content != null)
				{
					var contentHandler = content.GetWindowsHandler();
					contentHandler.SetScale(false, false);
					contentHandler.ContainerControl.Parent = null;
				}

				content = value;

				if (content != null)
				{
					SetContentScale(XScale, YScale);
					var contentHandler = content.GetWindowsHandler();
					contentHandler.BeforeAddControl(Widget.Loaded);
					SetContent(contentHandler.ContainerControl);
				}

				if (Widget.Loaded)
					ResumeLayout();
			}
		}

		public DockPosition Dock
		{
			get { return dock; }
			set { dock = value; }
		}

		public Size MinimumSize
		{
			get { return Widget.Properties.Get<Size?>(minimumSizeKey) ?? Size.Empty; }
			set
			{
				if (value != MinimumSize)
				{
					Widget.Properties[minimumSizeKey] = value;
					SetMinimumSize(useCache: true);
				}
			}
		}

		public virtual Padding Padding
		{
			get { return this.Control.Padding.ToEto(); }
			set { this.Control.Padding = value.ToSWF(); }
		}

		protected virtual void SetContent(swf.Control contentControl)
		{
			contentControl.Dock = swf.DockStyle.Fill;
			this.Control = contentControl as swf.ToolStrip;
		}
		
		protected virtual void SetContentScale(bool xscale, bool yscale)
		{
			if (content != null)
				content.SetScale(xscale, yscale);
		}
		
		public bool RecurseToChildren
		{
			get { return true; }
		}
	}
}

