using System;
using Eto.Drawing;
#if DESKTOP
using System.Windows.Markup;
#endif

namespace Eto.Forms
{
	public interface IListItem
	{
		string Text { get; }

		string Key { get; }
	}
	
#if DESKTOP
	[ContentProperty("Text")]
#endif
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
	
#if DESKTOP
	[ContentProperty("Item")]
#endif
	public class ObjectListItem : IListItem
	{
		public object Item { get; set; }
		
		public virtual string Text {
			get { return Convert.ToString (Item); }
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

