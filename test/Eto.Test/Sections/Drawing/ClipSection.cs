using Eto.Drawing;
using Eto.Forms;
using System.ComponentModel;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", "Clip")]
	public class ClipSection : Scrollable, INotifyPropertyChanged
	{
		bool resetClip;

		public bool ResetClip
		{
			get { return resetClip; }
			set
			{
				resetClip = value;
				OnPropertyChanged(new PropertyChangedEventArgs("ResetClip"));
			}
		}

		public ClipSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddSeparateRow(null, ResetClipControl(), null);
			layout.BeginVertical();
			layout.AddRow(new Label { Text = "Rectangle Clip" }, RectangleClip());
			layout.AddRow(new Label { Text = "GraphicsPath Clip" }, PathClip());
			layout.EndVertical();
			layout.Add(null);

			Content = layout;
		}

		Control RectangleClip()
		{
			var control = new Drawable { Size = new Size(300, 100) };
			control.Paint += (sender, e) =>
			{
				e.Graphics.SetClip(new RectangleF(25, 25, 50, 50));
				if (ResetClip)
					e.Graphics.ResetClip();
				e.Graphics.FillRectangle(Brushes.Blue, new RectangleF(25, 0, 100, 100));

				e.Graphics.SetClip(new RectangleF(125, 25, 50, 50));
				if (ResetClip)
					e.Graphics.ResetClip();
				e.Graphics.FillRectangle(Brushes.Red, new RectangleF(125, 0, 100, 100));

				e.Graphics.SetClip(new RectangleF(225, 25, 50, 50));
				if (ResetClip)
					e.Graphics.ResetClip();
				e.Graphics.FillRectangle(Brushes.Green, new RectangleF(225, 0, 100, 100));
			};
			PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "ResetClip")
					control.Invalidate();
			};
			return control;
		}

		Control PathClip()
		{
			var control = new Drawable { Size = new Size(350, 250) };
			control.Paint += (sender, e) =>
			{
				var path = new GraphicsPath();
				path.AddEllipse(25, 25, 50, 50);
				path.AddRectangle(125, 25, 50, 50);
				path.AddLines(new PointF(225, 25), new PointF(225, 75), new PointF(275, 50));
				path.CloseFigure();

				e.Graphics.SetClip(path);
				if (ResetClip)
					e.Graphics.ResetClip();
				e.Graphics.FillRectangle(Brushes.Blue, path.Bounds);

				path.Transform(Matrix.FromTranslation(0, 75));
				e.Graphics.SetClip(path);
				if (ResetClip)
					e.Graphics.ResetClip();
				e.Graphics.FillRectangle(Brushes.Red, path.Bounds);

				path.Transform(Matrix.FromTranslation(0, 75));
				e.Graphics.SetClip(path);
				if (ResetClip)
					e.Graphics.ResetClip();
				e.Graphics.FillRectangle(Brushes.Green, path.Bounds);
			};
			PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "ResetClip")
					control.Invalidate();
			};
			return control;
		}

		Control ResetClipControl()
		{
			var control = new CheckBox { Text = "Reset Clip" };
			control.CheckedBinding.Bind(() => ResetClip, v => ResetClip = v ?? false);
			return control;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
	}
}
