using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Serialization;
using System.Linq;

#if XAML
using System.Windows.Markup;
using System.Xaml;

#endif
namespace Eto.Forms
{
	public interface IPixelLayout : IPositionalLayout
	{
	}

	[ContentProperty ("Contents")]
	public class PixelLayout : Layout
	{
		new IPixelLayout Handler { get { return (IPixelLayout)base.Handler; } }

		List<Control> children;
		List<Control> controls = new List<Control>();

		public override IEnumerable<Control> Controls
		{
			get
			{
				return controls;
			}
		}

		public List<Control> Contents
		{
			get
			{
				if (children == null)
					children = new List<Control>();
				return children;
			}
		}

		public PixelLayout()
			: this(null)
		{
		}

		public PixelLayout(DockContainer container)
			: base (container != null ? container.Generator : Generator.Current, container, typeof (IPixelLayout), true)
		{
		}

		static EtoMemberIdentifier LocationProperty = new EtoMemberIdentifier(typeof(PixelLayout), "Location");

		public static Point GetLocation(Control control)
		{
			return control.Properties.Get<Point>(LocationProperty, Point.Empty);
		}

		public static void SetLocation(Control control, Point value)
		{
			control.Properties[LocationProperty] = value;
			var layout = control.ParentLayout as PixelLayout;
			if (layout != null)
				layout.Move(control, value);
		}

		public void Add(Control control, int x, int y)
		{
			control.Properties[LocationProperty] = new Point(x, y);
			controls.Add(control);
			control.SetParent(this);
			var load = Loaded && !control.Loaded;
			if (load)
			{
				control.OnPreLoad(EventArgs.Empty);
				control.OnLoad(EventArgs.Empty);
			}
			Handler.Add(control, x, y);
			if (load)
				control.OnLoadComplete(EventArgs.Empty);
		}

		public void Add(Control child, Point p)
		{
			Add(child, p.X, p.Y);
		}

		public void Move(Control child, int x, int y)
		{
			child.Properties[LocationProperty] = new Point(x, y);
			Handler.Move(child, x, y);
		}

		public void Move(Control child, Point p)
		{
			Move(child, p.X, p.Y);
		}

		public void Remove(IEnumerable<Control> controls)
		{
			foreach (var control in controls)
				Remove(control);
		}

		public void RemoveAll()
		{
			Remove(this.Controls.ToArray());
		}

		public void Remove(Control child)
		{
			if (controls.Remove(child))
			{
				Handler.Remove(child);
				child.SetParent(null);
			}
		}

		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			OnDeserialized();
		}

		public override void EndInit()
		{
			base.EndInit();
			OnDeserialized(ParentLayout != null); // mono calls EndInit BEFORE setting to parent
		}

		void OnDeserialized(bool direct = false)
		{
			if (Loaded || direct)
			{
				if (children != null)
				{
					foreach (var control in children)
					{
						Add(control, GetLocation(control));
					}
				}
			}
			else
			{
				this.PreLoad += HandleDeserialized;
			}
		}

		void HandleDeserialized(object sender, EventArgs e)
		{
			OnDeserialized(true);
			this.PreLoad -= HandleDeserialized;
		}
	}
}
