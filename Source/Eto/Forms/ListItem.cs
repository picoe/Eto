using System;
using Eto.Drawing;
using System.Globalization;

namespace Eto.Forms
{
	public interface IListItem
	{
		string Text { get; set; }

		string Key { get; }
	}
	
	[ContentProperty("Text")]
	public class ListItem : IListItem
	{
		string key;
		
		public string Text { get; set; }

		public string Key {
			get { return key ?? Text; }
			set { key = value; }
		}
		
		public object Tag { get; set; }
	}
	
	public class ImageListItem : ListItem, IImageListItem
	{
		public Image Image { get; set; }
	}
	
	[ContentProperty("Item")]
	public class ObjectListItem : IListItem
	{
		public object Item { get; set; }
		
		public virtual string Text
		{
			get { return Convert.ToString(Item, CultureInfo.CurrentCulture); }
			set { }
		}
		
		public virtual string Key {
			get { return Text; }
		}
		
		public ObjectListItem ()
		{
		}
		
		public ObjectListItem (object item)
		{
			this.Item = item;
		}
	}
}

