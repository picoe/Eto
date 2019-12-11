using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class ScrollableHandler : WindowsPanel<ScrollableHandler.CustomScrollable, Scrollable, Scrollable.ICallback>, Scrollable.IHandler
	{
		readonly swf.Panel content;
		bool expandWidth = true;
		bool expandHeight = true;
		bool finalLayoutPass;

		public class CustomScrollable : swf.Panel
		{
			public ScrollableHandler Handler { get; set; }

			protected override bool ProcessDialogKey(swf.Keys keyData)
			{
				var e = new swf.KeyEventArgs(keyData);
				OnKeyDown(e);
                e.Handled |= base.ProcessDialogKey(keyData);
				if (!e.Handled)
				{
					// Prevent firing the keydown event twice for the same key
					Handler.LastKeyDown = e.KeyData.ToEto();
				}
				return e.Handled;
			}

			protected override void OnCreateControl()
			{
				base.OnCreateControl();
				AutoSize = false;
			}

			protected override sd.Point ScrollToControl(swf.Control activeControl)
			{
				return AutoScrollPosition;
			}

			sd.Size lastClientSize;

			protected override void OnLayout(swf.LayoutEventArgs levent)
			{
				UpdateScrollSize();
				base.OnLayout(levent);
			}

			void UpdateScrollSize()
			{
				var clientSize = lastClientSize = ClientSize;

				var contentControl = Handler.Content.GetWindowsHandler();
				if (contentControl != null)
				{
					Handler.content.SuspendLayout();
					Handler.content.MinimumSize = sd.Size.Empty;
					contentControl.ParentMinimumSize = Eto.Drawing.Size.Empty;
					var preferred = contentControl.GetPreferredSize(Eto.Drawing.Size.Empty);
					if (Handler.finalLayoutPass)
					{
						if (Handler.ExpandContentWidth && preferred.Height > lastClientSize.Height)
							clientSize.Width -= swf.SystemInformation.VerticalScrollBarWidth;
						if (Handler.ExpandContentHeight && preferred.Width > lastClientSize.Width)
							clientSize.Height -= swf.SystemInformation.HorizontalScrollBarHeight;
					}
					var minSize = new Size
					{
						Width = Handler.ExpandContentWidth ? Math.Max(0, clientSize.Width) : 0,
						Height = Handler.ExpandContentHeight ? Math.Max(0, clientSize.Height) : 0
					};

					// set minimum size for the content if we want to extend to the size of the scrollable width/height
					contentControl.ParentMinimumSize = Eto.Drawing.Size.Max(Eto.Drawing.Size.Empty, minSize - Handler.Padding.Size);
					Handler.content.ResumeLayout();
				}
			}

            protected override void OnClientSizeChanged(EventArgs e)
			{
				base.OnClientSizeChanged(e);
				// when scrollbar is shown/hidden, need to perform layout
				if (ClientSize != lastClientSize)
					PerformLayout();
			}
		}

    
		public override Padding Padding
		{
			get { return Control.Padding.ToEto(); }
			set { Control.Padding = value.ToSWF(); }
		}

		protected override void ResumeControl(bool performLayout = true)
		{
			finalLayoutPass = true;
			base.ResumeControl(true); // always perform layout for Scrollable
			finalLayoutPass = false;
		}

		public override swf.Control ContainerContentControl
		{
			get { return content; }
		}

		protected override void SetContentScale(bool xscale, bool yscale)
		{
            base.SetContentScale(false, false);
		}

		public override Size GetPreferredSize(Size availableSize, bool useCache)
		{
			var baseSize = UserPreferredSize;
			var size = base.GetPreferredSize(availableSize, useCache);
			size -= Padding.Size;
			// if we have set to a specific size, then try to use that
			if (baseSize.Width >= 0)
				size.Width = baseSize.Width;
			if (baseSize.Height >= 0)
				size.Height = baseSize.Height;
			return size;
		}

		public BorderType Border
		{
			get { return Control.BorderStyle.ToEto(); }
			set
			{
				Control.BorderStyle = value.ToSWF();
                UpdateScrollSizes();
            }
		}

        class EtoContentPanel : swf.Panel
        {
            public override sd.Size GetPreferredSize(sd.Size proposedSize)
            {
                if (HasChildren)
                {
                    return Controls[0].GetPreferredSize(proposedSize);
                }
                return base.GetPreferredSize(proposedSize);
            }

            protected override void OnSizeChanged(EventArgs e)
            {
                if (HasChildren)
                {
                    Controls[0].Size = Size;
                }
            }
        }

		public ScrollableHandler()
		{
			Control = new CustomScrollable
			{
				Handler = this,
				Size = sd.Size.Empty,
				MinimumSize = sd.Size.Empty,
				BorderStyle = swf.BorderStyle.Fixed3D,
				AutoScroll = true,
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink
			};
			Control.VerticalScroll.SmallChange = 5;
			Control.VerticalScroll.LargeChange = 10;
			Control.HorizontalScroll.SmallChange = 5;
			Control.HorizontalScroll.LargeChange = 10;

			content = new EtoContentPanel
			{
				Size = sd.Size.Empty,
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink
			};
			Control.Controls.Add(content);
		}

		public override void ResumeLayout()
		{
			base.ResumeLayout();
			// HACK: Update scroll sizes twice here in case scrollbars are shown/hidden when resumed.
			// e.g. BrushSection
			UpdateScrollSizes();
			UpdateScrollSizes();
		}

		protected override bool SetMinimumSize(Size size)
		{
			var result = base.SetMinimumSize(size);
			// HACK: schedule re-jig at next run loop when min size changes so it updates
			// if scrollbars are shown/hidden during this cycle.
			if (result && Widget.Loaded)
				Application.Instance.AsyncInvoke(UpdateScrollSizes);
			return result;
		}

		protected override void SetContent(Control control, swf.Control contentControl)
		{
			content.Controls.Clear();
			content.Controls.Add(contentControl);
		}

        public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Scrollable.ScrollEvent:
					Control.Scroll += delegate
					{
						Callback.OnScroll(Widget, new ScrollEventArgs(ScrollPosition));
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public void UpdateScrollSizes()
		{
			Control.PerformLayout();
		}

		public Point ScrollPosition
		{
			get { return new Point(-Control.AutoScrollPosition.X, -Control.AutoScrollPosition.Y); }
			set
			{
				Control.AutoScrollPosition = value.ToSD();
			}
		}

		public Size ScrollSize
		{
			get { return Control.DisplayRectangle.Size.ToEto(); }
			set { Control.AutoScrollMinSize = value.ToSD(); }
		}

		public Rectangle VisibleRect
		{
			get { return new Rectangle(ScrollPosition, Size.Min(ScrollSize, ClientSize)); }
		}

		public override Size ClientSize
		{
			get { return Control.ClientSize.ToEto(); }
			set
			{
				Control.AutoSize = value.Width == -1 || value.Height == -1;
				Control.ClientSize = value.ToSD();
			}
		}

		public bool ExpandContentWidth
		{
			get { return expandWidth; }
			set
			{
				if (expandWidth != value)
				{
					expandWidth = value;
					SetScale();
					if (Widget.Loaded)
						Control.PerformLayout();
				}
			}
		}

		public bool ExpandContentHeight
		{
			get { return expandHeight; }
			set
			{
				if (expandHeight != value)
				{
					expandHeight = value;
					SetScale();
					if (Widget.Loaded)
						Control.PerformLayout();
				}
			}
		}

		public float MinimumZoom { get { return 1f; } set { } }

		public float MaximumZoom { get { return 1f; } set { } }

		public float Zoom { get { return 1f; } set { } }
	}
}
