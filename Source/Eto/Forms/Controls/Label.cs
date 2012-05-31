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
		ILabel handler;
		
		public Label() : this(Generator.Current) { }
		
		public Label(Generator g) : this (g, typeof(ILabel))
		{
		}
		
		protected Label (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (ILabel)Handler;
		}
		
		public WrapMode Wrap
		{
			get { return handler.Wrap; }
			set { handler.Wrap = value; }
		}
		
		public Color TextColor
		{
			get { return handler.TextColor; }
			set { handler.TextColor = value; }
		}
		
		public HorizontalAlign HorizontalAlign
		{
			get { return handler.HorizontalAlign; }
			set { handler.HorizontalAlign = value; }
		}
		
		public VerticalAlign VerticalAlign
		{
			get { return handler.VerticalAlign; }
			set { handler.VerticalAlign = value; }
		}
	}
}
