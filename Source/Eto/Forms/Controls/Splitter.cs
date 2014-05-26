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

	[Handler(typeof(Splitter.IHandler))]
	public class Splitter : Container
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

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

		public static bool IsSupported
		{
			get { return Platform.Instance.Supports<IHandler>(); }
		}

		#region Events

		public const string PositionChangedEvent = "Splitter.PositionChanged";

		/// <summary>
		/// Raised when the user moves the splitter.
		/// </summary>
		public event EventHandler<EventArgs> PositionChanged
		{
			add { Properties.AddHandlerEvent(PositionChangedEvent, value); }
			remove { Properties.RemoveEvent(PositionChangedEvent, value); }
		}

		protected virtual void OnPositionChanged(EventArgs e)
		{
			Properties.TriggerEvent(PositionChangedEvent, this, e);
		}

		#endregion

		static Splitter()
		{
			EventLookup.Register<Splitter>(c => c.OnPositionChanged(null), Splitter.PositionChangedEvent);
		}

		public Splitter()
		{
		}

		[Obsolete("Use default constructor instead")]
		public Splitter(Generator generator) : this (generator, typeof(IHandler))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Splitter(Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		public SplitterOrientation Orientation
		{
			get { return Handler.Orientation; }
			set { Handler.Orientation = value; }
		}

		public SplitterFixedPanel FixedPanel
		{
			get { return Handler.FixedPanel; }
			set { Handler.FixedPanel = value; }
		}

		public int Position
		{
			get { return Handler.Position; }
			set { Handler.Position = value; }
		}

		public Control Panel1
		{
			get { return Handler.Panel1; }
			set
			{ 
				if (Handler.Panel1 != null)
					RemoveParent(Handler.Panel1);
				if (value != null)
				{
					SetParent(value, () => Handler.Panel1 = value);
				}
				else
					Handler.Panel1 = value;
			}
		}

		public Control Panel2
		{
			get { return Handler.Panel2; }
			set
			{
				if (Handler.Panel2 != null)
					RemoveParent(Handler.Panel2);
				if (value != null)
				{
					SetParent(value, () => Handler.Panel2 = value);
				}
				else
					Handler.Panel2 = value;
			}
		}

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

		public new interface ICallback : Container.ICallback
		{
			void OnPositionChanged(Splitter widget, EventArgs e);
		}

		protected new class Callback : Container.Callback, ICallback
		{
			public void OnPositionChanged(Splitter widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnPositionChanged(e));
			}
		}

		public new interface IHandler : Container.IHandler
		{
			SplitterOrientation Orientation { get; set; }

			SplitterFixedPanel FixedPanel { get; set; }

			int Position { get; set; }

			Control Panel1 { get; set; }

			Control Panel2 { get; set; }
		}
	}
}
