
namespace Eto.Forms
{
	/// <summary>
	/// Enumeration of the types of separators for the <see cref="SeparatorToolBarItem"/>
	/// </summary>
	public enum SeparatorToolBarItemType
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
	
	public interface ISeparatorToolBarItem : IToolBarItem
	{
		SeparatorToolBarItemType Type { get; set; }
	}

	public class SeparatorToolBarItem : ToolBarItem
	{
		new ISeparatorToolBarItem Handler { get { return (ISeparatorToolBarItem)base.Handler; } }
		
		public SeparatorToolBarItem () : this(Generator.Current)
		{
		}

		public SeparatorToolBarItem (Generator g) : base(g, typeof(ISeparatorToolBarItem))
		{
		}
		
		public SeparatorToolBarItemType Type
		{
			get { return Handler.Type; }
			set { Handler.Type = value; }
		}
	}
}

