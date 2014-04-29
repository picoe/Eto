namespace Eto.Forms
{
	public partial interface IControl
	{
		/// <summary>
		/// Gets or sets the tool tip to show when the mouse is hovered over the control
		/// </summary>
		/// <value>The tool tip.</value>
		string ToolTip { get; set; }

		/// <summary>
		/// Gets or sets the type of cursor to use when the mouse is hovering over the control
		/// </summary>
		/// <value>The mouse cursor</value>
		Cursor Cursor { get; set; }
	}

	public partial class Control
	{
		/// <summary>
		/// Gets or sets the type of cursor to use when the mouse is hovering over the control
		/// </summary>
		/// <value>The mouse cursor</value>
		public virtual Cursor Cursor
		{
			get { return Handler.Cursor; }
			set { Handler.Cursor = value; }
		}

		/// <summary>
		/// Gets or sets the tool tip to show when the mouse is hovered over the control
		/// </summary>
		/// <value>The tool tip.</value>
		public virtual string ToolTip
		{
			get { return Handler.ToolTip; }
			set { Handler.ToolTip = value; }
		}
	}
}