using System;
namespace Eto
{
	/// <summary>
	/// Platform extension for 3rd party libraries to provide additional controls or functionality
	/// </summary>
	/// <remarks>
	/// This is implemented for types that are referenced using the <see cref="PlatformExtensionAttribute"/>.
	/// </remarks>
	public interface IPlatformExtension
	{
		/// <summary>
		/// Called to add specific functionality to the specified <paramref name="platform"/>
		/// </summary>
		/// <param name="platform">Platform to add the extension to</param>
		void Initialize(Platform platform);
	}

	/// <summary>
	/// Attribute to apply to 3rd party assemblies to load additional controls or functionality.
	/// </summary>
	/// <remarks>
	/// This can be used by both by the cross platform control library, and in each of the platform-specific libraries.
	/// </remarks>
	/// <example>
	/// For example, say you want to create a new control, called <code>MyControl</code>, you would have a shared assembly, plus implementation assemblies for each platform.
	/// 
	/// MyControls.dll:
	/// <code>
	/// using Eto;
	/// using Eto.Forms;
	/// 
	/// namespace MyControls
	/// {
	/// 	static class MyPlatform
	/// 	{
	/// 		bool initialized;
	/// 
	/// 		void Initialize()
	/// 		{
	/// 			if (initialized)
	/// 				return;
	/// 
	/// 			// load the platform specific assembly based on the ID of the current platform
	/// 			Platform.Instance.LoadAssembly($"MyControls.{platform.ID}");
	/// 			initialized = true;
	/// 		}
	/// 	}
	/// 
	/// 	[Handler(typeof(IHandler))]
	/// 	public class MyControl : Control
	/// 	{
	/// 		new IHandler Handler => base.Handler as IHandler;
	/// 
	/// 		static MyControl()
	/// 		{
	/// 			MyPlatform.Initialize(); // ensure platform specific assembly is loaded when your control is used
	/// 		}
	/// 
	/// 		public bool SomeProperty
	/// 		{
	/// 			get { return Handler.SomeProperty; }
	/// 			set { Handler.SomeProperty = value; }
	/// 		}
	/// 
	/// 		public new interface IHandler : IHandler
	/// 		{
	/// 			bool SomeProperty { get; set; }
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// 
	/// MyControls.Wpf.dll:
	/// <code>
	/// using Eto;
	/// using Eto.Forms;
	/// using Eto.Wpf.Forms;
	/// 
	/// [assembly:PlatformExtension(typeof(MyControls.Wpf.MyPlatform))]
	/// 
	/// namespace MyControls.Wpf
	/// {
	/// 	class MyPlatform : IPlatformExtension
	/// 	{
	/// 		public Initialize(Platform platform)
	/// 		{
	/// 			platform.Add&lt;MyControl.IHandler&gt;(() => new MyControlHandler());
	/// 		}
	/// 	}
	/// 
	/// 	public class MyControlHandler : WpfFrameworkElement&lt;SomeWpfControl, MyControl, MyControl.ICallback>, MyControl.IHandler
	/// 	{
	/// 		public MyControlHandler()
	/// 		{
	/// 			Control = new SomeWpfControl();
	/// 		}
	/// 
	/// 		// TODO:
	/// 		public bool SomeProperty
	/// 		{
	/// 			get { return false; }
	/// 			set { }
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
	public sealed class PlatformExtensionAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.PlatformExtensionAttribute"/> class.
		/// </summary>
		/// <param name="extensionType">Type of the extension class to instantiate which implements <see cref="IPlatformExtension"/>.</param>
		public PlatformExtensionAttribute(Type extensionType)
		{
			ExtensionType = extensionType;
		}

		/// <summary>
		/// Type of the extension class to instantiate when the assembly this attribute is supplied on is loaded by the platform.
		/// </summary>
		/// <remarks>
		/// This type should usually implement <see cref="IPlatformExtension"/> so that it can know what platform it is being loaded on.
		/// </remarks>
		/// <value>The type of the extension.</value>
		public Type ExtensionType { get; }

		/// <summary>
		/// Gets or sets the platform identifier this extension should be loaded on, or null to load for all platforms.
		/// </summary>
		/// <value>The platform identifier.</value>
		public string PlatformID { get; set; }
	}
}
