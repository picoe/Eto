using System;

namespace Eto.Drawing
{
	public enum FontFamily
	{
		Monospace,
		Sans,
		Serif
	}
	
	public enum SystemFont
	{
		Default,
		Bold,
		Label
#if DESKTOP		
		,
		TitleBar,
		ToolTip,
		MenuBar,
		Menu,
		Message,
		Palette,
		StatusBar
#endif
	}
	
	[Flags]
	public enum FontStyle
	{
		Normal = 0,
		Bold = 1 << 0,
		Italic = 1 << 1
	}
	
	public interface IFont : IInstanceWidget
	{
		void Create(FontFamily family, float size, FontStyle style);
		void Create(SystemFont systemFont, float? size);
		bool Bold { get; }
		bool Italic { get; }
		float Size { get; }
	}
	
	public class Font : InstanceWidget
	{
		IFont inner;

		public Font(FontFamily family, float size, FontStyle style = FontStyle.Normal)
			: this(Generator.Current, family, size, style)
		{
		}

		public Font(Generator g, FontFamily family, float size, FontStyle style = FontStyle.Normal)
			: base(g, typeof(IFont))
		{
			inner = (IFont)Handler;
			inner.Create(family, size, style);
		}

		public Font(SystemFont systemFont, float? size = null)
			: this(Generator.Current, systemFont, size)
		{
		}

		public Font(Generator g, SystemFont systemFont, float? size = null)
			: base(g, typeof(IFont))
		{
			inner = (IFont)Handler;
			inner.Create(systemFont, size);
		}
		
		public float Size
		{
			get { return inner.Size; }
		}

		public bool Bold
		{
			get { return inner.Bold; }
		}

		public bool Italic
		{
			get { return inner.Italic; }
		}
	}
}
