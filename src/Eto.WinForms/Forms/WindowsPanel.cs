using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms.Forms
{
	public abstract class WindowsPanel<TControl, TWidget, TCallback> : WindowsContainer<TControl, TWidget, TCallback>, Panel.IHandler
		where TControl : swf.Control
		where TWidget : Panel
		where TCallback : Panel.ICallback
	{
		Control content;

		public virtual swf.Control ContainerContentControl
		{
			get { return Control; }
		}

		protected virtual Size ContentPadding { get { return Padding.Size; } }

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
					control.ParentMinimumSize = value - ContentPadding;
				}
				base.ParentMinimumSize = value;
			}
		}

		public override Size GetPreferredSize(Size availableSize, bool useCache)
		{
            var userSize = UserPreferredSize;
			var desiredSize = base.GetPreferredSize(availableSize, useCache);

			var handler = content.GetWindowsHandler();
			if (handler != null && (userSize.Width == -1 || userSize.Height == -1))
			{
				var contentPadding = ContentPadding;
				var desiredContentSize = handler.GetPreferredSize(Size.Max(Size.Empty, availableSize - contentPadding)) + contentPadding;
                if (userSize.Width == -1)
				{
					if (desiredSize.Width > 0)
						desiredSize.Width = Math.Max(desiredSize.Width, desiredContentSize.Width);
					else
						desiredSize.Width = desiredContentSize.Width;
				}

                if (userSize.Height == -1)
                {
                    if (desiredSize.Height > 0)
						desiredSize.Height = Math.Max(desiredSize.Height, desiredContentSize.Height);
					else
						desiredSize.Height = desiredContentSize.Height;
				}
			}
			return desiredSize;
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			SetContentScale(xscale || UserPreferredSize.Width >= 0, yscale || UserPreferredSize.Height >= 0);
		}

		protected virtual void SetContentScale(bool xscale, bool yscale)
		{
			if (content != null)
				content.SetScale(xscale, yscale);
		}

		public virtual Padding Padding
		{
			get { return ContainerContentControl.Padding.ToEto(); }
			set { ContainerContentControl.Padding = value.ToSWF(); }
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
					var childHandler = content.GetWindowsHandler();
					childHandler.BeforeAddControl(Widget.Loaded);
					SetContent(value, childHandler.ContainerControl);
				}

				if (Widget.Loaded)
					ResumeLayout();
			}
		}

		protected virtual void SetContent(Control control, swf.Control contentControl)
		{
			contentControl.Dock = swf.DockStyle.Fill;
			ContainerContentControl.Controls.Add(contentControl);
		}
	}
}
