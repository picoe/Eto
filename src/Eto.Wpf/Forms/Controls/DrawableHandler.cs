using System;
using System.Collections.Generic;
using System.Linq;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;
using swi = System.Windows.Input;
using Eto.Wpf.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class DrawableHandler : WpfPanel<swc.Canvas, Drawable, Drawable.ICallback>, Drawable.IHandler
	{
		bool tiled;
		sw.FrameworkElement content;
		Scrollable scrollable;
		readonly Dictionary<int, EtoTile> visibleTiles = new Dictionary<int, EtoTile>();
		List<EtoTile> invalidateTiles;
		readonly List<EtoTile> unusedTiles = new List<EtoTile>();
		Size maxTiles;
		Size tileSize = new Size(100, 100);

		static readonly object OptimizedInvalidateRectKey = new object();
		public bool OptimizedInvalidateRect
		{
			get { return Widget.Properties.Get<bool>(OptimizedInvalidateRectKey, true); }
			set { Widget.Properties.Set(OptimizedInvalidateRectKey, value, true); }
		}

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
					var rect = new sw.Rect(0, 0, ActualWidth, ActualHeight);
					var cliprect = rect.ToEto();
					if (!cliprect.IsEmpty)
					{
						using (var graphics = new Graphics(new GraphicsHandler(this, dc, rect, new RectangleF(Handler.ClientSize), false)))
						{
							Handler.Callback.OnPaint(Handler.Widget, new PaintEventArgs(graphics, cliprect));
						}
					}
				}
			}

			protected override sw.Size MeasureOverride(sw.Size constraint)
			{
				var content = Handler.content;
				if (content != null)
				{
					content.Measure(constraint);
					return Handler.MeasureOverride(constraint, c => {
						base.MeasureOverride(c);
						return content.DesiredSize;
						});
				}
				return Handler.MeasureOverride(constraint, base.MeasureOverride);
			}

			protected override sw.Size ArrangeOverride(sw.Size arrangeSize)
			{
				base.ArrangeOverride(arrangeSize);
				var content = Handler.content;
				if (content != null)
					content.Arrange(new sw.Rect(arrangeSize));
				return arrangeSize;
			}
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
						RenderTransform = new swm.TranslateTransform(-bounds.X, -bounds.Y);
					}
				}
			}

			public Point Position { get; set; }

			public int Key { get { return Position.Y * Handler.maxTiles.Width + Position.X; } }

			protected override void OnRender(swm.DrawingContext drawingContext)
			{
				var graphics = new Graphics(new GraphicsHandler(this, drawingContext, bounds.ToWpf(), new RectangleF(Handler.ClientSize), false));
				Handler.Callback.OnPaint(Handler.Widget, new PaintEventArgs(graphics, Bounds));
			}
		}

		protected override bool NeedsPixelSizeNotifications {  get { return true; } }

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);

			RegisterScrollable();
		}

		protected override void OnLogicalPixelSizeChanged()
		{
			if (Control.IsLoaded)
				Invalidate(false);
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			UnRegisterScrollable();
		}

		public void Create()
		{
			Control = new EtoMainCanvas
			{
				Handler = this,
				SnapsToDevicePixels = true,
				FocusVisualStyle = null,
				Background = swm.Brushes.Transparent
			};
			Control.Loaded += Control_Loaded;
		}

		public void Create(bool largeCanvas)
		{
			AllowTiling = largeCanvas;
			Create();
		}

		void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			UpdateTiles(true);
			Control.Loaded -= Control_Loaded; // only perform once
        }

		public virtual Graphics CreateGraphics()
		{
			throw new NotSupportedException();
		}

		void Control_SizeChanged(object sender, sw.SizeChangedEventArgs e)
		{
			SetMaxTiles();
			UpdateTiles(true);
			Invalidate(false);
			content.Width = e.NewSize.Width;
			content.Height = e.NewSize.Height;
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
		}

		void UpdateTiles(bool rebuildKeys = false)
		{
			if (!tiled || !Control.IsLoaded)
				return;

			var controlSize = new Size((int)Control.ActualWidth, (int)Control.ActualHeight);
			var rect = new Rectangle(controlSize);
			var scroll = scrollable;
			if (scroll != null)
			{
				// only show tiles in the visible rect of the scrollable
				var visibleRect = new Rectangle(scroll.ClientSize);
				var scrollableHandler = (ScrollableHandler)scroll.Handler;
				visibleRect.Offset(-Control.TranslatePoint(new sw.Point(), scrollableHandler.ContentControl).ToEtoPoint());
				rect.Intersect(visibleRect);
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
							tile.InvalidateVisual();
						}
						else
						{
							// need a new tile, no cached ones left
							tile = new EtoTile
							{
								Handler = this,
								SnapsToDevicePixels = true,
								Position = position,
								Bounds = bounds
							};
							Control.Children.Add(tile);
						}
						// set tile as visible
						visibleTiles[key] = tile;
					}
					else
					{
						tile.Bounds = bounds;
						tile.InvalidateVisual();
					}

				}
			}
		}

		void scrollable_Scroll(object sender, ScrollEventArgs e)
		{
			UpdateTiles();
		}

		public override void Invalidate(bool invalidateChildren)
		{
			if (tiled)
			{
				foreach (var tile in visibleTiles.Values)
				{
					tile.InvalidateVisual();
				}
			}
			else
			{
				if (invalidateTiles != null)
				{
					invalidateTiles.ForEach(t => t.Visibility = sw.Visibility.Collapsed);
					unusedTiles.AddRange(invalidateTiles);
					invalidateTiles.Clear();
				}
				base.Invalidate(invalidateChildren);
			}
		}

		public override void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			if (tiled)
			{
				foreach (var tile in visibleTiles.Values)
				{
					if (tile.Bounds.Intersects(rect))
						tile.InvalidateVisual();
				}
			}
			else if (OptimizedInvalidateRect)
			{
				if (((rect.Width * rect.Height) / (Control.ActualWidth * Control.ActualHeight)) > 0.9)
				{
					Invalidate(false);
					return;
				}

				if (invalidateTiles == null)
					invalidateTiles = new List<EtoTile>();

				var overlappingTiles = new List<EtoTile>();
				foreach (var overlappingTile in invalidateTiles)
				{
					if (rect == overlappingTile.Bounds)
					{
						overlappingTile.InvalidateVisual();
						return;
					}
					else if (rect.Intersects(overlappingTile.Bounds))
					{
						rect.Union(overlappingTile.Bounds);
						overlappingTiles.Add(overlappingTile);
					}
				}

				EtoTile tile;
				if (unusedTiles.Count > 0)
				{
					tile = unusedTiles[unusedTiles.Count - 1];
					tile.Bounds = rect;
					tile.Visibility = sw.Visibility.Visible;
					unusedTiles.Remove(tile);
				}
				else
				{
					tile = new EtoTile
					{
						Handler = this,
						SnapsToDevicePixels = true
					};
					tile.Bounds = rect;
					Control.Children.Add(tile);
				}
				invalidateTiles.Add(tile);

				foreach (var overlappingTile in overlappingTiles)
				{
					overlappingTile.Visibility = sw.Visibility.Collapsed;
					invalidateTiles.Remove(overlappingTile);
					unusedTiles.Add(overlappingTile);
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
			get { return Control.Focusable; }
			set
			{
				if (value != Control.Focusable)
				{
					Control.Focusable = value;
				}
			}
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			this.content = content;
			Control.Children.Add(content);
			ContainerControl.InvalidateMeasure();
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}
	}
}