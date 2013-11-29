using System;
using System.Reflection;

namespace Eto
{
	/// <summary>
	/// Delegate to call back when unregistering an event for the <see cref="WeakEventHandler{T,E}"/>
	/// </summary>
	/// <typeparam name="TArgs">Type of <see cref="EventArgs"/> used in the event handler</typeparam>
	/// <param name="eventHandler">The event handler that was registered that should be unregistered</param>
	public delegate void UnregisterCallback<TArgs> (EventHandler<TArgs> eventHandler)
  		where TArgs: EventArgs;

	/// <summary>
	/// Interface to the <see cref="WeakEventHandler{T,E}"/>
	/// </summary>
	/// <remarks>
	/// This is used by the <see cref="WeakEventExtensions"/>
	/// </remarks>
	/// <typeparam name="TArgs"></typeparam>
	public interface IWeakEventHandler<TArgs>
  		where TArgs: EventArgs
	{
		/// <summary>
		/// Gets the event handler to attach to the event
		/// </summary>
		EventHandler<TArgs> Handler { get; }
	}

	/// <summary>
	/// Implementation of the Weak Event Handler pattern
	/// </summary>
	/// <remarks>
	/// This allows you to hook up events to an object without preventing
	/// the object from being garbage collected due to the lifetime of the event handler.
	/// 
	/// You should use the helper method <see cref="WeakEventExtensions.MakeWeak{E}"/> to set up the weak event.
	/// </remarks>
	/// <typeparam name="T">Type of the class the event handler is on</typeparam>
	/// <typeparam name="TArgs">Type of arguments for the event handler</typeparam>
	public class WeakEventHandler<T, TArgs>: IWeakEventHandler<TArgs>
  		where T: class
  		where TArgs: EventArgs
	{
		delegate void OpenEventHandler (T @this, object sender,TArgs e);

		readonly WeakReference reference;
		readonly OpenEventHandler openHandler;
		readonly EventHandler<TArgs> handler;
		UnregisterCallback<TArgs> unregister;

		/// <summary>
		/// Initializes a new instance of the WeakEventHandler
		/// </summary>
		/// <param name="eventHandler">event handler to hook up</param>
		/// <param name="unregister">callback to unregister the event handler</param>
		public WeakEventHandler (EventHandler<TArgs> eventHandler, UnregisterCallback<TArgs> unregister = null)
		{
#if WINRT
			throw new NotImplementedException();
#else
			this.reference = new WeakReference (eventHandler.Target);
			this.openHandler = (OpenEventHandler)Delegate.CreateDelegate (typeof(OpenEventHandler), null, eventHandler.Method);
			this.handler = Invoke;
			this.unregister = unregister;
#endif
		}

		/// <summary>
		/// Invokes the weak event handler
		/// </summary>
		/// <param name="sender">Sender of the event</param>
		/// <param name="e">Event arguments</param>
		public void Invoke (object sender, TArgs e)
		{
			var target = (T)reference.Target;

			if (target != null)
				openHandler.Invoke (target, sender, e);
			else if (unregister != null) {
				unregister (handler);
				unregister = null;
			}
		}

		/// <summary>
		/// Gets the handler to attach to the long lived object
		/// </summary>
		public EventHandler<TArgs> Handler {
			get { return handler; }
		}

		/// <summary>
		/// Implicitly converts
		/// </summary>
		/// <param name="weh"></param>
		/// <returns></returns>
		public static implicit operator EventHandler<TArgs> (WeakEventHandler<T, TArgs> weh) {
			return weh.handler;
		}
	}

	/// <summary>
	/// Extensions for weak events
	/// </summary>
	public static class WeakEventExtensions
	{
		/// <summary>
		/// Makes an event handler weak bound
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// 
		/// void SetupEvent (MyLongLived longLived)
		/// {
		///		longLived.SomeEvent += new EventHandler<EventArgs>(SomeEventHandler).MakeWeak(e => longLived.SomeEvent -= e);
		/// }
		/// 
		/// void SomeEventHandler(object sender, EventArgs e)
		/// {
		///		// handler on short lived object
		/// }
		/// ]]></code>
		/// </example>
		/// <typeparam name="E">Type of <see cref="EventArgs"/> for the event</typeparam>
		/// <param name="eventHandler">Event handler to wire up</param>
		/// <param name="unregister">Delegate to unregister the event handler</param>
		/// <returns>Event handler to attach to the long lived object</returns>
		public static EventHandler<TArgs> MakeWeak<TArgs> (this EventHandler<TArgs> eventHandler, UnregisterCallback<TArgs> unregister = null)
			where TArgs: EventArgs
		{
#if WINRT
			throw new NotImplementedException();
#else
			if (eventHandler == null)
				throw new ArgumentNullException ("eventHandler");
			if (eventHandler.Method.IsStatic || eventHandler.Target == null)
				throw new ArgumentException ("Only instance methods are supported.", "eventHandler");

			Type wehType = typeof(WeakEventHandler<,>).MakeGenericType (eventHandler.Method.DeclaringType, typeof(TArgs));
			var wehConstructor = wehType.GetConstructor (new Type[] { typeof(EventHandler<TArgs>), typeof(UnregisterCallback<TArgs>) });

			var weh = (IWeakEventHandler<TArgs>)wehConstructor.Invoke (new object[] { eventHandler, unregister });

			return weh.Handler;
#endif
		}
	}
}
