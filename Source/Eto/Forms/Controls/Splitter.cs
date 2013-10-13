using System;
using System.Collections;
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

		[Obsolete("Use IsSupported() instead")]
		public static bool Supported { get { return IsSupported(); } }

		public static bool IsSupported(Generator generator = null)
		{
			return (generator ?? Generator.Current).Supports<ISplitter>();
		}

		#region Events

		public const string PositionChangedEvent = "Control.PositionChanged";
		EventHandler<EventArgs> positionChanged;

		/// <summary>
		/// Raised when the user moves the splitter.
		/// </summary>
		public event EventHandler<EventArgs> PositionChanged
		{
			add
			{
				HandleEvent(PositionChangedEvent);
				positionChanged += value;
			}
			remove { positionChanged -= value; }
		}

		public virtual void OnPositionChanged(EventArgs e)
		{
			if (positionChanged != null)
				positionChanged(this, e);
		}

		#endregion

		public Splitter() : this (Generator.Current)
		{
		}

		public Splitter(Generator g) : this (g, typeof(ISplitter))
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
					RemoveParent(Handler.Panel1, true);
				if (value != null)
				{
					SetParent(value);
					if (Loaded && !value.Loaded)
					{
						value.OnPreLoad(EventArgs.Empty);
						value.OnLoad(EventArgs.Empty);
					}
				}
				Handler.Panel1 = value;
				if (Loaded && value != null && !value.Loaded)
					value.OnLoadComplete(EventArgs.Empty);
			}
		}

		public Control Panel2
		{
			get { return Handler.Panel2; }
			set
			{
				if (Handler.Panel2 != null)
					RemoveParent(Handler.Panel2, true);
				bool load = false;
				if (value != null)
				{
					SetParent(value);
					if (Loaded && !value.Loaded)
					{
						load = true;
						value.OnPreLoad(EventArgs.Empty);
						value.OnLoad(EventArgs.Empty);
					}
				}
				Handler.Panel2 = value; 
				if (load)
					value.OnLoadComplete(EventArgs.Empty);
			}
		}

		public override void Remove(Control child)
		{
			if (object.ReferenceEquals(Panel1, child))
			{
				Panel1 = null;
				RemoveParent(Panel1, true);
			}
			else if (object.ReferenceEquals(Panel2, child))
			{
				Panel2 = null;
				RemoveParent(Panel2, true);
			}
		}
	}
}
