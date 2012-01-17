using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Collections;
using Eto.Collections;
using System.Xaml;

namespace Eto.Forms
{
	public interface IPixelLayout : IPositionalLayout
	{
	}

	[ContentProperty ("Children")]
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

		public void Add (Control control, int x, int y)
		{
			control.SetParentLayout (this);
			var load = Loaded && !control.Loaded;
			if (load) {
				control.OnPreLoad (EventArgs.Empty);
				control.OnLoad (EventArgs.Empty);
			}
			inner.Add (control, x, y);
			if (load)
				control.OnLoadComplete (EventArgs.Empty);
			controls.Add (control);
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
			foreach (var control in children) {
				Add (control, GetLocation (control));
			}
		}
	}
}
