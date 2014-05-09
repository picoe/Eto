using System;


namespace Eto.Forms
{
	/// <summary>
	/// Enumeration of the types of separators for the <see cref="SeparatorToolItem"/>
	/// </summary>
	public enum SeparatorToolItemType
	{
		/// <summary>
		/// Line divider
		/// </summary>
		Divider,

		/// <summary>
		/// Fixed space divider
		/// </summary>
		Space,

		/// <summary>
		/// Flexible space divider (not available on all platforms)
		/// </summary>
		/// <remarks>
		/// This is (currently) only available for OS X applications as
		/// other platforms left-align their toolbars.
		/// </remarks>
		FlexibleSpace
	}
	
	[Handler(typeof(SeparatorToolItem.IHandler))]
	public class SeparatorToolItem : ToolItem
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }
		
		public SeparatorToolItem()
		{
		}

		[Obsolete("Use default constructor instead")]
		public SeparatorToolItem (Generator generator) : base(generator, typeof(IHandler))
		{
		}
		
		public SeparatorToolItemType Type
		{
			get { return Handler.Type; }
			set { Handler.Type = value; }
		}

		public interface IHandler : ToolItem.IHandler
		{
			SeparatorToolItemType Type { get; set; }
		}
	}
}

