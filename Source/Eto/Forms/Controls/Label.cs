using System;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public enum HorizontalAlign
	{
		Left,
		Center,
		Right
	}
	
	public enum VerticalAlign
	{
		Top,
		Middle,
		Bottom
	}
	
	public enum WrapMode
	{
		None,
		Word,
		Character
	}
	
	public interface ILabel : ITextControl
	{
		HorizontalAlign HorizontalAlign { get; set; }
		VerticalAlign VerticalAlign { get; set; }
		Font Font { get; set; }
		WrapMode Wrap { get; set; }
	}
	
	public class Label : TextControl
	{
		ILabel inner;
		
		public Label() : this(Generator.Current) { }
		
		public Label(Generator g) : base(g, typeof(ILabel))
		{
			inner = (ILabel)Handler;
		}
		
		public Font Font
		{
			get { return inner.Font; }
			set { inner.Font = value; }
		}
		
		public WrapMode Wrap
		{
			get { return inner.Wrap; }
			set { inner.Wrap = value; }
		}
		
		public HorizontalAlign HorizontalAlign
		{
			get { return inner.HorizontalAlign; }
			set { inner.HorizontalAlign = value; }
		}
		
		public VerticalAlign VerticalAlign
		{
			get { return inner.VerticalAlign; }
			set { inner.VerticalAlign = value; }
		}
	}
}
