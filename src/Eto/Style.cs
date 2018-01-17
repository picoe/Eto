using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eto
{
	/// <summary>
	/// Delegate to handle styling a widget
	/// </summary>
	/// <remarks>
	/// This allows you to add additional logic or set properties on the widget based on the styles set on the widget.
	/// </remarks>
	/// <typeparam name="TWidget">Type of widget to style</typeparam>
	/// <param name="widget">Widget instance that is being styled</param>
	public delegate void StyleWidgetHandler<TWidget>(TWidget widget)
		where TWidget: Widget;
	/// <summary>
	/// Delegate to handle styling a widget handler
	/// </summary>
	/// <remarks>
	/// This allows you to add additional logic or set properties on the widget and platform-specific control(s)
	/// based on the styles set on the widget.
	/// </remarks>
	/// <param name="handler">Handler instance that is being styled</param>
	/// <typeparam name="THandler">Type of the handler to style</typeparam>
	public delegate void StyleHandler<THandler>(THandler handler)
		where THandler: Widget.IHandler;
	/// <summary>
	/// Style manager for widgets
	/// </summary>
	/// <remarks>
	/// Styles allow you to attach custom platform-specific logic to a widget.
	/// In your platform-specific assembly, use Style.Add&lt;H&gt;(string, StyleHandler&lt;H&gt;)
	/// to add the style logic with the same id.
	/// 
	/// Typically, your styles will be added in your platform-specific executable,
	/// before your application is run.
	/// </remarks>
	/// <example>
	/// Style the widget, with no direct access to platform-specifics
	/// <code><![CDATA[
	/// Style.Add<Form>("mainForm", widget => {
	///		widget.Title = "Hello!";
	/// });
	/// ]]></code>
	/// 
	/// Style based on a platform-specific handler (this is for Mac OS X):
	/// <code><![CDATA[
	/// Style.Add<Eto.Mac.Forms.FormHandler>("mainForm", handler => {
	///		handler.Control.CollectionBehavior |= NSWindowCollectionBehavior.FullScreenPrimary;
	/// });
	/// Style.Add<Eto.Mac.Forms.ApplicationHandler>("application", handler => {
	///		handler.EnableFullScreen ();
	/// });
	/// 
	/// // create the UI (typically this is in your UI library, not in the platform-specific assembly)
	/// var app = new Application {
	///		Style = "application";  // will apply the style here
	///	};
	/// 
	/// app.Initialized += delegate {
	///		app.MainForm = new Form { Style = "mainForm" }; // apply the mainForm style
	///		app.MainForm.Show ();
	/// };
	/// 
	/// ]]></code>
	/// </example>
	public static class Style
	{
		static readonly Dictionary<object, IList<Action<Widget>>> styleMap = new Dictionary<object, IList<Action<Widget>>>();
		static readonly Dictionary<object, IList<Action<Widget>>> cascadingStyleMap = new Dictionary<object, IList<Action<Widget>>>();

		#region Events

		/// <summary>
		/// Event to handle when a widget has being styled
		/// </summary>
		public static event Action<Widget> StyleWidget;

		internal static void OnStyleWidget(Widget widget)
		{
			if (widget != null && !string.IsNullOrEmpty(widget.Style))
			{
				var styles = widget.Style.Split(' ');
				for (int i = 0; i < styles.Length; i++)
				{
					var style = styles[i];
					var styleHandlers = GetStyleList(style);
					if (styleHandlers != null)
					{
						for (int j = 0; j < styleHandlers.Count; j++)
						{
							var styleHandler = styleHandlers[j];
							styleHandler(widget);
						}
					}
				}
			}
			if (StyleWidget != null)
				StyleWidget(widget);
		}

		internal static void OnStyleWidgetDefaults(Widget widget)
		{
			if (widget == null)
				return;
			
			var styleHandlers = GetCascadingStyleList(widget.GetType());
			if (styleHandlers == null)
				return;
			
			for (int i = 0; i < styleHandlers.Count; i++)
			{
				var styleHandler = styleHandlers[i];
				styleHandler(widget);
			}
		}

		internal static void OnStyleWidgetDefaults(Widget.IHandler handler)
		{
			if (handler == null)
				return;
			
			var widget = handler.Widget;
			if (widget == null)
				return;
			
			var styleHandlers = GetCascadingStyleList(handler.GetType());
			if (styleHandlers == null)
				return;
			
			for (int i = 0; i < styleHandlers.Count; i++)
			{
				var styleHandler = styleHandlers[i];
				styleHandler(widget);
			}
		}

		#endregion

		/// <summary>
		/// Adds a style for a widget
		/// </summary>
		/// <remarks>
		/// Styling a widget allows you to access the widget, but not the platform-specific controls (in a type-safe way).
		/// 
		/// Typically, you'd use Style.Add&lt;H&gt;(string, StyleHandler&lt;H&gt;) instead, which will add a style based on the widget handler, which
		/// will give you direct and type safe access to platform-specifics of the widget.
		/// </remarks>
		/// <typeparam name="TWidget">Type of the widget to style</typeparam>
		/// <param name="style">Identifier of the style</param>
		/// <param name="handler">Delegate with your logic to style the widget</param>
		public static void Add<TWidget>(string style, StyleWidgetHandler<TWidget> handler)
			where TWidget: Widget
		{
			var list = CreateStyleList((object)style ?? typeof(TWidget));
			list.Add(widget =>
			{
				var control = widget as TWidget;
				if (control != null)
					handler(control);
			});
			cascadingStyleMap.Clear();
		}

		/// <summary>
		/// Adds a style for a widget handler
		/// </summary>
		/// <remarks>
		/// Styling a widget handler allows you to access both the widget and the platform-specifics for the widget.
		/// 
		/// To use this, you would have to add a reference to one of the Eto.*.dll's so that you can utilize
		/// the platform handler directly.  Typically this would be called before your application is run.
		/// </remarks>
		/// <typeparam name="THandler">Type of the handler that should be styled</typeparam>
		/// <param name="style">Identifier for the style</param>
		/// <param name="styleHandler">Delegate with your logic to style the widget and/or platform control</param>
		public static void Add<THandler>(string style, StyleHandler<THandler> styleHandler)
			where THandler: class, Widget.IHandler
		{
			var list = CreateStyleList((object)style ?? typeof(THandler));
			list.Add(widget =>
			{
				var handler = widget.Handler as THandler;
				if (handler != null)
					styleHandler(handler);
			});
			cascadingStyleMap.Clear();
		}

		static IList<Action<Widget>> CreateStyleList(object style)
		{
			IList<Action<Widget>> styleHandlers;
			if (!styleMap.TryGetValue(style, out styleHandlers))
			{
				styleHandlers = new List<Action<Widget>>();
				styleMap[style] = styleHandlers;
			}
			return styleHandlers;
		}

		static IList<Action<Widget>> GetStyleList(object style)
		{
			IList<Action<Widget>> styleHandlers;
			return styleMap.TryGetValue(style, out styleHandlers) ? styleHandlers : null;
		}

		static IList<Action<Widget>> GetCascadingStyleList(Type type)
		{
			// get a cached list of cascading styles so we don't have to traverse each time
			IList<Action<Widget>> childHandlers;
			if (cascadingStyleMap.TryGetValue(type, out childHandlers))
			{
				return childHandlers;
			}

			// don't have a cascading style set, so build one
			// styles are applied in order from superclass styles down to subclass styles.
			IEnumerable<Action<Widget>> styleHandlers = Enumerable.Empty<Action<Widget>>();
			Type currentType = type;
			do
			{
				IList<Action<Widget>> typeStyles;
				if (styleMap.TryGetValue(currentType, out typeStyles) && typeStyles != null)
					styleHandlers = typeStyles.Concat(styleHandlers);
			}
			while ((currentType = currentType.GetBaseType()) != null);

			// create a cached list, but if its empty don't store it
			childHandlers = styleHandlers.ToList();
			if (childHandlers.Count == 0)
				childHandlers = null;
			cascadingStyleMap.Add(type, childHandlers);

			return childHandlers;
		}

	}
}

