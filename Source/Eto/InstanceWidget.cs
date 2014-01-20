using System;
using System.Collections.Generic;

namespace Eto
{
	/// <summary>
	/// Handler implementation for the <see cref="InstanceWidget"/>
	/// </summary>
	public interface IInstanceWidget : IWidget
	{
		/// <summary>
		/// Gets or sets an ID for the widget
		/// </summary>
		/// <remarks>
		/// Some platforms may use this to identify controls (e.g. web)
		/// </remarks>
		string ID { get; set; }

		/// <summary>
		/// Gets the instance of the platform-specific object
		/// </summary>
		object ControlObject { get; }

		/// <summary>
		/// Called to handle a specific event
		/// </summary>
		/// <remarks>
		/// Most events are late bound by this method. Instead of wiring all events, this
		/// will be called with an event string that is defined by the control.
		/// 
		/// This is called automatically when attaching to events, but must be called manually
		/// when users of the control only override the event's On... method.
		/// </remarks>
		/// <param name="id">ID of the event to handle</param>
		/// <param name = "defaultEvent">True if the event is default (e.g. overridden or via an event handler subscription)</param>
		void HandleEvent(string id, bool defaultEvent = false);
	}

	/// <summary>
	/// Widget that represents an instance of an object
	/// </summary>
	/// <remarks>
	/// The InstanceWidget is used for controls and objects that represent an instance of an object.
	/// 
	/// Typically, most widgets will derive from this class. However, if you only have static methods of a class
	/// you will still need a handler implementation and the <see cref="Widget"/> class provides that.
	/// 
	/// InstanceWidgets also wrap the ability to handle late-bound events on the backend control.
	/// </remarks>
	#if XAML
	// Doesn't work in mono yet:
	// [RuntimeNameProperty("ID")]
	#endif
	public abstract class InstanceWidget : Widget, IControlObjectSource
	{
		new IInstanceWidget Handler { get { return (IInstanceWidget)base.Handler; } }

		PropertyStore properties;

		/// <summary>
		/// Gets the dictionary of properties for this widget
		/// </summary>
		public PropertyStore Properties
		{ 
			get { return properties ?? (properties = new PropertyStore(this)); } 
		}

		/// <summary>
		/// Gets or sets the ID of this widget
		/// </summary>
		public virtual string ID
		{
			get { return Handler.ID; }
			set { Handler.ID = value; }
		}

		/// <summary>
		/// Gets or sets the style of this widget
		/// </summary>
		/// <remarks>
		/// Styles allow you to attach custom platform-specific logic to a widget.
		/// In your platform-specific assembly, use <see cref="M:Style.Add{H}(string, StyleHandler{H})"/>
		/// to add the style logic with the same id.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// // in your UI
		/// var control = new Button { Style = "mystyle" };
		/// 
		/// // in your platform assembly
		/// using Eto.Platform.Mac.Forms.Controls;
		/// 
		/// Styles.AddHandler<ButtonHandler>("mystyle", handler => {
		///		// this is where you can use handler.Control to set properties, handle events, etc.
		///		handler.Control.BezelStyle = NSBezelStyle.SmallSquare;
		/// });
		/// ]]></code>
		/// </example>
		public string Style
		{
			get { return Properties.Get<string>(StyleKey); }
			set
			{
				var style = Style;
				if (style != value)
				{
					Properties[StyleKey] = value;
					OnStyleChanged(EventArgs.Empty);
				}
			}
		}

		static readonly object StyleKey = new object();

		#region Events

		static readonly object StyleChangedKey = new object();

		/// <summary>
		/// Occurs when the <see cref="InstanceWidget.Style"/> property has changed
		/// </summary>
		public event EventHandler<EventArgs> StyleChanged
		{
			add { Properties.AddEvent(StyleChangedKey, value); }
			remove { Properties.RemoveEvent(StyleChangedKey, value); }
		}

		/// <summary>
		/// Handles when the <see cref="Style"/> is changed.
		/// </summary>
		protected virtual void OnStyleChanged(EventArgs e)
		{
			Eto.Style.OnStyleWidget(this);
			Properties.TriggerEvent(StyleChangedKey, this, e);
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the InstanceWidget with the specified handler
		/// </summary>
		/// <param name="generator">Generator for the widget</param>
		/// <param name="handler">Pre-created handler to attach to this instance</param>
		/// <param name="initialize">True to call handler's Initialze method, false otherwise</param>
		protected InstanceWidget(Generator generator, IWidget handler, bool initialize = true)
			: base(generator, handler, false)
		{
			if (initialize)
				Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the InstanceWidget with the specified handler type
		/// </summary>
		/// <param name="generator">Generator for the widget</param>
		/// <param name="handlerType">Type of the handler to create as the backend for this widget</param>
		/// <param name="initialize">True to call handler's Initialze method, false otherwise</param>
		protected InstanceWidget(Generator generator, Type handlerType, bool initialize = true)
			: base(generator, handlerType, false)
		{
			if (initialize)
				Initialize();
		}

		/// <summary>
		/// Gets the instance of the platform-specific object
		/// </summary>
		/// <remarks>
		/// This can sometimes be useful to get the platform-specific object.
		/// It is more preferred to use the <see cref="Widget.Handler"/> and cast that to the platform-specific
		/// handler class which can give you additional methods and helpers to do common tasks.
		/// 
		/// For example, the <see cref="Forms.Application"/> object's handler for OS X has a AddFullScreenMenuItem
		/// property to specify if you want full screen support in your app.
		/// </remarks>
		public object ControlObject
		{
			get { return Handler.ControlObject; }
		}

		/// <summary>
		/// Attaches the specified late-bound event to the control to be handled
		/// </summary>
		/// <remarks>
		/// This needs to be called when you want to override the On... methods instead of attaching 
		/// to the associated event.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// // this will call HandleEvent automatically
		/// var textBox = new TextBox ();
		/// textBox.TextChanged += MyTextChangedHandler;
		/// 
		/// // must call HandleEvent when overriding OnTextChanged
		/// public class MyTextBox : TextBox
		/// {
		///		public MyTextBox()
		///		{
		///			HandleEvent (TextChangedEvent);
		///		}
		///		
		///		protected override void OnTextChanged (EventArgs e)
		///		{
		///			// your logic
		///		}
		/// }
		/// 
		/// ]]></code>
		/// </example>
		/// <param name="id">ID of the event to handle.  Usually a constant in the form of [Control].[EventName]Event (e.g. TextBox.TextChangedEvent)</param>
		[Obsolete("You no longer have to call this method, events are wired up automatically")]
		public void HandleEvent(string id)
		{
			Handler.HandleEvent(id, false);
		}

		/// <summary>
		/// Attaches the specified late-bound event(s) to the control to be handled
		/// </summary>
		/// <remarks>
		/// This needs to be called when you want to override the On... methods instead of attaching 
		/// to the associated event.
		/// </remarks>
		/// <example>
		/// Example of how to HandleEvent automatically
		/// <code><![CDATA[
		/// // 
		/// var textBox = new TextBox ();
		/// textBox.TextChanged += MyTextChangedHandler;
		/// textBox.SizeChanged += MySizeChangedHandler;
		/// ]]></code>
		/// 
		/// Example of when you must call HandleEvent when overriding OnTextChanged
		/// <code><![CDATA[
		/// public class MyTextBox : TextBox
		/// {
		///		public MyTextBox()
		///		{
		///			HandleEvent (TextChangedEvent, SizeChangedEvent);
		///		}
		///		
		///		protected override void OnTextChanged (EventArgs e)
		///		{
		///			// your logic
		///		}
		///		
		///		protected override void OnSizeChanged (EventArgs e)
		///		{
		///			// more logic
		///		}
		/// }
		/// ]]></code>
		/// </example>
		/// <param name="ids">ID of the event to handle.  Usually a constant in the form of [Control].[EventName]Event (e.g. TextBox.TextChangedEvent)</param>
		[Obsolete("You no longer have to call this method, events are wired up automatically")]
		public void HandleEvent(params string[] ids)
		{
			for (int i = 0; i < ids.Length; i++)
			{
				Handler.HandleEvent(ids[i], false);
			}
		}

		internal void HandleDefaultEvents(params string[] ids)
		{
			for (int i = 0; i < ids.Length; i++)
			{
				Handler.HandleEvent(ids[i], true);
			}
		}

		/// <summary>
		/// Initializes the widget handler
		/// </summary>
		/// <remarks>
		/// This is typically called from the constructor after all of the logic is completed to construct
		/// the object.
		/// 
		/// If you pass false to the constructor's initialize property, you should call this manually in your constructor
		/// after all of its logic has finished.
		/// </remarks>
		protected new void Initialize()
		{
			base.Initialize();
			Eto.Style.OnStyleWidgetDefaults(this);
			EventLookup.HookupEvents(this);
		}
	}
}
