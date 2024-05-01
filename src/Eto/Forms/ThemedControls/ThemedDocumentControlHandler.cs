namespace Eto.Forms.ThemedControls;

/// <summary>
/// A themed handler for the <see cref="DocumentControl"/> control.
/// </summary>
public class ThemedDocumentControlHandler : ThemedContainerHandler<TableLayout, DocumentControl, DocumentControl.ICallback>, DocumentControl.IHandler
{
	List<DocumentPage> pages = new List<DocumentPage>();
	ThemedDocumentPageHandler tabPrev, tabNext;

	bool allowNavigationButtons;
	PointF mousePos;
	int selectedIndex;
	float nextPrevWidth;
	float startx;
	Size maxImageSize;
	PointF? draggingLocation;
	bool useFixedTabHeight;

	Drawable tabDrawable;
	Panel contentPanel;
	Font font;

	Color backgroundColor;
	Color disabledForegroundColor;
	Color closeBackgroundColor;
	Color closeHighlightBackgroundColor;
	Color closeForegroundColor;
	Color closeHighlightForegroundColor;
	Color tabBackgroundColor;
	Color tabHighlightBackgroundColor;
	Color tabHoverBackgroundColor;
	Color tabForegroundColor;
	Color tabHighlightForegroundColor;
	Color tabHoverForegroundColor;
	Color unsavedBackgroundColor;

	int closeCornerRadius;

	static Padding DefaultTabPadding = 6;

	static readonly object TabPadding_Key = new object();
	int minImageSquareSide = 16;

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
	/// Gets or sets a value indicating the tabs can be navigated by next and previous buttons.
	/// </summary>
	/// <value><c>true</c> if can be navigated by next and previous buttons; otherwise, <c>false</c>.</value>
	public bool AllowNavigationButtons
	{
		get { return allowNavigationButtons; }
		set
		{
			allowNavigationButtons = value;
			Calculate();
			tabDrawable.Invalidate();
		}
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

	/// <inheritdoc />
	public override Color BackgroundColor
	{
		get => backgroundColor;
		set
		{
			backgroundColor = value;
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the disabled foreground color.
	/// </summary>
	/// <value>The disabled foreground color.</value>
	public Color DisabledForegroundColor
	{
		get { return disabledForegroundColor; }
		set
		{
			disabledForegroundColor = value;
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the background color for the close button.
	/// </summary>
	/// <value>The background color for the close button.</value>
	public Color CloseBackgroundColor
	{
		get { return closeBackgroundColor; }
		set
		{
			closeBackgroundColor = value;
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the highlight background color for the close button.
	/// </summary>
	/// <value>The highlight background color for the close button.</value>
	public Color CloseHighlightBackgroundColor
	{
		get { return closeHighlightBackgroundColor; }
		set
		{
			closeHighlightBackgroundColor = value;
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the corner radius of the close button.
	/// </summary>
	/// <value>The corner radius of the close button.</value>
	public int CloseCornerRadius
	{
		get { return closeCornerRadius; }
		set
		{
			closeCornerRadius = value;
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the foreground color for the close button.
	/// </summary>
	/// <value>The foreground color for the close button.</value>
	public Color CloseForegroundColor
	{
		get { return closeForegroundColor; }
		set
		{
			closeForegroundColor = value;
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the highlight foreground color for the close button.
	/// </summary>
	/// <value>The highlight foreground color for the close button.</value>
	public Color CloseHighlightForegroundColor
	{
		get { return closeHighlightForegroundColor; }
		set
		{
			closeHighlightForegroundColor = value;
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the background color for the tab.
	/// </summary>
	/// <value>The background color for the tab.</value>
	public Color TabBackgroundColor
	{
		get { return tabBackgroundColor; }
		set
		{
			tabBackgroundColor = value;
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the highlight background color for the highlighted or selected tab.
	/// </summary>
	/// <value>The highlight background color for the highlighted or selected tab.</value>
	public Color TabHighlightBackgroundColor
	{
		get { return tabHighlightBackgroundColor; }
		set
		{
			tabHighlightBackgroundColor = value;
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the background color for the tab under mouse.
	/// </summary>
	/// <value>The background color for the tab under mouse.</value>
	public Color TabHoverBackgroundColor
	{
		get { return tabHoverBackgroundColor; }
		set
		{
			tabHoverBackgroundColor = value;
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the foreground color for the tab.
	/// </summary>
	/// <value>The foreground color for the tab.</value>
	public Color TabForegroundColor
	{
		get { return tabForegroundColor; }
		set
		{
			tabForegroundColor = value;
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the highlight foreground color for the highlighted or selected tab.
	/// </summary>
	/// <value>The foreground color for the highlighted or selected tab.</value>
	public Color TabHighlightForegroundColor
	{
		get { return tabHighlightForegroundColor; }
		set
		{
			tabHighlightForegroundColor = value;
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the foreground color for the tab under mouse.
	/// </summary>
	/// <value>The foreground color for the tab under mouse.</value>
	public Color TabHoverForegroundColor
	{
		get { return tabHoverForegroundColor; }
		set
		{
			tabHoverForegroundColor = value;
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the background color for the unsaved changes indicator.
	/// </summary>
	/// <value>The background color for the unsaved changes indicator.</value>
	public Color UnsavedBackgroundColor
	{
		get { return unsavedBackgroundColor; }
		set
		{
			unsavedBackgroundColor = value;
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether to use a fixed tab height.
	/// </summary>
	/// <value><c>true</c> to use a fixed tab height.</value>
	public bool UseFixedTabHeight
	{
		get { return useFixedTabHeight; }
		set
		{
			useFixedTabHeight = value;
			Calculate();
			tabDrawable.Invalidate();
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Eto.Forms.ThemedControls.ThemedDocumentControlHandler"/> class.
	/// </summary>
	public ThemedDocumentControlHandler()
	{
		mousePos = new PointF(-1, -1);
		selectedIndex = -1;
		allowNavigationButtons = true;
		useFixedTabHeight = false;
		nextPrevWidth = 0;
		startx = 0;
		font = SystemFonts.Default();
		backgroundColor = SystemColors.Control;
		disabledForegroundColor = SystemColors.DisabledText;
		closeBackgroundColor = SystemColors.Control;
		closeHighlightBackgroundColor = SystemColors.Highlight;
		closeForegroundColor = SystemColors.ControlText;
		closeHighlightForegroundColor = SystemColors.HighlightText;
		tabBackgroundColor = SystemColors.Control;
		tabHighlightBackgroundColor = SystemColors.Highlight;
		tabHoverBackgroundColor = new Color(SystemColors.Highlight, 0.8f);
		tabForegroundColor = SystemColors.ControlText;
		tabHighlightForegroundColor = SystemColors.HighlightText;
		tabHoverForegroundColor = SystemColors.HighlightText;
		unsavedBackgroundColor = SystemColors.ControlText;

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

		var tabContentHeight = useFixedTabHeight ? minImageSquareSide : Math.Max(maxImageSize.Height, CalculateFontHeight());
		tabDrawable.Height = tabContentHeight + TabPadding.Vertical; // 2 px padding at top and bottom
	}

	private int CalculateFontHeight()
	{
		var scale = Widget.ParentWindow?.Screen?.Scale ?? 1f;
		return (int)Math.Ceiling(Font.Ascent * scale);
	}

	void CalculateImageSizes(bool force = false)
	{
		if (!force && !Widget.Loaded)
			return;

		maxImageSize = new Size(minImageSquareSide, minImageSquareSide);

		if (UseFixedTabHeight)
			return;

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
						var args = new DocumentPageClosingEventArgs(page);
						Callback.OnPageClosing(Widget, args);

						if (args.Cancel)
							break;

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
		if (allowNavigationButtons && nextPrevWidth == 0f)
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

		g.Clear(BackgroundColor);

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

		if (allowNavigationButtons)
		{
			DrawTab(g, tabPrev, -1);
			DrawTab(g, tabNext, -1);
		}
	}

	void CalculateTab(ThemedDocumentPageHandler tab, int i, ref float posX)
	{
		var tabLocation = new PointF(posX, 0);
		if (i == selectedIndex && draggingLocation != null)
		{
			tabLocation.Offset(mousePos.X - draggingLocation.Value.X, 0);
		}

		var imageLocation = new PointF(tabLocation.X + TabPadding.Left, (tabDrawable.Height - maxImageSize.Height) / 2f);
		var imageSize = tab.Image is null ? Size.Empty : maxImageSize;
		tab.ImageRect = new RectangleF(imageLocation, imageSize);

		var textSize = string.IsNullOrEmpty(tab.Text) ? Size.Empty : Size.Ceiling(Font.MeasureString(tab.Text));
		var textLocation = new PointF(tab.ImageRect.Right + (imageSize.IsEmpty ? 0 : TabPadding.Left), (tabDrawable.Height - textSize.Height) / 2);
		tab.TextRect = new RectangleF(textLocation, textSize);

		var unsavedSize = tab.HasUnsavedChanges ? new Size(6, 6) : Size.Empty;
		var unsavedLocation = new PointF(tab.TextRect.Right + (tab.HasUnsavedChanges ? TabPadding.Right : 0), (tabDrawable.Height - unsavedSize.Height) / 2);
		tab.UnsavedRect = new RectangleF(unsavedLocation, unsavedSize);

		var closeSize = tab.Closable
			? (int)Math.Floor(tabDrawable.Height * 0.6)
			: 0;
		var closeLocation = new PointF(tab.UnsavedRect.Right + (tab.Closable ? TabPadding.Right : 0), (tabDrawable.Height - closeSize) / 2);
		tab.CloseRect = new RectangleF(closeLocation, new SizeF(closeSize, closeSize));

		tab.Rect = new RectangleF(tabLocation, new SizeF(tab.CloseRect.Right + TabPadding.Right - tabLocation.X, tabDrawable.Height));

		posX += tab.Rect.Width;
	}

	bool IsCloseSelected(ThemedDocumentPageHandler tab)
	{
		var prevnextsel = mousePos.X > nextPrevWidth;
		return draggingLocation == null && tab.Closable && tab.CloseRect.Contains(mousePos) && prevnextsel && Enabled;
	}

	void DrawTab(Graphics g, ThemedDocumentPageHandler tab, int i)
	{
		var prevnextsel = mousePos.X > nextPrevWidth || i == -1;

		var textcolor = Enabled ? TabForegroundColor : DisabledForegroundColor;
		var backcolor = TabBackgroundColor;
		if (selectedIndex >= 0 && i == selectedIndex)
		{
			textcolor = Enabled ? TabHighlightForegroundColor : DisabledForegroundColor;
			backcolor = TabHighlightBackgroundColor;
		}
		else if (draggingLocation == null && tab.Rect.Contains(mousePos) && prevnextsel && Enabled)
		{
			textcolor = TabHoverForegroundColor;
			backcolor = TabHoverBackgroundColor;
		}

		g.FillRectangle(backcolor, tab.Rect);
		g.DrawText(Font, textcolor, tab.TextRect.Location, tab.Text);

		if (tab.Image is not null)
		{
			g.SaveTransform();
			g.ImageInterpolation = ImageInterpolation.High;
			g.DrawImage(tab.Image, tab.ImageRect);
			g.RestoreTransform();
		}

		if (tab.HasUnsavedChanges)
		{
			g.FillEllipse(UnsavedBackgroundColor, tab.UnsavedRect);
		}

		if (tab.Closable)
		{
			var closeSelected = IsCloseSelected(tab);

			var closeBackground = closeSelected ? CloseHighlightBackgroundColor : CloseBackgroundColor;
			if (closeCornerRadius > 0)
				g.FillPath(closeBackground, GraphicsPath.GetRoundRect(tab.CloseRect, closeCornerRadius));
			else
				g.FillRectangle(closeBackground, tab.CloseRect);

			var closeMargin = (int)tab.CloseRect.Height / 4;
			var closeForegroundRect = RectangleF.Inset(tab.CloseRect, new PaddingF(closeMargin));
			var closeForeground = Enabled ? closeSelected ? CloseHighlightForegroundColor : CloseForegroundColor : DisabledForegroundColor;

			g.SaveTransform();
			g.PixelOffsetMode = PixelOffsetMode.Half;
			g.DrawLine(closeForeground, closeForegroundRect.TopLeft, closeForegroundRect.BottomRight);
			g.DrawLine(closeForeground, closeForegroundRect.TopRight, closeForegroundRect.BottomLeft);
			g.RestoreTransform();
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