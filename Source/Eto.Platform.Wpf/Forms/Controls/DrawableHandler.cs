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
using Eto.Platform.Wpf.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class DrawableHandler : WpfPanel<swc.Canvas, Drawable>, IDrawable, ISupportVirtualize
	{
		List<EtoChild> virtualChildren;

		class EtoMainCanvas : swc.Canvas
		{
			public DrawableHandler Handler { get; set; }

			protected override void OnRender (swm.DrawingContext dc)
			{
				base.OnRender (dc);
				if (Handler.virtualChildren == null) {
					var graphics = new Graphics (Handler.Widget.Generator, new GraphicsHandler (this, dc));
					Handler.Widget.OnPaint (new PaintEventArgs (graphics, new Rectangle (Handler.Widget.Size)));
				}
			}
		}

		class EtoCanvas : swc.Canvas
		{
			public EtoChild Child { get; set; }

			public DrawableHandler Handler { get { return Child.Handler; } }

			protected override void OnRender (swm.DrawingContext dc)
			{
				var graphics = new Graphics (Handler.Widget.Generator, new GraphicsHandler (this, dc));
				dc.PushGuidelineSet(new swm.GuidelineSet(new double[] { Child.Bounds.Left, Child.Bounds.Right }, new double[] { Child.Bounds.Top, Child.Bounds.Bottom }));
				dc.PushClip (new swm.RectangleGeometry (new sw.Rect(Child.Bounds.X, Child.Bounds.Y, Child.Bounds.Width + 0.5, Child.Bounds.Height + 0.5)));
				Handler.Widget.OnPaint (new PaintEventArgs (graphics, Generator.Convert (Child.Bounds)));
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
				if (Handler.CanFocus) {
					Visual.MouseDown += delegate {
						Handler.Control.Focus ();
					};
					/*Visual.KeyDown += (sender, e) => {
						Handler.Widget.OnKeyDown(
					};*/
				}
				return Visual;
			}

			public void DisposeVisual ()
			{
				Visual = null;
			}


			public swc.Canvas ParentCanvas
			{
				get { return Handler.Control; }
				//get { return null; }
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

		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);

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
			var parent = Control.GetParent<Microsoft.Sample.Controls.VirtualCanvas> ();
			if (parent != null) {
				parent.InvalidateArrange ();
				parent.InvalidateVisual ();
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
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					var child = new EtoChild {
						Bounds = new sw.Rect (x * tileSize, y * tileSize, tileSize, tileSize),
						Handler = this
					};
					virtualChildren.Add (child);
				}
			}
		}

		public void Update (Eto.Drawing.Rectangle rect)
		{
			var parent = Control.GetParent<Microsoft.Sample.Controls.VirtualCanvas> ();
			if (parent != null) {
				parent.InvalidateMeasure ();
			}
			Control.InvalidateVisual ();
		}

		public bool CanFocus
		{
			get { return Control.Focusable; }
			set { Control.Focusable = value; }
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
