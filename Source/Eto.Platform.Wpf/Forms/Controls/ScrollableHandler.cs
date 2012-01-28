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
		swc.ScrollViewer scroller;
		msc.VirtualCanvas virtualCanvas;
		swc.Grid grid;

		public override Color BackgroundColor
		{
			get
			{
				var brush = virtualCanvas.Background as swm.SolidColorBrush;
				if (brush != null) return Generator.Convert (brush.Color);
				else return Color.Black;
			}
			set
			{
				virtualCanvas.Background = new swm.SolidColorBrush (Generator.Convert (value));
			}
		}

		public override Size Size
		{
			get
			{
				return new Size ((int)Control.Width, (int)Control.Height);
			}
			set
			{
				Control.Width = value.Width; Control.Height = value.Height;
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
				SnapsToDevicePixels = true
			};
			virtualCanvas = new msc.VirtualCanvas {
				SnapsToDevicePixels = true,
				OrderControls = false
			};

			scroller.Content = virtualCanvas;
			grid = new swc.Grid ();
			//swc.Grid.SetRow (virtualCanvas, 0);
			//swc.Grid.SetColumn (virtualCanvas, 0);
			//grid.Children.Add (virtualCanvas);
			//scroller.Content = grid;
			Control.Child = scroller;
			this.Border = BorderType.Bezel;
		}

		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			GetVirtualChildren ();
		}

		void GetVirtualChildren ()
		{
			var virtualChildren = new ObservableCollection<msc.IVirtualChild> ();
			foreach (var child in Widget.Children.Select (r => r.Handler).OfType<ISupportVirtualize> ()) {
				child.ClearChildren ();
				foreach (var item in child.Children)
					virtualChildren.Add (item);
			}
			virtualCanvas.VirtualChildren = virtualChildren;
		}

		public void UpdateScrollSizes ()
		{
			var layout = Widget.Layout.Handler as IWpfLayout;
			if (layout != null) layout.AutoSize ();
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
				return new Eto.Drawing.Size ((int)scroller.ScrollableWidth, (int)scroller.ScrollableHeight);
			}
			set
			{
				
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
				return new Eto.Drawing.Size ((int)scroller.ViewportWidth, (int)scroller.ViewportHeight);
			}
			set
			{
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
			//swc.Grid.SetRow (control, 0);
			//swc.Grid.SetColumn (control, 0);
			//grid.Children.Add (control);
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
	}
}
