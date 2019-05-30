using System;
namespace Eto
{
	/// <summary>
	/// Exports a handler from a 3rd party assembly.
	/// </summary>
	/// <remarks>
	/// Use this to register a custom control from your custom assembly.
	/// 
	/// Use <see cref="ExportInitializerAttribute"/> when registering a lot of controls or to perform additional logic.
	/// </remarks>
	[System.AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
	public sealed class ExportHandlerAttribute : PlatformExtensionAttribute
	{
		/// <summary>
		/// Gets the type of the widget or handler interface to map to
		/// </summary>
		/// <value>The type of the widget or handler.</value>
		public Type WidgetType { get; private set; }

		/// <summary>
		/// Gets the type of the handler to instantiate
		/// </summary>
		/// <value>The type of the handler for the widget.</value>
		public Type HandlerType { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.ExportHandlerAttribute"/> class.
		/// </summary>
		/// <param name="widgetType">Widget or handler interface type.</param>
		/// <param name="handlerType">Handler that implements the widget's handler interface.</param>
		public ExportHandlerAttribute(Type widgetType, Type handlerType)
		{
			WidgetType = widgetType ?? throw new ArgumentNullException(nameof(widgetType));
			HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
		}

		/// <summary>
		/// Registers the extension with the specified platform
		/// </summary>
		/// <returns><c>true</c>, if the extension was applied, <c>false</c> otherwise.</returns>
		/// <param name="platform">Platform to register this extension with.</param>
		public override void Register(Platform platform)
		{
			platform.Add(WidgetType, CreateHandler);
		}

		object CreateHandler() => Activator.CreateInstance(HandlerType);
	}
}
