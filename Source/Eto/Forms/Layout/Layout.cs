using System;
using System.Linq;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Platform handler interface for the the <see cref="Layout"/> class
	/// </summary>
	public interface ILayout : IContainer
	{
		/// <summary>
		/// Re-calculates the layout of the controls and re-positions them, if necessary
		/// </summary>
		/// <remarks>
		/// All layouts should theoretically work without having to manually update them, but in certain cases
		/// this may be necessary to be called.
		/// </remarks>
		void Update ();

	}

	/// <summary>
	/// Platform handler interface for positional layouts where controls are placed in an x, y grid
	/// </summary>
	public interface IPositionalLayout : ILayout
	{
		/// <summary>
		/// Adds the control to the layout given the specified co-ordinates
		/// </summary>
		/// <remarks>
		/// Adding a control typically will make it visible to the user immediately, assuming they can see the control
		/// in the current co-ordinates, and that the control's <see cref="Control.Visible"/> property is true
		/// </remarks>
		/// <param name="child">Child control to add to this layout</param>
		/// <param name="x">X co-ordinate</param>
		/// <param name="y">Y co-ordinate</param>
		void Add (Control child, int x, int y);

		/// <summary>
		/// Moves the control to the specified co-ordinates
		/// </summary>
		/// <remarks>
		/// This assumes that the control is already a child of this layout
		/// </remarks>
		/// <param name="child">Child control to move</param>
		/// <param name="x">New X co-ordinate</param>
		/// <param name="y">New Y co-ordinate</param>
		void Move (Control child, int x, int y);

		/// <summary>
		/// Removes the specified child from this layout
		/// </summary>
		/// <remarks>
		/// This assumes that the control is already a child of this layout.  This will make the child control
		/// invisible to the user
		/// </remarks>
		/// <param name="child">Child control to remove</param>
		void Remove (Control child);
	}

	public abstract class Layout : Container, ISupportInitialize
	{
		new ILayout Handler { get { return (ILayout)base.Handler; } }

		public bool Initializing { get; private set; }

		[Obsolete("Use Parent instead")]
		public Container Container { get { return Parent; } }
		
		protected Layout (Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
		}

		protected Layout (Generator g, ILayout handler, bool initialize = true)
			: base (g, handler, initialize)
		{
		}
		
		public virtual void Update ()
		{
			UpdateContainers(this);
			Handler.Update ();
		}
		
		static void UpdateContainers (Container container)
		{
			foreach (var c in container.Controls.OfType<Layout>())
			{
				c.Update();
			}
		}
		
		public virtual void BeginInit ()
		{
			Initializing = true;
		}

		public virtual void EndInit ()
		{
			Initializing = false;
		}
	}
}
