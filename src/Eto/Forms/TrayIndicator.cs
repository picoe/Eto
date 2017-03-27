using System;
using Eto.Drawing;

namespace Eto.Forms
{
    /// <summary>
    /// A system tray indicator.
    /// </summary>
    [Handler(typeof(TrayIndicator.IHandler))]
    public class TrayIndicator : Widget
    {
        new IHandler Handler { get { return (IHandler)base.Handler; } }

        static readonly object callback = new Callback();

        /// <summary>
        /// Gets an instance of an object used to perform callbacks to the widget from handler implementations.
        /// </summary>
        /// <returns>The callback instance to use for this widget.</returns>
        protected override object GetCallback() { return callback; }

        /// <summary>
        /// Gets or sets the icon for the <see cref="TrayIndicator"/>.
        /// </summary>
        /// <value>The icon of the <see cref="TrayIndicator"/>.</value>
		[Obsolete("Since 2.4. Use Image instead")]
        public Icon Icon
        {
            get { return Image as Icon; }
			set { Image = value; }
        }

		/// <summary>
		/// Gets or sets the image to display in the tray
		/// </summary>
		/// <value>The image to display in the tray.</value>
		public Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}

        /// <summary>
        /// Gets or sets the title/tooltip for the <see cref="TrayIndicator"/>.
        /// </summary>
        /// <value>The title/tooltip for the <see cref="TrayIndicator"/>.</value>
        public string Title
        {
            get { return Handler.Title; }
            set { Handler.Title = value; }
        }

		/// <summary>
        /// Gets or sets a value indicating whether this <see cref="TrayIndicator"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visible
        {
            get { return Handler.Visible; }
            set { Handler.Visible = value; }
        }

        /// <summary>
        /// Hide this instance of <see cref="TrayIndicator"/>.
        /// </summary>
        /// <remarks>
        /// Make sure to call this method before closing the application.
        /// </remarks>
        public void Hide()
        {
            Visible = false;
        }

		/// <summary>
		/// Sets the indicator menu.
		/// </summary>
		/// <remarks>
		/// Make sure to call this method every time you make changes to the context menu.
		/// </remarks>
		/// <param name="menu">The indicator menu.</param>
		[Obsolete("Since 2.4. Use Menu instead")]
		public void SetMenu(ContextMenu menu)
        {
			Menu = menu;
        }

		/// <summary>
		/// Gets or sets the menu shown when the user clicks on the tray icon.
		/// </summary>
		/// <value>The context menu.</value>
		public ContextMenu Menu
		{
			get { return Handler.Menu; }
			set { Handler.Menu = value; }
		}

        /// <summary>
        /// Show this instance of <see cref="TrayIndicator"/>.
        /// </summary>
        public void Show()
        {
            Visible = true;
        }

        /// <summary>
        /// Handler interface for the <see cref="TrayIndicator"/> control
        /// </summary>
        public new interface IHandler : Widget.IHandler
        {
            /// <summary>
            /// Gets or sets the title/tooltip for the <see cref="TrayIndicator"/>.
            /// </summary>
            /// <value>The title/tooltip for the <see cref="TrayIndicator"/>.</value>
            string Title { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="TrayIndicator"/> is visible.
            /// </summary>
            /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
            bool Visible { get; set; }

			/// <summary>
			/// Gets or sets the image to display in the tray
			/// </summary>
			/// <value>The image to display in the tray.</value>
			Image Image { get; set; }

			/// <summary>
			/// Gets or sets the menu shown when the user clicks on the tray icon.
			/// </summary>
			/// <value>The context menu.</value>
			ContextMenu Menu { get; set; }
        }

        /// <summary>
        /// Event identifier for handlers when attaching the <see cref="Activated"/> event.
        /// </summary>
        public const string ActivatedEvent = "TrayIndicator.Activated";
        
        /// <summary>
        /// Event to handle when the user left click the <see cref="TrayIndicator"/>.
        /// </summary>
        public event EventHandler<EventArgs> Activated
        {
            add { Properties.AddHandlerEvent(ActivatedEvent, value); }
            remove { Properties.RemoveEvent(ActivatedEvent, value); }
        }
        
        /// <summary>
        /// Raises the <see cref="Activated"/> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnActivated(EventArgs e)
        {
            Properties.TriggerEvent(ActivatedEvent, this, e);
        }

        /// <summary>
        /// Callback interface for <see cref="TrayIndicator"/>
        /// </summary>
        public new interface ICallback : Widget.ICallback
        {
            /// <summary>
            /// Raises activated event.
            /// </summary>
            void OnActivated(TrayIndicator widget, EventArgs e);
        }
        
        /// <summary>
        /// Callback implementation for handlers of <see cref="TrayIndicator"/>
        /// </summary>
        protected class Callback : ICallback
        {
            /// <summary>
            /// Raises activated event.
            /// </summary>
            public void OnActivated(TrayIndicator widget, EventArgs e)
            {
				using (widget.Platform.Context)
					widget.OnActivated(e);
            }
        }
    }
}
