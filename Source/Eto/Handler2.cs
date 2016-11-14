using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Eto
{
	public class DependencyProperty<TControl, TValue>
	{
		internal object EventKey = new object();

		public TValue DefaultValue { get; set; }

		public DependencyProperty()
		{
		}

		public DependencyProperty(TValue defaultValue)
		{
			DefaultValue = defaultValue;
		}

		public bool IsSupported
		{
			get
			{
				var h = Platform.Instance.CreateShared(typeof(TControl)) as IHandler2;
				return h.SupportsProperty(this);
			}
		}
	}

	public class DependencyEvent<TControl, TArgs>
		where TArgs : EventArgs
	{
		Action<TControl, TArgs> _callback;
		Expression<Action<TControl, TArgs>> _callbackExpression;
		Action<TControl, TArgs> Callback => _callback ?? (_callback = _callbackExpression.Compile());

		string _id;
		public string ID => _id ?? (_id = Guid.NewGuid().ToString());

		public DependencyEvent(Expression<Action<TControl, TArgs>> callback)
		{
			_callbackExpression = callback;
			EventLookup.Register(callback, this);
		}

		public void Raise(TControl widget, TArgs e)
		{
			var callback = Callback;
			if (callback != null)
			{
				using ((widget as Widget)?.Platform.Context)
					callback(widget, e);
			}
		}

		internal void Attach(TControl widget)
		{
			((IHandler2)(widget as Widget).Handler).AttachEvent(widget, this);
		}

		public bool IsSupported
		{
			get
			{
				var h = Platform.Instance.CreateShared(typeof(TControl)) as IHandler2;
				return h?.SupportsEvent(this) ?? false;
			}
		}

	}

	public interface IHandler2
	{
		void Initialize(object widget);
		void AttachEvent(object widget, object evt);
		bool TryGetValue(object widget, object prop, out object value);
		bool TrySetValue(object widget, object prop, object value);
		bool SupportsEvent(object evt);
		bool SupportsProperty(object prop);
	}

	public class WidgetHandler2<TWidget, TControl> : WidgetHandler2<TWidget>
		where TWidget : Widget
		where TControl : new()
	{
		static object CtlProp = new object();

		public override void Initialize(TWidget widget)
		{
			base.Initialize(widget);
			SetControl(widget, new TControl());
		}

		public static TControl GetControl(TWidget widget)
		{
			return widget.Properties.Get<TControl>(CtlProp);
		}
		public static void SetControl(TWidget widget, TControl value)
		{
			widget.Properties.Set(CtlProp, value);
		}
	}

	public class WidgetHandler2<TWidget> : IHandler2
	{
		public virtual void Initialize(TWidget widget)
		{
		}

		public virtual Action<TWidget> GetEvent(object evt)
		{
			return null;
		}

		public virtual Func<TWidget, object> GetProperty(object property)
		{
			return null;
		}

		public virtual Action<TWidget, object> SetProperty(object property)
		{
			return null;
		}

		static Dictionary<object, Action<TWidget>> s_events;
		static Dictionary<object, Action<TWidget>> events => s_events ?? (s_events = new Dictionary<object, Action<TWidget>>());
		static Dictionary<object, Func<TWidget, object>> s_getprops;
		static Dictionary<object, Func<TWidget, object>> getprops => s_getprops ?? (s_getprops = new Dictionary<object, Func<TWidget, object>>());
		static Dictionary<object, Action<TWidget, object>> s_setprops;
		static Dictionary<object, Action<TWidget, object>> setprops => s_setprops ?? (s_setprops = new Dictionary<object, Action<TWidget, object>>());


		void IHandler2.AttachEvent(object widget, object evt) => GetAttachEvent(evt)?.Invoke((TWidget)widget);

		Action<TWidget> GetAttachEvent(object evt)
		{
			Action<TWidget> del;
			if (events.TryGetValue(evt, out del))
				return del;

			del = GetEvent(evt);
			events.Add(evt, del);
			return del;
		}

		bool IHandler2.SupportsEvent(object evt) => GetAttachEvent(evt) != null;

		bool IHandler2.SupportsProperty(object prop) => GetSetProperty(prop) != null || GetGetProperty(prop) != null;

		bool IHandler2.TryGetValue(object widget, object prop, out object value)
		{
			var getDelegate = GetProperty(prop);
			if (getDelegate != null)
			{
				value = getDelegate((TWidget)widget);
				return true;
			}
			value = null;
			return false;
		}

		bool IHandler2.TrySetValue(object widget, object prop, object value)
		{
			var setDelegate = SetProperty(prop);
			if (setDelegate != null)
			{
				setDelegate((TWidget)widget, value);
				return true;
			}
			return false;
		}

		Action<TWidget, object> GetSetProperty(object prop)
		{
			Action<TWidget, object> del;
			if (setprops.TryGetValue(prop, out del))
				return del;

			del = SetProperty(prop);
			setprops.Add(prop, del);
			return del;
		}

		Func<TWidget, object> GetGetProperty(object prop)
		{
			Func<TWidget, object> del;
			if (getprops.TryGetValue(prop, out del))
				return del;

			del = GetProperty(prop);
			getprops.Add(prop, del);
			return del;
		}

		void IHandler2.Initialize(object widget) => Initialize((TWidget)widget);
	}
}
