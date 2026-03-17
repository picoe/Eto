namespace Eto.GirCore.Forms.Controls
{
	public class ScrollableHandler : GirPanel<Gtk.ScrolledWindow, Scrollable, Scrollable.ICallback>, Scrollable.IHandler
	{
		static readonly object BorderKey = new object();

		readonly Gtk.Box contentBox;
		Gtk.Widget? layoutWidget;
		bool expandWidth = true;
		bool expandHeight = true;

		public ScrollableHandler()
		{
			Control = Gtk.ScrolledWindow.New();
			Control.SetPolicy(Gtk.PolicyType.Automatic, Gtk.PolicyType.Automatic);

			contentBox = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
			Control.SetChild(contentBox);
		}

		protected override void Initialize()
		{
			base.Initialize();
			Border = BorderType.Bezel;
		}

		protected override void SetContainerContent(Gtk.Widget content)
		{
			layoutWidget = content;
			contentBox.Append(content);
			ApplyExpandSettings();
		}

		protected override void RemoveContainerContent(Gtk.Widget content)
		{
			contentBox.Remove(content);
			if (ReferenceEquals(layoutWidget, content))
				layoutWidget = null;
		}

		void ApplyExpandSettings()
		{
			if (layoutWidget == null)
				return;

			layoutWidget.Hexpand = expandWidth;
			layoutWidget.Vexpand = expandHeight;
			layoutWidget.Halign = expandWidth ? Gtk.Align.Fill : Gtk.Align.Start;
			layoutWidget.Valign = expandHeight ? Gtk.Align.Fill : Gtk.Align.Start;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Scrollable.ScrollEvent:
					Control.Hadjustment.OnValueChanged += HandleScrollChanged;
					Control.Vadjustment.OnValueChanged += HandleScrollChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void HandleScrollChanged(GObject.Object sender, EventArgs e)
		{
			Callback.OnScroll(Widget, new ScrollEventArgs(ScrollPosition));
		}

		public void UpdateScrollSizes()
		{
			Control.QueueResize();
			layoutWidget?.QueueResize();
		}

		public Point ScrollPosition
		{
			get => new Point((int)Control.Hadjustment.Value, (int)Control.Vadjustment.Value);
			set
			{
				var clientSize = ClientSize;
				var scrollSize = ScrollSize;
				Control.Hadjustment.Value = Math.Min(value.X, Math.Max(0, scrollSize.Width - clientSize.Width));
				Control.Vadjustment.Value = Math.Min(value.Y, Math.Max(0, scrollSize.Height - clientSize.Height));
			}
		}

		public Size ScrollSize
		{
			get => new Size((int)Control.Hadjustment.Upper, (int)Control.Vadjustment.Upper);
			set
			{
				Control.Hadjustment.Upper = value.Width;
				Control.Vadjustment.Upper = value.Height;
			}
		}

		public Rectangle VisibleRect => new Rectangle(ScrollPosition, Size.Min(ScrollSize, ClientSize));

		public BorderType Border
		{
			get => Widget.Properties.Get(BorderKey, BorderType.Bezel);
			set
			{
				if (Widget.Properties.TrySet(BorderKey, value, BorderType.Bezel))
					Control.HasFrame = value != BorderType.None;
			}
		}

		public bool ExpandContentWidth
		{
			get => expandWidth;
			set
			{
				if (expandWidth == value)
					return;
				expandWidth = value;
				ApplyExpandSettings();
			}
		}

		public bool ExpandContentHeight
		{
			get => expandHeight;
			set
			{
				if (expandHeight == value)
					return;
				expandHeight = value;
				ApplyExpandSettings();
			}
		}

		public float MinimumZoom { get; set; } = 1f;
		public float MaximumZoom { get; set; } = 1f;
		public float Zoom { get; set; } = 1f;
	}
}
