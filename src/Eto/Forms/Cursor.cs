using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Enumeration of the cursor types supported by the <see cref="Cursor"/> object
	/// </summary>
	public enum CursorType
	{
		/// <summary>
		/// Default cursor, which is usually an arrow but may be different depending on the control
		/// </summary>
		Default,

		/// <summary>
		/// Standard arrow cursor
		/// </summary>
		Arrow,

		/// <summary>
		/// Cursor with a cross hair
		/// </summary>
		Crosshair,

		/// <summary>
		/// Pointer cursor, which is usually a hand
		/// </summary>
		Pointer,

		/// <summary>
		/// All direction move cursor
		/// </summary>
		Move,

		/// <summary>
		/// I-beam cursor for selecting text or placing the text cursor
		/// </summary>
		IBeam,

		/// <summary>
		/// Vertical sizing cursor
		/// </summary>
		VerticalSplit,

		/// <summary>
		/// Horizontal sizing cursor
		/// </summary>
		HorizontalSplit
	}

	/// <summary>
	/// Class for a particular Mouse cursor type
	/// </summary>
	/// <remarks>
	/// This can be used to specify a cursor for a particular control
	/// using <see cref="Control.Cursor"/>
	/// </remarks>
	[Handler(typeof(Cursor.IHandler))]
	public class Cursor : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Cursor"/> class with the specified <paramref name="type"/>.
		/// </summary>
		/// <param name="type">Type of cursor.</param>
		public Cursor(CursorType type)
		{
			Handler.Create(type);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the Cursor class by loading the specified .cur <paramref name="fileName"/>
		/// </summary>
		/// <param name="fileName">Name of the cursor file to load from disk</param>
		public Cursor(string fileName)
		{
			Handler.Create(fileName);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the Cursor class by loading a cursor from the specified <paramref name="stream"/>
		/// </summary>
		/// <param name="stream">Stream to load from</param>
		public Cursor(Stream stream)
		{
			Handler.Create(stream);
			Initialize();
		}

		/// <summary>
		/// Loads a cursor from the resource in the specified or caller's <paramref name="assembly"/>
		/// </summary>
		/// <param name="resourceName">Name of the resource in the caller's assembly to load. E.g. "MyProject.SomeFolder.YourFile.cur"</param>
		/// <param name="assembly">Assembly to load the cursor from, or null to use the caller's assembly</param>
		/// <returns>A new instance of a Cursor loaded from the specified resource</returns>
		public static Cursor FromResource(string resourceName, Assembly assembly = null)
		{

			if (assembly == null)
			{
#if NETSTANDARD
				if (TypeHelper.GetCallingAssembly == null)
					throw new ArgumentNullException(nameof(assembly), string.Format(CultureInfo.CurrentCulture, "This platform doesn't support Assembly.GetCallingAssembly(), so you must pass the assembly directly"));
				assembly = (Assembly)TypeHelper.GetCallingAssembly.Invoke(null, null);
#else
				assembly = Assembly.GetCallingAssembly();
#endif
			}

			using (var stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Resource '{0}' not found in assembly '{1}'", resourceName, assembly.FullName));
				return new Cursor(stream);
			}
		}

		/// <summary>
		/// Loads a bitmap from a resource in the same assembly as the specified <paramref name="type"/>
		/// </summary>
		/// <returns>The bitmap instance.</returns>
		/// <param name="resourceName">Full name of the resource in the type's assembly. E.g. "MyProject.SomeFolder.YourFile.extension"</param>
		/// <param name="type">Type of the assembly to get the resource.</param>
		public static Cursor FromResource(string resourceName, Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			return FromResource(resourceName, type.GetAssembly());
		}

		/// <summary>
		/// Initializes a new instance of the Cursor with the specified <paramref name="image"/> and <paramref name="hotspot"/>. 
		/// </summary>
		/// <param name="image">Image for the cursor</param>
		/// <param name="hotspot">Hotspot for where the cursor pointer is located on the image</param>
		public Cursor(Bitmap image, PointF hotspot)
		{
			Handler.Create(image, hotspot);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the Cursor with the specified <paramref name="image"/> and <paramref name="hotspot"/>. 
		/// </summary>
		/// <param name="image">Image for the cursor</param>
		/// <param name="hotspot">Hotspot for where the cursor pointer is located on the image</param>
		/// TODO: Combine with above using Image base class in 3.x
		public Cursor(Icon image, PointF hotspot)
		{
			Handler.Create(image, hotspot);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the Cursor class with the specified <paramref name="handler"/>.
		/// </summary>
		/// <param name="handler">Handler to assign to this cursor for its implementation</param>
		public Cursor(IHandler handler) : base(handler)
		{
		}

		/// <summary>
		/// Platform interface for the <see cref="Cursor"/> class
		/// </summary>
		[AutoInitialize(false)]
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Creates the cursor instance with the specified <paramref name="type"/>.
			/// </summary>
			/// <param name="type">Cursor type.</param>
			void Create(CursorType type);
			/// <summary>
			/// Creates the cursor instance with the specified <paramref name="image"/> and <paramref name="hotspot"/>. 
			/// </summary>
			/// <param name="image">Image for the cursor</param>
			/// <param name="hotspot">Hotspot for where the cursor pointer is located on the image</param>
			void Create(Image image, PointF hotspot);
			/// <summary>
			/// Creates the cursor instance with the specified <paramref name="fileName"/>
			/// </summary>
			/// <param name="fileName">Name of the cursor to load from disk</param>
			void Create(string fileName);
			/// <summary>
			/// Creates the cursor instance with the specified <paramref name="stream"/>
			/// </summary>
			/// <param name="stream">Stream to load from</param>
			void Create(Stream stream);
		}
	}
}

