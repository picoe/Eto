using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows
{
	public abstract class WindowsDockContainer<T, W> : WindowsContainer<T, W>, IDockContainer
		where T : swf.Control
		where W : DockContainer
	{
		Control content;

		protected virtual bool UseContentScale { get { return true; } }

		protected virtual bool UseContentDesiredSize { get { return true; } }

		public WindowsDockContainer()
		{
		}

		protected override void Initialize()
		{
			base.Initialize();
			Padding = DockContainer.DefaultPadding;
		}

		public virtual swf.Control ContainerContentControl
		{
			get { return Control; }
		}

		public override Size ParentMinimumSize
		{
			get
			{
				return base.ParentMinimumSize;
			}
			set
			{
				var control = content.GetWindowsHandler();
				if (control != null)
				{
					control.ParentMinimumSize = value;
				}
				base.ParentMinimumSize = value;
			}
		}

		public override Size DesiredSize
		{
			get
			{
				if (UseContentDesiredSize)
				{
					var handler = content.GetWindowsHandler();
					if (handler != null)
					{
						return Size.Max(base.DesiredSize, handler.DesiredSize);
					}
				}
				return base.DesiredSize;
			}
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			if (UseContentScale && content != null)
				content.SetScale(xscale, yscale);
		}

		public Padding Padding
		{
			get { return ContainerContentControl.Padding.ToEto(); }
			set { ContainerContentControl.Padding = value.ToSWF(); }
		}

		public Control Content
		{
			get { return content; }
			set
			{
				Control.SuspendLayout();

				if (content != null)
				{
					if (UseContentScale)
						content.SetScale(false, false);
					var childControl = this.content.GetContainerControl();
					ContainerContentControl.Controls.Remove(childControl);
				}

				content = value;
				if (content != null)
				{
					if (UseContentScale)
						content.SetScale(XScale, YScale);
					var contentControl = content.GetContainerControl();
					contentControl.Dock = swf.DockStyle.Fill;
					ContainerContentControl.Controls.Add(contentControl);
				}

				Control.ResumeLayout();
			}
		}
	}
}
