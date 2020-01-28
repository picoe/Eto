using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Designer
{
	public class DesignSurface : Drawable
	{
		public static Size GripPadding = new Size(5, 5);
		static Size GripSize = new Size(2, 2);
		static Color GripColor = Colors.LightSkyBlue;
		List<Grip> _grips;
		bool _isSizing;
		Grip _currentGrip;
		PointF _startDrag;
		Grip _hoverGrip;
		Size? _originalContentSize;

		public bool EnableResizing { get; set; } = true;

		public DesignSurface()
		{
			if (!Platform.Instance.IsGtk) // doesn't work correctly on Gtk2 due to lack of control transparency
			{
				//var padding = GripPadding * 3;
				Padding = 32;// new Padding(padding.Width, 32, padding.Width, padding.Height);
				_grips = CreateGrips().ToList();
			}
			else
				_grips = new List<Grip>();
		}

		Control _content;

		public new Control Content
		{
			get { return _content; }
			set
			{
				_content = value;

				if (_content != null)
				{
					base.Content = TableLayout.AutoSized(_content, centered: true);
					_content.SizeChanged += content_SizeChanged;
				}
				else
				{
					base.Content = null;
				}
			}
		}

		private void content_SizeChanged(object sender, EventArgs e)
		{
			if (_content.Size == Size.Empty)
				return;
			_originalContentSize = _content?.Size;
			if (_sizeBounds != null)
				_content.Size = Size.Round(_sizeBounds.Value);
			_content.SizeChanged -= content_SizeChanged;
		}

		SizeF? _sizeBounds;
		RectangleF SizeBounds
		{
			get
			{
				if (_content == null)
					return RectangleF.Empty;
				var contentRect = RectangleFromScreen(_content.RectangleToScreen(new RectangleF(_content.Size)));
				return new RectangleF(contentRect.Location, _sizeBounds ?? contentRect.Size);
			}
			set
			{
				_sizeBounds = value.Size;
				_content.Size = Size.Round(value.Size);
				Invalidate();
			}
		}

		bool IsSizing
		{
			get { return _isSizing && EnableResizing; }
			set
			{
				if (_isSizing != value)
				{
					_isSizing = value;
					Invalidate();
				}
			}
		}

		class Grip
		{
			public Func<RectangleF> Location;
			public Action<SizeF> Update;
			public Action Start;
			public Action<Graphics> Draw;
			public Func<PointF, bool> ShouldDraw;
			public Cursor Cursor;
			public string ToolTip;

			public bool IsOver(PointF location)
			{
				return RectangleF.Inflate(Location(), new SizeF(2, 2)).Contains(location);
			}

			public bool GetShouldDraw(PointF location)
			{
				return ShouldDraw?.Invoke(location) == true || IsOver(location);
			}
		}

		RectangleF SizeBoundsWithPadding
		{
			get { return RectangleF.Inflate(SizeBounds, GripPadding); }
		}

		void UpdateSize(SizeF? topLeft = null, SizeF? bottomRight = null, SizeF? topRight = null, SizeF? bottomLeft = null)
		{
			var bounds = SizeBounds;

			if (topLeft != null)
				bounds.TopLeft += topLeft.Value;
			if (topRight != null)
				bounds.TopRight += topRight.Value;
			if (bottomLeft != null)
				bounds.BottomLeft += bottomLeft.Value;
			if (bottomRight != null)
				bounds.BottomRight += bottomRight.Value;
			SizeBounds = bounds;
		}

		IEnumerable<Grip> CreateGrips()
		{
			Func<PointF, RectangleF> gripRect = r => RectangleF.Inflate(new RectangleF(r, new SizeF(1, 1)), GripSize);

			yield return new Grip
			{
				Location = () => gripRect(SizeBoundsWithPadding.TopLeft),
				Update = diff => UpdateSize(topLeft: diff),
				Cursor = Cursors.Move
			};
			yield return new Grip
			{
				Location = () => gripRect(SizeBoundsWithPadding.TopRight),
				Update = diff => UpdateSize(topRight: diff),
				Cursor = Cursors.Move
			};
			yield return new Grip
			{
				Location = () => gripRect(SizeBoundsWithPadding.BottomLeft),
				Update = diff => UpdateSize(bottomLeft: diff),
				Cursor = Cursors.Move
			};
			yield return new Grip
			{
				Location = () => gripRect(SizeBoundsWithPadding.BottomRight),
				Update = diff => UpdateSize(bottomRight: diff),
				Cursor = Cursors.Move
			};
			yield return new Grip
			{
				Location = () => gripRect(SizeBoundsWithPadding.BottomRight),
				Update = diff => UpdateSize(bottomRight: diff),
				Cursor = Cursors.Move
			};
			Font font = SystemFonts.Default(8);
			SizeF gripSize = SizeF.Empty;
			yield return new Grip
			{
				Location = () => {
					var rect = SizeBoundsWithPadding;
					rect = new RectangleF(rect.Center.X - gripSize.Width / 2, rect.Top - gripSize.Height, gripSize.Width, gripSize.Height);
					return rect;
				},
				Draw = g =>
				{
					var rect = SizeBoundsWithPadding;
					var text = $"{_content?.Size.Width}x{_content?.Size.Height}";
					gripSize = g.MeasureString(font, text) + 4;
					//Padding = new Padding(15, Math.Max(15, (int)gripSize.Height + 5), 15, 15);
					rect = new RectangleF(rect.Center.X - gripSize.Width / 2, rect.Top - gripSize.Height, gripSize.Width, gripSize.Height);
					g.FillRectangle(GripColor, rect);
					g.DrawText(font, Colors.White, rect.Location + 2, text);
				},
				Start = () =>
				{
					_sizeBounds = null;
					_content.Size = _originalContentSize ?? new Size(-1, -1);
				},
				ShouldDraw = location => _sizeBounds != null,
				ToolTip = "Click to reset to auto size",
				Cursor = Cursors.Pointer
			};
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (_content != null)
			{
				var bounds = SizeBounds;
				bounds.Inflate(GripPadding);

				var mouseLocation = PointFromScreen(Mouse.Position);
				if (IsSizing)
					e.Graphics.DrawRectangle(GripColor, bounds);
				foreach (var grip in _grips)
				{
					if (!IsSizing && !grip.GetShouldDraw(mouseLocation))
						continue;
					if (grip.Draw != null)
						grip.Draw(e.Graphics);
					else
						e.Graphics.FillEllipse(GripColor, grip.Location());
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Buttons == MouseButtons.Primary)
			{
				_currentGrip = GetGrip(e.Location);
				if (_currentGrip != null)
				{
					_startDrag = e.Location;
					_currentGrip?.Start?.Invoke();
					Cursor = _currentGrip?.Cursor ?? Cursors.Default;
					e.Handled = true;
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (_currentGrip != null)
			{
				_currentGrip = null;
				Cursor = Cursors.Default;
				Invalidate();
				e.Handled = true;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (_currentGrip != null)
			{
				_currentGrip.Update?.Invoke((SizeF)(e.Location - _startDrag));
				_startDrag = _currentGrip.Location().Center;
				return;
			}

			var bounds = SizeBounds;
			var outer = RectangleF.Inflate(bounds, GripPadding * 2);
			IsSizing = outer.Contains(e.Location) && !bounds.Contains(e.Location);
			var grip = GetGrip(e.Location);
			if (grip != _hoverGrip)
			{
				_hoverGrip = grip;
				ToolTip = grip?.ToolTip;
				Invalidate();
			}
			Cursor = grip?.Cursor ?? Cursors.Default;
			Invalidate();
		}

		Grip GetGrip(PointF location)
		{
			return _grips.FirstOrDefault(r => r.IsOver(location));
		}
	}
}
