
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
	
	public interface ISeparatorToolItem : IToolItem
	{
		SeparatorToolItemType Type { get; set; }
	}

	public class SeparatorToolItem : ToolItem
	{
		new ISeparatorToolItem Handler { get { return (ISeparatorToolItem)base.Handler; } }
		
		public SeparatorToolItem()
			: this((Generator)null)
		{
		}

		public SeparatorToolItem (Generator generator) : base(generator, typeof(ISeparatorToolItem))
		{
		}
		
		public SeparatorToolItemType Type
		{
			get { return Handler.Type; }
			set { Handler.Type = value; }
		}
	}
}

