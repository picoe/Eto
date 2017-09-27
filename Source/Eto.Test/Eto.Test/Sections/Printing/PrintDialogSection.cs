using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Printing
{
	[Section("Printing", "Print Dialog")]
	public class PrintDialogSection : Panel
	{
		PrintSettings settings = new PrintSettings();
		NumericStepper selectedEnd;
		NumericStepper maximumEnd;
		CheckBox allowPageRange;
		CheckBox allowSelection;

		public PrintDialogSection()
		{
			this.DataContext = settings;

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.BeginVertical();
			layout.BeginHorizontal();
			layout.Add(null);
			layout.BeginVertical(Padding.Empty);
			layout.AddSeparateRow(null, ShowPrintDialog(), null);
			layout.AddSeparateRow(null, PrintFromGraphicsWithDialog(), null);
			layout.AddSeparateRow(null, PrintFromGraphics(), null);
			layout.Add(null);
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
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

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
				var ret = print.ShowDialog(ParentWindow);
				if (ret == DialogResult.Ok)
				{
					DataContext = settings = print.PrintSettings;
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
			document.PrintPage += (sender, e) =>
			{
				Size pageSize = Size.Round(e.PageSize);

				// draw a border around the printable area
				var rect = new Rectangle(pageSize);
				rect.Inflate(-1, -1);
				e.Graphics.DrawRectangle(Pens.Silver, rect);

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
						e.Graphics.DrawRectangle(Pens.Blue, new Rectangle(50, 50, 100, 100));
						e.Graphics.DrawRectangle(Pens.Green, new Rectangle(new Point(pageSize) - new Size(150, 150), new Size(100, 100)));
						break;
					case 1:
						e.Graphics.DrawRectangle(Pens.Blue, new Rectangle(pageSize.Width - 150, 50, 100, 100));
						e.Graphics.DrawRectangle(Pens.Green, new Rectangle(50, pageSize.Height - 150, 100, 100));
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
				DataContext = settings = document.PrintSettings;
			};

			return control;
		}

		Control Settings()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow(new Label { Text = "Orientation" }, TableLayout.AutoSized(PageOrientation()));

			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Copies" });
			layout.AddSeparateRow().Add(TableLayout.AutoSized(Copies()), TableLayout.AutoSized(Collate()));
			layout.EndHorizontal();

			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Maximum Pages" });
			layout.AddSeparateRow().Add(MaximumStart(), new Label { Text = "to" }, MaximumEnd());
			layout.EndHorizontal();

			layout.AddRow(null, Reverse());

			return new GroupBox { Text = "Settings", Content = layout };
		}

		Control PageRange()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow(new Label { Text = "Print Selection" }, TableLayout.AutoSized(PrintSelection()));

			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Selected Start" });
			layout.AddSeparateRow().Add(SelectedStart(), new Label { Text = "to" }, SelectedEnd());
			layout.EndHorizontal();

			layout.Add(null);

			return new GroupBox { Text = "Page Range", Content = layout };
		}

		static Control PageOrientation()
		{
			var control = new EnumDropDown<PageOrientation>();
			control.SelectedValueBinding.BindDataContext<PrintSettings>(r => r.Orientation, (r, v) => r.Orientation = v);
			return control;
		}

		static Control Copies()
		{
			var control = new NumericStepper { MinValue = 1 };
			control.ValueBinding.BindDataContext<PrintSettings>(r => r.Copies, (r, v) => r.Copies = (int)v, defaultGetValue: 1);
			return control;
		}

		static Control Collate()
		{
			var control = new CheckBox { Text = "Collate" };
			control.CheckedBinding.BindDataContext<PrintSettings>(r => r.Collate, (r, v) => r.Collate = v ?? false);
			return control;
		}

		static Control Reverse()
		{
			var control = new CheckBox { Text = "Reverse" };
			control.CheckedBinding.BindDataContext<PrintSettings>(r => r.Reverse, (r, v) => r.Reverse = v ?? false);
			return control;
		}

		Control MaximumStart()
		{
			var control = new NumericStepper { MinValue = 1 };
			control.DataContextChanged += delegate
			{
				control.Value = settings.MaximumPageRange.Start;
			};
			control.ValueChanged += delegate
			{
				var range = settings.MaximumPageRange;
				var end = range.End;
				range = new Range<int>((int)control.Value, Math.Max(end, range.Start));
				maximumEnd.MinValue = control.Value;
				maximumEnd.Value = range.End;
				settings.MaximumPageRange = range;
			};
			return control;
		}

		Control MaximumEnd()
		{
			var control = maximumEnd = new NumericStepper { MinValue = 1 };
			control.DataContextChanged += delegate
			{
				control.Value = settings.MaximumPageRange.End;
			};
			control.ValueChanged += delegate
			{
				var range = settings.MaximumPageRange;
				settings.MaximumPageRange = new Range<int>(range.Start, (int)control.Value);
			};
			return control;
		}

		Control SelectedStart()
		{
			var control = new NumericStepper { MinValue = 1 };
			control.DataContextChanged += delegate
			{
				control.Value = settings.SelectedPageRange.Start;
			};
			control.ValueChanged += delegate
			{
				var range = settings.SelectedPageRange;
				var end = range.End;
				range = new Range<int>((int)control.Value, Math.Max(range.Start, end));
				selectedEnd.MinValue = control.Value;
				selectedEnd.Value = range.End;
				settings.SelectedPageRange = range;
			};
			return control;
		}

		Control SelectedEnd()
		{
			var control = selectedEnd = new NumericStepper { MinValue = 1 };
			control.DataContextChanged += delegate
			{
				control.Value = settings.SelectedPageRange.End;
			};
			control.ValueChanged += delegate
			{
				var range = settings.SelectedPageRange;
				settings.SelectedPageRange = new Range<int>(range.Start, (int)control.Value);
			};
			return control;
		}

		static Control PrintSelection()
		{
			var control = new EnumDropDown<PrintSelection>();
			control.SelectedValueBinding.BindDataContext<PrintSettings>(r => r.PrintSelection, (r, v) => r.PrintSelection = v);
			return control;
		}
	}
}
