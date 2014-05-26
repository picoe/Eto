using System;
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
		#if PCL
		public static Icon FromResource (string resourceName, Assembly assembly)
		#else
		public static Icon FromResource (string resourceName, Assembly assembly = null)
		#endif
		{
			if (assembly == null)
			{
				#if PCL
				throw new ArgumentNullException("assembly");
				#else
				assembly = Assembly.GetCallingAssembly();
				#endif
			}
			using (var stream = assembly.GetManifestResourceStream(resourceName)) {
				if (stream == null)
					throw new ResourceNotFoundException (assembly, resourceName);
				return new Icon (stream);
			}
		}

		public static Icon FromResource (string resourceName, Type type)
		{
			#if PCL
			return FromResource(resourceName, type.GetTypeInfo().Assembly);
			#else
			return FromResource(resourceName, type.Assembly);
			#endif
		}

		#pragma warning disable 612,618

		/// <summary>
		/// Initializes a new instance of the Icon class with the specified handler
		/// </summary>
		/// <param name="generator">Generator for this widget</param>
		/// <param name="handler">Handler for the icon backend</param>
		[Obsolete("Use variation without generator instead")]
		public Icon (Generator generator, IHandler handler) : base(generator, handler)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Icon class with the contents of the specified <paramref name="stream"/>
		/// </summary>
		/// <param name="generator">Generator for this widget</param>
		/// <param name="stream">Stream to load the content from</param>
		[Obsolete("Use variation without generator instead")]
		public Icon (Stream stream, Generator generator) : base(generator, typeof(IHandler))
		{
			Handler.Create (stream);
		}

		/// <summary>
		/// Intitializes a new instanc of the Icon class with the contents of the specified <paramref name="fileName"/>
		/// </summary>
		/// <param name="generator">Generator for this widget</param>
		/// <param name="fileName">Name of the file to loat the content from</param>
		[Obsolete("Use variation without generator instead")]
		public Icon (string fileName, Generator generator) : base(generator, typeof(IHandler))
		{
			Handler.Create (fileName);
		}

		/// <summary>
		/// Loads an icon from an embedded resource of the specified assembly
		/// </summary>
		/// <param name="assembly">Assembly to load the resource from</param>
		/// <param name="resourceName">Fully qualified name of the resource to load</param>
		/// <param name="generator">Generator for this widget</param>
		/// <returns>A new instance of an Icon loaded with the contents of the specified resource</returns>
		[Obsolete("Use variation without generator instead")]
		#if PCL
		public static Icon FromResource (string resourceName, Assembly assembly, Generator generator)
		#else
		public static Icon FromResource (string resourceName, Assembly assembly, Generator generator)
		#endif
		{
			if (assembly == null)
			{
				#if PCL
				throw new ArgumentNullException("assembly");
				#else
				assembly = Assembly.GetCallingAssembly();
				#endif
			}
			using (var stream = assembly.GetManifestResourceStream(resourceName)) {
				if (stream == null)
					throw new ResourceNotFoundException (assembly, resourceName);
				return new Icon (stream, generator);
			}
		}

		[Obsolete("Use variation without generator instead")]
		public static Icon FromResource (string resourceName, Type type, Generator generator)
		{
			#if PCL
			return FromResource(resourceName, type.GetTypeInfo().Assembly, generator);
			#else
			return FromResource(resourceName, type.Assembly, generator);
			#endif
		}

		#pragma warning restore 612,618

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
