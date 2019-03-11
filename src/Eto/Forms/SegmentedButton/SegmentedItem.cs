using System;
using System.ComponentModel;
using Eto.Drawing;
using sc = System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Base class for items of the <see cref="SegmentedButton"/> control.
	/// </summary>
    [sc.TypeConverter(typeof(SegmentedItemConverter))]
    public abstract class SegmentedItem : Widget
    {
        new IHandler Handler => (IHandler)base.Handler;

		static SegmentedItem()
		{
			EventLookup.Register<SegmentedItem>(c => c.OnClick(null), ClickEvent);
		}

		/// <summary>
		/// Gets the parent button this item belongs to.
		/// </summary>
		/// <remarks>
		/// Note this is only set after adding your item to the <see cref="SegmentedButton.Items"/> collection.
		/// </remarks>
		/// <value>The parent control.</value>
		public SegmentedButton Parent { get; internal set; }

		/// <summary>
		/// Gets or sets the text to display for this segment.
		/// </summary>
		/// <value>The segment text.</value>
		public string Text
        {
            get => Handler.Text;
            set => Handler.Text = value;
        }

		/// <summary>
		/// Gets or sets the ToolTip to display for this segment
		/// </summary>
		/// <value>The segment tooltip.</value>
        public string ToolTip
        {
            get => Handler.ToolTip;
            set => Handler.ToolTip = value;
        }

		/// <summary>
		/// Gets or sets the image to display in this segment.
		/// </summary>
		/// <value>The segment image.</value>
        public Image Image
        {
            get => Handler.Image;
            set => Handler.Image = value;
        }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.SegmentedItem"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool Enabled
        {
            get => Handler.Enabled;
            set => Handler.Enabled = value;
        }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.SegmentedItem"/> is visible.
		/// </summary>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool Visible
        {
            get => Handler.Visible;
            set => Handler.Visible = value;
        }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.SegmentedItem"/> is selected.
		/// </summary>
		/// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public bool Selected
        {
            get => Handler.Selected;
            set => Handler.Selected = value;
        }

		/// <summary>
		/// Gets or sets the width of this segment, or -1 to auto size.
		/// </summary>
		/// <value>The width of this segment.</value>
		[DefaultValue(-1)]
        public int Width
        {
            get => Handler.Width;
            set => Handler.Width = value;
        }

		#region Events

		/// <summary>
		/// Identifier for handlers to attach the <see cref="Click"/> event.
		/// </summary>
		public const string ClickEvent = "SegmentedItem.Click";

		/// <summary>
		/// Occurs when this segment is clicked.
		/// </summary>
        public event EventHandler<EventArgs> Click
        {
            add => Properties.AddHandlerEvent(ClickEvent, value);
            remove => Properties.RemoveEvent(ClickEvent, value);
        }

		/// <summary>
		/// Raises the <see cref="Click"/> event.
		/// </summary>
		/// <param name="e">Event argments</param>
        protected virtual void OnClick(EventArgs e)
        {
            Properties.TriggerEvent(ClickEvent, this, e);
        }

		#endregion

		static readonly ICallback s_callback = new Callback();

		/// <inheritdoc />
        protected override object GetCallback() => s_callback;

		/// <summary>
		/// Callback interface for handlers of the <see cref="SegmentedItem"/>.
		/// </summary>
        public new interface ICallback : Widget.ICallback
        {
			/// <summary>
			/// Raises the Click event.
			/// </summary>
            void OnClick(SegmentedItem widget, EventArgs e);
        }

		/// <summary>
		/// Callback implementation for the <see cref="SegmentedItem"/>.
		/// </summary>
        protected class Callback : ICallback
        {
			/// <summary>
			/// Raises the Click event.
			/// </summary>
            public void OnClick(SegmentedItem widget, EventArgs e)
            {
                using (widget.Platform.Context)
                    widget.OnClick(e);
            }
        }

		/// <summary>
		/// Handler interface for the <see cref="SegmentedItem"/>.
		/// </summary>
        public new interface IHandler : Widget.IHandler
        {
			/// <summary>
			/// Gets or sets the text to display for this segment.
			/// </summary>
			/// <value>The segment text.</value>
			string Text { get; set; }
			/// <summary>
			/// Gets or sets the ToolTip to display for this segment
			/// </summary>
			/// <value>The segment tooltip.</value>
			string ToolTip { get; set; }
			/// <summary>
			/// Gets or sets the image to display in this segment.
			/// </summary>
			/// <value>The segment image.</value>
			Image Image { get; set; }
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.SegmentedItem"/> is enabled.
			/// </summary>
			/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
			bool Enabled { get; set; }
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.SegmentedItem"/> is visible.
			/// </summary>
			/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
			bool Visible { get; set; }
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="T:Eto.Forms.SegmentedItem"/> is selected.
			/// </summary>
			/// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
			bool Selected { get; set; }
			/// <summary>
			/// Gets or sets the width of this segment, or -1 to auto size.
			/// </summary>
			/// <value>The width of this segment.</value>
			int Width { get; set; }
        }

		/// <summary>
		/// Implicitly converts a string to a segmented item.
		/// </summary>
		/// <remarks>
		/// This allows you to do things like the following:
		/// <code><![CDATA[
		/// new SegmentedButton { Items = { "First", "Second", "Third" } };
		/// ]]></code>
		/// </remarks>
		/// <returns>A segmented item with the specified text.</returns>
		/// <param name="text">Text for the segmented item.</param>
        public static implicit operator SegmentedItem(string text)
        {
            return new ButtonSegmentedItem { Text = text };
        }

		/// <summary>
		/// Implicitly converts an image to a segmented item.
		/// </summary>
		/// <remarks>
		/// This allows you to do things like the following:
		/// <code><![CDATA[
		/// new SegmentedButton { Items = { myImage1, myImage2, myImage3 } };
		/// ]]></code>
		/// </remarks>
		/// <returns>A segmented item with the specified image.</returns>
		/// <param name="image">Image for the segmented item.</param>
		public static implicit operator SegmentedItem(Image image)
        {
            return new ButtonSegmentedItem { Image = image };
		}
	}
}
