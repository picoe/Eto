using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Represents a list item that can have an image associated with each item.
	/// </summary>
	/// <remarks>
	/// Not all controls that use the <see cref="IListItem"/> can support images, for example the <see cref="DropDown"/>.
	/// </remarks>
	public interface IImageListItem : IListItem
	{
		/// <summary>
		/// Gets the image for this item.
		/// </summary>
		/// <value>The item's image.</value>
		Image Image { get; }
	}

	class ImageListItemImageBinding : PropertyBinding<Image>
	{
		public ImageListItemImageBinding()
			: base("Image")
		{
		}

		protected override Image InternalGetValue(object dataItem)
		{
			var item = dataItem as IImageListItem;
			return item != null ? item.Image : base.InternalGetValue(dataItem);
		}
	}

	/// <summary>
	/// Control to show a list of items that the user can select
	/// </summary>
	[Handler(typeof(ListBox.IHandler))]
	public class ListBox : ListControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets or sets the binding for the Image of each item
		/// </summary>
		/// <remarks>
		/// By default will be an public Image property on your object
		/// </remarks>
		/// <value>The image binding.</value>
		public IIndirectBinding<Image> ItemImageBinding { get; set; }

		/// <summary>
		/// Gets or sets the binding for the Image of each item
		/// </summary>
		/// <remarks>
		/// By default will be an public Image property on your object
		/// </remarks>
		/// <value>The image binding.</value>
		[Obsolete("Since 2.1: Use ItemImageBinding instead")]
		public IIndirectBinding<Image> ImageBinding {
			get { return ItemImageBinding; }
			set { ItemImageBinding = value; }
		}

		/// <summary>
		/// Occurs when an item is activated, usually with a double click or by pressing enter.
		/// </summary>
		public event EventHandler<EventArgs> Activated;

		/// <summary>
		/// Raises the <see cref="Activated"/>event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnActivated(EventArgs e)
		{
			if (Activated != null)
				Activated(this, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ListBox"/> class.
		/// </summary>
		public ListBox()
		{
			ItemImageBinding = new ImageListItemImageBinding();
		}

		/// <summary>
		/// Gets or sets the context menu for the control.
		/// </summary>
		/// <value>The context menu.</value>
		public ContextMenu ContextMenu
		{
			get { return Handler.ContextMenu; }
			set { Handler.ContextMenu = value; }
		}

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for the <see cref="ListBox"/>
		/// </summary>
		public new interface ICallback : ListControl.ICallback
		{
			/// <summary>
			/// Raises the activated event.
			/// </summary>
			void OnActivated(ListBox widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of <see cref="ListBox"/>
		/// </summary>
		protected new class Callback : ListControl.Callback, ICallback
		{
			/// <summary>
			/// Raises the activated event.
			/// </summary>
			public void OnActivated(ListBox widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnActivated(e);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="ListBox"/>
		/// </summary>
		public new interface IHandler : ListControl.IHandler, IContextMenuHost
		{
		}
	}
}
