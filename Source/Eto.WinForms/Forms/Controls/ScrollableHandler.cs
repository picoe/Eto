using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.WinForms
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

			System.Drawing.Size lastClientSize;

			protected override void OnLayout(swf.LayoutEventArgs levent)
			{
				lastClientSize = ClientSize;

				var contentControl = Handler.Content.GetWindowsHandler();
				if (contentControl != null)
				{
					var minSize = new Size();

					var clientSize = lastClientSize;
					if (Handler.finalLayoutPass)
					{
						var preferred = contentControl.GetPreferredSize(Eto.Drawing.Size.Empty);
						if (Handler.ExpandContentWidth && preferred.Height > ClientSize.Height)
							clientSize.Width -= swf.SystemInformation.VerticalScrollBarWidth;
						if (Handler.ExpandContentHeight && preferred.Width > ClientSize.Width)
							clientSize.Height -= swf.SystemInformation.HorizontalScrollBarHeight;
					}
					if (Handler.ExpandContentWidth)
						minSize.Width = Math.Max(0, clientSize.Width);
					if (Handler.ExpandContentHeight)
						minSize.Height = Math.Max(0, clientSize.Height);

					// set minimum size for the content if we want to extend to the size of the scrollable width/height
					contentControl.ParentMinimumSize = minSize - Handler.Padding.Size;
				}
				base.OnLayout(levent);
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
			get
			{
				return Control.Padding.ToEto();
			}
			set
			{
				Control.Padding = value.ToSWF();
			}
		}

		protected override void ResumeControl(bool top = true)
		{
			finalLayoutPass = true;
			base.ResumeControl(true); // if scrollable's size is not changed, then the children don't get laid out
			finalLayoutPass = false;
		}

		public override swf.Control ContainerContentControl
		{
			get { return content; }
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			if (Content != null)
				Content.SetScale(!ExpandContentWidth, !ExpandContentHeight);
		}

		protected override void SetContentScale(bool xscale, bool yscale)
		{
			base.SetContentScale(!ExpandContentWidth, !ExpandContentHeight);
		}

		public override Size GetPreferredSize(Size availableSize, bool useCache)
		{
			var baseSize = UserDesiredSize;
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
			get
			{
				switch (Control.BorderStyle)
				{
					case swf.BorderStyle.FixedSingle:
						return BorderType.Line;
					case swf.BorderStyle.None:
						return BorderType.None;
					case swf.BorderStyle.Fixed3D:
						return BorderType.Bezel;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				switch (value)
				{
					case BorderType.Bezel:
						Control.BorderStyle = swf.BorderStyle.Fixed3D;
						break;
					case BorderType.Line:
						Control.BorderStyle = swf.BorderStyle.FixedSingle;
						break;
					case BorderType.None:
						Control.BorderStyle = swf.BorderStyle.None;
						break;
					default:
						throw new NotSupportedException();
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

			content = new swf.Panel
			{
				Size = sd.Size.Empty,
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink
			};
			Control.Controls.Add(content);
		}

		protected override void SetContent(swf.Control contentControl)
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
