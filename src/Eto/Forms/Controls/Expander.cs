using System;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// A control with a panel that can be expanded or collapsed with a header and button.
	/// </summary>
	[Handler(typeof(IHandler))]
	public class Expander : Panel
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Identifier for the <see cref="ExpandedChanged"/> event.
		/// </summary>
		public const string ExpandedChangedEvent = "Expander.ExpandedChanged";

		/// <summary>
		/// Occurs when the <see cref="Expanded"/> property changes.
		/// </summary>
		public event EventHandler<EventArgs> ExpandedChanged
		{
			add { Properties.AddHandlerEvent(ExpandedChangedEvent, value); }
			remove { Properties.RemoveEvent(ExpandedChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="ExpandedChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnExpandedChanged(EventArgs e)
		{
			Properties.TriggerEvent(ExpandedChangedEvent, this, e);
		}

		static Expander()
		{
			EventLookup.Register<Expander>(c => c.OnExpandedChanged(null), Expander.ExpandedChangedEvent);
		}

		/// <summary>
		/// Gets an enumeration of controls that are directly contained by this container
		/// </summary>
		/// <value>The contained controls.</value>
		public override System.Collections.Generic.IEnumerable<Control> Controls
		{
			get
			{
				if (Header != null)
					return base.Controls.Union(new [] { Header });
				else
					return base.Controls;
			}
		}

		static Callback callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback.</returns>
		protected override object GetCallback()
		{
			return callback;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="Panel.Content"/> is expanded/visible.
		/// </summary>
		/// <value><c>true</c> if expanded; otherwise, <c>false</c>.</value>
		public bool Expanded
		{
			get { return Handler.Expanded; }
			set { Handler.Expanded = value; }
		}

		/// <summary>
		/// Gets or sets the header control.
		/// </summary>
		/// <value>The header control.</value>
		public Control Header
		{
			get { return Handler.Header; }
			set 
			{ 
				SetParent(value, () => Handler.Header = value, Handler.Header);
			}
		}

		/// <summary>
		/// Callback interface for the <see cref="Expander"/>
		/// </summary>
		public new interface ICallback : Panel.ICallback
		{
			/// <summary>
			/// Raises the expanded changed event.
			/// </summary>
			/// <param name="widget">Widget to raise the event.</param>
			/// <param name="e">Event arguments.</param>
			void OnExpandedChanged(Expander widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for the <see cref="Expander"/>
		/// </summary>
		protected new class Callback : Panel.Callback, ICallback
		{
			/// <summary>
			/// Raises the expanded changed event.
			/// </summary>
			/// <param name="widget">Widget to raise the event.</param>
			/// <param name="e">Event arguments.</param>
			public void OnExpandedChanged(Expander widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnExpandedChanged(e);
			}
		}

		/// <summary>
		/// Handler interface for platform implementations of the <see cref="Expander"/>.
		/// </summary>
		public new interface IHandler : Panel.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether the <see cref="Panel.IHandler.Content"/> is expanded/visible.
			/// </summary>
			/// <value><c>true</c> if expanded; otherwise, <c>false</c>.</value>
			bool Expanded { get; set; }

			/// <summary>
			/// Gets or sets the header control.
			/// </summary>
			/// <value>The header control.</value>
			Control Header { get; set; }
		}
	}
}