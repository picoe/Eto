using System;
using System.Reflection;
namespace Eto
{
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
	/// 	[Handler(typeof(IHandler))]
	/// 	public class MyControl : Control
	/// 	{
	/// 		new IHandler Handler => base.Handler as IHandler;
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
	/// [assembly:ExportInitializer(typeof(MyControls.Wpf.MyPlatform))]
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
	[System.AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
	public sealed class ExportInitializerAttribute : PlatformExtensionAttribute
	{
		/// <summary>
		/// Type of the extension class to instantiate when the assembly this attribute is supplied on is loaded by the platform.
		/// </summary>
		/// <remarks>
		/// This type should usually implement <see cref="IPlatformInitializer"/> so that it can know what platform it is being loaded on.
		/// </remarks>
		/// <value>The type of the extension.</value>
		public Type InitializerType { get; }

		/// <summary>
		/// Initializes a new instance of the PlatformInitializerAttribute class
		/// </summary>
		/// <param name="initializerType">Type of the initializer to create</param>
		public ExportInitializerAttribute(Type initializerType)
		{
			InitializerType = initializerType ?? throw new ArgumentNullException(nameof(initializerType));
		}

		/// <summary>
		/// Registers the extension with the specified platform
		/// </summary>
		/// <returns><c>true</c>, if the extension was applied, <c>false</c> otherwise.</returns>
		/// <param name="platform">Platform to register this extension with.</param>
		public override void Register(Platform platform)
		{
			var extension = Activator.CreateInstance(InitializerType) as IPlatformInitializer;
			extension?.Initialize(platform);
			platform.SetSharedProperty(this, extension);
		}
	}

	/// <summary>
	/// Platform extension for 3rd party libraries to provide additional controls or functionality
	/// </summary>
	/// <remarks>
	/// This is implemented for types that are referenced using the <see cref="ExportInitializerAttribute"/>.
	/// </remarks>
	public interface IPlatformInitializer
	{
		/// <summary>
		/// Called to add specific functionality to the specified <paramref name="platform"/>
		/// </summary>
		/// <param name="platform">Platform to add the extension to</param>
		void Initialize(Platform platform);
	}

	/// <summary>
	/// Base extension attribute to wire up 3rd party controls and native handler implementations.
	/// </summary>
	/// <remarks>
	/// All extensions are loaded via <see cref="Platform.LoadAssembly(Assembly)"/>.  
	/// 
	/// Use the <see cref="PlatformID"/> property to specify which platform the extension applies to.
	/// </remarks>
	[System.AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
	public abstract class PlatformExtensionAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the platform identifier this extension should be loaded on, or null to load for all platforms.
		/// </summary>
		/// <value>The platform identifier.</value>
		public string PlatformID { get; set; }

		/// <summary>
		/// Gets a value indicating that this extension supports the specified platform.
		/// </summary>
		/// <remarks>
		/// <see cref="Register"/> will only be called if this returns <c>true</c>.
		/// </remarks>
		/// <returns><c>true</c>, if this extension supports the specified platform, <c>false</c> otherwise.</returns>
		/// <param name="platform">Platform to determine if this extension applies.</param>
		public virtual bool Supports(Platform platform)
		{
			if (string.IsNullOrEmpty(PlatformID) || platform.ID == PlatformID)
				return true;
			// support using the Eto.Platforms.* values
			var type = platform.GetType();
			while (type != null)
			{
				if (type.AssemblyQualifiedName == PlatformID)
					return true;
				type = type.GetBaseType();
			}
			return false;
		}

		/// <summary>
		/// Registers the extension with the specified platform
		/// </summary>
		/// <returns><c>true</c>, if the extension was applied, <c>false</c> otherwise.</returns>
		/// <param name="platform">Platform to register this extension with.</param>
		public abstract void Register(Platform platform);
	}

	

}
