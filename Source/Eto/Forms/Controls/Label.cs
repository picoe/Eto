using System;
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
	
	[Handler(typeof(Label.IHandler))]
	public class Label : TextControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }
		
		public Label()
		{
		}

		[Obsolete("Use default constructor instead")]
		public Label(Generator generator) : this (generator, typeof(IHandler))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
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

		public new interface IHandler : TextControl.IHandler
		{
			HorizontalAlign HorizontalAlign { get; set; }
			VerticalAlign VerticalAlign { get; set; }
			WrapMode Wrap { get; set; }
			Color TextColor { get; set; }
		}
	}
}
