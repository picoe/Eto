using System;
using Eto.Drawing;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Specifies the text alignment for a <see cref="Label"/>
	/// </summary>
	public enum TextAlignment
	{
		/// <summary>
		/// Text will be aligned to the left
		/// </summary>
		Left,
		/// <summary>
		/// Text will be aligned in the center of the label
		/// </summary>
		Center,
		/// <summary>
		/// Text will be aligned to the right
		/// </summary>
		Right
	}

	/// <summary>
	/// Specifies the wrapping mode for the text of a <see cref="Label"/>
	/// </summary>
	/// <remarks>
	/// Regardless of the mode, you can always add hard wraps by inserting newline characters.
	/// </remarks>
	public enum WrapMode
	{
		/// <summary>
		/// No wrapping, the text will clip when smaller than the required space for the text.
		/// </summary>
		None,
		/// <summary>
		/// Text will wrap by word to fit the horizontal space available
		/// </summary>
		Word,
		/// <summary>
		/// Text will wrap by character to fit the horizontal space available
		/// </summary>
		Character
	}

	/// <summary>
	/// Displays a string of text on the form
	/// </summary>
	[Handler(typeof(Label.IHandler))]
	public class Label : TextControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets or sets the wrap mode for the text
		/// </summary>
		/// <remarks>
		/// This defines the soft wrapping for the label's text. 
		/// Hard wraps can be placed in the text with newline characters.
		/// 
		/// Wrapping will only occur if the label's width is smaller than the space needed for the text.
		/// If you are autosizing your control, it may get autosized to the width so you will have to add constraints
		/// to the container or explicitly set the size.
		/// </remarks>
		/// <value>The wrapping mode for the text.</value>
		[DefaultValue(WrapMode.Word)]
		public WrapMode Wrap
		{
			get { return Handler.Wrap; }
			set { Handler.Wrap = value; }
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of the text.
		/// </summary>
		/// <remarks>
		/// When auto sizing the label, this won't have an affect unless the label's container is larger than the text.
		/// </remarks>
		/// <value>The horizontal alignment.</value>
		public TextAlignment TextAlignment
		{
			get { return Handler.TextAlignment; }
			set { Handler.TextAlignment = value; }
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of the text.
		/// </summary>
		/// <remarks>
		/// When auto sizing the label, this won't have an affect unless the label's container is larger than the text.
		/// </remarks>
		/// <value>The horizontal alignment.</value>
		[Obsolete("Since 2.1: Use TextAlignment instead")]
		public HorizontalAlign HorizontalAlign
		{
			get { return Handler.TextAlignment; }
			set { Handler.TextAlignment = value; }
		}

		/// <summary>
		/// Gets or sets the vertical alignment of the text.
		/// </summary>
		/// <remarks>
		/// When auto sizing the label, this won't have an affect unless the label's container is larger than the text.
		/// </remarks>
		/// <value>The vertical alignment.</value>
		public VerticalAlignment VerticalAlignment
		{
			get { return Handler.VerticalAlignment; }
			set { Handler.VerticalAlignment = value; }
		}

		/// <summary>
		/// Gets or sets the vertical alignment of the text.
		/// </summary>
		/// <remarks>
		/// When auto sizing the label, this won't have an affect unless the label's container is larger than the text.
		/// </remarks>
		/// <value>The vertical alignment.</value>
		[Obsolete("Since 2.1: Use VerticalAlignment instead")]
		public VerticalAlign VerticalAlign
		{
			get { return Handler.VerticalAlignment; }
			set { Handler.VerticalAlignment = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="Label"/>
		/// </summary>
		public new interface IHandler : TextControl.IHandler
		{
			/// <summary>
			/// Gets or sets the horizontal alignment of the text.
			/// </summary>
			/// <remarks>
			/// When auto sizing the label, this won't have an affect unless the label's container is larger than the text.
			/// </remarks>
			/// <value>The horizontal alignment.</value>
			TextAlignment TextAlignment { get; set; }

			/// <summary>
			/// Gets or sets the vertical alignment of the text.
			/// </summary>
			/// <remarks>
			/// When auto sizing the label, this won't have an affect unless the label's container is larger than the text.
			/// </remarks>
			/// <value>The vertical alignment.</value>
			VerticalAlignment VerticalAlignment { get; set; }

			/// <summary>
			/// Gets or sets the wrap mode for the text
			/// </summary>
			/// <remarks>
			/// This defines the soft wrapping for the label's text. 
			/// Hard wraps can be placed in the text with newline characters.
			/// 
			/// Wrapping will only occur if the label's width is smaller than the space needed for the text.
			/// If you are autosizing your control, it may get autosized to the width so you will have to add constraints
			/// to the container or explicitly set the size.
			/// </remarks>
			/// <value>The wrapping mode for the text.</value>
			WrapMode Wrap { get; set; }
		}
	}

	#region Obsolete

	/// <summary>
	/// Specifies the horizontal alignment for a <see cref="Label"/>
	/// </summary>
	[Obsolete("Since 2.1: Use TextAlignment instead")]
	public struct HorizontalAlign
	{
		readonly TextAlignment value;

		HorizontalAlign(TextAlignment value)
		{
			this.value = value;
		}

		/// <summary>
		/// Text will be aligned in the center of the label
		/// </summary>
		public static HorizontalAlign Center { get { return TextAlignment.Center; } }

		/// <summary>
		/// Text will be aligned to the left
		/// </summary>
		public static HorizontalAlign Left { get { return TextAlignment.Left; } }

		/// <summary>
		/// Text will be aligned to the right
		/// </summary>
		public static HorizontalAlign Right { get { return TextAlignment.Right; } }


		/// <summary>Converts to a TextAlignment</summary>
		public static implicit operator TextAlignment(HorizontalAlign value)
		{
			return value.value;
		}

		/// <summary>Converts an TextAlignment to a HorizontalAlign</summary>
		public static implicit operator HorizontalAlign(TextAlignment value)
		{
			return new HorizontalAlign(value);
		}

		/// <summary>Compares for equality</summary>
		/// <param name="value1">Value1.</param>
		/// <param name="value2">Value2.</param>
		public static bool operator ==(TextAlignment value1, HorizontalAlign value2)
		{
			return value1 == value2.value;
		}

		/// <summary>Compares for inequality</summary>
		/// <param name="value1">Value1.</param>
		/// <param name="value2">Value2.</param>
		public static bool operator !=(TextAlignment value1, HorizontalAlign value2)
		{
			return value1 != value2.value;
		}

		/// <summary>Convert from string to vertical align (for json/xaml compat)</summary>
		/// <param name="value">Value.</param>
		public static implicit operator HorizontalAlign(string value)
		{
			switch (value.ToLowerInvariant())
			{
				case "Center":
					return HorizontalAlign.Center;
				case "Bottom":
					return HorizontalAlign.Right;
				default:
				case "Top":
					return HorizontalAlign.Left;
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Eto.Forms.HorizontalAlign"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Eto.Forms.HorizontalAlign"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="Eto.Forms.HorizontalAlign"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return (obj is HorizontalAlign && (this == (HorizontalAlign)obj))
				|| (obj is TextAlignment && (this == (TextAlignment)obj));
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="Eto.Forms.HorizontalAlign"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return value.GetHashCode();
		}
	}

	/// <summary>
	/// Specifies the horizontal alignment for a <see cref="Label"/>
	/// </summary>
	[Obsolete("Since 2.1: Use VerticalAlignment instead")]
	public struct VerticalAlign
	{
		readonly VerticalAlignment value;

		VerticalAlign(VerticalAlignment value)
		{
			this.value = value;
		}

		/// <summary>
		/// Text will be aligned to the middle of the label
		/// </summary>
		public static VerticalAlign Middle { get { return VerticalAlignment.Center; } }

		/// <summary>
		/// Text will be aligned to the top of the label
		/// </summary>
		public static VerticalAlign Top { get { return VerticalAlignment.Top; } }

		/// <summary>
		/// Text will be aligned to the bottom of the label
		/// </summary>
		public static VerticalAlign Bottom { get { return VerticalAlignment.Bottom; } }

		/// <summary>Converts to an VerticalAlignment</summary>
		public static implicit operator VerticalAlignment(VerticalAlign value)
		{
			return value.value;
		}

		/// <summary>Converts an VerticalAlignment to a VerticalAlign</summary>
		public static implicit operator VerticalAlign(VerticalAlignment value)
		{
			return new VerticalAlign(value);
		}

		/// <summary>Compares for equality</summary>
		/// <param name="value1">Value1.</param>
		/// <param name="value2">Value2.</param>
		public static bool operator ==(VerticalAlignment value1, VerticalAlign value2)
		{
			return value1 == value2.value;
		}

		/// <summary>Compares for inequality</summary>
		/// <param name="value1">Value1.</param>
		/// <param name="value2">Value2.</param>
		public static bool operator !=(VerticalAlignment value1, VerticalAlign value2)
		{
			return value1 != value2.value;
		}

		/// <summary>Convert from string to vertical align (for json/xaml compat)</summary>
		/// <param name="value">Value.</param>
		public static implicit operator VerticalAlign(string value)
		{
			switch (value.ToLowerInvariant())
			{
				case "Middle":
					return VerticalAlign.Middle;
				case "Bottom":
					return VerticalAlign.Bottom;
				default:
				case "Top":
					return VerticalAlign.Top;
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Eto.Forms.VerticalAlign"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Eto.Forms.VerticalAlign"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="Eto.Forms.VerticalAlign"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return (obj is VerticalAlign && (this == (VerticalAlign)obj))
				|| (obj is VerticalAlignment && (this == (VerticalAlignment)obj));
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="Eto.Forms.VerticalAlign"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return value.GetHashCode();
		}
	}

	#endregion
}
