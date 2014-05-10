namespace Eto.Forms
{
	/// <summary>
	/// Control item for the <see cref="DynamicLayout"/>
	/// </summary>
	[ContentProperty("Control")]
	public class DynamicControl : DynamicItem
	{
		/// <summary>
		/// Creates the content for this item
		/// </summary>
		/// <param name="layout">Top level layout the item is being created for</param>
		public override Control Create (DynamicLayout layout)
		{
			return Control;
		}

		/// <summary>
		/// Gets or sets the control that this item contains
		/// </summary>
		/// <value>The control.</value>
		public Control Control { get; set; }
	}
}
