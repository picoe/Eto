using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;

namespace Eto.Test
{
	public interface ISection
	{
		string Text { get; }

		Control CreateContent();
	}

	public class Section : List<Section>, ITreeGridItem<Section>
	{
		public string Text { get; set; }

		public bool Expanded { get; set; }

		public bool Expandable { get { return Count > 0; } }

		public ITreeGridItem Parent { get; set; }

		public new ITreeGridItem this [int index]
		{
			get { return null; }
		}

		public Section()
		{
		}

		public Section(string text, IEnumerable<Section> sections)
			: base (sections.OrderBy (r => r.Text, StringComparer.CurrentCultureIgnoreCase).ToArray())
		{
			this.Text = text;
			this.Expanded = true;
			foreach (var r in this) r.Parent = this;
		}
	}

	public abstract class SectionBase : Section, ISection
	{
		public abstract Control CreateContent();
	}

	public class Section<T> : SectionBase
		where T: Control, new()
	{
		public Func<T> Creator { get; set; }

		public override Control CreateContent()
		{
			return Creator != null ? Creator() : new T();
		}
	}

	/// <summary>
	/// Tests for dialogs and forms use this.
	/// </summary>
	public class WindowSectionMethod : Section, ISection
	{
		Func<Window> Func { get; set; }

		public WindowSectionMethod(string text = null)
		{
			Text = text;
		}

		public WindowSectionMethod(string text, Func<Window> f)
		{
			Func = f;
			Text = text;
		}

		protected virtual Window GetWindow()
		{
			return null;
		}

		public Control CreateContent()
		{
			var button = new Button { Text = string.Format("Show the {0} test", Text) };
			var layout = new DynamicLayout();
			layout.AddCentered(button);
			button.Click += (sender, e) => {

				try
				{
					var window = Func != null ? Func() : null ?? GetWindow();

					if (window != null)
					{
						var dialog = window as Dialog;
						if (dialog != null)
						{
							dialog.ShowDialog(null);
							return;
						}
						var form = window as Form;
						if (form != null)
						{
							form.Show();
							return;
						}
					}
				}
				catch (Exception ex)
				{
					Log.Write(this, "Error loading section: {0}", ex.GetBaseException());
				}
			};
			return layout;
		}
	}

	public class SectionList : TreeGridView
	{
		public new ISection SelectedItem
		{
			get { return base.SelectedItem as ISection; }
		}


		public SectionList(IEnumerable<Section> topNodes)
		{
			this.Style = "sectionList";
			this.ShowHeader = false;

			Columns.Add(new GridColumn { DataCell = new TextBoxCell { Binding = new PropertyBinding ("Text") } });

			this.DataStore = new Section("Top", topNodes);
		}

		public string SectionTitle
		{
			get
			{
				var section = SelectedItem as Section;
				return section != null ? section.Text : null;
			}
		}
	}

	/// <summary>
	/// Allows a test case to use a different generator for drawing
	/// graphics than for windowing.
	/// </summary>
	public class DrawingToolkit
	{
		public virtual void Initialize(Drawable drawable)
		{
		}

		public virtual void Render(Graphics graphics, Action<Graphics> render)
		{
			render(graphics);
		}

		public virtual DrawingToolkit Clone()
		{
			return new DrawingToolkit();
		}

		public virtual GeneratorContext GetGeneratorContext()
		{
			return new GeneratorContext(Generator.Current); // don't change the context
		}
	}

	public class D2DToolkit : DrawingToolkit
	{
		Graphics graphics;
		Generator d2d;

		public D2DToolkit()
		{
			this.d2d = Generator.GetGenerator(Generators.Direct2DAssembly);
		}

		public override void Initialize(Drawable drawable)
		{
			base.Initialize(drawable);
			this.graphics = new Graphics(drawable, d2d);
		}

		public override void Render(Graphics g, Action<Graphics> render)
		{
			this.graphics.BeginDrawing();
			//this.graphics.Clear(Brushes.Black() as SolidBrush); // DirectDrawingSection's Drawable seems to automatically clear the background, but that doesn't happen in Direct2d, so we clear it explicitly.
			try
			{
				using (var context = GetGeneratorContext())
					render(this.graphics);
			}
			catch (Exception) { }
			this.graphics.EndDrawing();
		}

		public override GeneratorContext GetGeneratorContext()
		{
			return new GeneratorContext(d2d);
		}

		public override DrawingToolkit Clone()
		{
			return new D2DToolkit();
		}
	}
}

