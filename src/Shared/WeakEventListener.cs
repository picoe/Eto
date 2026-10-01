namespace Eto;

/// <summary>
/// Subscribes to an event while only holding the target weakly, so a long lived source doesn't keep it alive.
/// </summary>
/// <remarks>
/// Keep the listener in a field of the target. Once the target is collected the subscription is removed from the
/// source, even if the event never fires again.  Call <see cref="Detach"/> to unsubscribe sooner.
/// </remarks>
sealed class WeakEventListener
{
	// Only this is referenced by the source, so it must not reference us or we'd never be finalized.
	readonly Subscription _subscription;

	WeakEventListener(Subscription subscription)
	{
		_subscription = subscription;
	}

	/// <summary>
	/// Subscribes to an event of <paramref name="source"/>, forwarding it to <paramref name="target"/>.
	/// </summary>
	/// <example>
	/// <code>
	/// _listener = WeakEventListener.Create(source, this,
	/// 	static (s, l) => s.PropertyChanged += l.OnEvent,
	/// 	static (s, l) => s.PropertyChanged -= l.OnEvent,
	/// 	static (MyHandler h, object sender, PropertyChangedEventArgs e) => h.OnPropertyChanged(sender, e));
	/// </code>
	/// </example>
	/// <param name="source">Object that raises the event</param>
	/// <param name="target">Target to forward the event to, only held weakly</param>
	/// <param name="add">Adds <c>l.OnEvent</c> to the event</param>
	/// <param name="remove">Removes <c>l.OnEvent</c> from the event</param>
	/// <param name="handler">Called with the target; use a static lambda, and give it explicit parameter types so the generic types are inferred</param>
	public static WeakEventListener Create<TSource, TTarget, TArgs>(TSource source, TTarget target, Action<TSource, Subscription<TSource, TTarget, TArgs>> add, Action<TSource, Subscription<TSource, TTarget, TArgs>> remove, Action<TTarget, object, TArgs> handler)
		where TSource : class
		where TTarget : class
	{
		return new WeakEventListener(new Subscription<TSource, TTarget, TArgs>(source, target, add, remove, handler));
	}

	/// <summary>
	/// Subscribes to <see cref="INotifyCollectionChanged.CollectionChanged"/>, forwarding it to <paramref name="target"/>.
	/// </summary>
	/// <param name="source">Collection to listen to</param>
	/// <param name="target">Target to forward the event to, only held weakly</param>
	/// <param name="handler">Called with the target; use a static lambda</param>
	public static WeakEventListener Create<TTarget>(INotifyCollectionChanged source, TTarget target, Action<TTarget, object, NotifyCollectionChangedEventArgs> handler)
		where TTarget : class
	{
		return Create(source, target, static (s, l) => s.CollectionChanged += l.OnEvent, static (s, l) => s.CollectionChanged -= l.OnEvent, handler);
	}

	~WeakEventListener()
	{
		// An exception escaping a finalizer terminates the process.
		try
		{
			if (Environment.HasShutdownStarted)
				return;
			// Sources (e.g. collections) usually aren't thread safe, so unsubscribe on the UI thread.
			var application = Eto.Forms.Application.Instance;
			if (application != null)
				application.AsyncInvoke(_subscription.Detach);
			else
				_subscription.Detach();
		}
		catch
		{
		}
	}

	/// <summary>
	/// Stops listening to the source.
	/// </summary>
	public void Detach()
	{
		_subscription.Detach();
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Subscription held by the source.
	/// </summary>
	public abstract class Subscription
	{
		/// <summary>
		/// Removes the subscription from the source.
		/// </summary>
		public abstract void Detach();
	}

	/// <summary>
	/// Subscription held by the source, add <see cref="OnEvent"/> to the event.
	/// </summary>
	public sealed class Subscription<TSource, TTarget, TArgs> : Subscription
		where TSource : class
		where TTarget : class
	{
		readonly WeakReference<TTarget> _target;
		readonly Action<TSource, Subscription<TSource, TTarget, TArgs>> _remove;
		readonly Action<TTarget, object, TArgs> _handler;
		TSource _source;

		internal Subscription(TSource source, TTarget target, Action<TSource, Subscription<TSource, TTarget, TArgs>> add, Action<TSource, Subscription<TSource, TTarget, TArgs>> remove, Action<TTarget, object, TArgs> handler)
		{
			_source = source;
			_target = new WeakReference<TTarget>(target);
			_remove = remove;
			_handler = handler;
			add(source, this);
		}

		/// <summary>
		/// Event handler to add to and remove from the event.
		/// </summary>
		public void OnEvent(object sender, TArgs e)
		{
			if (_target.TryGetTarget(out var target))
				_handler(target, sender, e);
			else
				Detach();
		}

		/// <inheritdoc/>
		public override void Detach()
		{
			// Can race between the finalizer and an event when there's no UI thread to marshal to.
			var source = Interlocked.Exchange(ref _source, null);
			if (source != null)
				_remove(source, this);
		}
	}
}
