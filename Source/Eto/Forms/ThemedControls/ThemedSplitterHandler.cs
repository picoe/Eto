using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// ! UNDER CONSTRUCTION !
	/// Themed splitter handler for the <see cref="Splitter"/> control
	/// </summary>
	public class ThemedSplitterHandler : ThemedContainerHandler<TableLayout, Splitter, Splitter.ICallback>, Splitter.IHandler
	{
		readonly Panel splitter = new Panel();
		readonly Panel panel1 = new Panel();
		readonly Panel panel2 = new Panel();
		Orientation orient;
		SplitterFixedPanel fix;
		int? position;
		int swidth = 5;
		double relative = double.NaN;

		/// <summary>
		/// Called to initialize this widget after it has been constructed
		/// </summary>
		protected override void Initialize()
		{
			UpdateLayout();
			splitter.MouseMove += SplitterMouseMove;
			base.Initialize();
		}

		void SplitterMouseMove(object sender, MouseEventArgs e)
		{
			if (e.Buttons != MouseButtons.Primary)
				return;
			e.Handled = true;
			Position = (int)Math.Round((Orientation == Orientation.Horizontal
				? e.Location.X + splitter.Location.X : e.Location.Y + splitter.Location.Y) - swidth*0.5);
		}

		void UpdateLayout(bool positionOnly = false)
		{
			panel1.SuspendLayout();
			panel2.SuspendLayout();
			if (positionOnly)
				SuspendLayout();
			else
			{
				if (!Widget.Loaded)
				{
					if (fix != SplitterFixedPanel.Panel1)
						panel1.Size = new Size(-1, -1);
					if (fix != SplitterFixedPanel.Panel2)
						panel2.Size = new Size(-1, -1);
				}
			}
			if (position.HasValue)
			{
				if (orient == Orientation.Horizontal)
				{
					panel1.Width = position.Value;
					if (Widget.Loaded)
						panel2.Width = Math.Max(0, Widget.Width - swidth - panel1.Width);
				}
				else
				{
					panel1.Height = position.Value;
					if (Widget.Loaded)
						panel2.Height = Math.Max(0, Widget.Height - swidth - panel1.Height);
				}
			}
			else if (!double.IsNaN(relative)) switch (fix)
			{
				case SplitterFixedPanel.Panel1:
					if (orient == Orientation.Horizontal)
					{
						panel1.Width = (int)Math.Round(relative);
						if (Widget.Loaded)
							panel2.Width = Math.Max(0, Widget.Width - swidth - panel1.Width);
					}
					else
					{
						panel1.Height = (int)Math.Round(relative);
						if (Widget.Loaded)
							panel2.Height = Math.Max(0, Widget.Height - swidth - panel1.Height);
					}
					break;
				case SplitterFixedPanel.Panel2:
					if (orient == Orientation.Horizontal)
					{
						panel2.Width = (int)Math.Round(relative);
						if (Widget.Loaded)
							panel1.Width = Math.Max(0, Widget.Width - swidth - panel2.Width);
					}
					else
					{
						panel2.Height = (int)Math.Round(relative);
						if (Widget.Loaded)
							panel1.Height = Math.Max(0, Widget.Height - swidth - panel2.Height);
					}
					break;
				default:
					if (orient == Orientation.Horizontal)
					{
						var sz = Control.Width - swidth;
						panel1.Width = sz <= 0 ? -1 : (int)Math.Round(relative * sz);
						panel2.Width = sz <= 0 ? -1 : (int)Math.Round((1-relative) * sz);
					}
					else
					{
						var sz = Control.Height - swidth;
						panel1.Height = sz <= 0 ? -1 : (int)Math.Round(relative * sz);
						panel2.Height = sz <= 0 ? -1 : (int)Math.Round((1-relative) * sz);
					}
					break;
			}

			if (!positionOnly)
			{
				if (orient == Orientation.Horizontal)
				{
					splitter.Cursor = Cursors.VerticalSplit;
					splitter.Size = new Size(swidth, -1);
					Control = new TableLayout {
						Padding = Padding.Empty,
						Spacing = Size.Empty,
						Rows = { new TableRow(
							new TableCell(panel1, fix != SplitterFixedPanel.Panel1),
							new TableCell(splitter),
							new TableCell(panel2, fix != SplitterFixedPanel.Panel2)
						)}
					};
				}
				else
				{
					splitter.Cursor = Cursors.HorizontalSplit;
					splitter.Size = new Size(-1, swidth);
					Control = new TableLayout {
						Padding = Padding.Empty,
						Spacing = Size.Empty,
						Rows = { 
							new TableRow(new TableCell(panel1, true)) {
								ScaleHeight = fix != SplitterFixedPanel.Panel1
							},
							new TableRow(new TableCell(splitter, true)),
							new TableRow(new TableCell(panel2, true)) {
								ScaleHeight = fix != SplitterFixedPanel.Panel2
							}
						}
					};
				}
			}
			else
				ResumeLayout();
			panel1.ResumeLayout();
			panel2.ResumeLayout();
		}

		/// <summary>
		/// Gets or sets the orientation of the panels in the splitter.
		/// </summary>
		public Orientation Orientation
		{
			get { return orient; }
			set
			{
				if (value == orient)
					return;
				orient = value;
				UpdateLayout();
			}
		}

		/// <summary>
		/// Gets or sets the panel with fixed size.
		/// </summary>
		public SplitterFixedPanel FixedPanel
		{
			get { return fix; }
			set
			{
				if (value == fix)
					return;
				fix = value;
				UpdateLayout();
			}
		}

		/// <summary>
		/// Gets or sets the position of the splitter from the left or top, in pixels.
		/// </summary>
		public int Position
		{
			get
			{
				return panel1.Content == null || !panel1.Content.Visible ? 0
						: orient == Orientation.Horizontal
					? panel1.Width : panel2.Height;
			}
			set
			{
				if (value == position)
					return;
				position = value;
				relative = double.NaN;
				UpdateLayout(true);
				Callback.OnPositionChanged(Widget, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the relative position of the splitter which is based on <see cref="FixedPanel"/>.
		/// </summary>
		public double RelativePosition
		{
			get
			{
			//	if (!double.IsNaN(relative))
			//		return relative;
				switch (fix)
				{
					case SplitterFixedPanel.Panel1:
						return panel1.Width;
					case SplitterFixedPanel.Panel2:
						return panel2.Width;
					default:
						return panel1.Width / (double)(panel1.Width + panel2.Width);
				}
			}
			set
			{
				position = null;
				relative = value;
				UpdateLayout(true);
			}
		}

		/// <summary>
		/// Gets or sets size of the splitter/gutter
		/// </summary>
		public int SplitterWidth
		{
			get { return swidth; }
			set
			{
				if (value == swidth)
					return;
				swidth = value;
				if (orient == Orientation.Horizontal)
					splitter.Width = swidth;
				else
					splitter.Height = swidth;
				UpdateLayout(true);
			}
		}

		/// <summary>
		/// Gets or sets the top or left panel of the splitter.
		/// </summary>
		public Control Panel1
		{
			get { return panel1; }
			set { panel1.Content = value; }
		}

		/// <summary>
		/// Gets or sets the bottom or right panel of the splitter.
		/// </summary>
		public Control Panel2
		{
			get { return panel2; }
			set { panel2.Content = value; }
		}

		/// <summary>
		/// Gets the panel used as the splitter between the two panes.
		/// </summary>
		public Panel Splitter
		{
			get { return splitter; }
		}

		/// <summary>
		/// Gets or sets the minimal size of the first panel.
		/// </summary>
		/// <value>The minimal size of the first panel.</value>
        public int Panel1MinimumSize
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

		/// <summary>
		/// Gets or sets the minimal size of the second panel.
		/// </summary>
		/// <value>The minimal size of the second panel.</value>
        public int Panel2MinimumSize
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
