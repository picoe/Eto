using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Eto.Forms
{
	/// <summary>
	/// Layout to position controls by pixel coordinates
	/// </summary>
	/// <remarks>
	/// This layout can be used if you want to position controls based on pixel sizes.
	/// Note that controls will automatically size themselves and it is recommended to use
	/// a <see cref="DynamicLayout"/> or <see cref="TableLayout"/> instead, as this will better
	/// work across all platforms since each platform might have different standard sizes.
	/// </remarks>
	[ContentProperty("Contents")]
	[Handler(typeof(PixelLayout.IHandler))]
	public class PixelLayout : Layout
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		List<Control> children;
		readonly List<Control> controls = new List<Control>();

		/// <summary>
		/// Gets an enumeration of controls that are directly contained by this container
		/// </summary>
		/// <value>The contained controls.</value>
		public override IEnumerable<Control> Controls { get { return controls; } }

		/// <summary>
		/// Gets a collection of controls that are contained by this layout
		/// </summary>
		/// <remarks>
		/// When adding children using this, you can position them using the <see cref="SetLocation"/> static method.
		/// </remarks>
		/// <value>The contents of the container.</value>
		public List<Control> Contents
		{
			get
			{
				if (children == null)
					children = new List<Control>();
				return children;
			}
		}

		static readonly EtoMemberIdentifier LocationProperty = new EtoMemberIdentifier(typeof(PixelLayout), "Location");

		/// <summary>
		/// Gets the location of the control in the container
		/// </summary>
		/// <returns>The location.</returns>
		/// <param name="control">Control to get the location.</param>
		public static Point GetLocation(Control control)
		{
			return control.Properties.Get<Point>(LocationProperty);
		}

		/// <summary>
		/// Sets the location of the specified control
		/// </summary>
		/// <param name="control">Control to set the location.</param>
		/// <param name="value">Location of the control</param>
		public static void SetLocation(Control control, Point value)
		{
			control.Properties[LocationProperty] = value;
			var layout = control.Parent as PixelLayout;
			if (layout != null)
				layout.Move(control, value);
		}

		/// <summary>
		/// Adds a control to the layout with the specified pixel coordinates
		/// </summary>
		/// <param name="control">Control to add</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void Add(Control control, int x, int y)
		{
			control.Properties[LocationProperty] = new Point(x, y);
			controls.Add(control);
			SetParent(control, () => Handler.Add(control, x, y));
		}

		/// <summary>
		/// Adds a control at the specified location
		/// </summary>
		/// <param name="control">Control to add</param>
		/// <param name="location">Location to position the control</param>
		public void Add(Control control, Point location)
		{
			Add(control, location.X, location.Y);
		}

		/// <summary>
		/// Moves the control to the specified coordinates
		/// </summary>
		/// <param name="control">Control to move</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void Move(Control control, int x, int y)
		{
			control.Properties[LocationProperty] = new Point(x, y);
			Handler.Move(control, x, y);
		}

		/// <summary>
		/// Moves a control to the specified location.
		/// </summary>
		/// <param name="control">Control to move</param>
		/// <param name="location">Location to move to</param>
		public void Move(Control control, Point location)
		{
			Move(control, location.X, location.Y);
		}

		/// <summary>
		/// Remove the specified child control.
		/// </summary>
		/// <param name="child">Child to remove</param>
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

		/// <summary>
		/// Ends the initialization when loading from xaml or other code generated scenarios
		/// </summary>
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

		/// <summary>
		/// Handler interface for the <see cref="PixelLayout"/> control
		/// </summary>
		public new interface IHandler : Layout.IHandler, IPositionalLayoutHandler
		{
		}
	}
}
