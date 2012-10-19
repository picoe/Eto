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
		ISplitter handler;
		
		public static bool Supported { get { return Generator.Current.Supports<ISplitter> (); } }
		
		public Splitter () : this (Generator.Current)
		{
		}
		
		public Splitter (Generator g) : this (g, typeof(ISplitter))
		{
		}
		
		protected Splitter (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (ISplitter)base.Handler;
		}

		public SplitterOrientation Orientation {
			get { return handler.Orientation; }
			set { handler.Orientation = value; }
		}
		
		public SplitterFixedPanel FixedPanel {
			get { return handler.FixedPanel; }
			set { handler.FixedPanel = value; }
		}
		
		public int Position {
			get { return handler.Position; }
			set { handler.Position = value; }
		}

		public Control Panel1 {
			get { return handler.Panel1; }
			set { 
				if (handler.Panel1 != null)
					handler.Panel1.SetParent (null);
				if (value != null) {
					value.SetParent (this);
					if (Loaded && !value.Loaded) {
						value.OnPreLoad (EventArgs.Empty);
						value.OnLoad (EventArgs.Empty);
					}
				}
				handler.Panel1 = value;
				if (Loaded && value != null && !value.Loaded)
					value.OnLoadComplete (EventArgs.Empty);
			}
		}

		public Control Panel2 {
			get { return handler.Panel2; }
			set {
				if (handler.Panel2 != null)
					handler.Panel2.SetParent (null);
				bool load = false;
				if (value != null) {
					value.SetParent (this);
					if (Loaded && !value.Loaded) {
						load = true;
						value.OnPreLoad (EventArgs.Empty);
						value.OnLoad (EventArgs.Empty);
					}
				}
				handler.Panel2 = value; 
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
