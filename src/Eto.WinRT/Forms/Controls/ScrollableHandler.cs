using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using swm = Windows.UI.Xaml.Media;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	/// <summary>
	/// Scrollable handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ScrollableHandler : WpfPanel<swc.Border, Scrollable, Scrollable.ICallback>, Scrollable.IHandler
	{
		BorderType borderType;
		bool expandContentWidth = true;
		bool expandContentHeight = true;
		readonly swc.ScrollViewer scroller;

		public sw.FrameworkElement ContentControl { get { return scroller; } }

		protected override bool UseContentSize { get { return false; } }

		public override Color BackgroundColor
		{
			get { return scroller.Background.ToEtoColor(); }
			set { scroller.Background = value.ToWpfBrush(scroller.Background); }
		}

		public ScrollableHandler()
		{
			Control = new swc.Border
			{
#if TODO_XAML
				SnapsToDevicePixels = true,
				Focusable = false,
#endif
			};
			scroller = new swc.ScrollViewer
			{
				VerticalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				HorizontalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
#if TODO_XAML
				CanContentScroll = true,
				SnapsToDevicePixels = true,
				Focusable = false
#endif
			};
			scroller.SizeChanged += (s, e) => UpdateSizes();
			scroller.Loaded += (s, e) => UpdateSizes();

			Control.Child = scroller;
			this.Border = BorderType.Bezel;
		}

		void UpdateSizes()
		{
			//var info = scroller.GetScrollInfo();
			//if (info != null)
			{
				var content = (swc.Border)scroller.Content;
				var viewportSize = new wf.Size(scroller.ViewportWidth, scroller.ViewportHeight);
				var prefSize = Content.GetPreferredSize(Conversions.PositiveInfinitySize);

				// hack for when a scrollable is in a group box it expands vertically
				if (Widget.FindParent<GroupBox>() != null)
					viewportSize.Height = Math.Max(0, viewportSize.Height - 1);

				if (ExpandContentWidth)
					content.Width = Math.Max(0, Math.Max(prefSize.Width, viewportSize.Width));
				else
					content.Width = prefSize.Width;

				if (ExpandContentHeight)
					content.Height = Math.Max(0, Math.Max(prefSize.Height, viewportSize.Height));
				else
					content.Height = prefSize.Height;
			}
			scroller.InvalidateMeasure();
		}

		public override void UpdatePreferredSize()
		{
			UpdateSizes();
			base.UpdatePreferredSize();
		}

		public void UpdateScrollSizes()
		{
			Control.InvalidateMeasure();
			UpdateSizes();
		}

		public Point ScrollPosition
		{
			get
			{
				EnsureLoaded();
				return new Point((int)scroller.HorizontalOffset, (int)scroller.VerticalOffset);
			}
			set
			{
				scroller.ChangeView(value.X, value.Y, null);
			}
		}

		public Size ScrollSize
		{
			get
			{
				EnsureLoaded();
				return new Size((int)scroller.ExtentWidth, (int)scroller.ExtentHeight);
			}
			set
			{
				var content = (swc.Border)Control.Child;
				content.MinHeight = value.Height;
				content.MinWidth = value.Width;
				UpdateSizes();
			}
		}

		public BorderType Border
		{
			get { return borderType; }
			set
			{
				borderType = value;
				switch (value)
				{
					case BorderType.Bezel:
#if TODO_XAML
						Control.BorderBrush = sw.SystemColors.ControlDarkDarkBrush;
#endif
						Control.BorderThickness = new sw.Thickness(1.0);
						break;
					case BorderType.Line:
#if TODO_XAML
						Control.BorderBrush = sw.SystemColors.ControlDarkDarkBrush;
#endif
						Control.BorderThickness = new sw.Thickness(1);
						break;
					case BorderType.None:
						Control.BorderBrush = null;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public override Size ClientSize
		{
			get
			{
				if (!Widget.Loaded)
					return Size;
				EnsureLoaded();
				return new Size((int)scroller.ViewportWidth, (int)scroller.ViewportHeight);
			}
			set
			{
				Size = value;
			}
		}

		public Rectangle VisibleRect
		{
			get { return new Rectangle(ScrollPosition, ClientSize); }
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			content.HorizontalAlignment = sw.HorizontalAlignment.Left;
			content.VerticalAlignment = sw.VerticalAlignment.Top;
			content.SizeChanged += (s, e) => UpdateSizes();
			scroller.Content = content;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Scrollable.ScrollEvent:
					scroller.ViewChanged += (sender, e) =>
					{
						Callback.OnScroll(Widget, new ScrollEventArgs(new Point((int)scroller.HorizontalOffset, (int)scroller.VerticalOffset)));
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}


		public bool ExpandContentWidth
		{
			get { return expandContentWidth; }
			set
			{
				if (expandContentWidth != value)
				{
					expandContentWidth = value;
					UpdateSizes();
				}
			}
		}

		public bool ExpandContentHeight
		{
			get { return expandContentHeight; }
			set
			{
				if (expandContentHeight != value)
				{
					expandContentHeight = value;
					UpdateSizes();
				}
			}
		}

		public override void Invalidate(bool invalidateChildren)
		{
			base.Invalidate(invalidateChildren);
			if (invalidateChildren)
			{
				foreach (var control in Widget.VisualChildren)
				{
					control.Invalidate(invalidateChildren);
				}
			}
		}

		public override void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			base.Invalidate(rect, invalidateChildren);
			if (invalidateChildren)
			{
				foreach (var control in Widget.VisualChildren)
				{
					control.Invalidate(rect, invalidateChildren);
				}
			}
		}

        public float MinimumZoom { get { return 1f; } set { } }

        public float MaximumZoom { get { return 1f; } set { } }

        public float Zoom  { get { return 1f; } set { } }
    }
}