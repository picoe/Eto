using System;
using System.Globalization;
using System.Reflection;
using System.IO;

namespace Eto.Drawing
{
	/// <summary>
	/// Represents an icon which allows for multiple sizes of an image
	/// </summary>
	/// <remarks>
	/// The formats supported vary by platform, however all platforms do support loadin windows .ico format.
	/// 
	/// Using an icon for things like menus, toolbars, etc are preferred so that each platform can use the appropriate
	/// sized image.
	/// 
	/// For HiDPI/Retina displays (e.g. on OS X), this will allow using a higher resolution image automatically.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(Icon.IHandler))]
	public class Icon : Image
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }
		
		/// <summary>
		/// Initializes a new instance of the Icon class with the specified handler
		/// </summary>
		/// <param name="handler">Handler for the icon backend</param>
		public Icon (IHandler handler)
			: base(handler)
		{
		}
	
		/// <summary>
		/// Initializes a new instance of the Icon class with the contents of the specified <paramref name="stream"/>
		/// </summary>
		/// <param name="stream">Stream to load the content from</param>
		public Icon (Stream stream)
		{
			Handler.Create (stream);
		}

		/// <summary>
		/// Intitializes a new instanc of the Icon class with the contents of the specified <paramref name="fileName"/>
		/// </summary>
		/// <param name="fileName">Name of the file to loat the content from</param>
		public Icon (string fileName)
		{
			Handler.Create (fileName);
		}
		
		/// <summary>
		/// Loads an icon from an embedded resource of the specified assembly
		/// </summary>
		/// <param name="assembly">Assembly to load the resource from</param>
		/// <param name="resourceName">Fully qualified name of the resource to load</param>
		/// <returns>A new instance of an Icon loaded with the contents of the specified resource</returns>
		public static Icon FromResource(string resourceName, Assembly assembly = null)
		{
			if (assembly == null)
			{
				#if PCL
				if (TypeHelper.GetCallingAssembly == null)
					throw new ArgumentNullException("assembly", string.Format(CultureInfo.CurrentCulture, "This platform doesn't support Assembly.GetCallingAssembly(), so you must pass the assembly directly"));
				assembly = (Assembly)TypeHelper.GetCallingAssembly.Invoke(null, new object[0]);
				#else
				assembly = Assembly.GetCallingAssembly();
				#endif
			}
			using (var stream = assembly.GetManifestResourceStream(resourceName)) {
				if (stream == null)
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Resource '{0}' not found in assembly '{1}'", resourceName, assembly.FullName));
				return new Icon (stream);
			}
		}

		/// <summary>
		/// Loads an icon from a resource in the same assembly as the specified <paramref name="type"/>
		/// </summary>
		/// <returns>The icon instance.</returns>
		/// <param name="resourceName">Full name of the resource in the type's assembly.</param>
		/// <param name="type">Type of the assembly to get the resource.</param>
		public static Icon FromResource(string resourceName, Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			#if PCL
			return FromResource(resourceName, type.GetTypeInfo().Assembly);
			#else
			return FromResource(resourceName, type.Assembly);
			#endif
		}

		/// <summary>
		/// Platform handler for the <see cref="Icon"/> class
		/// </summary>
		public new interface IHandler : Image.IHandler
		{
			/// <summary>
			/// Called when creating an instance from a stream
			/// </summary>
			/// <param name="stream">Stream to load the icon from</param>
			void Create (Stream stream);

			/// <summary>
			/// Called when creating an instance from a file name
			/// </summary>
			/// <param name="fileName">File name to load the icon from</param>
			void Create (string fileName);
		}
	}
}
