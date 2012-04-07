using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Collections;
using Eto.Collections;
#if DESKTOP
using System.Windows.Markup;
using System.Xaml;
#endif

namespace Eto.Forms
{
	public interface IPixelLayout : IPositionalLayout
	{
	}

#if DESKTOP
	[ContentProperty ("Children")]
#endif
	public class PixelLayout : Layout
	{
		IPixelLayout inner;
		BaseList<Control> children;
		List<Control> controls = new List<Control> ();
		
		public override IEnumerable<Control> Controls {
			get {
				return controls;
			}
		}

		public BaseList<Control> Children
		{
			get
			{
				if (children == null) {
					children = new BaseList<Control> ();
				}
				return children;
			}
		}

		public PixelLayout ()
			: this(null)
		{
		}

		public PixelLayout (Container container)
			: base (container != null ? container.Generator : Generator.Current, container, typeof (IPixelLayout), true)
		{
			inner = (IPixelLayout)Handler;
		}
		
#if DESKTOP
		static AttachableMemberIdentifier LocationProperty = new AttachableMemberIdentifier (typeof (PixelLayout), "Location");

		public static Point GetLocation (Control control)
		{
			return control.Properties.Get<Point> (LocationProperty, Point.Empty);
		}

		public static void SetLocation (Control control, Point value)
		{
			control.Properties.Set (LocationProperty, value);
			var layout = control.ParentLayout as TableLayout;
			if (layout != null)
				layout.Move (control, value);
		}
#endif

		public void Add (Control control, int x, int y)
		{
			controls.Add (control);
			control.SetParentLayout (this);
			var load = Loaded && !control.Loaded;
			if (load) {
				control.OnPreLoad (EventArgs.Empty);
				control.OnLoad (EventArgs.Empty);
			}
			inner.Add (control, x, y);
			if (load)
				control.OnLoadComplete (EventArgs.Empty);
		}

		public void Add (Control child, Point p)
		{
			Add (child, p.X, p.Y);
		}
		
		public void Move (Control child, int x, int y)
		{
			inner.Move (child, x, y);
		}
		
		public void Move (Control child, Point p)
		{
			Move (child, p.X, p.Y);
		}
		
		public void Remove (IEnumerable<Control> controls)
		{
			foreach (var control in controls)
				Remove (control);
		}
		
		public void Remove (Control child)
		{
			if (controls.Remove (child))
				inner.Remove (child);
		}

		public override void EndInit ()
		{
			base.EndInit ();
#if DESKTOP
			foreach (var control in children) {
				Add (control, GetLocation (control));
			}
#endif
		}
	}
}
