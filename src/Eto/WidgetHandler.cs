using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Diagnostics;

namespace Eto
{
	/// <summary>
	/// Base platform handler for widgets
	/// </summary>
	/// <remarks>
	/// This is the base class for platform handlers. 
	/// It is used to help wire up events and provide base functionality of a widget.
	/// 
	/// If you are creating an InstanceWidget, you should use <see cref="WidgetHandler{T,W}"/>.
	/// </remarks>
	/// <example>
	/// This example shows how to implement a platform handler for a widget called StaticWidget
	/// <code><![CDATA[
	/// // override the class and implement widget-specific interface
	/// public MyStaticWidgetHandler : WidgetHandler<StaticWidget>, IStaticWidget
	/// {
	///		// implement IStaticWidget's properties and methods
	/// }
	/// ]]></code>
	/// </example>
	/// <seealso cref="WidgetHandler{T,W}"/>
	/// <typeparam name="TWidget">Type of widget the handler is for</typeparam>
	public abstract class WidgetHandler<TWidget> : Widget.IHandler, IDisposable
		where TWidget: Widget
	{
		const string InstanceEventSuffix = ".Instance";

		/// <summary>
		/// Gets the widget that this platform handler is attached to
		/// </summary>
		public TWidget Widget { get; private set; }

		static readonly object IDKey = new object();

		/// <summary>
		/// Gets or sets the ID of this widget
		/// </summary>
		public virtual string ID
		{
			get { return Widget.Properties.Get<string>(IDKey); }
			set { Widget.Properties[IDKey] = value; }
		}

		/// <summary>
		/// Gets a value indicating that the specified event is handled
		/// </summary>
		/// <param name="id">Identifier of the event</param>
		/// <returns>True if the event is handled, otherwise false</returns>
		public bool IsEventHandled(string id)
		{
			return Widget.Properties.ContainsKey(id) || EventLookup.IsDefault(Widget, id) || Widget.Properties.ContainsKey(id + InstanceEventSuffix);
		}

		/// <summary>
		/// Gets the callback object for the control
		/// </summary>
		/// <remarks>
		/// This object is typically a single static instance that is used by the platform handlers to call private or protected
		/// methods on the widget, such as protected event methods e.g. protected virtual void OnClick(EventArgs e)
		/// </remarks>
		/// <value>The callback.</value>
		protected object Callback { get { return ((ICallbackSource)Widget).Callback; } }

		/// <summary>
		/// Called to handle a specific event
		/// </summary>
		/// <remarks>
		/// Most events are late bound by this method. Instead of wiring all events, this
		/// will be called with an event string that is defined by the control.
		/// 
		/// This is called automatically when attaching to events, but must be called manually
		/// when users of the control only override the event's On... method.
		/// 
		/// Override the <see cref="AttachEvent"/> to attach your events
		/// </remarks>
		/// <seealso cref="AttachEvent"/>
		/// <param name="id">ID of the event to handle</param>
		/// <param name="defaultEvent">True if the event is default (e.g. overridden or via an event handler subscription)</param>
		public void HandleEvent(string id, bool defaultEvent = false)
		{
			var instanceId = id + InstanceEventSuffix;
			if (Widget.Properties.ContainsKey(instanceId))
				return;

			if (defaultEvent)
			{
				AttachEvent(id);
			}
			else if (!EventLookup.IsDefault(Widget, id))
			{
				Widget.Properties.Add(instanceId, true);
				AttachEvent(id);
			}
		}

		/// <summary>
		/// Attaches the specified event to the platform-specific control
		/// </summary>
		/// <remarks>
		/// Implementors should override this method to handle any events that the widget
		/// supports. Ensure to call the base class' implementation if the event is not
		/// one the specific widget supports, so the base class' events can be handled as well.
		/// </remarks>
		/// <param name="id">Identifier of the event</param>
		public virtual void AttachEvent(string id)
		{
			// if we got here, the specified event was not implemented by this platform
			Debug.WriteLine($"WARNING: Event '{id}' not supported by type {GetType()}");
		}

		/// <summary>
		/// Called to initialize this widget after it has been constructed
		/// </summary>
		/// <remarks>
		/// Override this to initialize any of the platform objects.  This is called
		/// in the widget constructor, after all of the widget's constructor code has been called.
		/// </remarks>
		protected virtual void Initialize()
		{
		}

		void Widget.IHandler.Initialize()
		{
			Initialize();
			// apply styles after the handler is fully initialized.
			Style.Provider?.ApplyDefault(this);
		}

		/// <summary>
		/// Gets or sets the widget instance
		/// </summary>
		Widget Widget.IHandler.Widget
		{
			get { return Widget; }
			set
			{
				Widget = (TWidget)value;
			}
		}

		/// <summary>
		/// Gets the native platform-specific handle for integration purposes
		/// </summary>
		/// <value>The native handle.</value>
		public virtual IntPtr NativeHandle { get { return IntPtr.Zero; } }

		#region IDisposable Members

		/// <summary>
		/// Disposes this object
		/// </summary>
		/// <remarks>
		/// To handle disposal logic, use the <see cref="Dispose(bool)"/> method.
		/// </remarks>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		/// <summary>
		/// Disposes the object
		/// </summary>
		/// <param name="disposing">True when disposed manually, false if disposed via the finalizer</param>
		protected virtual void Dispose(bool disposing)
		{
		}
	}

	/// <summary>
	/// Base platform handler for <see cref="Widget"/> objects that have a backing platform object
	/// </summary>
	/// <remarks>
	/// This is the base class for platform handlers. 
	/// It is used to help wire up events and provide base functionality of a widget.
	/// </remarks>
	/// <example>
	/// This example shows how to implement a platform handler for a widget
	/// <code><![CDATA[
	/// // override the class and implement widget-specific interface
	/// public MyWidgetHandler : WidgetHandler<MyPlatformControl, MyWidget>, IMyWidget
	/// {
	///		// implement IStaticWidget's properties and methods
	/// }
	/// ]]></code>
	/// </example>
	/// <seealso cref="WidgetHandler{T,W}"/>
	/// <typeparam name="TControl">Type of the platform-specific object</typeparam>
	/// <typeparam name="TWidget">Type of widget the handler is for</typeparam>
	public abstract class WidgetHandler<TControl, TWidget> : WidgetHandler<TWidget>, IControlObjectSource
		where TWidget: Widget
	{
		TControl control;

		/// <summary>
		/// Creates the control if not already set.
		/// </summary>
		/// <remarks>
		/// Override this to create the control instance for the handler.
		/// This makes it easy to extend existing handler implementations with different control implementations.
		/// Some platforms (e.g. Mac) require subclasses to implement/override functionality.
		/// </remarks>
		/// <returns>The control.</returns>
		protected virtual TControl CreateControl()
		{
			return default(TControl);
		}

		/// <summary>
		/// Gets a value indicating that control should automatically be disposed when this widget is disposed
		/// </summary>
		protected virtual bool DisposeControl { get { return true; } }

		/// <summary>
		/// Gets or sets the platform-specific control object
		/// </summary>
		public TControl Control
		{
			get { return !ReferenceEquals(control, default(TControl)) ? control : (control = CreateControl()); }
			set { control = value; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance has a <see cref="Control"/> instance.
		/// </summary>
		/// <value><c>true</c> if this instance has a control; otherwise, <c>false</c>.</value>
		public bool HasControl { get { return !ReferenceEquals(control, default(TControl)); } }

		/// <summary>
		/// Gets the platform-specific control object
		/// </summary>
		object IControlObjectSource.ControlObject { get { return Control; } }


		/// <summary>
		/// Disposes this widget and the associated control if <see cref="DisposeControl"/> is <c>true</c>
		/// </summary>
		/// <param name="disposing">True if <see cref="Dispose"/> was called manually, false if called from the finalizer</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && DisposeControl)
			{
				//Console.WriteLine ("{0}: 1. Disposing control {1}, {2}", this.WidgetID, this.Control.GetType (), this.GetType ());
				var disposable = control as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}
			//Console.WriteLine ("{0}: 2. Disposed handler {1}", this.WidgetID, this.GetType ());
			control = default(TControl);
			base.Dispose(disposing);
		}

		/// <summary>
		/// Gets the platform-specific control object of the specified widget using this handler
		/// </summary>
		/// <remarks>
		/// The widget must be using a handler that returns the same control.
		/// 
		/// This can be used very easily by platform code:
		/// <code>
		///		MyControl mycontrol;
		///		var platformControl = MyControlHandler.GetControl(mycontrol);
		/// </code>
		/// 
		/// Note that even if the specified handler is used, the control might not actually be using that
		/// handler.  This method will still work as long as the handler implements using the same base platform-specific control.
		/// </remarks>
		/// <param name="widget">The widget to get the platform-specific control from</param>
		/// <returns>The platform-specific control used for the specified widget</returns>
		public static TControl GetControl(TWidget widget)
		{
			if (ReferenceEquals(widget, null))
				return default(TControl);
			var handler = (WidgetHandler<TControl, TWidget>)widget.Handler;
			return handler.Control;
		}

		static readonly object connector_key = new object();

		/// <summary>
		/// Gets a weak connector class to hook up events to the underlying control
		/// </summary>
		/// <remarks>
		/// Some frameworks (e.g. gtk, monomac, ios, android) keep track of references in a way that leak objects when
		/// there is a circular reference between the control and the handler.  This is the case when registering events
		/// from the control to a method implemented in the handler.
		/// This instance can be used to connect the objects together using a weak reference to the handler, allowing
		/// controls to be garbage collected.
		/// </remarks>
		/// <value>The connector instance</value>
		protected WeakConnector Connector
		{ 
			get
			{
				object connectorObject;
				if (Widget.Properties.TryGetValue(connector_key, out connectorObject))
					return (WeakConnector)connectorObject;

				var connector = CreateConnector();
				connector.Handler = this;
				Widget.Properties[connector_key] = connector;
				return connector;
			} 
		}

		/// <summary>
		/// Creates the event connector for this control
		/// </summary>
		/// <remarks>
		/// This creates the weak connector to use for event registration and other purposes.
		/// </remarks>
		/// <seealso cref="Connector"/>
		protected virtual WeakConnector CreateConnector()
		{
			return new WeakConnector();
		}

		/// <summary>
		/// Connector for events to keep a weak reference to allow controls to be garbage collected when no longer referenced
		/// </summary>
		/// <seealso cref="Connector"/>
		protected class WeakConnector
		{
			WeakReference handler;

			/// <summary>
			/// Gets the handler that the connector is associated with
			/// </summary>
			/// <remarks>
			/// This property is used to access the handler instance to trigger events.
			/// </remarks>
			/// <value>The handler.</value>
			public WidgetHandler<TControl, TWidget> Handler { get { return (WidgetHandler<TControl, TWidget>)handler.Target; } internal set { handler = new WeakReference(value); } }
		}

	}

	/// <summary>
	/// Widget handler with type-specific callback
	/// </summary>
	/// <remarks>
	/// This can be used by controls that have events to trigger using a callback class.
	/// </remarks>
	/// <example>
	/// This is a full example showing a new control with a handler-triggered event and a property.
	/// <code>
	/// 	// in your eto-only dll:
	/// 	public class MyEtoControl : Eto.Forms.Control
	/// 	{
	/// 
	/// 		// define an event that is triggered by the handler
	/// 		public const string MySomethingEvent = "MyEtoControl.MySomething";
	/// 		
	/// 		public event EventHandler&lt;EventArgs&gt; MySomething
	/// 		{
	/// 			add { Properties.AddHandlerEvent(MySomethingEvent, value); }
	/// 			remove { Properties.RemoveEvent(MySomethingEvent, value); }
	/// 		}
	/// 		
	/// 		// allow subclasses to override the event
	/// 		protected virtual void OnMySomething(EventArgs e)
	/// 		{
	/// 			Properties.TriggerEvent(MySomethingEvent, this, e);
	/// 		}
	/// 
	/// 		static MyEtoControl()
	/// 		{
	/// 			RegisterEvent&lt;MyEtoControl&gt;(c => c.OnMySomething(null), MySomethingEvent);
	/// 		}
	/// 
	/// 		// defines the callback interface to trigger the event from handlers
	/// 		public interface ICallback : Eto.Control.ICallback
	/// 		{
	/// 			void OnMySomething(MyEtoControl widget, EventArgs e);
	/// 		}
	/// 
	/// 		// defines the callback implementation
	/// 		protected class Callback : Eto.Control.Callback, ICallback
	/// 		{
	/// 			public void OnMySomething(MyEtoControl widget, EventArgs e)
	/// 			{
	/// 				using (widget.Platform.Context)
	/// 					widget.OnMySomething(e);
	/// 			}
	/// 		}
	/// 
	/// 		// create single instance of the callback, and tell Eto we want to use it
	/// 		static readonly object callback = new Callback();
	/// 		protected override object GetCallback() { return callback; }
	/// 
	/// 		// handler interface for other methods/properties
	/// 		public interface IHandler : Eto.Control.IHandler
	/// 		{
	/// 			string MyProperty { get; set; }
	/// 		}
	/// 
	/// 		new IHandler Handler { get { (IHandler)base.Handler; } }
	/// 
	/// 		public string MyProperty { get { return Handler.MyProperty; } set { Handler.MyProperty = value; } }
	/// 	}
	/// 
	/// 
	/// 	// in each platform-specific dll:
	/// 	public class MyHandler : WidgetHandler&lt;PlatformSpecificControl, MyEtoControl, MyEtoControl.ICallback&gt; : MyEtoControl.IHandler
	/// 	{
	/// 		public MyHandler()
	/// 		{
	/// 			Control = new PlatformSpecificControl();
	/// 		}
	/// 
	/// 		public string MyProperty { get; set; }
	/// 
	/// 		public override void AttachEvent(string id)
	/// 		{
	/// 			switch (id)
	/// 			{
	/// 				case MyEtoControl.MySomethingEvent:
	/// 					Control.SomeEvent += (sender, e) => Callback.OnMySomething(EventArgs.Empty);
	/// 					break;
	/// 
	/// 				default:
	/// 					base.AttachEvent(id);
	/// 					break;
	/// 			}
	/// 		}
	/// 	}
	/// </code>
	/// </example>
	/// <typeparam name="TControl">Type of the platform-specific object</typeparam>
	/// <typeparam name="TWidget">Type of widget the handler is for</typeparam>
	/// <typeparam name="TCallback">Type of the callback</typeparam>
	public abstract class WidgetHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget>
		where TWidget: Widget
	{
		/// <summary>
		/// Gets the callback object for the control
		/// </summary>
		/// <remarks>
		/// This object is typically a single static instance that is used by the platform handlers to call private or protected
		/// methods on the widget, such as protected event methods e.g. protected virtual void OnClick(EventArgs e)
		/// </remarks>
		/// <value>The callback.</value>
		public new TCallback Callback { get { return (TCallback)base.Callback; } }
	}

}
