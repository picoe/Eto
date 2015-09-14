using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", "Draw Loop")]
	public class DrawLoopSection : Scrollable
	{
		readonly Drawable drawable;
		readonly DirectDrawingRenderer renderer;
		readonly Panel content;
		bool useCreateGraphics;
		Action drawFrame;
		Status status = new Status();

		class Status
		{
			public bool Stop { get; set; }
		}

		public DrawLoopSection()
		{
			drawable = new Drawable
			{
				Style = "direct",
				BackgroundColor = Colors.Black
			};
			drawable.Paint += (sender, e) => renderer.DrawFrame(e.Graphics, drawable.Size);
			renderer = new DirectDrawingRenderer();

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };
			layout.AddSeparateRow(null, UseTexturesAndGradients(), UseCreateGraphics(), null);
			layout.Add(content = new Panel { Content = drawable });
			this.Content = layout;
		}

		async void DrawLoop(object data)
		{
			var currentStatus = (Status)data;
			renderer.RestartFPS();
			while (!currentStatus.Stop)
			{
				var draw = drawFrame;
				if (draw != null)
					Application.Instance.Invoke(draw);
				await Task.Delay(0);
			}
		}

		void SetMode()
		{
			drawFrame = null;
			status.Stop = true;
			if (useCreateGraphics)
			{
				renderer.EraseBoxes = true;
				if (!drawable.SupportsCreateGraphics)
				{
					content.BackgroundColor = Colors.Red;
					content.Content = new Label { Text = "This platform does not support Drawable.CreateGraphics", TextColor = Colors.White, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center };
					return;
				}
				drawFrame = DrawWithCreateGraphics;
				using (var graphics = drawable.CreateGraphics())
					graphics.Clear();
			}
			else
			{
				renderer.EraseBoxes = false;
				drawFrame = () => { if (!status.Stop) drawable.Invalidate(); };
			}
			content.Content = drawable;
			status = new Status();
			Task.Run(() => DrawLoop(status));
		}

		protected override void OnUnLoad(EventArgs e)
		{
			status.Stop = true;
			base.OnUnLoad(e);
		}

		void DrawWithCreateGraphics()
		{
			if (status.Stop)
				return;
			using (var graphics = drawable.CreateGraphics())
			{
				renderer.DrawFrame(graphics, drawable.Size);
			}
		}

		Control UseTexturesAndGradients()
		{
			var control = new CheckBox
			{
				Text = "Use Textures && Gradients",
				Checked = renderer.UseTexturesAndGradients
			};
			control.CheckedChanged += (sender, e) =>
			{
				renderer.UseTexturesAndGradients = control.Checked ?? false;
				lock (renderer.Boxes)
				{
					renderer.Boxes.Clear();
					renderer.RestartFPS();
				}
				if (useCreateGraphics && drawable.SupportsCreateGraphics)
					using (var graphics = drawable.CreateGraphics())
						graphics.Clear();
			};
			return control;
		}

		Control UseCreateGraphics()
		{
			var control = new CheckBox
			{
				Text = "Use CreateGraphics",
				Checked = useCreateGraphics
			};
			control.CheckedChanged += (sender, e) =>
			{
				useCreateGraphics = control.Checked ?? false;
				renderer.RestartFPS();
				Application.Instance.AsyncInvoke(SetMode);
			};
			return control;
		}

		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			SetMode();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				status.Stop = true;
			base.Dispose(disposing);
		}
	}

	public class DirectDrawingRenderer
	{
		readonly Image texture;
		readonly Font font;
		readonly SolidBrush textBrush;

		public readonly Stopwatch Watch = new Stopwatch();
		public int TotalFrames { get; set; }
		public long PreviousFrameStartTicks { get; set; }
		public readonly List<Box> Boxes = new List<Box>();
		public bool UseTexturesAndGradients { get; set; }
		public bool EraseBoxes { get; set; }

		public DirectDrawingRenderer()
		{
			texture = TestIcons.Textures;
			font = SystemFonts.Default();
			textBrush = new SolidBrush(Colors.White);
		}

		public void RestartFPS()
		{
			Watch.Restart();
			TotalFrames = 0;
		}

		public class Box
		{
			static readonly Random random = new Random();
			SizeF increment;
			readonly Color color;
			readonly float rotation;
			float angle;
			readonly Action<Graphics> draw;
			readonly Action<Graphics> erase;
			readonly Brush fillBrush;
			RectangleF position;
			IMatrix transform;

			public SizeF Increment { get { return increment; } set { increment = value; } }

			static Color GetRandomColor(Random random)
			{
				return Color.FromArgb(random.Next(byte.MaxValue), random.Next(byte.MaxValue), random.Next(byte.MaxValue));
			}

			public Box(Size canvasSize, bool useTexturesAndGradients, DirectDrawingRenderer renderer)
			{
				var size = new SizeF(random.Next(50) + 50, random.Next(50) + 50);
				var location = new PointF(random.Next(canvasSize.Width - (int)size.Width), random.Next(canvasSize.Height - (int)size.Height));
				position = new RectangleF(location, size);
				increment = new SizeF(random.Next(3) + 1, random.Next(3) + 1);
				if (random.Next(2) == 1)
					increment.Width = -increment.Width;
				if (random.Next(2) == 1)
					increment.Height = -increment.Height;

				angle = random.Next(360);
				rotation = (random.Next(20) - 10f) / 4f;

				var rect = new RectangleF(size);
				color = GetRandomColor(random);
				var colorPen = new Pen(color);
				var blackPen = Pens.Black;
				var blackBrush = Brushes.Black;
				switch (random.Next(useTexturesAndGradients ? 5 : 2))
				{
					case 0:
						draw = g => g.DrawRectangle(colorPen, rect);
						erase = g => g.DrawRectangle(blackPen, rect);
						break;
					case 1:
						draw = g => g.DrawEllipse(colorPen, rect);
						erase = g => g.DrawEllipse(blackPen, rect);
						break;
					case 2:
						switch (random.Next(2))
						{
							case 0:
								fillBrush = new LinearGradientBrush(GetRandomColor(random), GetRandomColor(random), PointF.Empty, new PointF(size.Width, size.Height));
								break;
							case 1:
								fillBrush = new TextureBrush(renderer.texture)
								{
									Transform = Matrix.FromScale(size / 80)
								};
								break;
						}
						draw = g => g.FillEllipse(fillBrush, rect);
						erase = g => g.FillEllipse(blackBrush, rect);
						break;
					case 3:
						switch (random.Next(3))
						{
							case 0:
								fillBrush = new LinearGradientBrush(GetRandomColor(random), GetRandomColor(random), PointF.Empty, new PointF(size.Width, size.Height));
								break;
							case 1:
								fillBrush = new TextureBrush(renderer.texture)
								{
									Transform = Matrix.FromScale(size / 80)
								};
								break;
							case 2:
								fillBrush = new RadialGradientBrush(GetRandomColor(random), GetRandomColor(random), (PointF)size / 2, (PointF)size / 2, size);
								break;
						}
						draw = g => g.FillRectangle(fillBrush, rect);
						erase = g => g.FillRectangle(blackBrush, rect);
						break;
					case 4:
						var font = Fonts.Sans(random.Next(20) + 4);
						draw = g => g.DrawText(font, color, 0, 0, "Some Text");
						erase = g => g.DrawText(font, Colors.Black, 0, 0, "Some Text");
						break;
				}
			}

			public void Move(Size bounds)
			{
				position.Offset(increment);
				var center = position.Center;
				if (increment.Width > 0 && center.X >= bounds.Width)
					increment.Width = -increment.Width;
				else if (increment.Width < 0 && center.X < 0)
					increment.Width = -increment.Width;

				if (increment.Height > 0 && center.Y >= bounds.Height)
					increment.Height = -increment.Height;
				else if (increment.Height < 0 && center.Y < 0)
					increment.Height = -increment.Height;
				angle += rotation;

				transform = Matrix.FromTranslation(position.Location);
				transform.RotateAt(angle, position.Width / 2, position.Height / 2);
			}

			public void Erase(Graphics graphics)
			{
				if (transform != null)
				{
					graphics.SaveTransform();
					graphics.MultiplyTransform(transform);
					erase(graphics);
					graphics.RestoreTransform();
				}
			}

			public void Draw(Graphics graphics)
			{
				graphics.SaveTransform();
				graphics.MultiplyTransform(transform);
				draw(graphics);
				graphics.RestoreTransform();
			}
		}

		void InitializeBoxes(Size canvasSize)
		{
			for (int i = 0; i < 20; i++)
				Boxes.Add(new Box(canvasSize, UseTexturesAndGradients, this));
		}

		public void DrawFrame(Graphics graphics, Size canvasSize)
		{
			if (graphics == null)
				return;
			lock (Boxes)
			{
				if (Boxes.Count == 0 && canvasSize.Width > 1 && canvasSize.Height > 1)
					InitializeBoxes(canvasSize);

				var fps = TotalFrames / Watch.Elapsed.TotalSeconds;
				// The frames per second as determined by the last frame. Measuring a single frame
				// must include EndDraw, since that is when the pipeline is flushed to the device.
				var frameTicks = Watch.ElapsedTicks - PreviousFrameStartTicks;
				var lastFrameFps = Stopwatch.Frequency / Math.Max(frameTicks, 1);
				PreviousFrameStartTicks = Watch.ElapsedTicks;
				var fpsText = string.Format("Frames per second since start: {0:0.00}, last: {1:0.00}", fps, lastFrameFps);
				var start = Watch.ElapsedTicks;
				if (EraseBoxes)
					graphics.FillRectangle(Colors.Black, new RectangleF(graphics.MeasureString(font, fpsText)));
				graphics.DrawText(font, textBrush, 0, 0, fpsText);

				var bounds = canvasSize;
				graphics.AntiAlias = false;
				foreach (var box in Boxes)
				{
					if (EraseBoxes)
						box.Erase(graphics);
					box.Move(bounds);
					box.Draw(graphics);
				}
				TotalFrames++;
			}
		}
	}
}
