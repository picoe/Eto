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
		
		public static bool Supported
		{
			get { return Generator.Current.Supports<ISplitter>(); }
		}
		
		public Splitter() : this(Generator.Current)
		{
		}
		
		public Splitter(Generator g) : base(g, typeof(ISplitter))
		{
			inner = (ISplitter)base.Handler;
		}

		public SplitterOrientation Orientation
		{
			get { return inner.Orientation; }
			set { inner.Orientation = value; }
		}
		
		public int Position
		{
			get { return inner.Position; }
			set { inner.Position = value; }
		}

		public Control Panel1
		{
			get { return inner.Panel1; }
			set { 
				if (inner.Panel1 != null) inner.Panel1.SetParent(this);
				inner.Panel1 = value;
				if (inner.Panel1 != null) inner.Panel1.SetParent(this);
			}
		}

		public Control Panel2
		{
			get { return inner.Panel2; }
			set { 
				if (inner.Panel2 != null) inner.Panel2.SetParent(this);
				inner.Panel2 = value; 
				if (inner.Panel2 != null) inner.Panel2.SetParent(this);
			}
		}


		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			if (Panel1 != null) Panel1.OnLoad(e);
			if (Panel2 != null) Panel2.OnLoad(e);
		}

		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			if (Panel1 != null) Panel1.OnLoadComplete (e);
			if (Panel2 != null) Panel2.OnLoadComplete (e);
		}
		
	}
}
