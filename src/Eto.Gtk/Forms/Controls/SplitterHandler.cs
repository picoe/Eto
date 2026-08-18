namespace Eto.GtkSharp.Forms.Controls
{
	public class SplitterHandler : GtkContainer<Gtk.Paned, Splitter, Splitter.ICallback>, Splitter.IHandler
	{
		readonly Gtk.EventBox container;
		Control panel1;
		Control panel2;
		SplitterFixedPanel fixedPanel;
		int? position;
		double relative = double.NaN;
		int suppressSplitterMoved;
		bool initialPositionSet;
		int _panel1MinimumSize, _panel2MinimumSize;

		int GetPreferredPanelSize(int width1, int width2)
		{
			if (position.HasValue)
				width1 = position.Value;
			else
			{
				if (!double.IsNaN(relative))
				{
					if (fixedPanel == SplitterFixedPanel.Panel1)
						width1 = (int)Math.Round(relative);
					else if (fixedPanel == SplitterFixedPanel.Panel2)
						width2 = (int)Math.Round(relative);
					else if (relative <= 0.0)
						width1 = 0;
					else if (relative >= 1.0)
						width2 = 0;
					else
					{
						// both get at least the preferred size
						return (int)Math.Round(Math.Max(width1 / relative, width2 / (1 - relative))) + SplitterWidth;
					}
				}
			}
			return width1 + width2 + SplitterWidth;
		}
		
		class EtoPaned : Gtk.Paned
		{
			WeakReference handler;

			public EtoPaned(Gtk.Orientation orientation) : base(orientation)
			{
			}

			public SplitterHandler Handler
			{
				get => handler?.Target as SplitterHandler;
				set => handler = new WeakReference(value);
			}

			int? realHandleSize;
			bool measuringHandleSize;

			/// <summary>
			/// Gets the size GTK actually allocates for the handle.
			/// </summary>
			/// <remarks>
			/// This is the size of the handle's "separator" CSS node, which the legacy handle-size style
			/// property that <see cref="Gtk.Paned.HandleSize"/> reports does not reflect - it is 5 regardless
			/// of the theme or <see cref="Gtk.Paned.WideHandle"/> (which is typically 1 and 5 respectively).
			/// GTK's own preferred size does include it though, so derive it from there.
			/// </remarks>
			public int RealHandleSize
			{
				get
				{
					// not measured yet, ask GTK for our preferred size to calculate it
					if (realHandleSize == null && !measuringHandleSize)
					{
						measuringHandleSize = true;
						try
						{
							if (Orientation == Gtk.Orientation.Horizontal)
								GetPreferredWidth(out _, out _);
							else
								GetPreferredHeight(out _, out _);
						}
						finally
						{
							measuringHandleSize = false;
						}
					}
					return realHandleSize ?? HandleSize;
				}
			}

			void UpdateHandleSize(int naturalSize, int size1, int size2)
			{
				// GTK only accounts for the handle when both children are visible
				if (Child1?.Visible == true && Child2?.Visible == true)
					realHandleSize = Math.Max(0, naturalSize - size1 - size2);
			}

			public void InvalidateHandleSize() => realHandleSize = null;

			protected override void OnStyleUpdated()
			{
				base.OnStyleUpdated();
				InvalidateHandleSize();
			}

			// The splitter only divides the available space along its own orientation; in the other
			// direction both panels get the full size, so it is the largest of the two (which is what
			// the base implementation reports).

			protected override void OnGetPreferredWidthForHeight(int height, out int minimum_width, out int natural_width)
			{
				if (Orientation == Gtk.Orientation.Horizontal)
				{
					Child1.GetPreferredWidthForHeight(height, out int min1, out int width1);
					Child2.GetPreferredWidthForHeight(height, out int min2, out int width2);
					base.OnGetPreferredWidthForHeight(height, out _, out int natural);
					UpdateHandleSize(natural, width1, width2);
					minimum_width = Handler.GetPreferredPanelSize(min1, min2);
					natural_width = Handler.GetPreferredPanelSize(width1, width2);
				}
				else
				{
					base.OnGetPreferredWidthForHeight(height, out minimum_width, out natural_width);
				}
			}

			protected override void OnGetPreferredWidth(out int minimum_width, out int natural_width)
			{
				if (Orientation == Gtk.Orientation.Horizontal)
				{
					Child1.GetPreferredWidth(out int min1, out int width1);
					Child2.GetPreferredWidth(out int min2, out int width2);
					base.OnGetPreferredWidth(out _, out int natural);
					UpdateHandleSize(natural, width1, width2);
					minimum_width = Handler.GetPreferredPanelSize(min1, min2);
					natural_width = Handler.GetPreferredPanelSize(width1, width2);
				}
				else
				{
					base.OnGetPreferredWidth(out minimum_width, out natural_width);
				}
			}

			protected override void OnGetPreferredHeight(out int minimum_height, out int natural_height)
			{
				if (Orientation == Gtk.Orientation.Vertical)
				{
					Child1.GetPreferredHeight(out int min1, out int height1);
					Child2.GetPreferredHeight(out int min2, out int height2);
					base.OnGetPreferredHeight(out _, out int natural);
					UpdateHandleSize(natural, height1, height2);
					minimum_height = Handler.GetPreferredPanelSize(min1, min2);
					natural_height = Handler.GetPreferredPanelSize(height1, height2);
				}
				else
				{
					base.OnGetPreferredHeight(out minimum_height, out natural_height);
				}
			}

			protected override void OnGetPreferredHeightForWidth(int width, out int minimum_height, out int natural_height)
			{
				if (Orientation == Gtk.Orientation.Vertical)
				{
					Child1.GetPreferredHeightForWidth(width, out int min1, out int height1);
					Child2.GetPreferredHeightForWidth(width, out int min2, out int height2);
					base.OnGetPreferredHeightForWidth(width, out _, out int natural);
					UpdateHandleSize(natural, height1, height2);
					minimum_height = Handler.GetPreferredPanelSize(min1, min2);
					natural_height = Handler.GetPreferredPanelSize(height1, height2);
				}
				else
				{
					base.OnGetPreferredHeightForWidth(width, out minimum_height, out natural_height);
				}
			}

			protected override void OnSizeAllocated(Gdk.Rectangle allocation)
			{
				var it = Handler;
				if (it == null || double.IsNaN(it.relative))
				{
					base.OnSizeAllocated(allocation);
					return;
				}
				it.suppressSplitterMoved++;
				base.OnSizeAllocated(allocation);
				it.suppressSplitterMoved--;

				it.EnsurePosition();
			}
		}

		public override Gtk.Widget ContainerControl
		{
			get { return container; }
		}

		public SplitterHandler()
		{
			container = new Gtk.EventBox();
			Create();
		}

		public int Position
		{
			get { return position ?? Control.Position; }
			set
			{
				if (value != position)
				{
					position = value;
					lastPosition = value;
					relative = double.NaN;
					if (Control.IsRealized)
						SetPosition(value);
				}
			}
		}

		EtoPaned Paned => (EtoPaned)Control;

		public int SplitterWidth
		{
			get => Paned.RealHandleSize;
			set
			{
				Control.WideHandle = value >= 4;
				Paned.InvalidateHandleSize();
			}
		}

		int GetAvailableSize()
		{
			return GetAvailableSize(!Control.IsRealized);
		}
		int GetAvailableSize(bool desired)
		{
			if (desired)
			{
				var size = UserPreferredSize;
				var pick = Orientation == Orientation.Horizontal ?
					size.Width : size.Height;
				if (pick >= 0)
					return pick - SplitterWidth;
			}
			return (Orientation == Orientation.Horizontal ?
				Control.Allocation.Width : Control.Allocation.Height) - SplitterWidth;
		}

		void UpdateRelative()
		{
			var pos = Position;
			if (fixedPanel == SplitterFixedPanel.Panel1)
				relative = pos;
			else
			{
				var sz = GetAvailableSize();
				if (fixedPanel == SplitterFixedPanel.Panel2)
					relative = sz <= 0 ? 0 : sz - pos;
				else
					relative = sz <= 0 ? 0.5 : pos / (double)sz;
			}
		}

		public double RelativePosition
		{
			get
			{
				if (double.IsNaN(relative))
					UpdateRelative();
				return relative;
			}
			set
			{
				if (relative == value)
					return;
				relative = value;
				position = null;
				if (Control.IsRealized)
					SetRelative(value);
				EnsurePosition();
				Callback.OnPositionChanged(Widget, EventArgs.Empty);
			}
		}

		void SetPosition(int newPosition)
		{
			position = null;
			var size = GetAvailableSize();
			relative = fixedPanel == SplitterFixedPanel.Panel1 ? Math.Max(0, newPosition)
				: fixedPanel == SplitterFixedPanel.Panel2 ? Math.Max(0, size - newPosition)
				: size <= 0 ? 0.5 : Math.Max(0.0, Math.Min(1.0, newPosition / (double)size));
			Control.Position = newPosition;
			lastPosition = newPosition;
		}

		void SetRelative(double newRelative)
		{
			position = null;
			relative = newRelative;
			var size = GetAvailableSize();
			if (size <= 0)
				return;
			switch (fixedPanel)
			{
				case SplitterFixedPanel.Panel1:
					lastPosition = Math.Max(0, Math.Min(size, (int)Math.Round(relative)));
					break;
				case SplitterFixedPanel.Panel2:
					lastPosition = Math.Max(0, Math.Min(size, size - (int)Math.Round(relative)));
					break;
				case SplitterFixedPanel.None:
					lastPosition = Math.Max(0, Math.Min(size, (int)Math.Round(size * relative)));
					break;
			}
			Control.Position = lastPosition;
		}

		public SplitterFixedPanel FixedPanel
		{
			get { return fixedPanel; }
			set
			{
				if (fixedPanel != value)
				{
					fixedPanel = value;
					var position = Position;
					if (WasLoaded)
						UpdateRelative();

					((Gtk.Paned.PanedChild)Control[Control.Child1]).Resize = value != SplitterFixedPanel.Panel1;
					((Gtk.Paned.PanedChild)Control[Control.Child2]).Resize = value != SplitterFixedPanel.Panel2;

					if (Control.IsRealized)
						SetPosition(position);
					else if (WasLoaded)
						SetRelative(relative);
				}
			}
		}

		public Orientation Orientation
		{
			get => Control.Orientation == Gtk.Orientation.Horizontal ? Orientation.Horizontal : Orientation.Vertical;
			set => Control.Orientation = value == Orientation.Horizontal ? Gtk.Orientation.Horizontal : Gtk.Orientation.Vertical;
		}

		protected override void RealizedSetup()
		{
			base.RealizedSetup();
			HookEvents();

			if (Control.Handle != IntPtr.Zero) // happens in VS for Mac..
				EnsurePosition();
		}

		void Create()
		{
			Control = new EtoPaned(Gtk.Orientation.Horizontal) { Handler = this };

			Control.ShowAll();

			Control.Pack1(EmptyContainer(), fixedPanel != SplitterFixedPanel.Panel1, true);
			Control.Pack2(EmptyContainer(), fixedPanel != SplitterFixedPanel.Panel2, true);

			container.Child = Control;
		}

		void HookEvents()
		{
			Control.AddNotification("position", PositionChanged);
		}
		int lastPosition;
		UITimer timer;

		void PositionChanged(object o, GLib.NotifyArgs args)
		{
			if (!Widget.Loaded || suppressSplitterMoved > 0)
				return;

			suppressSplitterMoved++;
			// keep track of the desired position (for removing/re-adding/resizing the control)
			EnsurePosition();

			var newPosition = Position;
			if (newPosition == lastPosition)
			{
				suppressSplitterMoved--;
				return;
			}
			position = lastPosition;
			if (timer == null)
			{
				timer = new UITimer(TriggerChangeCompleted) { Interval = 0.5 };
				Callback.OnPositionChangeStarted(Widget, EventArgs.Empty);
			}
			timer.Start();

			var e = new SplitterPositionChangingEventArgs(newPosition);
			Callback.OnPositionChanging(Widget, e);
			position = null;
			if (e.Cancel)
			{
				Position = lastPosition;
				args.RetVal = false;
			}
			else
			{
				UpdateRelative();
				lastPosition = newPosition;
				Callback.OnPositionChanged(Widget, EventArgs.Empty);
			}
			suppressSplitterMoved--;
		}

		private void TriggerChangeCompleted(object sender, EventArgs e)
		{
			Callback.OnPositionChangeCompleted(Widget, EventArgs.Empty);
			timer?.Dispose();
			timer = null;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Splitter.PositionChangedEvent:
				case Splitter.PositionChangingEvent:
				case Splitter.PositionChangeStartedEvent:
				case Splitter.PositionChangeCompletedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			
			Widget.MouseUp += Widget_MouseUp;
		}

		private void Widget_MouseUp(object sender, MouseEventArgs e)
		{
			if (timer != null)
				TriggerChangeCompleted(sender, EventArgs.Empty);
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			WasLoaded = false;
			suppressSplitterMoved++;
			if (Control.IsRealized)
			SetInitialPosition();
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			suppressSplitterMoved--;
		}

		static readonly object WasLoaded_Key = new object();

		bool WasLoaded
		{
			get { return Widget.Properties.Get<bool>(WasLoaded_Key); }
			set { Widget.Properties.Set(WasLoaded_Key, value); }
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			WasLoaded = true;
			position = null;
			relative = RelativePosition;
		}

		void SetInitialPosition()
		{
			suppressSplitterMoved++;
			try
			{
				if (position != null)
				{
					var pos = position.Value;
					if (fixedPanel != SplitterFixedPanel.Panel1)
					{
						var size = GetAvailableSize(false);
						var want = GetAvailableSize(true);
						if (size != want)
						{
							if (FixedPanel == SplitterFixedPanel.Panel2)
								pos += size - want;
							else
							{
								SetRelative(pos / (double)want);
								return;
							}
						}

					}
					SetPosition(pos);
				}
				else if (!double.IsNaN(relative))
				{
					SetRelative(relative);
				}
				else if (fixedPanel == SplitterFixedPanel.Panel1)
				{
					var size1 = Control.Child1.GetPreferredSize();
					SetRelative(Orientation == Orientation.Horizontal ? size1.Width : size1.Height);
				}
				else if (fixedPanel == SplitterFixedPanel.Panel2)
				{
					var size2 = Control.Child2.GetPreferredSize();
					SetRelative(Orientation == Orientation.Horizontal ? size2.Width : size2.Height);
				}
				else
				{
					var size1 = Control.Child1.GetPreferredSize();
					var size2 = Control.Child2.GetPreferredSize();
					SetRelative(Orientation == Orientation.Horizontal
						? size1.Width / (double)(size1.Width + size2.Width)
						: size1.Height / (double)(size1.Height + size2.Height));
				}
			}
			finally
			{
				suppressSplitterMoved--;
			}

		}

		void EnsurePosition()
		{
			var size = Orientation == Orientation.Horizontal ? Widget.Width : Widget.Height;
			if (size <= 0)
				return;

			if (!initialPositionSet && Control.IsRealized && GetAvailableSize() > 0)
			{
				initialPositionSet = true;
				SetInitialPosition();
			}


			if (_panel1MinimumSize + _panel2MinimumSize > size || Control.Position < _panel1MinimumSize)
				Control.Position = _panel1MinimumSize;
			else if (Position > size - _panel2MinimumSize)
				Control.Position = size - _panel2MinimumSize;
		}

		static Gtk.Widget EmptyContainer()
		{
			var bin = new Gtk.Box(Gtk.Orientation.Vertical, 0);
			bin.Visible = false;
			bin.NoShowAll = true;
			return bin;
		}

		public Control Panel1
		{
			get { return panel1; }
			set
			{
				panel1 = value;
				var setposition = position != null && (Control.Child1 == null || Control.Child2 == null);
				if (Control.Child1 != null)
					Control.Remove(Control.Child1);
				var widget = panel1 != null ? panel1.GetContainerWidget() : EmptyContainer();
				Control.Pack1(widget, fixedPanel != SplitterFixedPanel.Panel1, true);
				if (setposition)
					Control.Position = position.Value;
				widget.ShowAll();
			}
		}

		public Control Panel2
		{
			get { return panel2; }
			set
			{
				panel2 = value;
				var setposition = position != null && (Control.Child1 == null || Control.Child2 == null);
				if (Control.Child2 != null)
					Control.Remove(Control.Child2);
				var widget = panel2 != null ? panel2.GetContainerWidget() : EmptyContainer();
				Control.Pack2(widget, fixedPanel != SplitterFixedPanel.Panel2, true);
				if (setposition)
					Control.Position = position.Value;
				widget.ShowAll();
			}
		}

		public int Panel1MinimumSize
		{
			get
			{
				return _panel1MinimumSize;
			}
			set
			{
				_panel1MinimumSize = value;
				EnsurePosition();
			}
		}

		public int Panel2MinimumSize
		{
			get
			{
				return _panel2MinimumSize;
			}
			set
			{
				_panel2MinimumSize = value;
				EnsurePosition();
			}
		}
	}
}
