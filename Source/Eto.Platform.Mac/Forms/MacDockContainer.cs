using System;
using Eto.Forms;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using SD = System.Drawing;
using System.Linq;

namespace Eto.Platform.Mac.Forms
{
	public abstract class MacDockContainer<T, W> : MacContainer<T, W>, IDockContainer
		where T: NSResponder
		where W: DockContainer
	{
		Control content;
		Padding padding;

		public Eto.Drawing.Padding Padding
		{
			get { return padding; }
			set
			{
				padding = value;
				LayoutParent();
			}
		}

		public Control Content
		{
			get { return content; }
			set
			{
				if (content != null)
				{ 
					content.GetContainerView().RemoveFromSuperview(); 
				}

				content = value;
				var control = value.GetContainerView();
				if (control != null)
				{
					var container = ContentControl;
					control.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
					control.SetFrameSize(container.Frame.Size);
					container.AddSubview(control);
				}

				if (Widget.Loaded)
				{
					LayoutParent();
				}
			}
		}

		protected virtual bool UseContentSize { get { return true; } }

		public override Size GetPreferredSize(Size availableSize)
		{
			if (UseContentSize)
				return Size.Max(base.GetPreferredSize(availableSize), Widget.Content.GetPreferredSize(availableSize) + Padding.Size);
			else
				return base.GetPreferredSize(availableSize);
		}

		protected override Size GetNaturalSize(Size availableSize)
		{
			if (UseContentSize)
			{
				var content = Widget.Content.GetMacAutoSizing();
				if (content != null)
					return content.GetPreferredSize(availableSize);
			}
			return base.GetNaturalSize(availableSize);
		}

		public override void LayoutChildren()
		{
			base.LayoutChildren();

			if (content == null)
				return;

			NSView parent = this.ContentControl;

			NSView childControl = content.GetContainerView();
			var frame = parent.Frame;

			if (frame.Width > padding.Horizontal && frame.Height > padding.Vertical)
			{
				frame.X = padding.Left;
				frame.Width -= padding.Horizontal;
				frame.Y = padding.Bottom;
				frame.Height -= padding.Vertical;
			}
			else
			{
				frame.X = 0;
				frame.Y = 0;
			}

			if (childControl.Frame != frame)
				childControl.Frame = frame;
		}

		public override void SetContentSize(SD.SizeF contentSize)
		{
			base.SetContentSize(contentSize);
			if (MinimumSize != Size.Empty)
			{
				contentSize.Width = Math.Max(contentSize.Width, MinimumSize.Width);
				contentSize.Height = Math.Max(contentSize.Height, MinimumSize.Height);
			}
			if (Widget.Content != null)
			{
				var child = Widget.Content.Handler as IMacContainer;
				if (child != null)
				{
					child.SetContentSize(contentSize);
				}
			}
		}

		bool sizeChangedAdded;

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);

			if (!sizeChangedAdded)
			{
				Widget.SizeChanged += (sender, ev) => {
					this.LayoutChildren();
				};
				sizeChangedAdded = true;
			}
		}
	}
}

