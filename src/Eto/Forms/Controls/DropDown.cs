using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Arguments for formatting items in a DropDown using the <see cref="DropDown.FormatItem"/> event.
	/// </summary>
	public class DropDownFormatEventArgs : EventArgs
	{
		Font _font;

		/// <summary>
		/// Gets or sets the font to use for this item
		/// </summary>
		public virtual Font Font
		{
			get => _font;
			set
			{
				_font = value;
				IsFontSet = true;
			}
		}

		/// <summary>
		/// Gets a value indicating that the font was set during the FormatItem event.
		/// </summary>
		/// <remarks>
		/// This is useful for handler implementations to determine if something needs to be done to format the item.
		/// </remarks>
		public bool IsFontSet { get; private set; }

		/// <summary>
		/// Item to specify the format for
		/// </summary>
		public object Item { get; }

		/// <summary>
		/// Row number in the list of items to format for
		/// </summary>
		public int Row { get; }

		/// <summary>
		/// Initializes a new instance of the DropDownFormatEventArgs class
		/// </summary>
		/// <param name="item">Item to format</param>
		/// <param name="row">Row of the item to format</param>
		/// <param name="font">Font to use if no other is specified</param>
		public DropDownFormatEventArgs(object item, int row, Font font)
		{
			Item = item;
			Row = row;
			_font = font;
		}
	}

	/// <summary>
	/// Presents a drop down to select from a list of items
	/// </summary>
	[Handler(typeof(DropDown.IHandler))]
	public class DropDown : ListControl
	{
		new IHandler Handler => (IHandler)base.Handler;

		static DropDown()
		{
			EventLookup.Register<DropDown>(c => c.OnDropDownClosed(null), DropDownClosedEvent);
			EventLookup.Register<DropDown>(c => c.OnDropDownOpening(null), DropDownOpeningEvent);
			EventLookup.Register<DropDown>(c => c.OnFormatItem(null), FormatItemEvent);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.DropDown"/> class.
		/// </summary>
		public DropDown()
		{
			ItemImageBinding = new ListItemImageBinding();
		}

		/// <summary>
		/// Gets or sets the binding to get the image for each item. 
		/// </summary>
		/// <remarks>
		/// By default this looks for the "Image" property of the item, and also works if you use <see cref="ImageListItem"/>.
		/// 
		/// This will be ignored when creating a <see cref="ComboBox"/>, and is only supported with the <see cref="DropDown"/> directly.
		/// </remarks>
		/// <value>The binding to get the image for each item.</value>
		public IIndirectBinding<Image> ItemImageBinding { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to show the control's border.
		/// </summary>
		/// <remarks>
		/// This is a hint to omit the border of the control and show it as plainly as possible.
		/// 
		/// Typically used when you want to show the control within a cell of the <see cref="GridView"/>.
		/// </remarks>
		/// <value><c>true</c> to show the control border; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool ShowBorder
		{
			get { return Handler.ShowBorder; }
			set { Handler.ShowBorder = value; }
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="DropDownOpening"/> event
		/// </summary>
		public const string DropDownOpeningEvent = "DropDown.DropDownOpening";

		/// <summary>
		/// Occurs right before the drop down is opened.
		/// </summary>
		/// <remarks>
		/// This is useful so you can fill the items of the drop down only when they are needed.
		/// </remarks>
		public event EventHandler<EventArgs> DropDownOpening
		{
			add { Properties.AddHandlerEvent(DropDownOpeningEvent, value); }
			remove { Properties.RemoveEvent(DropDownOpeningEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="DropDownOpening"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnDropDownOpening(EventArgs e)
		{
			Properties.TriggerEvent(DropDownOpeningEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="DropDownClosed"/> event.
		/// </summary>
		public const string DropDownClosedEvent = "DropDown.DropDownClosed";

		/// <summary>
		/// Occurs when the drop down is closed.
		/// </summary>
		public event EventHandler<EventArgs> DropDownClosed
		{
			add { Properties.AddHandlerEvent(DropDownClosedEvent, value); }
			remove { Properties.RemoveEvent(DropDownClosedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="DropDownClosed"/> event
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnDropDownClosed(EventArgs e)
		{
			Properties.TriggerEvent(DropDownClosedEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="FormatItem"/> event.
		/// </summary>
		public const string FormatItemEvent = "DropDown.FormatItem";

		/// <summary>
		/// Occurs for each item to provide formatting.
		/// </summary>
		public event EventHandler<DropDownFormatEventArgs> FormatItem
		{
			add => Properties.AddHandlerEvent(FormatItemEvent, value);
			remove => Properties.RemoveEvent(FormatItemEvent, value);
		}

		/// <summary>
		/// Raises the <see cref="FormatItem"/> event.
		/// </summary>
		/// <param name="e">Event Arguments</param>
		protected virtual void OnFormatItem(DropDownFormatEventArgs e) => Properties.TriggerEvent(FormatItemEvent, this, e);

		/// <summary>
		/// Gets the callback.
		/// </summary>
		/// <returns>The callback.</returns>
		protected override object GetCallback() => new Callback();

		/// <summary>
		/// Callback interface for the DropDown
		/// </summary>
		public new interface ICallback : ListControl.ICallback
		{
			/// <summary>
			/// Raises the <see cref="DropDownOpening"/> event.
			/// </summary>
			/// <param name="widget">Widget to raise the event</param>
			/// <param name="e">Event arguments</param>
			void OnDropDownOpening(DropDown widget, EventArgs e);

			/// <summary>
			/// Raises the <see cref="DropDownClosed"/> event
			/// </summary>
			/// <param name="widget">Widget to raise the event</param>
			/// <param name="e">Event arguments</param>
			void OnDropDownClosed(DropDown widget, EventArgs e);

			/// <summary>
			/// Raises the <see cref="FormatItem"/> event.
			/// </summary>
			/// <param name="widget">Widget to raise the event</param>
			/// <param name="e">Event Arguments</param>
			void OnFormatItem(DropDown widget, DropDownFormatEventArgs e);
		}

		/// <summary>
		/// Callback implementation for the DropDown
		/// </summary>
		protected new class Callback : ListControl.Callback, ICallback
		{
			/// <summary>
			/// Raises the <see cref="DropDownOpening"/> event.
			/// </summary>
			/// <param name="widget">Widget to raise the event</param>
			/// <param name="e">Event arguments</param>
			public void OnDropDownOpening(DropDown widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnDropDownOpening(e);
			}

			/// <summary>
			/// Raises the <see cref="DropDownClosed"/> event
			/// </summary>
			/// <param name="widget">Widget to raise the event</param>
			/// <param name="e">Event arguments</param>
			public void OnDropDownClosed(DropDown widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnDropDownClosed(e);
			}

			/// <summary>
			/// Raises the <see cref="FormatItem"/> event.
			/// </summary>
			/// <param name="widget">Widget to raise the event</param>
			/// <param name="e">Event Arguments</param>
			public void OnFormatItem(DropDown widget, DropDownFormatEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnFormatItem(e);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="DropDown"/>
		/// </summary>
		public new interface IHandler : ListControl.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether to show the control's border.
			/// </summary>
			/// <remarks>
			/// This is a hint to omit the border of the control and show it as plainly as possible.
			/// 
			/// Typically used when you want to show the control within a cell of the <see cref="GridView"/>.
			/// </remarks>
			/// <value><c>true</c> to show the control border; otherwise, <c>false</c>.</value>
			bool ShowBorder { get; set; }
		}
	}
}
