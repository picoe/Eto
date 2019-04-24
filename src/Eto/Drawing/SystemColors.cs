
namespace Eto.Drawing
{
	/// <summary>
	/// Methods to get colors of system elements
	/// </summary>
	/// <copyright>(c) 2015 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(IHandler))]
	public static class SystemColors
	{
		static IHandler Handler => Platform.Instance.CreateShared<IHandler>();

		/// <summary>
		/// Gets the color of disabled text.
		/// </summary>
		/// <value>The color of disabled text.</value>
		public static Color DisabledText => Handler.DisabledText;

		/// <summary>
		/// Gets the color of text in a control, such as a TextBox or GridView.
		/// </summary>
		/// <value>The color of control text.</value>
		public static Color ControlText => Handler.ControlText;

		/// <summary>
		/// Gets the color of highlighted text in controls such as a TextBox or GridView.
		/// </summary>
		/// <value>The color of highlighted text.</value>
		public static Color HighlightText => Handler.HighlightText;

		/// <summary>
		/// Gets the color of a control.
		/// </summary>
		/// <value>The control color.</value>
		public static Color Control => Handler.Control;

		/// <summary>
		/// Gets the color of a control's background, such as the entry area of a TextBox.
		/// </summary>
		/// <value>The control background color.</value>
		public static Color ControlBackground => Handler.ControlBackground;

		/// <summary>
		/// Gets the highlight color.
		/// </summary>
		/// <value>The highlight.</value>
		public static Color Highlight => Handler.Highlight;

		/// <summary>
		/// Gets the color of a window background.
		/// </summary>
		/// <value>The window background.</value>
		public static Color WindowBackground => Handler.WindowBackground;

		/// <summary>
		/// Gets the color of selected text
		/// </summary>
		/// <value>The selection text c.</value>
		public static Color SelectionText => Handler.SelectionText;

		/// <summary>
		/// Gets the background color of selected text
		/// </summary>
		/// <value>The selection background color.</value>
		public static Color Selection => Handler.Selection;

		/// <summary>
		/// Gets the color of hyperlink text
		/// </summary>
		/// <value>The hyperlink text color.</value>
		public static Color LinkText => Handler.LinkText;

		/// <summary>
		/// Handler interface for <see cref="SystemColors"/>
		/// </summary>
		public interface IHandler
		{
			/// <summary>
			/// Gets the color of disabled text.
			/// </summary>
			/// <value>The color of disabled text.</value>
			Color DisabledText { get; }

			/// <summary>
			/// Gets the color of text in a control, such as a TextBox or GridView.
			/// </summary>
			/// <value>The color of control text.</value>
			Color ControlText { get; }

			/// <summary>
			/// Gets the color of highlighted text in controls such as a TextBox or GridView.
			/// </summary>
			/// <value>The color of highlighted text.</value>
			Color HighlightText { get; }

			/// <summary>
			/// Gets the color of a control.
			/// </summary>
			/// <value>The control color.</value>
			Color Control { get; }

			/// <summary>
			/// Gets the color of a control's background, such as the entry area of a TextBox.
			/// </summary>
			/// <value>The control background color.</value>
			Color ControlBackground { get; }

			/// <summary>
			/// Gets the highlight color.
			/// </summary>
			/// <value>The highlight.</value>
			Color Highlight { get; }

			/// <summary>
			/// Gets the color of a window background.
			/// </summary>
			/// <value>The window background.</value>
			Color WindowBackground { get; }

			/// <summary>
			/// Gets the color of selected text
			/// </summary>
			/// <value>The selection text c.</value>
			Color SelectionText { get; }

			/// <summary>
			/// Gets the background color of selected text
			/// </summary>
			/// <value>The selection background color.</value>
			Color Selection { get; }

			/// <summary>
			/// Gets the color of hyperlink text
			/// </summary>
			/// <value>The hyperlink text color.</value>
			Color LinkText { get; }
		}
	}
}

