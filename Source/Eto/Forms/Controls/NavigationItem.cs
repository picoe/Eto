using System;

namespace Eto.Forms
{
	public interface INavigationItem : IListItem
	{
		Control Content { get; }
	}
	
	public class NavigationItem : ListItem, INavigationItem
	{
		
		public Control Content { get; set; }
	}
}

