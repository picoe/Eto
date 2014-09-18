using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Orientation of a <see cref="Splitter"/> control.
	/// </summary>
	public enum SplitterOrientation
	{
		/// <summary>
		/// Controls are in horizontal orientation, with a vertical divider between them.
		/// </summary>
		Horizontal,
		/// <summary>
		/// Controls are in vertical orientation, with a horizontal divider betwen them.
		/// </summary>
		Vertical
	}

	/// <summary>
	/// Specifies which panel has a fixed size the parent container is resized.
	/// </summary>
	public enum SplitterFixedPanel
	{
		/// <summary>
		/// The first panel will be fixed size, where the second will resize along with the splitter's container.
		/// </summary>
		Panel1,
		/// <summary>
		/// The second panel will be fixed size, where the first will resize along with the splitter's container.
		/// </summary>
		Panel2,
		/// <summary>
		/// Both panels will resize along with the splitter's container.
		/// </summary>
		None
	}

	/// <summary>
	/// Control to show two panels separated by a splitter
	/// </summary>
	/// <remarks>
	/// Most desktop platforms allow the user to modify the position of the splitter, though some (notibly iOS) do
	/// not.
	/// The <see cref="Orientation"/> of the splitter determines how the controls are laid out, either horizontally
	/// or vertically.
	/// </remarks>
	[Handler(typeof(Splitter.IHandler))]
	public class Splitter : Container
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets an enumeration of controls that are directly contained by this container
		/// </summary>
		/// <value>The contained controls.</value>
		public override IEnumerable<Control> Controls
		{
			get
			{
				if (Panel1 != null)
					yield return Panel1;
				if (Panel2 != null)
					yield return Panel2;
			}
		}

		/// <summary>
		/// Gets a value indicating the <see cref="Splitter"/> is supported in the platform
		/// </summary>
		/// <value><c>true</c> if supported; otherwise, <c>false</c>.</value>
		public static bool IsSupported
		{
			get { return Platform.Instance.Supports<IHandler>(); }
		}

		#region Events

		/// <summary>
		/// Identifier for the <see cref="PositionChanged"/> event
		/// </summary>
		public const string PositionChangedEvent = "Splitter.PositionChanged";

		/// <summary>
		/// Raised when the user moves the splitter.
		/// </summary>
		public event EventHandler<EventArgs> PositionChanged
		{
			add { Properties.AddHandlerEvent(PositionChangedEvent, value); }
			remove { Properties.RemoveEvent(PositionChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="PositionChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnPositionChanged(EventArgs e)
		{
			Properties.TriggerEvent(PositionChangedEvent, this, e);
		}

		#endregion

		static Splitter()
		{
			EventLookup.Register<Splitter>(c => c.OnPositionChanged(null), Splitter.PositionChangedEvent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Splitter"/> class.
		/// </summary>
		public Splitter()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Splitter"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public Splitter(Generator generator) : this (generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Splitter"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Splitter(Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		/// <summary>
		/// Gets or sets the orientation of the panels in the splitter.
		/// </summary>
		/// <value>The orientation of the panels.</value>
		public SplitterOrientation Orientation
		{
			get { return Handler.Orientation; }
			set { Handler.Orientation = value; }
		}

		/// <summary>
		/// Gets or sets the panel with fixed size.
		/// </summary>
		/// <remarks>
		/// This specifies which panel will not change its size when the splitter's container is resized.
		/// If <see cref="SplitterFixedPanel.None"/>, both panels will resize.
		/// </remarks>
		/// <value>The fixed panel.</value>
		public SplitterFixedPanel FixedPanel
		{
			get { return Handler.FixedPanel; }
			set { Handler.FixedPanel = value; }
		}

		/// <summary>
		/// Gets or sets the position of the splitter from the left or top, in pixels.
		/// </summary>
		/// <value>The position of the splitter.</value>
		public int Position
		{
			get { return Handler.Position; }
			set { Handler.Position = value; }
		}

		/// <summary>
		/// Gets or sets the top or left panel of the splitter.
		/// </summary>
		/// <value>The first panel.</value>
		public Control Panel1
		{
			get { return Handler.Panel1; }
			set
			{
				SetParent(value, () => Handler.Panel1 = value, Handler.Panel1);
			}
		}

		/// <summary>
		/// Gets or sets the bottom or right panel of the splitter.
		/// </summary>
		/// <value>The second panel.</value>
		public Control Panel2
		{
			get { return Handler.Panel2; }
			set
			{
				SetParent(value, () => Handler.Panel2 = value, Handler.Panel2);
			}
		}

		/// <summary>
		/// Removes the specified child from the container.
		/// </summary>
		/// <param name="child">Child to remove.</param>
		public override void Remove(Control child)
		{
			if (object.ReferenceEquals(Panel1, child))
			{
				Panel1 = null;
			}
			else if (object.ReferenceEquals(Panel2, child))
			{
				Panel2 = null;
			}
		}

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for the <see cref="Splitter"/>
		/// </summary>
		public new interface ICallback : Container.ICallback
		{
			/// <summary>
			/// Raises the position changed event.
			/// </summary>
			void OnPositionChanged(Splitter widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="Splitter"/>
		/// </summary>
		protected new class Callback : Container.Callback, ICallback
		{
			/// <summary>
			/// Raises the position changed event.
			/// </summary>
			public void OnPositionChanged(Splitter widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnPositionChanged(e));
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="Splitter"/>
		/// </summary>
		public new interface IHandler : Container.IHandler
		{
			/// <summary>
			/// Gets or sets the orientation of the panels in the splitter.
			/// </summary>
			/// <value>The orientation of the panels.</value>
			SplitterOrientation Orientation { get; set; }

			/// <summary>
			/// Gets or sets the panel with fixed size.
			/// </summary>
			/// <remarks>
			/// This specifies which panel will not change its size when the splitter's container is resized.
			/// If <see cref="SplitterFixedPanel.None"/>, both panels will resize.
			/// </remarks>
			/// <value>The fixed panel.</value>
			SplitterFixedPanel FixedPanel { get; set; }

			/// <summary>
			/// Gets or sets the position of the splitter from the left or top, in pixels.
			/// </summary>
			/// <value>The position of the splitter.</value>
			int Position { get; set; }

			/// <summary>
			/// Gets or sets the top or left panel of the splitter.
			/// </summary>
			/// <value>The first panel.</value>
			Control Panel1 { get; set; }

			/// <summary>
			/// Gets or sets the bottom or right panel of the splitter.
			/// </summary>
			/// <value>The second panel.</value>
			Control Panel2 { get; set; }
		}
	}
}
