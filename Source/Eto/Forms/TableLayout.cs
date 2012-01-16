using System;
using Eto.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Collections;
using Eto.Collections;
using System.Xaml;

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
				return controls.OfType<Control> ();
			}
		}
		
		public IList Children {
			get { 
				if (children == null) {
					children = new BaseList<Control> ();
					children.Added += delegate(object sender, ListEventArgs<Control> e) {
						e.Item.SetParentLayout (this);
					};
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
		
		public static Point GetPosition (Control control)
		{
			var layout = control.ParentLayout as TableLayout;
			if (layout != null) {
				var controls = layout.controls;
				for (int y=0; y<controls.GetLength (1); y++)
					for (int x=0; x<controls.GetLength (0); x++) {
						if (controls [x, y] == control)
							return new Point (x, y);
					}
			}
			return Point.Empty;
		}
		
		public static void SetPosition (Control control, Point value)
		{
			var layout = control.ParentLayout as TableLayout;
			if (layout != null) 
				layout.Add (control, value);
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
			set { inner.Padding = value; }
		}
	}
}
