using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Printing
{
	public class PrintDialogSection : Panel
	{
		PrintSettings settings = new PrintSettings();
		NumericUpDown selectedEnd;
		NumericUpDown maximumEnd;
		CheckBox allowPageRange;
		CheckBox allowSelection;

		public PrintDialogSection()
		{
			this.DataContext = settings;

			var layout = new DynamicLayout();

			layout.BeginVertical();
			layout.BeginHorizontal();
			layout.Add(null);
			layout.BeginVertical(Padding.Empty);
			layout.AddSeparateRow(null, ShowPrintDialog(), null);
			layout.AddSeparateRow(null, PrintFromGraphicsWithDialog(), null);
			layout.AddSeparateRow(null, PrintFromGraphics(), null);
			layout.EndBeginVertical();
			layout.Add(PrintDialogOptions());
			layout.Add(null);
			layout.EndVertical();
			layout.Add(null);
			layout.EndHorizontal();
			layout.EndVertical();
			layout.AddSeparateRow(null, PageRange(), Settings(), null);

			layout.Add(null);
			Content = layout;
		}

		Control PrintDialogOptions()
		{
			var layout = new DynamicLayout();

			layout.AddRow(null, AllowPageRange());
			layout.AddRow(null, AllowSelection());

			return new GroupBox { Text = "Print Dialog Options", Content = layout };
		}

		Control AllowPageRange()
		{
			return allowPageRange = new CheckBox { Text = "Allow Page Range", Checked = new PrintDialog().AllowPageRange };
		}

		Control AllowSelection()
		{
			return allowSelection = new CheckBox { Text = "Allow Selection", Checked = new PrintDialog().AllowSelection };
		}

		PrintDialog CreatePrintDialog()
		{
			return new PrintDialog
			{ 
				PrintSettings = settings,
				AllowSelection = allowSelection.Checked ?? false,
				AllowPageRange = allowPageRange.Checked ?? false
			};
		}

		Control ShowPrintDialog()
		{
			var control = new Button { Text = "Show Print Dialog" };

			control.Click += delegate
			{
				var print = CreatePrintDialog();
				var ret = print.ShowDialog(this.ParentWindow);
				if (ret == DialogResult.Ok)
				{
					this.DataContext = settings = print.PrintSettings;
				}
			};

			return control;
		}

		PrintDocument GetPrintDocument()
		{
			var document = new PrintDocument();
			document.PrintSettings = settings;
			var font = Fonts.Serif(16);
			var printTime = DateTime.Now;
			document.PrintPage += (sender, e) => {
				Size pageSize = Size.Round(e.PageSize);

				// draw a border around the printable area
				var rect = new Rectangle(pageSize);
				rect.Inflate(-1, -1);
				e.Graphics.DrawRectangle(Pens.Silver(), rect);

				// draw title
				e.Graphics.DrawText(font, Colors.Black, new Point(50, 20), document.Name);

				// draw page number
				var text = string.Format("page {0} of {1}", e.CurrentPage + 1, document.PageCount);
				var textSize = Size.Round(e.Graphics.MeasureString(font, text));
				e.Graphics.DrawText(font, Colors.Black, new Point(pageSize.Width - textSize.Width - 50, 20), text);

				// draw date
				text = string.Format("Printed on {0:f}", printTime);
				textSize = Size.Round(e.Graphics.MeasureString(font, text));
				e.Graphics.DrawText(font, Colors.Black, new Point(pageSize.Width - textSize.Width - 50, pageSize.Height - textSize.Height - 20), text);

				// draw some rectangles
				switch (e.CurrentPage)
				{
					case 0:
						e.Graphics.DrawRectangle(Pens.Blue(), new Rectangle(50, 50, 100, 100));
						e.Graphics.DrawRectangle(Pens.Green(), new Rectangle(new Point(pageSize) - new Size(150, 150), new Size(100, 100)));
						break;
					case 1:
						e.Graphics.DrawRectangle(Pens.Blue(), new Rectangle(pageSize.Width - 150, 50, 100, 100));
						e.Graphics.DrawRectangle(Pens.Green(), new Rectangle(50, pageSize.Height - 150, 100, 100));
						break;
				}
			};
			document.Name = "Name Of Document";
			document.PageCount = 2;
			return document;
		}

		Control PrintFromGraphics()
		{
			var control = new Button { Text = "Print From Graphics" };

			control.Click += delegate
			{
				var document = GetPrintDocument();
				document.Print();
			};

			return control;
		}

		Control PrintFromGraphicsWithDialog()
		{
			var control = new Button { Text = "Print From Graphics With Dialog" };

			control.Click += delegate
			{
				var document = GetPrintDocument();
				var dialog = CreatePrintDialog();
				dialog.ShowDialog(this, document);
				this.DataContext = settings = document.PrintSettings;
			};

			return control;
		}

		Control Settings()
		{
			var layout = new DynamicLayout();

			layout.AddRow(new Label { Text = "Orientation" }, TableLayout.AutoSized(PageOrientation()));

			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Copies" });
			layout.AddSeparateRow(Padding.Empty).Add(TableLayout.AutoSized(Copies()), TableLayout.AutoSized(Collate()));
			layout.EndHorizontal();

			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Maximum Pages" });
			layout.AddSeparateRow(Padding.Empty).Add(MaximumStart(), new Label { Text = "to" }, MaximumEnd());
			layout.EndHorizontal();

			layout.AddRow(null, Reverse());

			return new GroupBox { Text = "Settings", Content = layout };
		}

		Control PageRange()
		{
			var layout = new DynamicLayout();

			layout.AddRow(new Label { Text = "Print Selection" }, TableLayout.AutoSized(PrintSelection()));

			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Selected Start" });
			layout.AddSeparateRow(Padding.Empty).Add(SelectedStart(), new Label { Text = "to" }, SelectedEnd());
			layout.EndHorizontal();

			layout.Add(null);

			return new GroupBox { Text = "Page Range", Content = layout };
		}

		Control PageOrientation()
		{
			var control = new EnumComboBox<PageOrientation>();
			control.Bind(r => r.SelectedValue, (PrintSettings s) => s.Orientation);
			return control;
		}

		Control Copies()
		{
			var control = new NumericUpDown { MinValue = 1 };
			control.Bind(r => r.Value, (PrintSettings s) => s.Copies, defaultControlValue: 1);
			return control;
		}

		Control Collate()
		{
			var control = new CheckBox { Text = "Collate" };
			control.Bind(r => r.Checked, (PrintSettings s) => s.Collate);
			return control;
		}

		Control Reverse()
		{
			var control = new CheckBox { Text = "Reverse" };
			control.Bind(r => r.Checked, (PrintSettings s) => s.Reverse);
			return control;
		}

		Control MaximumStart()
		{
			var control = new NumericUpDown { MinValue = 1 };
			control.DataContextChanged += delegate
			{
				control.Value = settings.MaximumPageRange.Start;
			};
			control.ValueChanged += delegate
			{
				var range = settings.MaximumPageRange;
				var end = range.End;
				range.Start = (int)control.Value;
				range.End = Math.Max(end, range.Start);
				maximumEnd.MinValue = control.Value;
				maximumEnd.Value = range.End;
				settings.MaximumPageRange = range;
			};
			return control;
		}

		Control MaximumEnd()
		{
			var control = maximumEnd = new NumericUpDown { MinValue = 1 };
			control.DataContextChanged += delegate
			{
				control.Value = settings.MaximumPageRange.End;
			};
			control.ValueChanged += delegate
			{
				var range = settings.MaximumPageRange;
				range.End = (int)control.Value;
				settings.MaximumPageRange = range;
			};
			return control;
		}

		Control SelectedStart()
		{
			var control = new NumericUpDown { MinValue = 1 };
			control.DataContextChanged += delegate
			{
				control.Value = settings.SelectedPageRange.Start;
			};
			control.ValueChanged += delegate
			{
				var range = settings.SelectedPageRange;
				var end = range.End;
				range.Start = (int)control.Value;
				range.End = Math.Max(range.Start, end);
				selectedEnd.MinValue = control.Value;
				selectedEnd.Value = range.End;
				settings.SelectedPageRange = range;
			};
			return control;
		}

		Control SelectedEnd()
		{
			var control = selectedEnd = new NumericUpDown { MinValue = 1 };
			control.DataContextChanged += delegate
			{
				control.Value = settings.SelectedPageRange.End;
			};
			control.ValueChanged += delegate
			{
				var range = settings.SelectedPageRange;
				range.End = (int)control.Value;
				settings.SelectedPageRange = range;
			};
			return control;
		}

		Control PrintSelection()
		{
			var control = new EnumComboBox<PrintSelection>();
			control.Bind(r => r.SelectedValue, (PrintSettings s) => s.PrintSelection);
			return control;
		}
	}
}
