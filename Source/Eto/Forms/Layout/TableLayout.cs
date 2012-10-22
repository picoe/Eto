using System;
using Eto.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;


#if XAML
using System.Windows.Markup;
using System.Xaml;
#endif

namespace Eto.Forms
{
	public interface ITableLayout : IPositionalLayout
	{
		void CreateControl (int cols, int rows);

		bool GetColumnScale (int column);

		void SetColumnScale (int column, bool scale);

		bool GetRowScale (int row);

		void SetRowScale (int row, bool scale);

		Size Spacing { get; set; }

		Padding Padding { get; set; }
	}

#if XAML
	[ContentProperty("Children")]
#endif
	public class TableLayout : Layout
	{
		ITableLayout inner;
		Control[,] controls;
		Size size;
		IList<Control> children;
		public static Size DefaultSpacing = new Size (5, 5);
		public static Padding DefaultPadding = new Padding (5);
		
		public override IEnumerable<Control> Controls {
			get {
				if (controls == null)
					return Enumerable.Empty<Control> ();
				return controls.OfType<Control> ();
			}
		}
		
		public IList<Control> Children {
			get { 
				if (children == null) {
					children = new List<Control> ();
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

		[TypeConverter(typeof(Int32ArrayConverter))]
		public int[] ColumnScale
		{
			set {
				for (int col = 0; col < Size.Width; col++) {
					SetColumnScale (col, false);
				}
				foreach (var col in value) {
					SetColumnScale (col, true);
				}
			}
			get {
				var vals = new List<int>();
				for (int col = 0; col < Size.Width; col++) {
					if (GetColumnScale (col))
						vals.Add(col);
				}
				return vals.ToArray ();
			}
		}

		[TypeConverter (typeof (Int32ArrayConverter))]
		public int[] RowScale
		{
			set {
				for (int row = 0; row < Size.Height; row++) {
					SetRowScale (row, false);
				}
				foreach (var row in value) {
					SetRowScale (row, true);
				}
			}
			get {
				var vals = new List<int>();
				for (int row = 0; row < Size.Height; row++) {
					if (GetRowScale (row))
						vals.Add (row);
				}
				return vals.ToArray ();
			}
		}

		#region Attached Properties

		static EtoMemberIdentifier LocationProperty = new EtoMemberIdentifier (typeof(TableLayout), "Location");
		
		public static Point GetLocation (Control control)
		{
			return control.Properties.Get<Point> (LocationProperty, Point.Empty);
		}
		
		public static void SetLocation (Control control, Point value)
		{
			control.Properties[LocationProperty] = value;
			var layout = control.ParentLayout as TableLayout;
			if (layout != null)
				layout.Move (control, value);
		}

		static EtoMemberIdentifier ColumnScaleProperty = new EtoMemberIdentifier (typeof(TableLayout), "ColumnScale");
		
		public static bool GetColumnScale (Control control)
		{
			return control.Properties.Get<bool> (ColumnScaleProperty, false);
		}
		
		public static void SetColumnScale (Control control, bool value)
		{
			control.Properties[ColumnScaleProperty] = value;
		}

		static EtoMemberIdentifier RowScaleProperty = new EtoMemberIdentifier (typeof(TableLayout), "RowScale");
		
		public static bool GetRowScale (Control control)
		{
			return control.Properties.Get<bool> (RowScaleProperty, false);
		}
		
		public static void SetRowScale (Control control, bool value)
		{
			control.Properties[RowScaleProperty] = value;
		}

		#endregion

		public static Control AutoSized (Control control, Padding? padding = null, bool centered = false)
		{
			var layout = new TableLayout(new Panel(), 3, 3);
			layout.Padding = padding ?? Padding.Empty;
			layout.Spacing = Size.Empty;
			if (centered)
			{
				layout.SetColumnScale (0);
				layout.SetColumnScale (2);
				layout.SetRowScale (0);
				layout.SetRowScale (2);
			}
			layout.Add (control, 1, 1);
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

		public bool GetColumnScale (int column)
		{
			return inner.GetColumnScale (column);
		}

		public void SetRowScale (int row, bool scale = true)
		{
			inner.SetRowScale (row, scale);
		}

		public bool GetRowScale (int row)
		{
			return inner.GetRowScale (row);
		}

		public void Add (Control control, int x, int y)
		{
			if (control != null)
				SetLocation (control, new Point(x, y));
			InnerAdd (control, x, y);
		}

		void InnerAdd (Control control, int x, int y)
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
			child.Properties[LocationProperty] = new Point(x, y);
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
			child.Properties[LocationProperty] = new Point(x, y);
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

		[OnDeserialized]
		internal void OnDeserialized (StreamingContext context)
		{
			OnDeserialized ();
		}

		public override void EndInit ()
		{
			base.EndInit ();
			OnDeserialized (Container != null); // mono calls EndInit BEFORE setting to parent
		}

		void OnDeserialized (bool direct = false)
		{
			if (Loaded || direct) {
				if (children != null) {
					foreach (var control in children) {
						var location = GetLocation (control);
						Add (control, location);
						if (GetColumnScale(control))
							SetColumnScale (location.X);
						if (GetRowScale(control))
							SetRowScale (location.Y);
					}
				}
			} else {
				this.PreLoad += HandleDeserialized;
			}
		}
		
		void HandleDeserialized (object sender, EventArgs e)
		{
			OnDeserialized(true);
			this.PreLoad -= HandleDeserialized;
		}
	}
}
