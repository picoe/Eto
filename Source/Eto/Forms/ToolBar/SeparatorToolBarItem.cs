using System;

namespace Eto.Forms
{
	public enum SeparatorToolBarItemType
	{
		Divider,
		Space,
		FlexibleSpace
	}
	
	public interface ISeparatorToolBarItem : IToolBarItem
	{
		SeparatorToolBarItemType Type { get; set; }
	}

	public class SeparatorToolBarItem : ToolBarItem
	{
		ISeparatorToolBarItem inner;
		
		public SeparatorToolBarItem () : this(Generator.Current)
		{
		}

		public SeparatorToolBarItem (Generator g) : base(g, typeof(ISeparatorToolBarItem))
		{
			inner = (ISeparatorToolBarItem)Handler;
		}
		
		public SeparatorToolBarItemType Type
		{
			get { return inner.Type; }
			set { inner.Type = value; }
		}
	}
}

