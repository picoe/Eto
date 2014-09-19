
namespace Eto.Forms
{
	/// <summary>
	/// Item for panes on a <see cref="Navigation"/> control.
	/// </summary>
	/// <seealso cref="NavigationItem"/>
	public interface INavigationItem : IListItem
	{
		/// <summary>
		/// Gets the content for the pane.
		/// </summary>
		/// <value>The pane's content.</value>
		Control Content { get; }
	}

	/// <summary>
	/// Item for a pane on a <see cref="Navigation"/> control.
	/// </summary>
	/// <remarks>
	/// This defines an item on a <see cref="Navigation"/> control that contains the item's content and title.
	/// </remarks>
	public class NavigationItem : ListItem, INavigationItem
	{
		/// <summary>
		/// Gets the content for the pane.
		/// </summary>
		/// <value>The pane's content.</value>
		public Control Content { get; set; }
	}
}

