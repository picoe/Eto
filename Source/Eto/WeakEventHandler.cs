using System;
using System.Reflection;

namespace Eto
{
	public delegate void UnregisterCallback<E> (EventHandler<E> eventHandler)
  		where E: EventArgs;

	public interface IWeakEventHandler<E>
  		where E: EventArgs
	{
		EventHandler<E> Handler { get; }
	}

	public class WeakEventHandler<T, E>: IWeakEventHandler<E>
  		where T: class
  		where E: EventArgs
	{
		delegate void OpenEventHandler (T @this, object sender,E e);

		WeakReference reference;
		OpenEventHandler openHandler;
		EventHandler<E> handler;
		UnregisterCallback<E> unregister;

		public WeakEventHandler (EventHandler<E> eventHandler, UnregisterCallback<E> unregister = null)
		{
			this.reference = new WeakReference (eventHandler.Target);
			this.openHandler = (OpenEventHandler)Delegate.CreateDelegate (typeof(OpenEventHandler), null, eventHandler.Method);
			this.handler = Invoke;
			this.unregister = unregister;
		}

		public void Invoke (object sender, E e)
		{
			T target = (T)reference.Target;

			if (target != null)
				openHandler.Invoke (target, sender, e);
			else if (unregister != null) {
				unregister (handler);
				unregister = null;
			}
		}

		public EventHandler<E> Handler {
			get { return handler; }
		}

		public static implicit operator EventHandler<E> (WeakEventHandler<T, E> weh) {
			return weh.handler;
		}
	}

	public static class EventHandlerUtils
	{
				
		public static EventHandler<E> MakeWeak<E> (this EventHandler<E> eventHandler, UnregisterCallback<E> unregister = null)
    		where E: EventArgs
		{
			if (eventHandler == null)
				throw new ArgumentNullException ("eventHandler");
			if (eventHandler.Method.IsStatic || eventHandler.Target == null)
				throw new ArgumentException ("Only instance methods are supported.", "eventHandler");

			Type wehType = typeof(WeakEventHandler<,>).MakeGenericType (eventHandler.Method.DeclaringType, typeof(E));
			var wehConstructor = wehType.GetConstructor (new Type[] { typeof(EventHandler<E>), typeof(UnregisterCallback<E>) });

			var weh = (IWeakEventHandler<E>)wehConstructor.Invoke (new object[] { eventHandler, unregister });

			return weh.Handler;
		}
	}
}
