using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Enumeration of the standard Font Families for a <see cref="Font"/>
	/// </summary>
	public enum FontFamily
	{
		/// <summary>
		/// Font family with each character having the same width
		/// </summary>
		Monospace,

		/// <summary>
		/// Font family with no serifs (e.g. Arial, Helvetica, etc)
		/// </summary>
		Sans,

		/// <summary>
		/// Font family with serifs (e.g. Times New Roman, etc)
		/// </summary>
		Serif
	}
	
	/// <summary>
	/// Enumeration of the different system fonts
	/// </summary>
	/// <remarks>
	/// This is useful when you want to use a font that is the same as standard UI elements.
	/// </remarks>
	public enum SystemFont
	{
		/// <summary>
		/// Default system font
		/// </summary>
		Default,

		/// <summary>
		/// Default system font in BOLD
		/// </summary>
		Bold,

		/// <summary>
		/// Default label font
		/// </summary>
		Label
#if DESKTOP
		,

		/// <summary>
		/// Default title bar font (window title)
		/// </summary>
		TitleBar,

		/// <summary>
		/// Default tool top font
		/// </summary>
		ToolTip,

		/// <summary>
		/// Default menu bar font
		/// </summary>
		MenuBar,

		/// <summary>
		/// Default font for items in a menu
		/// </summary>
		Menu,

		/// <summary>
		/// Default font for message boxes
		/// </summary>
		Message,

		/// <summary>
		/// Default font for palette dialogs
		/// </summary>
		Palette,

		/// <summary>
		/// Default font for status bars
		/// </summary>
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
