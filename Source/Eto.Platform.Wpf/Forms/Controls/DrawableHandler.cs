using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;
using msc = Microsoft.Sample.Controls;
using swi = System.Windows.Input;
using Eto.Platform.Wpf.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class DrawableHandler : WpfPanel<swc.Canvas, Drawable>, IDrawable, ISupportVirtualize
	{
		List<EtoChild> virtualChildren;

		class EtoMainCanvas : swc.Canvas
		{
			public DrawableHandler Handler { get; set; }

			protected override void OnMouseDown (sw.Input.MouseButtonEventArgs e)
			{
				if (Handler.CanFocus) {
					swi.Keyboard.Focus (this);
				}
				base.OnMouseDown (e);
			}

			protected override void OnRender (swm.DrawingContext dc)
			{
				base.OnRender (dc);
				if (Handler.virtualChildren == null) {
					var rect = new Rectangle (Handler.Widget.Size);
					var graphics = new Graphics (Handler.Widget.Generator, new GraphicsHandler (this, dc, rect.ToWpf ()));
					Handler.Widget.OnPaint (new PaintEventArgs (graphics, rect));
				}
			}
		}

		class EtoCanvas : swc.Canvas
		{
			public EtoChild Child { get; set; }

			public DrawableHandler Handler { get { return Child.Handler; } }

			protected override void OnRender (swm.DrawingContext dc)
			{
				var rect = new sw.Rect (Child.Bounds.X, Child.Bounds.Y, Child.Bounds.Width + 0.5, Child.Bounds.Height + 0.5);
				var graphics = new Graphics (Handler.Widget.Generator, new GraphicsHandler (this, dc, rect));
				dc.PushGuidelineSet(new swm.GuidelineSet(new double[] { Child.Bounds.Left, Child.Bounds.Right }, new double[] { Child.Bounds.Top, Child.Bounds.Bottom }));
				Handler.Widget.OnPaint (new PaintEventArgs (graphics, Child.Bounds.ToEto ()));
			}
		}

		class EtoChild : msc.IVirtualChild
		{
			public DrawableHandler Handler { get; set; }

			public sw.Rect Bounds { get; set; }

			#pragma warning disable 67
			public event EventHandler BoundsChanged;
			#pragma warning restore 67

			public sw.UIElement Visual
			{
				get; private set;
			}

			public sw.UIElement CreateVisual (msc.VirtualCanvas parent)
			{
				var transform = new swm.TransformGroup ();
				transform.Children.Add(new swm.TranslateTransform (-Bounds.X, -Bounds.Y));
				Visual = new EtoCanvas {
					Child = this,
					SnapsToDevicePixels = true,
					RenderTransform = transform
				};
				return Visual;
			}

			public void DisposeVisual ()
			{
				Visual = null;
			}


			public swc.Canvas ParentCanvas
			{
				get { return Handler.Control; }
			}
		}

		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			var layout = Widget.ParentLayout;
			if (layout != null) {
				var scrollable = layout.Container.ContainerObject as swc.ScrollViewer;
				if (scrollable != null) {
					var parent = Control.Parent as swc.Panel;
					if (parent != null)
						parent.Children.Remove (Control);
					scrollable.Content = Control;
				}
			}
		}

		public override Size Size
		{
			get { return base.Size; }
			set {
				if (value != base.Size) {
					base.Size = value;
					if (virtualChildren != null)
						UpdateCanvas ();
				}
			}
		}

		public void Create ()
		{
			Control = new EtoMainCanvas {
				Handler = this,
				SnapsToDevicePixels = true
			};
			Control.SizeChanged += Control_SizeChanged;
		}

		void Control_SizeChanged (object sender, sw.SizeChangedEventArgs e)
		{
			if (virtualChildren == null)
				return;
			UpdateCanvas ();
            var parentScrollable = Widget.FindParent<Scrollable>();
            var parentHandler = parentScrollable.Handler as ScrollableHandler;
            if (parentHandler != null)
            {

                parentHandler.UpdateVisualChildren ();
            }
		}

		void UpdateCanvas ()
		{
			if (virtualChildren == null)
				virtualChildren = new List<EtoChild> ();
			virtualChildren.Clear ();
			var tileSize = 100;
			var width = Control.Width / tileSize;
			var height = Control.Height / tileSize;
			int totalheight = 0;
			for (int y = 0; y < height; y++) {
				int totalwidth = 0;
				for (int x = 0; x < width; x++) {
					var xsize = Math.Min (tileSize, Control.Width - totalwidth);
					var ysize = Math.Min (tileSize, Control.Height - totalheight);
					var child = new EtoChild {
						Bounds = new sw.Rect (x * tileSize, y * tileSize, xsize, ysize),
						Handler = this
					};
					virtualChildren.Add (child);
					totalwidth += tileSize;
				}
				totalheight += tileSize;
			}
		}

		public override void Invalidate ()
		{
			if (virtualChildren != null) {
				foreach (var child in virtualChildren) {
					var visual = child.Visual;
					if (visual != null)
						visual.InvalidateVisual ();
				}
			}
			else
				base.Invalidate ();
		}

		public override void Invalidate (Rectangle rect)
		{
			if (virtualChildren != null) {
				foreach (var child in virtualChildren) {
					var visual = child.Visual;
					if (visual != null && rect.Intersects (child.Bounds.ToEto ()))
						visual.InvalidateVisual ();
				}
			}
			else
				base.Invalidate (rect);
		}

		public void Update (Eto.Drawing.Rectangle rect)
		{
			Invalidate (rect);
		}

		public bool CanFocus
		{
			get { return Control.Focusable; }
			set {
				if (value != Control.Focusable) {
					Control.Focusable = value;
				}
			}
		}

		public IEnumerable<msc.IVirtualChild> Children
		{
			get {
				UpdateCanvas ();
				return virtualChildren;
			}
		}

		public void ClearChildren ()
		{
			Control.Children.Clear ();
		}
	}
}
