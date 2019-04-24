using System;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// A themed handler for the <see cref="DocumentControl"/> control.
	/// </summary>
	public class ThemedDocumentControlHandler : ThemedContainerHandler<TableLayout, DocumentControl, DocumentControl.ICallback>, DocumentControl.IHandler
    {
		List<DocumentPage> pages = new List<DocumentPage>();
		ThemedDocumentPageHandler tabPrev, tabNext;

		PointF mousePos;
		int selectedIndex;
		float nextPrevWidth;
		float startx;
		Size maxImageSize;
		PointF? draggingLocation;

		Drawable tabDrawable;
		Panel contentPanel;
		Font font;

		static Padding DefaultTabPadding = 6;

		static readonly object TabPadding_Key = new object();

		/// <summary>
		/// Gets or sets the padding inside each tab around the text.
		/// </summary>
		/// <value>The tab padding.</value>
		public Padding TabPadding
		{
			get => Widget.Properties.Get<Padding?>(TabPadding_Key) ?? DefaultTabPadding;
			set => Widget.Properties.Set(TabPadding_Key, value, DefaultTabPadding);
		}

		/// <summary>
		/// Gets or sets the font for the tab text.
		/// </summary>
		/// <value>The font for the tabs.</value>
		public Font Font
		{
			get { return font; }
			set
			{
				font = value;
				Calculate();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.ThemedControls.ThemedDocumentControlHandler"/> class.
		/// </summary>
        public ThemedDocumentControlHandler()
        {
			mousePos = new PointF(-1, -1);
			selectedIndex = -1;
			nextPrevWidth = 0;
			startx = 0;
			font = SystemFonts.Default();

			tabDrawable = new Drawable();

			contentPanel = new Panel();

			tabPrev = new ThemedDocumentPageHandler { Text = "<", Closable = false };
			tabNext = new ThemedDocumentPageHandler { Text = ">", Closable = false };

			tabDrawable.MouseMove += Drawable_MouseMove;
			tabDrawable.MouseLeave += Drawable_MouseLeave;
			tabDrawable.MouseDown += Drawable_MouseDown;
			tabDrawable.Paint += Drawable_Paint;
			tabDrawable.MouseUp += Drawable_MouseUp;

			Control = new TableLayout(tabDrawable, contentPanel);
			Control.SizeChanged += Widget_SizeChanged;
        }

		void Widget_SizeChanged(object sender, EventArgs e)
		{
			Calculate();
		}

		/// <summary>
		/// Performs calculations when loaded.
		/// </summary>
		/// <param name="e">Event arguments</param>
		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Calculate(true);
		}

		internal void Update(ThemedDocumentPageHandler handler)
		{
			Calculate();
			var index = pages.FindIndex(obj => ReferenceEquals(obj.Handler, handler));

			if (index != -1)
			{
				if (SelectedIndex == index)
					SetPageContent();
				else
					tabDrawable.Invalidate();
			}
		}

		ThemedDocumentPageHandler GetPageHandler(int index) => pages[index].Handler as ThemedDocumentPageHandler;

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
				if (selectedIndex != value)
				{
					if (value >= pages.Count || (pages.Count == 0 && value != -1))
						throw new ArgumentOutOfRangeException();

					selectedIndex = value;

					SetPageContent();

					Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to allow page reordering.
		/// </summary>
		/// <value><c>true</c> to allow reordering; otherwise, <c>false</c>.</value>
		public bool AllowReordering { get; set; }

		void SetPageContent()
		{
			contentPanel.Content = null;

			if (selectedIndex >= 0)
			{
				var tab = GetPageHandler(selectedIndex);

				contentPanel.Content = tab.Control;
				var activerec = tab.Rect;
				if (activerec.X + activerec.Width > tabDrawable.Width)
					startx += tabDrawable.Width - activerec.X - activerec.Width;
				if (activerec.X < nextPrevWidth)
					startx += nextPrevWidth - activerec.X;
			}
			else
				startx = 0;
			CalculateTabs();
			tabDrawable.Invalidate();
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
			var pageHandler = page.Handler as ThemedDocumentPageHandler;
			pageHandler.DocControl = this;

			Calculate();
			if (SelectedIndex == -1)
				SelectedIndex = 0;
			else if (SelectedIndex >= index)
				SelectedIndex++;
			else
				tabDrawable.Invalidate();
		}

		/// <summary>
		/// Removes a page.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemovePage(int index)
		{
			var tab = GetPageHandler(index);
			tab.DocControl = null;

			pages.RemoveAt(index);

			if (pages.Count == 0)
				SelectedIndex = -1;
			else if (SelectedIndex > index)
				SelectedIndex--;
			else if (SelectedIndex > pages.Count - 1)
				SelectedIndex = pages.Count - 1;
			else if (SelectedIndex == index)
				SetPageContent();

			Calculate();
		}

		void Calculate(bool force = false)
		{
			nextPrevWidth = 0f;
			CalculateImageSizes(force);
			CalculateTabHeight(force);
			CalculateTabs(force);
		}

		void CalculateTabHeight(bool force = false)
		{
			if (!force && !Widget.Loaded)
				return;
			var scale = Widget.ParentWindow?.Screen?.Scale ?? 1f;
			var fontHeight = (int)Math.Ceiling(Font.Ascent * scale);

			var height = Math.Max(maxImageSize.Height, fontHeight);
			tabDrawable.Height = height + TabPadding.Vertical; // 2 px padding at top and bottom
		}

		void CalculateImageSizes(bool force = false)
		{
			if (!force && !Widget.Loaded)
				return;
			maxImageSize = Size.Empty;
			for (int i = 0; i < pages.Count; i++)
			{
				var img = pages[i].Image;
				if (img != null)
					maxImageSize = Size.Max(maxImageSize, img.Size);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this control is enabled
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public override bool Enabled
		{
			get { return base.Enabled; }
			set
			{
				base.Enabled = value;
				tabDrawable.Invalidate();
			}
		}

		void Drawable_MouseDown(object sender, MouseEventArgs e)
		{
			mousePos = e.Location;
			if (!Enabled)
				return;

			if (tabPrev.Rect.Contains(e.Location))
			{
				if (selectedIndex > 0)
					SelectedIndex--;
			}
			else if (tabNext.Rect.Contains(e.Location))
			{
				if (selectedIndex < pages.Count - 1)
					SelectedIndex++;
			}
			else
			{
				for (int i = 0; i < pages.Count; i++)
				{
					var tab = GetPageHandler(i);
					if (tab.Rect.Contains(mousePos))
					{
						if (IsCloseSelected(tab))
						{
							var page = tab.Widget;
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

		void Drawable_MouseUp(object sender, Forms.MouseEventArgs e)
		{
			draggingLocation = null;
			CalculateTabs();
			tabDrawable.Invalidate();
		}

		void Drawable_MouseMove(object sender, MouseEventArgs e)
		{
			mousePos = e.Location;
			if (!Enabled)
				return;
			if (AllowReordering && draggingLocation == null && e.Buttons == MouseButtons.Primary)
				draggingLocation = mousePos;
			if (draggingLocation != null && selectedIndex >= 0)
			{
				var selectedPage = GetPageHandler(selectedIndex);
				var point = selectedPage.Rect.Center;

				int newIndex = -1;
				if (selectedIndex < pages.Count - 1)
				{
					var nextPage = GetPageHandler(selectedIndex + 1);
					var nextRect = nextPage.Rect;
					if (nextRect.Width > selectedPage.Rect.Width)
					{
						nextRect.Offset(nextRect.Width - selectedPage.Rect.Width, 0);
					}
					if (point.X > nextRect.X)
						newIndex = selectedIndex + 1;
				}

				if (selectedIndex > 0)
				{
					var prevPage = GetPageHandler(selectedIndex - 1);
					var prevRect = prevPage.Rect;
					if (prevRect.Width > selectedPage.Rect.Width)
					{
						prevRect.Width = selectedPage.Rect.Width;
					}
					if (point.X < prevRect.Right)
						newIndex = selectedIndex - 1;
				}

				if (newIndex >= 0 && newIndex != selectedIndex)
				{
					var newPage = GetPageHandler(newIndex);
					var loc = draggingLocation.Value;
					loc.Offset(newIndex > selectedIndex ? newPage.Rect.Width : -newPage.Rect.Width, 0);
					draggingLocation = loc;
					var temp = pages[selectedIndex];
					pages[selectedIndex] = pages[newIndex];
					pages[newIndex] = temp;

					Callback.OnPageReordered(Widget, new DocumentPageReorderEventArgs(temp, selectedIndex, newIndex));

					selectedIndex = newIndex;
				}
				CalculateTabs();
			}

			tabDrawable.Invalidate();
		}

		void Drawable_MouseLeave(object sender, MouseEventArgs e)
		{
			mousePos = new PointF(-1, -1);
			tabDrawable.Invalidate();
		}

		void CalculateTabs(bool force = false)
		{
			if (!force && !Widget.Loaded)
				return;
			var posx = 0f;
			if (nextPrevWidth == 0f)
			{
				CalculateTab(tabPrev, -1, ref posx);
				CalculateTab(tabNext, -1, ref posx);
				nextPrevWidth = tabPrev.Rect.Width + tabNext.Rect.Width;
			}

			posx = nextPrevWidth + startx;

			for (int i = 0; i < pages.Count; i++)
			{
				var tab = GetPageHandler(i);
				CalculateTab(tab, i, ref posx);
			}
		}

		void Drawable_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;

			g.Clear(SystemColors.Control);

			var posx = nextPrevWidth + startx;

			for (int i = 0; i < pages.Count; i++)
			{
				var tab = GetPageHandler(i);
				if (i != selectedIndex)
					DrawTab(g, tab, i);
			}
			if (selectedIndex >= 0)
			{
				DrawTab(g, GetPageHandler(selectedIndex), selectedIndex);
			}

			posx = 0;

			DrawTab(g, tabPrev, -1);
			DrawTab(g, tabNext, -1);
		}

		void CalculateTab(ThemedDocumentPageHandler tab, int i, ref float posx)
		{
			var tabPadding = TabPadding;
			var textSize = string.IsNullOrEmpty(tab.Text) ? Size.Empty : Size.Ceiling(Font.MeasureString(tab.Text));
			var size = textSize;
			var prevnextsel = mousePos.X > nextPrevWidth || i == -1;
			var textoffset = 0;
			if (tab.Image != null)
			{
				textoffset = tab.Image.Size.Width + tabPadding.Left;
				size.Width += textoffset;
			}

			var closesize = tabDrawable.Height / 2;
			var tabRect = new RectangleF(posx, 0, size.Width + (tab.Closable ? closesize + tabPadding.Horizontal + tabPadding.Right : tabPadding.Horizontal), tabDrawable.Height);

			if (i == selectedIndex && draggingLocation != null)
			{
				tabRect.Offset(mousePos.X - draggingLocation.Value.X, 0);
			}

			tab.Rect = tabRect;

			tab.CloseRect = new RectangleF(tabRect.X + tab.Rect.Width - tabDrawable.Height / 4 - closesize, tabDrawable.Height / 4, closesize, closesize);
			tab.TextRect = new RectangleF(tabRect.X + tabPadding.Left + textoffset, (tabDrawable.Height - size.Height) / 2, textSize.Width, textSize.Height);

			posx += tab.Rect.Width;
		}

		bool IsCloseSelected(ThemedDocumentPageHandler tab)
		{
			var prevnextsel = mousePos.X > nextPrevWidth;
			return draggingLocation == null && tab.Closable && tab.CloseRect.Contains(mousePos) && prevnextsel && Enabled;
		}

		void DrawTab(Graphics g, ThemedDocumentPageHandler tab, int i)
		{
			var prevnextsel = mousePos.X > nextPrevWidth || i == -1;
			var closeSelected = IsCloseSelected(tab);
			var tabRect = tab.Rect;
			var textRect = tab.TextRect;
			var closerect = tab.CloseRect;
			var closemargin =  closerect.Height / 3;
			var size = tabRect.Size;

			var textcolor = Enabled ? SystemColors.ControlText : SystemColors.DisabledText;
			var backcolor = SystemColors.Control;
			if (selectedIndex >= 0 && i == selectedIndex)
			{
				textcolor = Enabled ? SystemColors.HighlightText : SystemColors.DisabledText;
				backcolor = SystemColors.Highlight;
				backcolor.A *= 0.8f;
			}

			if (draggingLocation == null && tabRect.Contains(mousePos) && prevnextsel && !closeSelected && Enabled)
			{
				textcolor = SystemColors.HighlightText;
				backcolor = SystemColors.Highlight;
			}

			g.FillRectangle(backcolor, tabRect);
			if (tab.Image != null)
			{
				var imageSize = tab.Image.Size;
				g.DrawImage(tab.Image, tabRect.X + TabPadding.Left + (maxImageSize.Width - imageSize.Width) / 2, (tabDrawable.Height - imageSize.Height) / 2);
			}
			g.DrawText(Font, textcolor, textRect.Location, tab.Text);

			if (tab.Closable)
			{
				g.FillRectangle(closeSelected ? SystemColors.Highlight : SystemColors.Control, closerect);
				var closeForeground = Enabled ? closeSelected ? SystemColors.HighlightText : SystemColors.ControlText : SystemColors.DisabledText;
				g.DrawLine(closeForeground, closerect.X + closemargin, closerect.Y + closemargin, closerect.X + closerect.Width - 1 - closemargin, closerect.Y + closerect.Height - 1 - closemargin);
				g.DrawLine(closeForeground, closerect.X + closemargin, closerect.Y + closerect.Height - 1 - closemargin, closerect.X + closerect.Width - 1 - closemargin, closerect.Y + closemargin);
			}

		}

		/// <summary>
		/// Attaches the specified event.
		/// </summary>
		/// <param name="id">Event identifier</param>
		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case DocumentControl.PageReorderedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
