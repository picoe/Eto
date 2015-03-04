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

	/// <summary>
	/// Tool item to separate groups of items using a divider, space, etc.
	/// </summary>
	[Handler(typeof(SeparatorToolItem.IHandler))]
	public class SeparatorToolItem : ToolItem
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets or sets the type of the separator.
		/// </summary>
		/// <value>The separator type.</value>
		public SeparatorToolItemType Type
		{
			get { return Handler.Type; }
			set { Handler.Type = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="SeparatorToolItem"/>.
		/// </summary>
		public new interface IHandler : ToolItem.IHandler
		{
			/// <summary>
			/// Gets or sets the type of the separator.
			/// </summary>
			/// <value>The separator type.</value>
			SeparatorToolItemType Type { get; set; }
		}
	}
}

