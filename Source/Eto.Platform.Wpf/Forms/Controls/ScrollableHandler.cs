using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swm = System.Windows.Media;
using msc = Microsoft.Sample.Controls;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public interface ISupportVirtualize
	{
        IEnumerable<msc.IVirtualChild> Children { get; }

		void ClearChildren ();
	}

	public class ScrollableHandler : WpfFrameworkElement<swc.Border, Scrollable>, IScrollable
	{
		BorderType borderType;
		bool expandContentWidth = true;
		bool expandContentHeight = true;
		swc.ScrollViewer scroller;
		msc.VirtualCanvas virtualCanvas;

		class EtoScrollViewer : swc.ScrollViewer
		{
		}

		public override Color BackgroundColor
		{
			get
			{
				var brush = virtualCanvas.Background as swm.SolidColorBrush;
				if (brush != null) return brush.Color.ToEto ();
				else return Colors.Black;
			}
			set
			{
				virtualCanvas.Background = new swm.SolidColorBrush (value.ToWpf ());
			}
		}

		public ScrollableHandler ()
		{
			Control = new swc.Border {
				SnapsToDevicePixels = true
			};
			scroller = new swc.ScrollViewer {
				VerticalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				HorizontalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				CanContentScroll = true,
				SnapsToDevicePixels = true,
				Focusable = false
			};
			virtualCanvas = new msc.VirtualCanvas {
				SnapsToDevicePixels = true,
				OrderControls = false
			};
            scroller.ScrollChanged += HandleChangedSize;
            scroller.Loaded += HandleChangedSize;

			scroller.Content = virtualCanvas;
			Control.Child = scroller;
			this.Border = BorderType.Bezel;
		}

        void HandleChangedSize (object sender, EventArgs e)
        {
			UpdateSizes ();
        }

		void UpdateSizes ()
		{
			if (!Control.IsLoaded)
				return;
			var c = virtualCanvas.Backdrop;
			if (c != null && Widget.Layout != null && Widget.Layout.InnerLayout != null) {
				var layoutHandler = Widget.Layout.InnerLayout.Handler as IWpfLayout;
				if (layoutHandler == null)
					return;
				var preferredSize = layoutHandler.PreferredSize;
				if (this.ExpandContentWidth) {
					var margins = c.Margin.Left + c.Margin.Right;
					c.Width = Math.Max (0, Math.Max(c.MinWidth, Math.Max (preferredSize.Width, scroller.ViewportWidth)));
				}
				else
					c.Width = preferredSize.Width;

				if (this.ExpandContentHeight) {
					var margins = c.Margin.Top + c.Margin.Bottom;
					c.Height = Math.Max (0, Math.Max(c.MinHeight, Math.Max (preferredSize.Height, scroller.ViewportHeight)));
				}
				else
					c.Height = preferredSize.Height;
				//virtualCanvas.RecalculateExtent ();
			}
		}

        public override void OnLoadComplete (EventArgs e)
        {
            base.OnLoadComplete (e);
            GetVirtualChildren ();
            Control.InvalidateMeasure ();
        }

        public void UpdateVisualChildren ()
        {
            GetVirtualChildren ();
            Control.InvalidateMeasure ();
        }

        void ScanChildren (ICollection<msc.IVirtualChild> virtualChildren, Container container)
        {
            foreach (var control in container.Controls) {
                if (control.Handler is ScrollableHandler)
                    continue;
                var child = control.Handler as ISupportVirtualize;
                if (child != null) {
                    child.ClearChildren();
                    foreach (var item in child.Children)
                        virtualChildren.Add (item);
                }
                var childContainer = control as Container;
                if (childContainer != null) {
                    ScanChildren (virtualChildren, childContainer);
                }
            }
        }

		void GetVirtualChildren ()
		{
			var virtualChildren = new ObservableCollection<msc.IVirtualChild> ();
            ScanChildren (virtualChildren, Widget);
			virtualCanvas.VirtualChildren = virtualChildren;
		}

		public void UpdateScrollSizes ()
		{
			var layout = Widget.Layout.Handler as IWpfLayout;
			if (layout != null) layout.AutoSize ();
			GetVirtualChildren ();
			Control.InvalidateMeasure ();
			UpdateSizes ();
		}

		public Eto.Drawing.Point ScrollPosition
		{
			get
			{
				return new Eto.Drawing.Point ((int)scroller.HorizontalOffset, (int)scroller.VerticalOffset);
			}
			set
			{
				scroller.ScrollToVerticalOffset (value.Y);
				scroller.ScrollToHorizontalOffset (value.X);
			}
		}

		public Eto.Drawing.Size ScrollSize
		{
			get
			{
				return new Eto.Drawing.Size ((int)scroller.ExtentWidth, (int)scroller.ExtentHeight);
			}
			set
			{
                virtualCanvas.Backdrop.MinHeight = value.Height;
                virtualCanvas.Backdrop.MinWidth = value.Width;
				UpdateSizes ();
			}
		}

		public BorderType Border
		{
			get { return borderType; }
			set
			{
				borderType = value;
				switch (value) {
					case BorderType.Bezel:
						Control.BorderBrush = sw.SystemColors.ControlDarkDarkBrush;
						Control.BorderThickness = new sw.Thickness (1.0);
						break;
					case BorderType.Line:
						Control.BorderBrush = sw.SystemColors.ControlDarkDarkBrush;
						Control.BorderThickness = new sw.Thickness (1);
						break;
					case BorderType.None:
						Control.BorderBrush = null;
						//Control.BorderThickness = new W.Thickness(0);
						break;
					default:
						throw new NotSupportedException ();
				}
			}
		}

		public Eto.Drawing.Size ClientSize
		{
			get
			{
				return this.Size; // new Eto.Drawing.Size ((int)scroller.ViewportWidth, (int)scroller.ViewportHeight);
			}
			set
			{
				this.Size = value;
			}
		}

		public Eto.Drawing.Rectangle VisibleRect
		{
			get { return new Eto.Drawing.Rectangle (ScrollPosition, ScrollSize); }
		}

		public object ContainerObject
		{
			get { return virtualCanvas.ContentCanvas; }
		}

		public void SetLayout (Layout layout)
		{
			var control = layout.ControlObject as sw.UIElement;
			virtualCanvas.Backdrop.Child = control;
		}

		public Eto.Drawing.Size? MinimumSize
		{
			get
			{
				if (Control.MinWidth == 0 && Control.MinHeight == 0)
					return new Eto.Drawing.Size ((int)Control.MinWidth, (int)Control.MinHeight);
				else
					return null;
			}
			set
			{
				if (value != null) {
					Control.MinWidth = value.Value.Width;
					Control.MinHeight = value.Value.Height;
				}
				else {
					Control.MinHeight = 0;
					Control.MinWidth = 0;
				}
			}
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
				case Scrollable.ScrollEvent:
					scroller.ScrollChanged += (sender, e) => {
						Widget.OnScroll (new ScrollEventArgs (new Point ((int)e.HorizontalOffset, (int)e.VerticalOffset)));
					};
					break;
				default:
					base.AttachEvent (handler);
					break;
			}
		}


		public bool ExpandContentWidth
		{
			get { return expandContentWidth; }
			set
			{
				if (expandContentWidth != value) {
					expandContentWidth = value;
					UpdateSizes ();
				}
			}
		}

		public bool ExpandContentHeight
		{
			get { return expandContentHeight; }
			set
			{
				if (expandContentHeight != value) {
					expandContentHeight = value;
					UpdateSizes ();
				}
			}
		}
	}
}
