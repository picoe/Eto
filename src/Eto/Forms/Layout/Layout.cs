using System;
using System.Linq;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Base class for all layout-based containers
	/// </summary>
	/// <remarks>
	/// Layout based containers are used to position child controls, and provides extra functionality
	/// to update the layout manually.
	/// </remarks>
	public abstract class Layout : Container, ISupportInitialize
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Layout"/> class.
		/// </summary>
		protected Layout()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Layout"/> class with the specified handler.
		/// </summary>
		/// <param name="handler">Handler to use for the widget</param>
		protected Layout(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Re-calculates the layout of the controls and re-positions them, if necessary
		/// </summary>
		/// <remarks>
		/// All layouts should theoretically work without having to manually update them, but in certain cases
		/// this may be necessary to be called.
		/// </remarks>
		public virtual void Update()
		{
			UpdateContainers(this);
			Handler.Update();
		}

		static void UpdateContainers(Container container)
		{
			foreach (var c in container.VisualControls.OfType<Layout>())
			{
				c.Update();
			}
		}

		/// <summary>
		/// Begins the initialization when loading from xaml or other code generated scenarios
		/// </summary>
		public virtual void BeginInit()
		{
		}

		/// <summary>
		/// Ends the initialization when loading from xaml or other code generated scenarios
		/// </summary>
		public virtual void EndInit()
		{
		}

		/// <summary>
		/// Platform handler interface for the the <see cref="Layout"/> class
		/// </summary>
		public new interface IHandler : Container.IHandler
		{
			/// <summary>
			/// Re-calculates the layout of the controls and re-positions them, if necessary
			/// </summary>
			/// <remarks>
			/// All layouts should theoretically work without having to manually update them, but in certain cases
			/// this may be necessary to be called.
			/// </remarks>
			void Update();
		}

		/// <summary>
		/// Platform handler interface for positional layouts where controls are placed in an x, y grid
		/// </summary>
		public interface IPositionalLayoutHandler
		{
			/// <summary>
			/// Adds the control to the layout given the specified co-ordinates
			/// </summary>
			/// <remarks>
			/// Adding a control typically will make it visible to the user immediately, assuming they can see the control
			/// in the current co-ordinates, and that the control's <see cref="Control.Visible"/> property is true
			/// </remarks>
			/// <param name="control">Child control to add to this layout</param>
			/// <param name="x">X co-ordinate</param>
			/// <param name="y">Y co-ordinate</param>
			void Add(Control control, int x, int y);

			/// <summary>
			/// Moves the control to the specified co-ordinates
			/// </summary>
			/// <remarks>
			/// This assumes that the control is already a child of this layout
			/// </remarks>
			/// <param name="control">Child control to move</param>
			/// <param name="x">New X co-ordinate</param>
			/// <param name="y">New Y co-ordinate</param>
			void Move(Control control, int x, int y);

			/// <summary>
			/// Removes the specified child from this layout
			/// </summary>
			/// <remarks>
			/// This assumes that the control is already a child of this layout.  This will make the child control
			/// invisible to the user
			/// </remarks>
			/// <param name="control">Child control to remove</param>
			void Remove(Control control);
		}

	}
}
