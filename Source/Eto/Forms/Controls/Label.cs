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
		WrapMode Wrap { get; set; }
		Color TextColor { get; set; }
	}
	
	public class Label : TextControl
	{
		new ILabel Handler { get { return (ILabel)base.Handler; } }
		
		public Label() : this(Generator.Current) { }
		
		public Label(Generator g) : this (g, typeof(ILabel))
		{
		}
		
		protected Label (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}
		
		public WrapMode Wrap
		{
			get { return Handler.Wrap; }
			set { Handler.Wrap = value; }
		}
		
		public Color TextColor
		{
			get { return Handler.TextColor; }
			set { Handler.TextColor = value; }
		}
		
		public HorizontalAlign HorizontalAlign
		{
			get { return Handler.HorizontalAlign; }
			set { Handler.HorizontalAlign = value; }
		}
		
		public VerticalAlign VerticalAlign
		{
			get { return Handler.VerticalAlign; }
			set { Handler.VerticalAlign = value; }
		}
	}
}
