using System;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using sc = System.ComponentModel;

namespace Eto.Drawing
{
	/// <summary>
	/// Represents an icon which allows for multiple sizes and resolutions of an image
	/// </summary>
	/// <remarks>
	/// The formats supported vary by platform, however all platforms do support loading windows .ico format.
	/// 
	/// Using an icon for things like menus, toolbars, etc are preferred so that each platform can use the appropriate
	/// sized image.
	/// 
	/// For High DPI/Retina displays (e.g. on OS X), this will allow using a higher resolution image automatically.
	/// </remarks>
	/// <copyright>(c) 2016 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(Icon.IHandler))]
	[sc.TypeConverter(typeof(IconConverter))]
	public class Icon : Image
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the Icon class with the specified handler
		/// </summary>
		/// <param name="handler">Handler for the icon backend</param>
		public Icon(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Icon class with the contents of the specified <paramref name="stream"/>
		/// </summary>
		/// <param name="stream">Stream to load the content from</param>
		public Icon(Stream stream)
		{
			Handler.Create(stream);
			Initialize();
		}

		/// <summary>
		/// Intitializes a new instanc of the Icon class with the contents of the specified <paramref name="fileName"/>
		/// </summary>
		/// <param name="fileName">Name of the file to loat the content from</param>
		public Icon(string fileName)
		{
			Handler.Create(fileName);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Drawing.Icon"/> class with the specified frames.
		/// </summary>
		/// <remarks>
		/// This is used when you want to create an icon with specific bitmap frames at different scales or sizes.
		/// </remarks>
		/// <param name="frames">Frames for the icon.</param>
		public Icon(IEnumerable<IconFrame> frames)
		{
			Handler.Create(frames);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Drawing.Icon"/> class with the specified frames.
		/// </summary>
		/// <remarks>
		/// This is used when you want to create an icon with specific bitmap frames at different scales or sizes.
		/// </remarks>
		/// <param name="frames">Frames for the icon.</param>
		public Icon(params IconFrame[] frames)
		{
			Handler.Create(frames);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Drawing.Icon"/> class with the specified bitmap.
		/// </summary>
		/// <remarks>
		/// This is used when you want to create an icon with a single bitmap frame with the specified logical scale.
		/// </remarks>
		/// <param name="scale">Logical pixel scale of the specified bitmap.</param>
		/// <param name="bitmap">Bitmap for the frame.</param>
		public Icon(float scale, Bitmap bitmap)
			: this(new IconFrame(scale, bitmap))
		{
		}

		/// <summary>
		/// Loads an icon from an embedded resource of the specified assembly
		/// </summary>
		/// <param name="assembly">Assembly to load the resource from</param>
		/// <param name="resourceName">Fully qualified name of the resource to load. E.g. "MyProject.SomeFolder.YourFile.extension"</param>
		/// <returns>A new instance of an Icon loaded with the contents of the specified resource</returns>
		public static Icon FromResource(string resourceName, Assembly assembly = null)
		{
			if (assembly == null)
			{
				#if NETSTANDARD
				assembly = (Assembly)TypeHelper.GetCallingAssembly.Invoke(null, null);
				#else
				assembly = Assembly.GetCallingAssembly();
				#endif
			}

			if (resourceName.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
			{
				using (var stream = assembly.GetManifestResourceStream(resourceName))
				{
					if (stream == null)
						throw new ArgumentException($"Resource '{resourceName}' not found in assembly '{assembly.FullName}'", nameof(resourceName));
					return new Icon(stream);
				}
			}

			var frames = new List<IconFrame>();
			GetResources(resourceName, assembly, frames);
			if (frames.Count == 0)
				throw new ArgumentException($"Resource '{resourceName}' not found in assembly '{assembly.FullName}'", nameof(resourceName));
			
			return new Icon(frames);
		}

		static void GetResources(string resourceName, Assembly assembly, List<IconFrame> frames)
		{
			var info = assembly.GetManifestResourceInfo(resourceName);
			if (info != null)
				frames.Add(IconFrame.FromResource(1f, resourceName, assembly));

			// no extension? don't look for others
			var extensionIndex = resourceName.LastIndexOf('.');
			if (extensionIndex < 0)
				return;

			// .ico files already have multiple resolutions
			var extension = resourceName.Substring(extensionIndex);
			if (extension.Equals(".ico", StringComparison.OrdinalIgnoreCase))
				return;
			
			var resourceWithoutExtension = resourceName.Substring(0, extensionIndex);

			var nameWithAt = resourceWithoutExtension + "@";

			foreach (var entryName in assembly.GetManifestResourceNames()
				.Where(r => r.StartsWith(nameWithAt, StringComparison.Ordinal))
				.OrderByDescending(r => r))
			{
				// must be same extension, if one is supplied with the resourceName
				if (extension != null && !entryName.EndsWith(extension, StringComparison.Ordinal))
					continue;

				// get the scale, if supplied
				extensionIndex = entryName.LastIndexOf('.');
				if (extensionIndex < 0)
					extensionIndex = entryName.Length;

				// parse out scale, e.g. @2x, @0.5x
				var scaleString = entryName.Substring(nameWithAt.Length, extensionIndex - nameWithAt.Length);
				float scale;
				if (!scaleString.EndsWith("x", StringComparison.Ordinal)
					|| !float.TryParse(scaleString.TrimEnd('x'), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out scale))
					continue;

				frames.Add(IconFrame.FromResource(scale, entryName, assembly));
			}
		}


		/// <summary>
		/// Loads an icon from a resource in the same assembly as the specified <paramref name="type"/>
		/// </summary>
		/// <returns>The icon instance.</returns>
		/// <param name="resourceName">Full name of the resource in the type's assembly E.g. "MyProject.SomeFolder.YourFile.extension"</param>
		/// <param name="type">Type of the assembly to get the resource.</param>
		public static Icon FromResource(string resourceName, Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			return FromResource(resourceName, type.GetAssembly());
		}

		/// <summary>
		/// Find based on the fitting pixel size
		/// </summary>
		IconFrame FindBySize(float scale, Size fittingSize)
		{
			// adjust fitting size to scale to get pixel size desired
			fittingSize = Size.Ceiling((SizeF)fittingSize * scale);
			var fs = fittingSize.Width * fittingSize.Height;
			var frame = Frames
				.Select(r => Tuple.Create(r.PixelSize.Width * r.PixelSize.Height, r))
				.OrderBy(r => r.Item1)
				.FirstOrDefault(r => r.Item1 >= fs)?.Item2;

			return frame ?? Frames.OrderBy(r => r.PixelSize.Width * r.PixelSize.Height).Last();
		}

		/// <summary>
		/// find based on scale alone, we don't know the final render size
		/// </summary>
		IconFrame FindByScale(float scale)
		{
			IconFrame selected = null;
			float scaleDiff = 0;
			float lastScale = 0;
			foreach (var frame in Frames.OrderBy(r => r.Scale).ThenByDescending(r => r.Size.Width * r.Size.Height))
			{
				var ps = Size.Ceiling((SizeF)Size * scale);
				var pixelSizeNeeded = ps.Width * ps.Height;
				var diff = scale - frame.Scale;
				if (selected == null || (diff < scaleDiff && (diff >= 0 || scaleDiff > 0)))
				{
					// found where scale is the next greater or equal to desired scale
					scaleDiff = diff;
					lastScale = frame.Scale;
					selected = frame;
				}
				else if (lastScale == frame.Scale)
				{
					// multiple resolutions with the same scale, use the one with the best pixel size
					// (e.g. ico file with multiple resolutions)
					ps = frame.PixelSize;
					if (ps.Width * ps.Height >= pixelSizeNeeded)
					{
						selected = frame;
						scaleDiff = diff;
						lastScale = frame.Scale;
					}
					else
						break;
				}
				else
					break;
			}
			if (selected == null)
			{
				// get largest
				selected = Frames.OrderBy(r => r.PixelSize.Width * r.PixelSize.Height).Last();
			}
			return selected;
		}

		/// <summary>
		/// Gets the frame with the specified scale that can fit into the <paramref name="fittingSize"/> if specified.
		/// </summary>
		/// <remarks>
		/// This can be used to determine which frame should be used to draw to the screen, based on the desired logical pixel
		/// scale and final drawn size of the icon.
		/// </remarks>
		/// <returns>The frame that is the closest match for the specified scale and fitting size.</returns>
		/// <param name="scale">Logical scale to find for, 1 for normal size, 2 for retina, etc.</param>
		/// <param name="fittingSize">Fitting size that the icon will be drawn to, if known.</param>
		public IconFrame GetFrame(float scale, Size? fittingSize = null)
		{
			if (fittingSize != null)
			{
				// we know the final render size, find the best match based on pixel size vs scale
				// We always prefer size that matches best with the input scale
				// e.g. when we need scale 2 at 64x64, it will prefer the frame that is greater in pixel size to 128x128.
				return FindBySize(scale, fittingSize.Value);
			}
			else
			{
				return FindByScale(scale);
			}
		}

		/// <summary>
		/// Gets a copy of this Icon with frames scaled to draw within the specified fitting size.
		/// </summary>
		/// <remarks>
		/// This is useful when you want to draw an Icon at a different size than the default size. 
		/// Note that the <paramref name="fittingSize"/> specifies the maxiumum drawing size of the Icon, but will not
		/// change the aspect of each frame's bitmap.  For example, if an existing frame is 128x128, and you specify 16x32,
		/// then the resulting frame will draw at 16x16.
		/// </remarks>
		/// <returns>A new icon that will draw within the fitting size.</returns>
		/// <param name="fittingSize">The maximum size to draw the Icon.</param>
		public Icon WithSize(Size fittingSize)
		{
			var frames = Frames.Select(frame =>
			{
				var scale = Math.Max((float)frame.PixelSize.Width / (float)fittingSize.Width, (float)frame.PixelSize.Height / (float)fittingSize.Height);
				return new IconFrame(scale, frame.Bitmap);
			});
			return new Icon(frames);
		}

		/// <summary>
		/// Gets a copy of this Icon with frames scaled to draw within the specified fitting size.
		/// </summary>
		/// <remarks>
		/// This is useful when you want to draw an Icon at a different size than the default size. 
		/// Note that the <paramref name="width"/> and <paramref name="height"/> specifies the maxiumum drawing size of the Icon, but will not
		/// change the aspect of each frame's bitmap.  For example, if an existing frame is 128x128, and you specify 16x32,
		/// then the resulting frame will draw at 16x16.
		/// </remarks>
		/// <returns>A new icon that will draw within the fitting size.</returns>
		/// <param name="width">Maxiumum drawing width for the new icon.</param>
		/// <param name="height">Maxiumum drawing height for the new icon.</param>
		public Icon WithSize(int width, int height)
		{
			return WithSize(new Size(width, height));
		}

		/// <summary>
		/// Gets the definition for each frame in this icon.
		/// </summary>
		/// <value>The frames of the icon.</value>
		public IEnumerable<IconFrame> Frames { get { return Handler.Frames; } }


		/// <summary>
		/// Platform handler for the <see cref="Icon"/> class
		/// </summary>
		[AutoInitialize(false)]
		public new interface IHandler : Image.IHandler
		{
			/// <summary>
			/// Called when creating an instance from a stream
			/// </summary>
			/// <param name="stream">Stream to load the icon from</param>
			void Create(Stream stream);

			/// <summary>
			/// Called when creating an instance from a file name
			/// </summary>
			/// <param name="fileName">File name to load the icon from</param>
			void Create(string fileName);

			/// <summary>
			/// Initializes a new instance of the <see cref="Eto.Drawing.Icon"/> class with the specified frames.
			/// </summary>
			/// <remarks>
			/// This is used when you want to create an icon with specific bitmap frames at different scales or sizes.
			/// </remarks>
			/// <param name="frames">Frames for the icon.</param>
			void Create(IEnumerable<IconFrame> frames);

			/// <summary>
			/// Gets the definition for each frame in this icon.
			/// </summary>
			/// <value>The frames of the icon.</value>
			IEnumerable<IconFrame> Frames { get; }
		}
	}
}
