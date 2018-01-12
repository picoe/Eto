using System;
using System.Collections.Generic;
using System.Linq;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using swm = Windows.UI.Xaml.Media;
using Eto.Forms;
using Eto.Drawing;
using swi = Windows.UI.Xaml.Input;
using Eto.Direct2D.Drawing;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using System.Runtime.InteropServices;

namespace Eto.WinRT.Forms.Controls
{
	/// <summary>
	/// Drawable handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class DrawableHandler : WpfPanel<swc.Image, Drawable, Drawable.ICallback>, Drawable.IHandler
	{
		bool tiled;
		Scrollable scrollable;
		readonly Dictionary<int, EtoTile> visibleTiles = new Dictionary<int, EtoTile>();
		readonly List<EtoTile> unusedTiles = new List<EtoTile>();
		Size maxTiles;
		Size tileSize = new Size(100, 100);

		public bool AllowTiling { get; set; }

		public bool SupportsCreateGraphics { get { return false; } }

		public Size TileSize
		{
			get { return tileSize; }
			set
			{
				if (tileSize != value)
				{
					tileSize = value;
					if (Widget.Loaded)
					{
						ClearTiles();
						SetMaxTiles();
						UpdateTiles();
					}
				}
			}
		}

		class EtoMainCanvas : swc.Canvas
		{
			public DrawableHandler Handler { get; set; }

#if TODO_XAML
			protected override void OnMouseDown(sw.Input.MouseButtonEventArgs e)
			{
				if (Handler.CanFocus)
				{
					swi.Keyboard.Focus(this);
				}
				base.OnMouseDown(e);
			}

			protected override void OnRender(swm.DrawingContext dc)
			{
				base.OnRender(dc);
				if (!Handler.tiled)
				{
					var rect = new wf.Rect(0, 0, ActualWidth, ActualHeight);
					var graphics = new Graphics(Handler.Widget.Generator, new GraphicsHandler(this, dc, rect, false));
					Handler.Widget.OnPaint(new PaintEventArgs(graphics, rect.ToEto()));
				}
			}
#endif
		}

		class EtoTile : sw.FrameworkElement
		{
			Rectangle bounds;
			public DrawableHandler Handler { get; set; }

			public Rectangle Bounds
			{
				get { return bounds; }
				set
				{
					if (bounds != value)
					{
						bounds = value;

						swc.Canvas.SetLeft(this, bounds.X);
						swc.Canvas.SetTop(this, bounds.Y);
						Width = Handler.tileSize.Width;
						Height = Handler.tileSize.Height;
						RenderTransform = new swm.TranslateTransform { X = -bounds.X, Y = -bounds.Y };
#if TODO_XAML
						if (IsVisible)
							InvalidateVisual();
#endif					
					}
				}
			}

			public Point Position { get; set; }

			public int Key { get { return Position.Y * Handler.maxTiles.Width + Position.X; } }

#if TODO_XAML
			protected override void OnRender(swm.DrawingContext drawingContext)
			{
				var graphics = new Graphics(Handler.Widget.Generator, new GraphicsHandler(this, drawingContext, bounds.ToWpf(), false));
				Handler.Widget.OnPaint(new PaintEventArgs(graphics, Bounds));
			}
#endif
		}

		[DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
		public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

		bool isRendering;
		public void Render()
		{
			// Note: there ought to be a more efficient way to do this, as this is
			// critical. However, after much searching, the only way I found to
			// convert a Wic bitmap to a BitmapImage was by serializing to/from
			// a memory stream.

			if (!isRendering &&
				GraphicsHandler != null &&
				GraphicsHandler.Control != null)
			{
				isRendering = true;
				GraphicsHandler.BeginDrawing();
				Callback.OnPaint(Widget, new PaintEventArgs(graphics, Widget.Bounds)); // renders into the bitmap
				GraphicsHandler.EndDrawing();
				var bitmap = GraphicsHandler.Image;
				if (bitmap != null)
				{
					var bitmapHandler = bitmap.Handler as BitmapHandler;
					var bitmapObject = bitmapHandler.Control;
					var size = bitmapObject.Size;
					var writeableBitmap = new WriteableBitmap(size.Width, size.Height);

					// Get a pointer to the bitmap pixels
					unsafe
					{
						byte* ptr = null;
						((IBufferByteAccess)writeableBitmap.PixelBuffer).Buffer(out ptr);
						var len = size.Width * size.Height;
						var pixels = new SharpDX.ColorBGRA[len];
						fixed (SharpDX.ColorBGRA* pixelsPtr = &pixels[0])
						{
							bitmapObject.CopyPixels(pixels);
							CopyMemory((IntPtr)ptr, (IntPtr)pixelsPtr, (uint)len * 4);
						}
					}
					Control.Source = writeableBitmap;
				}
				isRendering = false;
			}
		}

		async Task SaveToFile(SharpDX.WIC.Bitmap bitmap) // debugging helper method, remove when no longer needed.
		{
			
			var width = bitmap.Size.Width;
			var height = bitmap.Size.Height;
			var pixels = new SharpDX.ColorBGRA[bitmap.Size.Width * bitmap.Size.Height];
			bitmap.CopyPixels(pixels);
			var bytes = new byte[width * height * 4];
			for (var i = 0; i < pixels.Length; ++i)
			{
				bytes[i * 4 + 0] = (byte)pixels[i].B;
				bytes[i * 4 + 1] = (byte)pixels[i].G;
				bytes[i * 4 + 2] = (byte)pixels[i].R;
				bytes[i * 4 + 3] = (byte)pixels[i].A;
			}

			var folder = ApplicationData.Current.LocalFolder;
			var file = await folder.CreateFileAsync("test.png", CreationCollisionOption.ReplaceExisting);
			using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
			{
				var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
				encoder.SetPixelData(BitmapPixelFormat.Bgra8,
									 BitmapAlphaMode.Ignore,
									 (uint)bitmap.Size.Width, (uint)bitmap.Size.Height,
									 96, 96, bytes);

				await encoder.FlushAsync();
			}
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);

			RegisterScrollable();
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			UnRegisterScrollable();
		}

		Graphics graphics;
		GraphicsHandler GraphicsHandler { get { return graphics != null ? graphics.Handler as GraphicsHandler : null; } }

		public DrawableHandler()
		{
#if TODO_XAML
			AllowTiling = true;
#endif
		}

		protected override void Initialize()
		{
			base.Initialize();
			graphics = new Graphics(new GraphicsHandler(this));
		}

		public void Create()
		{
			Control = new swc.Image
			{
				HorizontalAlignment = sw.HorizontalAlignment.Stretch,
				VerticalAlignment = sw.VerticalAlignment.Stretch,
#if TODO_XAML
				Handler = this,
				SnapsToDevicePixels = true,
				FocusVisualStyle = null,
				Background = swm.Brushes.Transparent;
#endif
			};
			Control.Loaded += Control_Loaded;
		}

        public void Create(bool largeCanvas)
        {
            Create();
        }

		void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			UpdateTiles(true); 
		}

		public virtual Graphics CreateGraphics()
		{
			return graphics != null ? new Graphics((Graphics.IHandler)graphics.Handler) : null;
		}

		void Control_SizeChanged(object sender, sw.SizeChangedEventArgs e)
		{
			SetMaxTiles();
			UpdateTiles(true);
			Invalidate(false);
		}

		void SetMaxTiles()
		{
			maxTiles = new Size(((int)Control.ActualWidth + tileSize.Width - 1) / tileSize.Width, ((int)Control.ActualHeight + tileSize.Height - 1) / tileSize.Height);
		}

		void RebuildKeys()
		{
			var tiles = visibleTiles.Values.ToArray();
			visibleTiles.Clear();
			foreach (var tile in tiles)
			{
				visibleTiles[tile.Key] = tile;
			}
		}

		void UnRegisterScrollable()
		{
			if (scrollable != null)
			{
				scrollable.Scroll -= scrollable_Scroll;
				scrollable = null;
			}
			if (tiled)
			{
				tiled = false;
				Control.SizeChanged -= Control_SizeChanged;
			}
		}

		void RegisterScrollable()
		{
			UnRegisterScrollable();
			if (AllowTiling)
			{
				scrollable = Widget.FindParent<Scrollable>();
				if (scrollable != null)
					scrollable.Scroll += scrollable_Scroll;

				Control.SizeChanged += Control_SizeChanged;
				SetMaxTiles();
				tiled = true;
			}
		}

		void ClearTiles()
		{
#if TODO_XAML
			foreach (var tile in unusedTiles)
			{
				Control.Children.Remove(tile);
			}
			unusedTiles.Clear();
			foreach (var tile in visibleTiles.Values)
			{
				Control.Children.Remove(tile);
			}
			visibleTiles.Clear();
#endif
		}

		void UpdateTiles(bool rebuildKeys = false)
		{
			if (!tiled
#if TODO_XAML
				|| !Control.IsLoaded
#endif
)
			{
				Control.Width = Widget.Size.Width;
				Control.Height = Widget.Size.Height;
				Invalidate(false);
				return;
			}

			var controlSize = new Size((int)Control.ActualWidth, (int)Control.ActualHeight);
			var rect = new Rectangle(controlSize);
			var scroll = scrollable;
			if (scroll != null)
			{
				// only show tiles in the visible rect of the scrollable
				var visibleRect = new Rectangle(scroll.ClientSize);
#if TODO_XAML
				var scrollableHandler = (ScrollableHandler)scroll.Handler;
				visibleRect.Offset(-Control.TranslatePoint(new wf.Point(), scrollableHandler.ContentControl).ToEtoPoint());
				rect.Intersect(visibleRect);
#else
				throw new NotImplementedException();
#endif
			}

			// cache unused tiles and remove them from the visible tiles list
			var keys = visibleTiles.Keys.ToArray();
			for (int i = 0; i < keys.Length; i++)
			{
				var key = keys[i];
				var tile = visibleTiles[keys[i]];
				if (!tile.Bounds.Intersects(rect))
				{
					visibleTiles.Remove(key);
					// keep tile, but make it invisible when needed later
					tile.Visibility = sw.Visibility.Collapsed;
					unusedTiles.Add(tile);
				}
			}

			// rebuild keys (e.g. when the size of the control changes)
			if (rebuildKeys)
				RebuildKeys();

			// calculate tile range that is visible
			var top = rect.Top / tileSize.Height;
			var bottom = (rect.Bottom + tileSize.Height - 1) / tileSize.Height;
			var left = rect.Left / tileSize.Width;
			var right = (rect.Right + tileSize.Width - 1) / tileSize.Width;

			// make sure all needed tiles are created/visible
			for (var y = top; y < bottom; y++)
			{
				for (var x = left; x < right; x++)
				{
					var position = new Point(x, y);
					if (!maxTiles.Contains(position))
						continue;
					var key = position.Y * maxTiles.Width + position.X;

					// calculate bounds of tile
					var xpos = x * tileSize.Width;
					var ypos = y * tileSize.Height;
					var xsize = Math.Min(tileSize.Width, controlSize.Width - xpos);
					var ysize = Math.Min(tileSize.Height, controlSize.Height - ypos);
					if (xsize < 0 || ysize < 0)
						continue;

					var bounds = new Rectangle(xpos, ypos, xsize, ysize);
					EtoTile tile;

					if (!visibleTiles.TryGetValue(key, out tile))
					{
						tile = unusedTiles.FirstOrDefault();
						if (tile != null)
						{
							// use existing cached tile and make it visible again
							unusedTiles.Remove(tile);
							tile.Position = position;
							tile.Bounds = bounds;
							tile.Visibility = sw.Visibility.Visible;
						}
						else
						{
#if TODO_XAML
							// need a new tile, no cached ones left
							tile = new EtoTile
							{
								Handler = this,
								SnapsToDevicePixels = true,
								Position = position,
								Bounds = bounds
							};
							Control.Children.Add(tile);
#endif
						}
						// set tile as visible
						visibleTiles[key] = tile;
					}
					else
						tile.Bounds = bounds;

				}
			}
		}

		void scrollable_Scroll(object sender, ScrollEventArgs e)
		{
			UpdateTiles();
		}

		void Invalidate(EtoTile tile)
		{
#if TODO_XAML
			tile.InvalidateVisual();
#else
			tile.InvalidateMeasure(); // this should be sufficient, but also invaliding Arrange to be sure.
			tile.InvalidateArrange();
#endif
		}

		public override void Invalidate(bool invalidateChildren)
		{
			if (tiled)
			{
				foreach (var tile in visibleTiles.Values)
				{
					Invalidate(tile);
				}
			}
			else
			{
				Render();
				if (Control != null)
				{
					//Control.InvalidateMeasure();
					//Control.InvalidateArrange();
				}
			}
		}

		public override void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			if (tiled)
			{
				foreach (var tile in visibleTiles.Values)
				{
					if (tile.Bounds.Intersects(rect))
						Invalidate(tile);
				}
			}
			else
				base.Invalidate(rect, invalidateChildren);
		}

		public void Update(Rectangle rect)
		{
			Invalidate(rect, false);
		}

		public bool CanFocus
		{
			get { 
#if TODO_XAML
				return Control.Focusable; 
#else
				throw new NotImplementedException();
#endif
			}
			set
			{
#if TODO_XAML
				if (value != Control.Focusable)
					Control.Focusable = value;
#else
				throw new NotImplementedException();
#endif
			}
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
#if TODO_XAML
			Control.Children.Add(content);
#endif
		}

		public override Color BackgroundColor
		{
#if TODO_XAML
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
#else
			get; set;
#endif
		}
	}

	/// <summary>
	/// See http://www.charlespetzold.com/blog/2012/08/WriteableBitmap-Pixel-Arrays-in-CSharp-and-CPlusPlus.html
	/// </summary>
	[Guid("905a0fef-bc53-11df-8c49-001e4fc686da"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	interface IBufferByteAccess
	{
		unsafe void Buffer(out byte* pByte);
	}
}
