
// uncomment to track garbage collection of widgets
//#define TRACK_GC

using System;
using System.Globalization;

namespace Eto
{
	/// <summary>
	/// Handler interface for the <see cref="Widget"/> class
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IWidget : IGeneratorSource
	{
		/// <summary>
		/// Gets the widget this handler is implemented for
		/// </summary>
		Widget Widget { get; set; }

		void Initialize();

		new Platform Platform { get; set; }
	}

	/// <summary>
	/// Interface for widgets that have a control object
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IControlObjectSource
	{
		/// <summary>
		/// Gets the control object for this widget
		/// </summary>
		/// <value>The control object for the widget</value>
		object ControlObject { get; }
	}

	/// <summary>
	/// Interface for widgets that have a handler
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IHandlerSource
	{
		/// <summary>
		/// Gets the platform handler object for the widget
		/// </summary>
		/// <value>The handler for the widget</value>
		object Handler { get; }
	}

	/// <summary>
	/// Interface for widgets that are created for a specific generator
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IGeneratorSource
	{
		/// <summary>
		/// Gets the generator associated with the widget
		/// </summary>
		/// <value>The generator</value>
		Platform Platform { get; }
	}

	/// <summary>
	/// Base widget class for all objects requiring a platform-specific implementation
	/// </summary>
	/// <remarks>
	/// The Widget is the base of all abstracted objects that have platform-specific implementations.
	///
	/// The <see cref="InstanceWidget"/> is the class that's typically used as a base as it provides
	/// an instance of the platform-specific object, as well as adds the ability to handle events
	/// on the object.  This is used as the base for static objects such as the <see cref="Forms.MessageBox"/>
	/// or <see cref="EtoEnvironment"/>.
	/// 
	/// To implement the handler for a widget, use the <see cref="WidgetHandler{T}"/> as the base class.
	/// </remarks>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract partial class Widget : IHandlerSource, IDisposable, IGeneratorSource
	{
		/// <summary>
		/// Gets the generator that was used to create the <see cref="Handler"/> for this widget
		/// </summary>
		/// <remarks>
		/// The generator is typically either passed to the constructor of the control, or the
		/// <see cref="P:Eto.Generator.Current"/> is used.
		/// </remarks>
		public Platform Platform { get { return ((IWidget)Handler).Platform; } }

		/// <summary>
		/// Gets the platform-specific handler for this widget
		/// </summary>
		public object Handler { get; internal set; }

		#if TRACK_GC
		~Widget()
		{
			Dispose(false);
		}
		#endif

		/// <summary>
		/// Initializes a new instance of the Widget class
		/// </summary>
		/// <param name="generator">Generator the widget handler was created with, or null to use <see cref="Eto.Generator.Current"/></param>
		/// <param name="handler">Handler to assign to this widget for its implementation</param>
		/// <param name="initialize">True to initialize the widget, false to defer that to the caller</param>
		[Obsolete("Use Widget(IHandler) instead")]
		protected Widget(Generator generator, IWidget handler, bool initialize = true)
		{
			if (generator == null)
				generator = Platform.Instance;
			this.Handler = handler;
			if (handler != null)
			{
				handler.Platform = (Platform)generator;
				handler.Widget = this; // tell the handler who we are
			}
			if (initialize)
				Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the Widget class
		/// </summary>
		/// <param name="generator">Generator to create the handler with, or null to use <see cref="Eto.Generator.Current"/></param>
		/// <param name="type">Type of widget handler to create from the generator for this widget</param>
		/// <param name="initialize">True to initialize the widget, false to defer that to the caller</param>
		[Obsolete("Use default constructor and HandlerAttribute to specify handler to use")]
		protected Widget(Generator generator, Type type, bool initialize = true)
		{
			var platform = (Platform)generator ?? Platform.Instance;
			this.Handler = platform.Create(type);
			var widgetHandler = this.Handler as IWidget;
			if (widgetHandler != null)
			{
				widgetHandler.Platform = (Platform)generator;
				widgetHandler.Widget = this;
			}
		}

		/// <summary>
		/// Initializes a new instance of the Widget class
		/// </summary>
		protected Widget()
		{
			var info = Platform.Instance.FindHandler(GetType());
			if (info == null)
				throw new HandlerInvalidException(string.Format(CultureInfo.CurrentCulture, "type for '{0}' could not be found in this platform", GetType().FullName));
			this.Handler = info.Instantiator();
			var widgetHandler = this.Handler as IWidget;
			if (widgetHandler != null)
			{
				widgetHandler.Platform = Platform.Instance;
				widgetHandler.Widget = this;
			}
			if (info.Initialize)
				Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the Widget class
		/// </summary>
		/// <param name="handler">Handler to assign to this widget for its implementation</param>
		protected Widget(IWidget handler)
		{
			this.Handler = handler;
			if (handler != null)
			{
				handler.Platform = Platform.Instance;
				handler.Widget = this; // tell the handler who we are
			}
			Initialize();
		}

		/// <summary>
		/// Initializes the widget handler
		/// </summary>
		/// <remarks>
		/// This is typically called from the constructor after all of the logic is completed to construct
		/// the object.
		/// 
		/// If your handler interface has the <see cref="AutoInitializeAttribute"/> set to false, then you are responsible
		/// for calling this method in your constructor after calling the creation method on your custom handler.
		/// </remarks>
		protected void Initialize()
		{
			var handler = Handler as IWidget;
			if (handler != null)
				handler.Initialize();
			Eto.Style.OnStyleWidgetDefaults(this);
			EventLookup.HookupEvents(this);
		}

		/// <summary>
		/// Disposes of this widget, supressing the finalizer
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Handles the disposal of this widget
		/// </summary>
		/// <param name="disposing">True if the caller called <see cref="Dispose()"/> manually, false if being called from a finalizer</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				var handler = Handler as IDisposable;
				if (handler != null)
					handler.Dispose();
				Handler = null;
			}
			#if TRACK_GC
			Console.WriteLine ("{0}: {1}", disposing ? "Dispose" : "GC", GetType().Name);
			#endif
		}
	}
}

