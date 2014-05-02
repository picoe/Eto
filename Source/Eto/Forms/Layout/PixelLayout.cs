using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Runtime.Serialization;

#if XAML
using System.Windows.Markup;

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
		readonly List<Control> controls = new List<Control>();

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
			: this((Generator)null)
		{
		}

		public PixelLayout(Generator generator)
			: base(generator, typeof(IPixelLayout))
		{
		}

		[Obsolete("Add a PixelLayout to a Panel using the Panel.Content property")]
		public PixelLayout(Panel container)
			: this(container == null ? null : container.Generator)
		{
			if (container != null)
				container.Content = this;
		}

		static readonly EtoMemberIdentifier LocationProperty = new EtoMemberIdentifier(typeof(PixelLayout), "Location");

		public static Point GetLocation(Control control)
		{
			return control.Properties.Get<Point>(LocationProperty);
		}

		public static void SetLocation(Control control, Point value)
		{
			control.Properties[LocationProperty] = value;
			var layout = control.Parent as PixelLayout;
			if (layout != null)
				layout.Move(control, value);
		}

		public void Add(Control control, int x, int y)
		{
			control.Properties[LocationProperty] = new Point(x, y);
			controls.Add(control);
			var load = SetParent(control);
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

		public override void Remove(Control child)
		{
			if (controls.Remove(child))
			{
				Handler.Remove(child);
				RemoveParent(child);
			}
		}

		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
			OnDeserialized();
		}

		public override void EndInit()
		{
			base.EndInit();
			OnDeserialized(Parent != null); // mono calls EndInit BEFORE setting to parent
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
				PreLoad += HandleDeserialized;
			}
		}

		void HandleDeserialized(object sender, EventArgs e)
		{
			OnDeserialized(true);
			PreLoad -= HandleDeserialized;
		}
	}
}
