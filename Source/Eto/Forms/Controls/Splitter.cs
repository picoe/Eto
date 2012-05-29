using System;
using System.Collections;

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

	public interface ISplitter : IControl
	{
		SplitterOrientation Orientation { get; set; }
		
		SplitterFixedPanel FixedPanel { get; set; }

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
		
		public SplitterFixedPanel FixedPanel {
			get { return inner.FixedPanel; }
			set { inner.FixedPanel = value; }
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
					if (Loaded && !value.Loaded) {
						value.OnPreLoad (EventArgs.Empty);
						value.OnLoad (EventArgs.Empty);
					}
				}
				inner.Panel1 = value;
				if (Loaded && value != null && !value.Loaded)
					value.OnLoadComplete (EventArgs.Empty);
			}
		}

		public Control Panel2 {
			get { return inner.Panel2; }
			set {
				if (inner.Panel2 != null)
					inner.Panel2.SetParent (null);
				bool load = false;
				if (value != null) {
					value.SetParent (this);
					if (Loaded && !value.Loaded) {
						load = true;
						value.OnPreLoad (EventArgs.Empty);
						value.OnLoad (EventArgs.Empty);
					}
				}
				inner.Panel2 = value; 
				if (load)
					value.OnLoadComplete (EventArgs.Empty);
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
