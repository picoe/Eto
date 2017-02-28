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
        private Icon icon;

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
        public Icon Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                Handler.SetIcon(value);
            }
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
        public void SetMenu(ContextMenu menu)
        {
            Handler.SetMenu(menu);
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
            /// Sets the icon for the <see cref="TrayIndicator"/>.
            /// </summary>
            /// <param name="icon">The icon of the <see cref="TrayIndicator"/>.</param>
            void SetIcon(Icon icon);

            /// <summary>
            /// Sets the indicator menu.
            /// </summary>
            /// <param name="menu">The indicator menu.</param>
            void SetMenu(ContextMenu menu);
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
                widget.Platform.Invoke(() => widget.OnActivated(e));
            }
        }
    }
}
