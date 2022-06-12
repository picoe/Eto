using System;
using System.Reflection;

namespace Eto
{
	/// <summary>
	/// Interface for a style provider
	/// </summary>
	/// <remarks>
	/// This can be used to create your own style engine to apply styles to widgets.
	/// The <see cref="DefaultStyleProvider"/> is a good example of how to implement
	/// a style provider.
	/// </remarks>
	public interface IStyleProvider
	{
		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Eto.IStyleProvider"/> allows inheriting styles from the parent object.
		/// </summary>
		/// <remarks>
		/// This is used when applying cascading styles, sometimes the user or the provider will not want
		/// parent styles to be applicable.
		/// </remarks>
		/// <value><c>true</c> to inherit parent styles; otherwise, <c>false</c>.</value>
		bool Inherit { get; }

		/// <summary>
		/// Applies cascading styles to the specified <paramref name="widget"/>.
		/// </summary>
		/// <remarks>
		/// Cascading styles are applied on a widget when it is loaded onto a form, 
		/// where styles are usually applied based on the container it is in.
		/// </remarks>
		/// <param name="container">Container that is applying styles. This can be the same as <paramref name="widget"/>.</param>
		/// <param name="widget">Widget to apply the styles to.</param>
		/// <param name="style">Style to apply to the widget, usually a space delimited list of styles.</param>
		void ApplyCascadingStyle(object container, object widget, string style);

		/// <summary>
		/// Applies styles to the specified <paramref name="widget"/>
		/// </summary>
		/// <remarks>
		/// This is used for global style providers when applying styles to a widget directly based on its style alone.
		/// 
		/// This is usually applied when the style itself is updated.
		/// </remarks>
		/// <param name="widget">Widget to apply the styles to.</param>
		/// <param name="style">Style to apply to the widget, usually a space delimited list of styles.</param>
		void ApplyStyle(object widget, string style);

		/// <summary>
		/// Applies default styling to the specified widget after creation.
		/// </summary>
		/// <param name="widget">Widget to apply default styles to.</param>
		void ApplyDefault(object widget);
	}

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
		static DefaultStyleProvider styles;

		/// <summary>
		/// Gets or sets the style provider.
		/// </summary>
		/// <remarks>
		/// By default, this is an instance of <see cref="DefaultStyleProvider"/>.
		/// If you set this value, any styles applied using <see cref="Add{THandler}(string, StyleHandler{THandler})"/> or
		/// <see cref="Add{TWidget}(string, StyleWidgetHandler{TWidget})"/> will no longer be used.
		/// </remarks>
		/// <value>The provider.</value>
		public static IStyleProvider Provider { get; set; }


		static DefaultStyleProvider Styles => styles ?? CreateDefaultProvider();

		static DefaultStyleProvider CreateDefaultProvider()
		{
			styles = new DefaultStyleProvider();
			styles.StyleWidget += OnStyleWidget;
			Provider = styles;
			return styles;
		}

		static void OnStyleWidget(object obj)
		{
			if (obj is Widget w)
				StyleWidget?.Invoke(w);
		}

		#region Events

		/// <summary>
		/// Event to handle when a widget has being styled
		/// </summary>
		public static event Action<Widget> StyleWidget;

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
			Styles.Add<TWidget>(style, w => handler(w));
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
			Styles.Add<THandler>(style, w => styleHandler(w));
		}
	}
}

