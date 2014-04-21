using System;
using System.Collections.Generic;

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
		where TWidget: InstanceWidget;
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
		where THandler: IWidget;
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
	/// Style.Add<Eto.Platform.Mac.Forms.FormHandler>("mainForm", handler => {
	///		handler.Control.CollectionBehavior |= NSWindowCollectionBehavior.FullScreenPrimary;
	/// });
	/// Style.Add<Eto.Platform.Mac.Forms.ApplicationHandler>("application", handler => {
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
		static readonly Dictionary<object, IList<Action<InstanceWidget>>> styleMap = new Dictionary<object, IList<Action<InstanceWidget>>>();

		#region Events

		/// <summary>
		/// Event to handle when a widget has being styled
		/// </summary>
		public static event Action<InstanceWidget> StyleWidget;

		internal static void OnStyleWidget(InstanceWidget widget)
		{
			if (widget != null && !string.IsNullOrEmpty(widget.Style))
			{
				var styles = widget.Style.Split(' ');
				foreach (var style in styles)
				{
					var styleHandlers = GetStyleList(style, false);
					if (styleHandlers != null)
					{
						foreach (var styleHandler in styleHandlers)
							styleHandler(widget);
					}
				}
			}
			if (StyleWidget != null)
				StyleWidget(widget);
		}

		internal static void OnStyleWidgetDefaults(InstanceWidget widget)
		{
			if (widget != null)
			{
				var styleHandlers = GetStyleList(widget.GetType(), false);
				if (styleHandlers != null)
				{
					foreach (var styleHandler in styleHandlers)
						styleHandler(widget);
				}
			}
		}

		internal static void OnStyleWidgetDefaults(IInstanceWidget handler)
		{
			if (handler != null)
			{
				var styleHandlers = GetStyleList(handler.GetType(), false);
				if (styleHandlers != null)
				{
					var widget = handler.Widget as InstanceWidget;
					if (widget != null)
						foreach (var styleHandler in styleHandlers)
							styleHandler(widget);
				}
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
			where TWidget: InstanceWidget
		{
			var list = GetStyleList((object)style ?? typeof(TWidget));
			list.Add(delegate (InstanceWidget widget)
			{
				var control = widget as TWidget;
				if (control != null)
					handler(control);
			});
		}

		/// <summary>
		/// Adds a style for a widget handler
		/// </summary>
		/// <remarks>
		/// Styling a widget handler allows you to access both the widget and the platform-specifics for the widget.
		/// 
		/// To use this, you would have to add a reference to one of the Eto.Platform.*.dll's so that you can utilize
		/// the platform handler directly.  Typically this would be called before your application is run.
		/// </remarks>
		/// <typeparam name="THandler">Type of the handler that should be styled</typeparam>
		/// <param name="style">Identifier for the style</param>
		/// <param name="styleHandler">Delegate with your logic to style the widget and/or platform control</param>
		public static void Add<THandler>(string style, StyleHandler<THandler> styleHandler)
			where THandler: class, IWidget
		{
			var list = GetStyleList((object)style ?? typeof(THandler));
			list.Add(delegate (InstanceWidget widget)
			{
				var handler = widget.Handler as THandler;
				if (handler != null)
					styleHandler(handler);
			});
		}

		static IList<Action<InstanceWidget>> GetStyleList(object style, bool create = true)
		{
			IList<Action<InstanceWidget>> styleHandlers;
			if (!styleMap.TryGetValue(style, out styleHandlers) && create)
			{
				styleHandlers = new List<Action<InstanceWidget>>();
				styleMap[style] = styleHandlers;
			}
			return styleHandlers;
		}
	}
}

