using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Interface elment to group controls inside a box with an optional title
	/// </summary>
	[Handler(typeof(GroupBox.IHandler))]
	public class GroupBox : Panel
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets or sets the font used for the title
		/// </summary>
		/// <value>The title font.</value>
		public Font Font
		{
			get { return Handler.Font; }
			set { Handler.Font = value; }
		}

		/// <summary>
		/// Gets or sets the title text.
		/// </summary>
		/// <value>The title text.</value>
		public string Text
		{
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

		/// <summary>
		/// Gets or sets the color of the text.
		/// </summary>
		/// <remarks>
		/// By default, the text will get a color based on the user's theme. However, this is usually black.
		/// </remarks>
		/// <value>The color of the text.</value>
		public Color TextColor
		{
			get { return Handler.TextColor; }
			set { Handler.TextColor = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="GroupBox"/>
		/// </summary>
		public new interface IHandler : Panel.IHandler
		{
			/// <summary>
			/// Gets or sets the font used for the title
			/// </summary>
			/// <value>The title font.</value>
			Font Font { get; set; }

			/// <summary>
			/// Gets or sets the title text.
			/// </summary>
			/// <value>The title text.</value>
			string Text { get; set; }

			/// <summary>
			/// Gets or sets the color of the text.
			/// </summary>
			/// <remarks>
			/// By default, the text will get a color based on the user's theme. However, this is usually black.
			/// </remarks>
			/// <value>The color of the text.</value>
			Color TextColor { get; set; }
		}
	}
}
