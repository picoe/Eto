using Eto.Drawing;
using System;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// Display Mode of the <see cref="ThemedSpinnerHandler"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum ThemedSpinnerMode
	{
		/// <summary>
		/// Shows lines for each element in the spinner
		/// </summary>
		Line,
		/// <summary>
		/// Shows dots for each element in the spinner
		/// </summary>
		Circle,
	}

	/// <summary>
	/// Direction to spin the <see cref="ThemedSpinnerHandler"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum ThemedSpinnerDirection
	{
		/// <summary>
		/// Spins clockwise
		/// </summary>
		Clockwise = 1,
		/// <summary>
		/// Spins counter-clockwise
		/// </summary>
		CounterClockwise = -1
	}

	/// <summary>
	/// Themed spinner handler for the <see cref="Spinner"/> control
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ThemedSpinnerHandler : ThemedControlHandler<Drawable, Spinner, Spinner.ICallback>, Spinner.IHandler
	{
		UITimer timer;
		int numberOfElements;
		int numberOfVisibleElements;
		float currentValue;

		/// <summary>
		/// Gets or sets the increment for each step when the spinner is animating, where 1 is equal to the distance from one element to the other
		/// </summary>
		/// <value>The increment.</value>
		public float Increment { get; set; }

		/// <summary>
		/// Gets or sets the direction to spin
		/// </summary>
		/// <value>The direction.</value>
		public ThemedSpinnerDirection Direction { get; set; }

		/// <summary>
		/// Gets or sets the alpha of the marks when the spinner is not spinning (disabled)
		/// </summary>
		/// <value>The disabled alpha.</value>
		public float DisabledAlpha { get; set; }

		/// <summary>
		/// Gets or sets the main color of each element
		/// </summary>
		/// <value>The color of the element.</value>
		public Color ElementColor { get; set; }

		/// <summary>
		/// Gets or sets the line thickness relative to the control size, when the <see cref="Mode"/> is a Line
		/// </summary>
		/// <value>The line thickness.</value>
		public float LineThickness { get; set; }

		/// <summary>
		/// Gets or sets the line cap when the <see cref="Mode"/> is set to line
		/// </summary>
		/// <value>The line cap.</value>
		public PenLineCap LineCap { get; set; }

		/// <summary>
		/// Gets or sets the size of each element, relative to the control size
		/// </summary>
		/// <value>The size of the element.</value>
		public float ElementSize { get; set; }

		/// <summary>
		/// Gets or sets the display mode of the spinner (e.g. Line/Circle)
		/// </summary>
		/// <value>The mode.</value>
		public ThemedSpinnerMode Mode { get; set; }

		/// <summary>
		/// Gets or sets the number of elements to display
		/// </summary>
		/// <value>The number of elements to display</value>
		public int NumberOfElements
		{
			get { return numberOfElements; }
			set
			{
				numberOfElements = value;
				numberOfVisibleElements = Math.Max(numberOfElements, numberOfVisibleElements);
			}
		}

		/// <summary>
		/// Gets or sets the number of visible elements while animating. This must be less than or equal to <see cref="NumberOfElements"/>
		/// </summary>
		/// <value>The number of visible elements.</value>
		public int NumberOfVisibleElements
		{
			get { return numberOfVisibleElements; }
			set { numberOfVisibleElements = Math.Min(numberOfElements, value); }
		}

		/// <summary>
		/// Gets or sets the speed of the spinner, in seconds for each tick.
		/// </summary>
		/// <value>The speed in seconds</value>
		public double Speed
		{
			get { return timer.Interval; }
			set { timer.Interval = value; }
		}

		/// <summary>
		/// Called to initialize this widget after it has been constructed
		/// </summary>
		/// <remarks>Override this to initialize any of the platform objects. This is called
		/// in the widget constructor, after all of the widget's constructor code has been called.</remarks>
		protected override void Initialize()
		{
			Control = new Drawable();
			Control.Enabled = false;
			Control.Size = new Size(16, 16);
			Control.Paint += HandlePaint;
			timer = new UITimer();
			timer.Interval = 0.05f;
			timer.Elapsed += HandleElapsed;

			// defaults
			numberOfElements = 12;
			numberOfVisibleElements = 10;
			currentValue = 0.5f; // so line starts vertical at top
			Increment = 1f;
			DisabledAlpha = 1f / 8f;
			ElementColor = Colors.Black;
			ElementSize = 0.6f;
			LineThickness = 1f;
			LineCap = PenLineCap.Round;
			Direction = ThemedSpinnerDirection.Clockwise;
			base.Initialize();
		}

		void HandleElapsed(object sender, EventArgs e)
		{
			currentValue += Increment;
			Control.Invalidate();
		}

		/// <summary>
		/// Called after all other controls have been loaded
		/// </summary>
		/// <param name="e">Event arguments</param>
		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			if (Enabled)
				timer.Start();
		}

		/// <summary>
		/// Called when the control is unloaded, which is when it is not currently on a displayed window
		/// </summary>
		/// <param name="e">Event arguments</param>
		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			if (Enabled)
				timer.Stop();
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.ThemedControls.ThemedSpinnerHandler"/> is enabled (spinning)
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public override bool Enabled
		{
			get { return Control.Enabled; }
			set
			{
				if (Enabled != value)
				{
					Control.Enabled = value;
					if (Widget.Loaded)
					{
						if (value)
							timer.Start();
						else
							timer.Stop();
						Control.Invalidate();
					}
				}
			}
		}

		void HandlePaint(object sender, PaintEventArgs e)
		{
			var controlSize = Control.Size;
			var minSize = Math.Min(controlSize.Width, controlSize.Height);
			float angle = 360.0F / numberOfElements;

			e.Graphics.TranslateTransform(controlSize.Width / 2.0F, controlSize.Height / 2.0F);
			e.Graphics.RotateTransform(angle * currentValue * (int)Direction);
			e.Graphics.AntiAlias = true;

			for (int i = 1; i <= numberOfElements; i++)
			{
				float alphaValue = (float)i / numberOfVisibleElements;
				if (alphaValue > 1f)
					alphaValue = 0f;
				float alpha = Enabled ? alphaValue : DisabledAlpha;
				var elementColor = new Color(ElementColor, alpha);

				float rate = 5F / ElementSize;
				float size = controlSize.Width / rate;

				float diff = (controlSize.Width / 5F) - size;
				float x = (controlSize.Width / 9.0F) + diff;
				float y = (controlSize.Height / 9.0F) + diff;
				e.Graphics.RotateTransform(angle * (int)Direction);

				switch (Mode)
				{
					case ThemedSpinnerMode.Circle:
						using (var brush = new SolidBrush(elementColor))
						{
							e.Graphics.FillEllipse(brush, x, y, size, size);
						}
						break;
					case ThemedSpinnerMode.Line:
						using (var pen = new Pen(elementColor, LineThickness * minSize / 16))
						{
							pen.LineCap = LineCap;
							x -= pen.Thickness / 2;
							y -= pen.Thickness / 2;
							e.Graphics.DrawLine(pen, x, y, x + size, y + size);
						}
						break;
				}
			}
		}
	}
}

