using System;
using System.Collections;

namespace Eto.Forms
{
	public enum SplitterOrientation
	{
		Horizontal,
		Vertical
	}

	public interface ISplitter : IControl
	{
		SplitterOrientation Orientation { get; set; }

		int Position { get; set; }

		Control Panel1 { get; set; }

		Control Panel2 { get; set; }
	}
	
	public class Splitter : Control
	{
		ISplitter inner;
		
		public static bool Supported {
			get { return Generator.Current.Supports<ISplitter> (); }
		}
		
		public Splitter () : this(Generator.Current)
		{
		}
		
		public Splitter (Generator g) : base(g, typeof(ISplitter))
		{
			inner = (ISplitter)base.Handler;
		}

		public SplitterOrientation Orientation {
			get { return inner.Orientation; }
			set { inner.Orientation = value; }
		}
		
		public int Position {
			get { return inner.Position; }
			set { inner.Position = value; }
		}

		public Control Panel1 {
			get { return inner.Panel1; }
			set { 
				if (inner.Panel1 != null)
					inner.Panel1.SetParent (null);
				if (value != null) {
					value.SetParent (this);
					if (Loaded) {
						value.OnPreLoad (EventArgs.Empty);
						value.OnLoad (EventArgs.Empty);
						value.OnLoadComplete (EventArgs.Empty);
					}
				}
				inner.Panel1 = value;
			}
		}

		public Control Panel2 {
			get { return inner.Panel2; }
			set { 
				if (inner.Panel2 != null)
					inner.Panel2.SetParent (null);
				if (value != null) {
					value.SetParent (this);
					if (Loaded) {
						value.OnPreLoad (EventArgs.Empty);
						value.OnLoad (EventArgs.Empty);
						value.OnLoadComplete (EventArgs.Empty);
					}
				}
				inner.Panel2 = value; 
			}
		}
		
		public override void OnPreLoad (EventArgs e)
		{
			if (Panel1 != null)
				Panel1.OnPreLoad (e);
			if (Panel2 != null)
				Panel2.OnPreLoad (e);
			base.OnPreLoad (e);
		}

		public override void OnLoad (EventArgs e)
		{
			if (Panel1 != null)
				Panel1.OnLoad (e);
			if (Panel2 != null)
				Panel2.OnLoad (e);
			base.OnLoad (e);
		}

		public override void OnLoadComplete (EventArgs e)
		{
			if (Panel1 != null)
				Panel1.OnLoadComplete (e);
			if (Panel2 != null)
				Panel2.OnLoadComplete (e);
			base.OnLoadComplete (e);
		}
		
	}
}
