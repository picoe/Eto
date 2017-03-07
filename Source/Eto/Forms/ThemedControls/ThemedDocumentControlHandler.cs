using System;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// A themed handler for the <see cref="DocumentControl"/> control.
	/// </summary>
    public class ThemedDocumentControlHandler : ThemedContainerHandler<DynamicLayout, DocumentControl, DocumentControl.ICallback>, DocumentControl.IHandler
    {
		List<ThemedDocumentPageHandler> tabs;
		List<Control> cons = new List<Control>(); // Need to prevent disposing
		List<DocumentPage> pages = new List<DocumentPage>();
		ThemedDocumentPageHandler tabPrev, tabNext;

		PointF mousePos;
		int selectedIndex;
		float extraspacing;
		float startx;

		Drawable drawable;
		Panel panel;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.ThemedControls.ThemedDocumentControlHandler"/> class.
		/// </summary>
        public ThemedDocumentControlHandler()
        {
			mousePos = new PointF(-1, -1);
			selectedIndex = -1;
			extraspacing = 0;
			startx = 0;

			Control = new DynamicLayout();
			Control.BeginVertical();

			drawable = new Drawable();
			drawable.Height = (int)SystemFonts.Default().MeasureString("H").Height + 16;
			Control.Add(drawable, true, false);

			panel = new Panel();
			Control.Add(panel, true, true);

			tabPrev = new ThemedDocumentPageHandler { Text = "<", Closable = false };
			tabNext = new ThemedDocumentPageHandler { Text = ">", Closable = false };

            tabs = new List<ThemedDocumentPageHandler>();

			drawable.MouseMove += Drawable_MouseMove;
			drawable.MouseLeave += Drawable_MouseLeave;
			drawable.MouseDown += Drawable_MouseDown;
			drawable.Paint += Drawable_Paint;
        }

		internal void Update(ThemedDocumentPageHandler handler)
		{
			var index = tabs.FindIndex((obj) => obj == handler);

			if (index != -1)
			{
				if (SelectedIndex == index)
					SelectedIndex = index;
				else
					drawable.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the index of the selected.
		/// </summary>
		/// <value>The index of the selected.</value>
		public int SelectedIndex
		{
			get
			{
				return selectedIndex;
			}
			set
			{
				if (panel.Content != null)
					panel.Remove(panel.Content);
				
				if (value >= 0)
					panel.Content = cons[value];

				selectedIndex = value;

				var activerec = tabs[selectedIndex].Rect;
				if (activerec.X + activerec.Width > drawable.Width)
					startx += drawable.Width - activerec.X - activerec.Width;
				if (activerec.X < extraspacing)
					startx += extraspacing - activerec.X;
				
				drawable.Invalidate();
				Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets the page.
		/// </summary>
		/// <returns>The page.</returns>
		/// <param name="index">Index.</param>
		public DocumentPage GetPage(int index)
		{
			return pages[index];
		}

		/// <summary>
		/// Gets the page count.
		/// </summary>
		/// <returns>The page count.</returns>
		public int GetPageCount()
		{
			return pages.Count;
		}

		/// <summary>
		/// Inserts the page.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="page">Page.</param>
		public void InsertPage(int index, DocumentPage page)
		{
			pages.Insert(index, page);
			cons.Insert(index, page.Content);
			var pagehan = page.Handler as ThemedDocumentPageHandler;
			pagehan.DocControl = this;
			tabs.Insert(index, pagehan);

			if (SelectedIndex == -1)
				SelectedIndex = 0;

			drawable.Invalidate();
		}

		/// <summary>
		/// Removes a page.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemovePage(int index)
		{
			tabs[index].DocControl = null;

			pages.RemoveAt(index);
			cons.RemoveAt(index);
			tabs.RemoveAt(index);

			if (pages.Count == 0)
				SelectedIndex = -1;
			else if (SelectedIndex > index)
				SelectedIndex--;
			else if (SelectedIndex == index)
				SelectedIndex = SelectedIndex;
			
			drawable.Invalidate();
		}

		private void Drawable_MouseDown(object sender, MouseEventArgs e)
		{
			mousePos = e.Location;

			if (tabPrev.Rect.Contains(e.Location))
			{
				if (selectedIndex > 0)
					SelectedIndex--;
			}
			else if (tabNext.Rect.Contains(e.Location))
			{
				if (selectedIndex < tabs.Count - 1)
					SelectedIndex++;
			}
			else
			{
				for (int i = 0; i < tabs.Count; i++)
				{
					if (tabs[i].Rect.Contains(mousePos))
					{
						if (tabs[i].CloseSelected)
						{
							var page = pages[i];
							RemovePage(i);
							Callback.OnPageClosed(Widget, new DocumentPageEventArgs(page));
						}
						else
							SelectedIndex = i;
						
						break;
					}
				}
			}
		}

		private void Drawable_MouseMove(object sender, MouseEventArgs e)
		{
			mousePos = e.Location;

			// TODO: Implement rearanging of tabs

			drawable.Invalidate();
		}

		private void Drawable_MouseLeave(object sender, MouseEventArgs e)
		{
			mousePos = new PointF(-1, -1);
			drawable.Invalidate();
		}

		private void Drawable_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			var posx = 0f;

			g.Clear(SystemColors.ControlBackground);

			if (extraspacing == 0f)
			{
				DrawTab(g, tabPrev, -1, ref posx);
				DrawTab(g, tabNext, -1, ref posx);
				extraspacing = tabPrev.Rect.Width + tabNext.Rect.Width;
			}

			posx = extraspacing + startx;

			for (int i = 0; i < tabs.Count; i++)
			{
				var tab = tabs[i];
				DrawTab(g, tab, i, ref posx);
			}

			posx = 0;

			DrawTab(g, tabPrev, -1, ref posx);
			DrawTab(g, tabNext, -1, ref posx);
			extraspacing = tabPrev.Rect.Width + tabNext.Rect.Width;
		}

		private void DrawTab(Graphics g, ThemedDocumentPageHandler tab, int i, ref float posx)
		{
			var size = SystemFonts.Default().MeasureString(tab.Text);
			var prevnextsel = mousePos.X > extraspacing || i == -1;
			tab.Rect = new RectangleF(posx, 0, size.Width + (tab.Closable ? drawable.Height + 10 : 20), drawable.Height);

			var textcolor = SystemColors.ControlText;
			var backcolor = SystemColors.ControlBackground;

			var closesize = drawable.Height / 2;
			var closemargin = closesize / 3;
			var closerect = new RectangleF(posx + tab.Rect.Width - drawable.Height / 4 - closesize, drawable.Height / 4, closesize, closesize);
			tab.CloseSelected = tab.Closable && closerect.Contains(mousePos) && prevnextsel;

			if (i == selectedIndex)
				backcolor = drawable.BackgroundColor;

			if (tab.Rect.Contains(mousePos) && prevnextsel && !tab.CloseSelected)
			{
				textcolor = SystemColors.HighlightText;
				backcolor = SystemColors.Highlight;
			}

			g.FillRectangle(backcolor, tab.Rect);
			g.DrawText(SystemFonts.Default(), textcolor, posx + 10, (drawable.Height - size.Height) / 2, tab.Text);

			if (tab.Closable)
			{
				g.FillRectangle(tab.CloseSelected ? SystemColors.Highlight : SystemColors.ControlBackground, closerect);
				g.DrawLine(tab.CloseSelected ? SystemColors.HighlightText : SystemColors.ControlText, closerect.X + closemargin, closerect.Y + closemargin, closerect.X + closesize - 1 - closemargin, closerect.Y + closesize - 1 - closemargin);
				g.DrawLine(tab.CloseSelected ? SystemColors.HighlightText : SystemColors.ControlText, closerect.X + closemargin, closerect.Y + closesize - 1 - closemargin, closerect.X + closesize - 1 - closemargin, closerect.Y + closemargin);
			}

			posx += tab.Rect.Width;
		}
	}
}
