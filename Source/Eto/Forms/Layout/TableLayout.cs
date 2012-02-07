using System;
using Eto.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Collections;
using Eto.Collections;
using System.Xaml;
using System.ComponentModel;

namespace Eto.Forms
{
	public interface ITableLayout : IPositionalLayout
	{
		void CreateControl (int cols, int rows);

		void SetColumnScale (int column, bool scale);

		void SetRowScale (int row, bool scale);

		Size Spacing { get; set; }

		Padding Padding { get; set; }
	}

	[ContentProperty("Children")]
	public class TableLayout : Layout
	{
		ITableLayout inner;
		Control[,] controls;
		Size size;
		BaseList<Control> children;
		public static Size DefaultSpacing = new Size (5, 5);
		public static Padding DefaultPadding = new Padding (5);
		
		public override IEnumerable<Control> Controls {
			get {
				if (controls == null)
					return Enumerable.Empty<Control> ();
				return controls.OfType<Control> ();
			}
		}
		
		public BaseList<Control> Children {
			get { 
				if (children == null) {
					children = new BaseList<Control> ();
				}
				return children; 
			}
		}
		
		public Size Size {
			get { return size; }
			set {
				size = value;
				if (!size.IsEmpty) {
					controls = new Control[size.Width, size.Height];
					inner.CreateControl (size.Width, size.Height);
				}
			}
		}

		static AttachableMemberIdentifier LocationProperty = new AttachableMemberIdentifier (typeof(TableLayout), "Location");
		
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
		
		public static Control AutoSized (Control control, Padding? padding = null)
		{
			var layout = new TableLayout(new Panel(), 2, 2);
			layout.Padding = padding ?? Padding.Empty;
			layout.Spacing = Size.Empty;
			layout.Add (control, 0, 0);
			return layout.Container;
		}
		
		public TableLayout ()
			: this(null, Size.Empty)
		{
		}

		public TableLayout (Size size)
			: this(null, size)
		{
		}
		
		public TableLayout (Container container, int width, int height)
			: this(container, new Size(width, height))
		{
		}

		public TableLayout (Container container, Size size)
			: base(container != null ? container.Generator : Generator.Current, container, typeof(ITableLayout), false)
		{
			inner = (ITableLayout)Handler;
			this.Size = size;
			Initialize ();
			if (this.Container != null)
				this.Container.Layout = this;
		}

		public void SetColumnScale (int column, bool scale = true)
		{
			inner.SetColumnScale (column, scale);
		}
		
		public void SetRowScale (int row, bool scale = true)
		{
			inner.SetRowScale (row, scale);
		}
		
		public void Add (Control control, int x, int y)
		{
			controls [x, y] = control;
			bool load = Loaded;
			if (control != null) {
				control.SetParentLayout (this);
				load &= !control.Loaded;
				if (load) {
					control.OnPreLoad (EventArgs.Empty);
					control.OnLoad (EventArgs.Empty);
				}
			}
			inner.Add (control, x, y);
			if (control != null && load)
				control.OnLoadComplete (EventArgs.Empty);
		}

		public void Add (Control child, int x, int y, bool xscale, bool yscale)
		{
			SetColumnScale (x, xscale);
			SetRowScale (y, yscale);
			Add (child, x, y);
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
		
		public Size Spacing {
			get { return inner.Spacing; }
			set { inner.Spacing = value; }
		}
		
		public Padding Padding {
			get { return inner.Padding; }
			set { 
				inner.Padding = value;
			}
		}
		
		public override void EndInit ()
		{
			base.EndInit ();
			foreach (var child in children) {
				this.Add (child, GetLocation (child));
			}
		}

	}
}
