using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
	public abstract class WidgetHandler<TWidget> : IWidget, IDisposable
		where TWidget: Widget
	{
		Generator generator;

		/// <summary>
		/// Gets the widget that this platform handler is attached to
		/// </summary>
		public TWidget Widget { get; private set; }

		/// <summary>
		/// Gets the generator that was used to create this handler
		/// </summary>
		public Generator Generator
		{
			get { return generator; }
			set
			{
				generator = value;
				
				// validate the generator
				Eto.Generator.Validate(generator);
			}
		}

		#region IWidget Members

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

		protected virtual void PostInitialize()
		{
		}

		void IWidget.Initialize()
		{
			Initialize();
			PostInitialize();
		}

		/// <summary>
		/// Gets or sets the widget instance
		/// </summary>
		Widget IWidget.Widget
		{
			get { return Widget; }
			set { Widget = (TWidget)value; }
		}

		#endregion

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
	/// Base platform handler for <see cref="InstanceWidget"/> objects
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
	/// <typeparam name="T">Type of the platform-specific object</typeparam>
	/// <typeparam name="TWidget">Type of widget the handler is for</typeparam>
	public abstract class WidgetHandler<T, TWidget> : WidgetHandler<TWidget>, IInstanceWidget
		where TWidget: InstanceWidget
	{
		const string InstanceEventSuffix = ".Instance";

				/// <summary>
		/// Initializes a new instance of the WidgetHandler class
		/// </summary>
		protected WidgetHandler()
		{
		}

		/// <summary>
		/// Called to create the platform control for this widget
		/// </summary>
		/// <remarks>
		/// This is used so that it is easy to override the control that is created for a handler.
		/// If you derive from an existing platform handler to override it's behaviour, you can change
		/// the class that is created via this method. You will still need that control to be of the same
		/// type that the original handler has defined, but you will be able to subclass the platform control
		/// to provide any specific functionality if needed.
		/// 
		/// This gets called automatically by <see cref="Initialize"/>, so you should only set properties on your control
		/// in the handler's overridden initialize method, after the base class' initialize has been called.
		/// </remarks>
		/// <returns>A new instance of the platform-specific control this handler encapsulates</returns>
		public virtual T CreateControl()
		{
			return default(T);
		}

		/// <summary>
		/// Called to initialize this widget after it has been constructed
		/// </summary>
		/// <remarks>
		/// Override this to initialize any of the platform objects.  This is called
		/// in the widget constructor, after all of the widget's constructor code has been called.
		/// </remarks>
		protected override void Initialize()
		{
			if (EqualityComparer<T>.Default.Equals(Control, default(T)))
				Control = CreateControl();

			base.Initialize();
		}

		protected override void PostInitialize()
		{
			base.PostInitialize();
			Style.OnStyleWidgetDefaults(this);
		}

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
		/// Gets or sets a value indicating that control should automatically be disposed when this widget is disposed
		/// </summary>
		protected virtual bool DisposeControl { get { return true; } }

		/// <summary>
		/// Gets or sets the platform-specific control object
		/// </summary>
		public virtual T Control { get; protected set; }

		/// <summary>
		/// Gets the platform-specific control object
		/// </summary>
		object IInstanceWidget.ControlObject { get { return Control; } }

		/// <summary>
		/// Gets a value indicating that the specified event is handled
		/// </summary>
		/// <param name="id">Identifier of the event</param>
		/// <returns>True if the event is handled, otherwise false</returns>
		public bool IsEventHandled(string id)
		{
			return Widget.Properties.ContainsKey(id) || EventLookup.IsDefault(Widget, id) || Widget.Properties.ContainsKey(id + InstanceEventSuffix);
		}

		public void HandleEvent(string id, bool defaultEvent = false)
		{
			if (defaultEvent)
			{
				if (!Widget.Properties.ContainsKey(id + InstanceEventSuffix))
					AttachEvent(id);
			}
			else if (!Widget.Properties.ContainsKey(id) && !EventLookup.IsDefault(Widget, id))
			{
				var instanceId = id + InstanceEventSuffix;
				if (Widget.Properties.ContainsKey(instanceId))
					return;
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
			// only use for desktop until mobile controls are working
			#if DESKTOP
			throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Event {0} not supported by this control", id));
			#endif
		}

		/// <summary>
		/// Disposes this widget and the associated control if <see cref="DisposeControl"/> is <c>true</c>
		/// </summary>
		/// <param name="disposing">True if <see cref="Dispose"/> was called manually, false if called from the finalizer</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && DisposeControl)
			{
				//Console.WriteLine ("{0}: 1. Disposing control {1}, {2}", this.WidgetID, this.Control.GetType (), this.GetType ());
				var control = Control as IDisposable;
				if (control != null)
					control.Dispose();
			}
			//Console.WriteLine ("{0}: 2. Disposed handler {1}", this.WidgetID, this.GetType ());
			Control = default(T);
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
		public static T GetControl(TWidget widget)
		{
			var handler = (WidgetHandler<T, TWidget>)widget.Handler;
			return handler.Control;
		}
	}
}
