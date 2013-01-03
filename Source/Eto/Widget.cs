using System;
using System.Collections.Generic;

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

		/// <summary>
		/// Initializes the widget after it has been constructed
		/// </summary>
		/// <remarks>
		/// This is typically called automatically when passing the initialize value to 
		/// a constructor of the widget to true.
		/// 
		/// For widget implementors, if you have any constructor code that must be called before Initialize
		/// is called on the widget handler, then you would pass false to the constructor's initialize parameter,
		/// then call this manually (via <see cref="M:Widget.Initialize()"/>
		/// </remarks>
		void Initialize ();

		Generator Generator { get; set; }
	}

	/// <summary>
	/// Interface for widgets that have a control object
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IControlObjectSource
	{
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
		Generator Generator { get; }
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
		BindingCollection bindings;
		PropertyStore properties;

		/// <summary>
		/// Gets the attached properties for this widget
		/// </summary>
		public PropertyStore Properties
		{
			get
			{
				if (properties == null) properties = new PropertyStore (this);
				return properties;
			}
		}

		/// <summary>
		/// Gets the generator that was used to create the <see cref="Handler"/> for this widget
		/// </summary>
		/// <remarks>
		/// The generator is typically either passed to the constructor of the control, or the
		/// <see cref="P:Generator.Current"/> is used.
		/// </remarks>
		public Generator Generator { get { return ((IWidget)Handler).Generator; } }
		
		/// <summary>
		/// Gets the collection of bindings that are attached to this widget
		/// </summary>
		public BindingCollection Bindings {
			get {
				if (bindings == null) bindings = new BindingCollection (); 
				return bindings;
			}
		}

		/// <summary>
		/// Gets or sets a user-defined object that contains data about the control
		/// </summary>
		/// <remarks>
		/// A common use of the tag property is to store data that is associated with the control that you can later
		/// retrieve.
		/// </remarks>
		public object Tag { get; set; }

		/// <summary>
		/// Gets the platform-specific handler for this widget
		/// </summary>
		public object Handler { get; internal set; }

		/// <summary>
		/// Finalizes this widget
		/// </summary>
		~Widget ()
		{
			//Console.WriteLine ("GC: {0}", this.GetType ().FullName);
			Dispose (false);
		}
		
		/// <summary>
		/// Initializes a new instance of the Widget class
		/// </summary>
		/// <param name="generator">Generator the widget handler was created with, or null to use <see cref="Eto.Generator.Current"/></param>
		/// <param name="handler">Handler to assign to this widget for its implementation</param>
		/// <param name="initialize">True to initialize the widget, false to defer that to the caller</param>
		protected Widget (Generator generator, IWidget handler, bool initialize = true)
		{
			if (generator == null)
				generator = Generator.Current;
			this.Handler = handler;
			handler.Generator = generator;
			handler.Widget = this; // tell the handler who we are
			if (initialize)
				Initialize ();
		}

		/// <summary>
		/// Initializes a new instance of the Widget class
		/// </summary>
		/// <param name="generator">Generator to create the handler with, or null to use <see cref="Eto.Generator.Current"/></param>
		/// <param name="type">Type of widget handler to create from the generator for this widget</param>
		/// <param name="initialize">True to initialize the widget, false to defer that to the caller</param>
		protected Widget (Generator generator, Type type, bool initialize = true)
		{
			if (generator == null)
				generator = Generator.Current;
			this.Handler = generator.Create (type);
			var widgetHandler = this.Handler as IWidget;
			if (widgetHandler != null) {
				widgetHandler.Generator = generator;
				widgetHandler.Widget = this;
			}

			if (initialize)
				Initialize ();
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
		protected void Initialize ()
		{
			((IWidget)Handler).Initialize ();
		}
		
		#region IDisposable Members

		/// <summary>
		/// Disposes of this widget, supressing the finalizer
		/// </summary>
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}
		
		#endregion
		
		/// <summary>
		/// Unbinds any bindings in the <see cref="Bindings"/> collection and removes the bindings
		/// </summary>
		public virtual void Unbind ()
		{
			if (bindings != null) {
				bindings.Unbind();
				bindings = null;
			}
		}
		
		/// <summary>
		/// Updates all bindings in this widget
		/// </summary>
		public virtual void UpdateBindings ()
		{
			if (bindings != null) {
				bindings.Update ();
			}
		}
		
		/// <summary>
		/// Handles the disposal of this widget
		/// </summary>
		/// <param name="disposing">True if the caller called <see cref="Dispose()"/> manually, false if this is called from the finalizer</param>
		protected virtual void Dispose (bool disposing)
		{
			Unbind ();
			if (disposing) {
				var handler = this.Handler as IDisposable;
				if (handler != null)
					handler.Dispose ();
				this.Handler = null;
			}
		}		
	}
}

