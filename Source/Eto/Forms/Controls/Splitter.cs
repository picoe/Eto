using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public enum SplitterOrientation
	{
		Horizontal,
		Vertical
	}

	public enum SplitterFixedPanel
	{
		Panel1,
		Panel2,
		None
	}

	public interface ISplitter : IContainer
	{
		SplitterOrientation Orientation { get; set; }

		SplitterFixedPanel FixedPanel { get; set; }

		int Position { get; set; }

		Control Panel1 { get; set; }

		Control Panel2 { get; set; }
	}

	public class Splitter : Container
	{
		new ISplitter Handler { get { return (ISplitter)base.Handler; } }

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
			get { return Platform.Instance.Supports<ISplitter>(); }
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

		public virtual void OnPositionChanged(EventArgs e)
		{
			Properties.TriggerEvent(PositionChangedEvent, this, e);
		}

		#endregion

		static Splitter()
		{
			EventLookup.Register<Splitter>(c => c.OnPositionChanged(null), Splitter.PositionChangedEvent);
		}

		public Splitter()
			: this((Generator)null)
		{
		}

		public Splitter(Generator generator) : this (generator, typeof(ISplitter))
		{
		}

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
					var load = SetParent(value);
					Handler.Panel1 = value;
					if (load)
						value.OnLoadComplete(EventArgs.Empty);
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
					var load = SetParent(value);
					Handler.Panel2 = value;
					if (load)
						value.OnLoadComplete(EventArgs.Empty);
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
	}
}
