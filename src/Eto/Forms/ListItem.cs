using System;
using Eto.Drawing;
using System.Globalization;

namespace Eto.Forms
{
	/// <summary>
	/// Interface for an item in a list control.
	/// </summary>
	/// <remarks>
	/// If you have a list of your own objects, it is more efficient to use them directly with the list control by 
	/// passing a collection to the DataStore property, and using TextBinding/KeyBinding to specify how to get/set the
	/// Text and Key properties.
	/// 
	/// Otherwise, use <see cref="ListItem"/> to define items.  This may be deprecated in the future.
	/// </remarks>
	public interface IListItem
	{
		/// <summary>
		/// Gets or sets the text of the item.
		/// </summary>
		/// <value>The text.</value>
		string Text { get; set; }

		/// <summary>
		/// Gets or sets the unique key of the item.
		/// </summary>
		/// <remarks>
		/// The key is typically used to identify each item uniquely.  If no key is specified, the <see cref="Text"/>
		/// is used as the key.
		/// </remarks>
		/// <value>The key of the item.</value>
		string Key { get; }
	}

	/// <summary>
	/// Represents an item for list controls.
	/// </summary>
	/// <remarks>
	/// If you have a list of your own objects, it is more efficient to use them directly with the list control by 
	/// passing a collection to the DataStore property, and use <see cref="ListControl.ItemTextBinding"/> and <see cref="ListControl.ItemKeyBinding"/> instead.
	/// </remarks>
	/// <seealso cref="ListItemCollection"/>
	[ContentProperty("Text")]
	public class ListItem : IListItem
	{
		string key;

		/// <summary>
		/// Gets or sets the text of the item.
		/// </summary>
		/// <value>The text.</value>
		public string Text { get; set; }

		/// <summary>
		/// Gets or sets the unique key of the item.
		/// </summary>
		/// <remarks>
		/// The key is typically used to identify each item uniquely.  If no key is specified, the <see cref="Text"/>
		/// is used as the key.
		/// </remarks>
		/// <value>The key of the item.</value>
		public string Key
		{
			get { return key ?? Text; }
			set { key = value; }
		}

		/// <summary>
		/// Gets or sets custom data for the item.
		/// </summary>
		/// <value>The custom data.</value>
		public object Tag { get; set; }

		/// <summary>
		/// Gets the listItem's string
		/// </summary>
		/// <returns>The string</returns>
		public override string ToString()
		{
			return Text;
		}

		/// <summary>>
		/// Converts a string to a list item implicitly
		/// </summary>
		/// <remarks>
		/// This is so you can initialize an array of ListItem objects by using string constant values.
		/// </remarks>
		/// <param name="text">Text to create the list item with</param>
		public static implicit operator ListItem(string text)
		{
			return new ListItem { Text = text };
		}
	}

	/// <summary>
	/// List item for list controls that accept an image (e.g. <see cref="ListBox"/>)
	/// </summary>
	/// <remarks>
	/// If you have a list of your own objects, it is more efficient to use them directly with the list control by 
	/// passing a collection to the DataStore property, and use <see cref="ListControl.ItemTextBinding"/>, <see cref="ListControl.ItemKeyBinding"/>,
	/// and <see cref="ListBox.ItemImageBinding"/>.
	/// </remarks>
	public class ImageListItem : ListItem, IImageListItem
	{
		/// <summary>
		/// Gets or sets the image for this item.
		/// </summary>
		/// <value>The item's image.</value>
		public Image Image { get; set; }
	}
}

