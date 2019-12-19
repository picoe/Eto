using System;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Drawing
{
	/// <summary>
	/// Represents a frame in an <see cref="Icon"/>.
	/// </summary>
	/// <remarks>
	/// The IconFrame represents a single frame in an Icon.  
	/// Each IconFrame can have a specific pixel size and scale, which will automatically be chosen based on the display and
	/// draw size of the image in various Eto controls.  
	/// 
	/// You can load an icon from an .ico, where all frames will have a 1.0 scale (pixel size equals logical size)
	/// </remarks>
	[Handler(typeof(IHandler))]
	public class IconFrame : IControlObjectSource, IHandlerSource
	{
		object IHandlerSource.Handler
		{
			get { return Handler; }
		}

		IHandler Handler { get; set; }

		/// <summary>
		/// Gets the control object for this widget
		/// </summary>
		/// <value>The control object for the widget</value>
		public object ControlObject { get; private set; }

		/// <summary>
		/// Gets the pixel size of the frame's bitmap
		/// </summary>
		/// <value>The size in pixels of the frame.</value>
		public Size PixelSize { get { return Handler.GetPixelSize(this); } }

		/// <summary>
		/// Gets the scale of this frame. 1.0 means the <see cref="Size"/> and <see cref="PixelSize"/> will be equal.
		/// </summary>
		/// <remarks>
		/// When loading from an .ico, all frames will get a scale of 1.0.
		/// </remarks>
		/// <value>The scale of this frame.</value>
		public float Scale { get; }

		/// <summary>
		/// Gets the logical size of the frame.
		/// </summary>
		/// <seealso cref="Scale"/>
		/// <value>The logical size of the frame.</value>
		public Size Size
		{
			get { return Size.Ceiling((SizeF)PixelSize / Scale); }
		}

		/// <summary>
		/// Gets the bitmap for this frame.
		/// </summary>
		/// <value>The frame's bitmap.</value>
		public Bitmap Bitmap
		{
			get { return Handler.GetBitmap(this); }
		}

		IconFrame(float scale)
		{
			Handler = Platform.Instance.CreateShared<IHandler>();
			Scale = scale;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Drawing.IconFrame"/> class.
		/// </summary>
		/// <param name="scale">Scale of logical to physical pixels.</param>
		/// <param name="load">Delegate to load the stream when the frame's data is required.</param>
		public IconFrame(float scale, Func<Stream> load)
			: this(scale)
		{
			ControlObject = Handler.Create(this, load);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Drawing.IconFrame"/> class.
		/// </summary>
		/// <param name="scale">Scale of logical to physical pixels.</param>
		/// <param name="stream">Stream for the bitmap data.</param>
		public IconFrame(float scale, Stream stream)
			: this(scale)
		{
			ControlObject = Handler.Create(this, stream);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Drawing.IconFrame"/> class.
		/// </summary>
		/// <param name="scale">Scale of logical to physical pixels.</param>
		/// <param name="bitmap">Bitmap for the frame</param>
		public IconFrame(float scale, Bitmap bitmap)
			: this(scale)
		{
			ControlObject = Handler.Create(this, bitmap);
		}

		/// <summary>
		/// Creates an instance of the <see cref="IconFrame"/> with the specified native control object.
		/// </summary>
		/// <remarks>
		/// This is used by platform implementations to create instances of this class with the appropriate control object.
		/// This is not intended to be called directly.
		/// </remarks>
		/// <returns>A new instance of the IconFrame with the native control object and scale.</returns>
		/// <param name="scale">Scale of logical to physical pixels.</param>
		/// <param name="controlObject">Native control object.</param>
		public static IconFrame FromControlObject(float scale, object controlObject)
		{
			return new IconFrame(scale) { ControlObject = controlObject };
		}

		/// <summary>
		/// Creates an instance of the <see cref="IconFrame"/> from an embedded resource.
		/// </summary>
		/// <returns>A new instance.</returns>
		/// <param name="scale">Scale of logical to physical pixels.</param>
		/// <param name="resourceName">Name of the embedded resource to load.</param>
		/// <param name="assembly">Assembly to load the embedded resource from, or null to use the calling assembly.</param>
		public static IconFrame FromResource(float scale, string resourceName, Assembly assembly = null)
		{
			if (assembly == null)
			{
				#if NETSTANDARD
				if (TypeHelper.GetCallingAssembly == null)
					throw new ArgumentNullException("assembly", string.Format(CultureInfo.CurrentCulture, "This platform doesn't support Assembly.GetCallingAssembly(), so you must pass the assembly directly"));
                assembly = (Assembly)TypeHelper.GetCallingAssembly.Invoke(null, null);
				#else
				assembly = Assembly.GetCallingAssembly();
				#endif
			}

			return new IconFrame(scale, () =>
			{
				//Debug.WriteLine($"Loading Resource '{resourceName}'");
				return assembly.GetManifestResourceStream(resourceName);
			});

		}

		/// <summary>
		/// Handler interface for platform implementations of the <see cref="IconFrame"/>
		/// </summary>
		[AutoInitialize(false)]
		public interface IHandler
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="Eto.Drawing.IconFrame"/> class.
			/// </summary>
			/// <param name="frame">Frame instance to create for.</param>
			/// <param name="stream">Stream for the bitmap data.</param>
			object Create(IconFrame frame, Stream stream);

			/// <summary>
			/// Initializes a new instance of the <see cref="Eto.Drawing.IconFrame"/> class.
			/// </summary>
			/// <param name="frame">Frame instance to create for.</param>
			/// <param name="load">Delegate to load the stream when the frame's data is required.</param>
			object Create(IconFrame frame, Func<Stream> load);

			/// <summary>
			/// Initializes a new instance of the <see cref="Eto.Drawing.IconFrame"/> class.
			/// </summary>
			/// <param name="frame">Frame instance to create for.</param>
			/// <param name="bitmap">Bitmap for the frame</param>
			object Create(IconFrame frame, Bitmap bitmap);

			/// <summary>
			/// Gets the bitmap for this frame.
			/// </summary>
			/// <param name="frame">Frame instance to get the bitmap for</param>
			/// <value>The frame's bitmap.</value>
			Bitmap GetBitmap(IconFrame frame);

			/// <summary>
			/// Gets the pixel size of the frame's bitmap
			/// </summary>
			/// <param name="frame">Frame instance to get the pixel size for</param>
			/// <value>The size in pixels of the frame.</value>
			Size GetPixelSize(IconFrame frame);
		}
	}
	
}
