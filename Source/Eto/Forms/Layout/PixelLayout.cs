using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Eto.Forms
{
	[ContentProperty ("Contents")]
	[Handler(typeof(PixelLayout.IHandler))]
	public class PixelLayout : Layout
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

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
		{
		}

		[Obsolete("Use default constructor instead")]
		public PixelLayout(Generator generator)
			: base(generator, typeof(IHandler))
		{
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
			SetParent(control, () => Handler.Add(control, x, y));
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

		public interface IHandler : Layout.IHandler, IPositionalLayoutHandler
		{
		}
	}
}
