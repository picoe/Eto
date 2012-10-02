using System;
using System.Reflection;
using System.IO;

namespace Eto.Drawing
{
	/// <summary>
	/// Platform handler for the <see cref="Icon"/> class
	/// </summary>
	public interface IIcon : IImage
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
	public class Icon : Image
	{
		IIcon handler;
		
		/// <summary>
		/// Initializes a new instance of the Icon class with the specified handler
		/// </summary>
		/// <param name="generator">Generator for this widget</param>
		/// <param name="handler">Handler for the icon backend</param>
		public Icon (Generator generator, IIcon handler) : base(generator, handler)
		{
			this.handler = handler;
		}
	
		/// <summary>
		/// Initializes a new instance of the Icon class with the contents of the specified <paramref name="stream"/>
		/// </summary>
		/// <param name="stream">Stream to load the content from</param>
		public Icon (Stream stream) : base(Generator.Current, typeof(IIcon))
		{
			handler = (IIcon)Handler;
			handler.Create (stream);
		}

		/// <summary>
		/// Intitializes a new instanc of the Icon class with the contents of the specified <paramref name="fileName"/>
		/// </summary>
		/// <param name="fileName">Name of the file to loat the content from</param>
		public Icon (string fileName) : base(Generator.Current, typeof(IIcon))
		{
			handler = (IIcon)Handler;
			handler.Create (fileName);
		}
		
		public static Icon FromResource (Assembly asm, string resourceName)
		{
			if (asm == null)
				asm = Assembly.GetCallingAssembly ();
			using (var stream = asm.GetManifestResourceStream(resourceName)) {
				if (stream == null)
					throw new ResourceNotFoundException (asm, resourceName);
				return new Icon (stream);
			}
		}

		public static Icon FromResource (string resourceName)
		{
			var asm = Assembly.GetCallingAssembly ();
			return FromResource (asm, resourceName);
		}
		
		[Obsolete("Use Icon.FromResource instead")]
		public Icon (Assembly asm, string resourceName) : base(Generator.Current, typeof(IIcon))
		{
			handler = (IIcon)Handler;
			if (asm == null)
				asm = Assembly.GetCallingAssembly ();
			using (var stream = asm.GetManifestResourceStream (resourceName)) {
				if (stream == null)
					throw new ResourceNotFoundException (asm, resourceName);
				handler.Create (stream);
			}
		}
	}
}
