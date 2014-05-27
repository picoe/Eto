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
	/// Otherwise, use <see cref="ListItem"/> to define items.  This may be depricated in the future.
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
	/// passing a collection to the DataStore property, and use <see cref="ListControl.TextBinding"/> and <see cref="ListControl.KeyBinding"/> instead.
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
	}

	/// <summary>
	/// List item for list controls that accept an image (e.g. <see cref="ListBox"/>)
	/// </summary>
	/// <remarks>
	/// If you have a list of your own objects, it is more efficient to use them directly with the list control by 
	/// passing a collection to the DataStore property, and use <see cref="ListControl.TextBinding"/>, <see cref="ListControl.KeyBinding"/>,
	/// and <see cref="ListBox.ImageBinding"/>.
	/// </remarks>
	public class ImageListItem : ListItem, IImageListItem
	{
		/// <summary>
		/// Gets or sets the image for this item.
		/// </summary>
		/// <value>The item's image.</value>
		public Image Image { get; set; }
	}

	/// <summary>
	/// Translates an object to text using Convert.ToString for use in list controls.
	/// </summary>
	[Obsolete("Use your objects directly and use the TextBinding/KeyBinding of the list control")]
	[ContentProperty("Item")]
	public class ObjectListItem : IListItem
	{
		/// <summary>
		/// Gets or sets the object this item represents.
		/// </summary>
		/// <value>The item.</value>
		public object Item { get; set; }

		/// <summary>
		/// Gets or sets the text of the item.
		/// </summary>
		/// <value>The text.</value>
		public virtual string Text
		{
			get { return Convert.ToString(Item, CultureInfo.CurrentCulture); }
			set { }
		}

		/// <summary>
		/// Gets or sets the unique key of the item.
		/// </summary>
		/// <value>The key.</value>
		public virtual string Key
		{
			get { return Text; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ObjectListItem"/> class.
		/// </summary>
		public ObjectListItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ObjectListItem"/> class with the specified item.
		/// </summary>
		/// <param name="item">Item to get the text for this list item.</param>
		public ObjectListItem(object item)
		{
			this.Item = item;
		}
	}
}

