using System.Windows.Controls.Primitives;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoGrid : swc.Grid, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }
		
		internal sw.Size BaseMeasureOverride(sw.Size constraint) => base.MeasureOverride(constraint);

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}
	
	public class SplitterHandler : WpfContainer<EtoGrid, Splitter, Splitter.ICallback>, Splitter.IHandler
	{
		readonly swc.GridSplitter _splitter;
		readonly swc.DockPanel _pane1;
		readonly swc.DockPanel _pane2;
		Orientation _orientation;
		SplitterFixedPanel _fixedPanel;
		int? _position;
		int _splitterWidth = 5;
		double _relative = double.NaN;
		bool _panel1Visible, _panel2Visible;
		int _panel1MinimumSize, _panel2MinimumSize;
		Control _panel1, _panel2;
		PropertyChangeNotifier _panel1VisibilityNotifier;
		PropertyChangeNotifier _panel2VisibilityNotifier;
		bool _positionChanged;


		public override sw.Size MeasureOverride(sw.Size constraint, Func<sw.Size, sw.Size> measure)
		{
			return base.MeasureOverride(constraint, BetterMeasure);
		}

		private sw.Size BetterMeasure(sw.Size constraint)
		{
			// call base measure override so everything gets set up correctly
			var size = Control.BaseMeasureOverride(constraint);
			
			if (FixedPanel == SplitterFixedPanel.Panel2)
			{
				// provide our own measuring when FixedPanel is Panel2, as WPF gets it wrong sometimes
				var panel1Size = _panel1?.GetContainerControl()?.DesiredSize ?? sw.Size.Empty;
				var panel2Size = _panel2?.GetContainerControl()?.DesiredSize ?? sw.Size.Empty;
				if (Orientation == Orientation.Horizontal)
					size.Width = Math.Max(size.Width, panel1Size.Width + panel2Size.Width + SplitterWidth);
				else
					size.Height = Math.Max(size.Height, panel1Size.Height + panel2Size.Height + SplitterWidth); 
			}
			return size;
		}

		public SplitterHandler()
		{
			Control = new EtoGrid { Handler = this };

			Control.ColumnDefinitions.Add(new swc.ColumnDefinition());
			Control.ColumnDefinitions.Add(new swc.ColumnDefinition() { Width = sw.GridLength.Auto });
			Control.ColumnDefinitions.Add(new swc.ColumnDefinition());

			Control.RowDefinitions.Add(new swc.RowDefinition());
			Control.RowDefinitions.Add(new swc.RowDefinition { Height = sw.GridLength.Auto });
			Control.RowDefinitions.Add(new swc.RowDefinition());

			_splitter = new swc.GridSplitter
			{
				//Background = sw.SystemColors.ControlLightLightBrush,
				ResizeBehavior = swc.GridResizeBehavior.PreviousAndNext
			};
			_pane1 = new swc.DockPanel { LastChildFill = true };
			_pane2 = new swc.DockPanel { LastChildFill = true };


			Control.Children.Add(_pane1);
			Control.Children.Add(_splitter);
			Control.Children.Add(_pane2);

			UpdateOrientation();
			Control.Loaded += Control_Loaded;
			Control.SizeChanged += (sender, e) => ResetMinMax();

			_panel1VisibilityNotifier = new PropertyChangeNotifier(sw.UIElement.VisibilityProperty);
			_panel1VisibilityNotifier.ValueChanged += HandlePanel1IsVisibleChanged;

			_panel2VisibilityNotifier = new PropertyChangeNotifier(sw.UIElement.VisibilityProperty);
			_panel2VisibilityNotifier.ValueChanged += HandlePanel2IsVisibleChanged;
		}

		private void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			// only set on initial load, subsequent loads should keep the last position
			Control.Loaded -= Control_Loaded;
			ResetMinMax();
			SetInitialPosition();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Splitter.PositionChangedEvent:
					PositionChangedEnabled++;
					AttachPropertyChanged(swc.RowDefinition.HeightProperty, HandlePositionChanged, Control.RowDefinitions[0], new object());
					AttachPropertyChanged(swc.RowDefinition.HeightProperty, HandlePositionChanged, Control.RowDefinitions[2], new object());
					AttachPropertyChanged(swc.ColumnDefinition.WidthProperty, HandlePositionChanged, Control.ColumnDefinitions[0], new object());
					AttachPropertyChanged(swc.ColumnDefinition.WidthProperty, HandlePositionChanged, Control.ColumnDefinitions[2], new object());
					PositionChangedEnabled--;
					break;
				case Splitter.PositionChangingEvent:
					_splitter.DragStarted += splitter_DragStarted;
					_splitter.DragCompleted += splitter_DragCompleted;
					_splitter.DragDelta += splitter_DragDelta;
					break;
				case Splitter.PositionChangeStartedEvent:
				case Splitter.PositionChangeCompletedEvent:
					HandleEvent(Splitter.PositionChangingEvent);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void splitter_DragDelta(object sender, DragDeltaEventArgs e)
		{
			_positionChanged = false;
			var pos = Position;
			e.Handled = HandlePositionChanging(e.HorizontalChange, e.VerticalChange);
			if (e.Handled && !_positionChanged)
			{
				Position = pos;
			}
		}

		private void splitter_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			e.Handled = HandlePositionChanging(e.HorizontalChange, e.VerticalChange);
			Callback.OnPositionChangeCompleted(Widget, EventArgs.Empty);
		}

		private void splitter_DragStarted(object sender, DragStartedEventArgs e)
		{
			Callback.OnPositionChangeStarted(Widget, EventArgs.Empty);
			e.Handled = HandlePositionChanging(e.HorizontalOffset, e.VerticalOffset);
		}

		void HandlePositionChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
		{
			if (PositionChangedEnabled > 0)
				return;
			// we use actual width vs. width itself, so we have to use the value passed in
			var old = _position;
			var pos = (sw.GridLength)e.NewValue;
			if (pos.GridUnitType != sw.GridUnitType.Pixel)
				return;
				
			var newPosition = (int)Math.Round(pos.Value);
			
			if (sender is PropertyChangeNotifier notifier && 
				(notifier.PropertySource == Control.ColumnDefinitions[2] || notifier.PropertySource == Control.RowDefinitions[2]))
			{
				// invert position, we are resizing the second pane, not the first
				newPosition = (int)Math.Round(GetAvailableSize() - newPosition);
			}
			if (newPosition != Position)
			{
				var args = new SplitterPositionChangingEventArgs(newPosition);
				// Console.WriteLine($"Source: PositionChanged: {newPosition}");
				Callback.OnPositionChanging(Widget, args);
				if (args.Cancel)
				{
					return;
				}
				_position = newPosition;
				Callback.OnPositionChanged(Widget, EventArgs.Empty);
				_position = old;
			}
		}

		bool HandlePositionChanging(double horizontal, double vertical, [System.Runtime.CompilerServices.CallerMemberName] string source = null)
		{
			var position = DoublePosition;
			if (_orientation == Orientation.Horizontal)
				position += horizontal;
			else
				position += vertical;
				
			// restrict to control size
			position = Math.Max(0, Math.Min(GetAvailableSize(false), position));
			
			var intPosition = (int)Math.Round(position);

			// Console.WriteLine($"Source: {source}, {intPosition}");
			var args = new SplitterPositionChangingEventArgs(intPosition);
			Callback.OnPositionChanging(Widget, args);
			if (args.Cancel)
				return true;

			if (intPosition == Position)
				return false;

			if (_fixedPanel == SplitterFixedPanel.None)
			{
				Callback.OnPositionChanged(Widget, EventArgs.Empty);
			}
			
			return false;
		}

		static object PositionChangedEnabled_Key = new object();
		int PositionChangedEnabled
		{
			get { return Widget.Properties.Get(PositionChangedEnabled_Key, 0); }
			set { Widget.Properties.Set(PositionChangedEnabled_Key, value, 0); }
		}

		void SetInitialPosition()
		{
			_panel1Visible = _panel1?.Visible ?? false;
			_panel2Visible = _panel2?.Visible ?? false;

			// controls should be stretched to fit panels
			SetStretch(_panel1);
			SetStretch(_panel2);
			UpdateColumnSizing(false);

			if (_position != null)
			{
				var pos = _position.Value;
				if (_fixedPanel != SplitterFixedPanel.Panel1)
				{
					var size = GetAvailableSize(false);
					var want = GetAvailableSize(true);
					if (size != want)
					{
						if (FixedPanel == SplitterFixedPanel.Panel2)
							pos += (int)Math.Round(size - want);
						else
						{
							SetRelative(pos / (double)want);
							return;
						}
					}

				}
				SetPosition(pos);
			}
			else if (!double.IsNaN(_relative))
			{
				SetRelative(_relative);
			}
			else if (_fixedPanel == SplitterFixedPanel.Panel1)
			{
				var size1 = _panel1?.GetPreferredSize() ?? SizeF.Empty;
				SetRelative(_orientation == Orientation.Horizontal ? size1.Width : size1.Height);
			}
			else if (_fixedPanel == SplitterFixedPanel.Panel2)
			{
				var size2 = _panel2?.GetPreferredSize() ?? SizeF.Empty;
				SetRelative(_orientation == Orientation.Horizontal ? size2.Width : size2.Height);
			}
			else
			{
				var size1 = _panel1?.GetPreferredSize() ?? SizeF.Empty;
				var size2 = _panel2?.GetPreferredSize() ?? SizeF.Empty;
				SetRelative(_orientation == Orientation.Horizontal
					? size1.Width / (double)(size1.Width + size2.Width)
					: size1.Height / (double)(size1.Height + size2.Height));
			}
		}

		static void SetStretch(Control panel)
		{
			if (panel != null)
			{
				var control = panel.GetContainerControl();
				control.VerticalAlignment = sw.VerticalAlignment.Stretch;
				control.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
				/*
				((sw.FrameworkElement)panel.ControlObject).Width = double.NaN;
				((sw.FrameworkElement)panel.ControlObject).Height = double.NaN;
				 * */
			}
		}

		void UpdateOrientation()
		{
			if (_orientation == Orientation.Horizontal)
			{

				_splitter.ResizeDirection = swc.GridResizeDirection.Columns;
				_splitter.HorizontalAlignment = sw.HorizontalAlignment.Left;
				_splitter.VerticalAlignment = sw.VerticalAlignment.Stretch;

				_splitter.SetValue(swc.Grid.RowSpanProperty, 3);
				_pane1.SetValue(swc.Grid.RowSpanProperty, 3);
				_pane2.SetValue(swc.Grid.RowSpanProperty, 3);

				_splitter.SetValue(swc.Grid.ColumnSpanProperty, 1);
				_pane1.SetValue(swc.Grid.ColumnSpanProperty, 1);
				_pane2.SetValue(swc.Grid.ColumnSpanProperty, 1);

				swc.Grid.SetColumn(_splitter, 1);
				swc.Grid.SetRow(_splitter, 0);
				swc.Grid.SetColumn(_pane2, 2);
				swc.Grid.SetRow(_pane2, 0);

				_splitter.Width = _splitterWidth;
				_splitter.Height = double.NaN;
			}
			else
			{
				_splitter.ResizeDirection = swc.GridResizeDirection.Rows;
				_splitter.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
				_splitter.VerticalAlignment = sw.VerticalAlignment.Top;
				_pane2.VerticalAlignment = sw.VerticalAlignment.Stretch;

				_splitter.SetValue(swc.Grid.RowSpanProperty, 1);
				_pane1.SetValue(swc.Grid.RowSpanProperty, 1);
				_pane2.SetValue(swc.Grid.RowSpanProperty, 1);

				_splitter.SetValue(swc.Grid.ColumnSpanProperty, 3);
				_pane1.SetValue(swc.Grid.ColumnSpanProperty, 3);
				_pane2.SetValue(swc.Grid.ColumnSpanProperty, 3);

				swc.Grid.SetColumn(_splitter, 0);
				swc.Grid.SetRow(_splitter, 1);
				swc.Grid.SetColumn(_pane2, 0);
				swc.Grid.SetRow(_pane2, 2);

				_splitter.Width = double.NaN;
				_splitter.Height = _splitterWidth;
			}
			UpdateColumnSizing(_position.HasValue || !double.IsNaN(_relative));
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		void UpdateColumnSizing(bool updatePosition)
		{
			if (updatePosition && _position == null && double.IsNaN(_relative))
				UpdateRelative();

			//SetLength(1, sw.GridLength.Auto);
			switch (FixedPanel)
			{
				case SplitterFixedPanel.Panel1:
					SetLength(0, new sw.GridLength(1, sw.GridUnitType.Star));
					break;
				case SplitterFixedPanel.Panel2:
					SetLength(0, new sw.GridLength(1, sw.GridUnitType.Star));
					break;
				case SplitterFixedPanel.None:
					SetLength(0, new sw.GridLength(1, sw.GridUnitType.Star));
					SetLength(2, new sw.GridLength(1, sw.GridUnitType.Star));
					break;
			}

			if (updatePosition)
			{
				SetPositionOrRelative();
			}
		}

		void SetPositionOrRelative()
		{
			if (_position.HasValue)
				SetPosition(_position.Value);
			else if (!double.IsNaN(_relative))
				SetRelative(_relative);
			else
				SetHiddenPanels();
		}

		public Orientation Orientation
		{
			get
			{
				switch (_splitter.ResizeDirection)
				{
					case swc.GridResizeDirection.Columns:
						return Orientation.Horizontal;
					case swc.GridResizeDirection.Rows:
						return Orientation.Vertical;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				_orientation = value;
				UpdateOrientation();
			}
		}

		public SplitterFixedPanel FixedPanel
		{
			get { return _fixedPanel; }
			set
			{
				if (Control.IsLoaded)
				{
					if (HasHiddenPanels)
						UpdateRelativePosition(value);
					else
						_position = Position;
				}
				else if (WasLoaded)
					UpdateRelativePosition(value);

				_fixedPanel = value;
				
				if (_fixedPanel == SplitterFixedPanel.None)
				{
					// positionchanged events get triggered here when the fixed panel is none
					HandleEvent(Splitter.PositionChangingEvent);
				}

				UpdateColumnSizing(Control.IsLoaded);
			}
		}

		private void UpdateRelativePosition(SplitterFixedPanel newFixedPanel)
		{
			if (double.IsNaN(_relative))
				return;

			// translate relative position from old fixed panel to new fixed panel
			var width = _orientation == Orientation.Horizontal ? Control.ActualWidth : Control.ActualHeight;
			switch (_fixedPanel)
			{
				case SplitterFixedPanel.Panel1:
					switch (newFixedPanel)
					{
						case SplitterFixedPanel.Panel2:
							_relative = width - _relative - SplitterWidth;
							break;
						case SplitterFixedPanel.None:
							_relative = _relative / width;
							break;
					}
					break;
				case SplitterFixedPanel.Panel2:
					switch (newFixedPanel)
					{
						case SplitterFixedPanel.Panel1:
							_relative = width - _relative - SplitterWidth;
							break;
						case SplitterFixedPanel.None:
							_relative = (width - _relative) / width;
							break;
					}
					break;
				case SplitterFixedPanel.None:
					switch (newFixedPanel)
					{
						case SplitterFixedPanel.Panel1:
							_relative = width * _relative;
							break;
						case SplitterFixedPanel.Panel2:
							_relative = width - (width * _relative);
							break;
					}
					break;
			}
		}

		double DoublePosition
		{
			get
			{
				if (_position != null)
					return _position.Value;
				if (!Control.IsLoaded && !WasLoaded)
					return 0;
				if (_splitter.ResizeDirection == swc.GridResizeDirection.Columns)
					return Control.ColumnDefinitions[0].ActualWidth;
				return Control.RowDefinitions[0].ActualHeight;
			}
		}

		public int Position
		{
			get => (int)Math.Round(DoublePosition);
			set
			{
				SetPosition(value);
			}
		}

		public int SplitterWidth
		{
			get { return _splitterWidth; }
			set
			{
				if (_splitterWidth == value)
					return;
				_splitterWidth = value;
				if (_orientation == Orientation.Horizontal)
					_splitter.Width = value;
				else
					_splitter.Height = value;
			}
		}

		double GetAvailableSize()
		{
			return GetAvailableSize(!Control.IsLoaded);
		}

		double GetAvailableSize(bool desired)
		{
			if (desired)
			{
				var size = UserPreferredSize;
				var pick = Orientation == Orientation.Horizontal ? size.Width : size.Height;
				if (pick >= 0)
					return pick - _splitterWidth;
			}
			var width = Orientation == Orientation.Horizontal ? Control.ActualWidth : Control.ActualHeight;
			if (double.IsNaN(width))
				width = Orientation == Orientation.Horizontal ? Control.Width : Control.Height;
			return width - _splitterWidth;
		}

		void UpdateRelative()
		{
			var pos = Position;
			if (_fixedPanel == SplitterFixedPanel.Panel1)
				_relative = pos;
			else
			{
				var sz = GetAvailableSize();
				if (_fixedPanel == SplitterFixedPanel.Panel2)
					_relative = sz <= 0 ? 0 : sz - pos;
				else
					_relative = sz <= 0 ? 0.5 : pos / (double)sz;
			}
		}

		public double RelativePosition
		{
			get
			{
				if (double.IsNaN(_relative) || Widget.Loaded)
					UpdateRelative();
				return _relative;
			}
			set
			{
				if (_relative == value)
					return;
				SetRelative(value);
				Callback.OnPositionChanged(Widget, EventArgs.Empty);
			}
		}

		double GetSize(int panel)
		{
			if (_splitter.ResizeDirection == swc.GridResizeDirection.Columns)
				return Control.ColumnDefinitions[panel].ActualWidth;
			else
				return Control.RowDefinitions[panel].ActualHeight;
		}


		sw.GridLength GetLength(int panel)
		{
			if (_splitter.ResizeDirection == swc.GridResizeDirection.Columns)
				return Control.ColumnDefinitions[panel].Width;
			else
				return Control.RowDefinitions[panel].Height;
		}

		void SetLength(int panel, sw.GridLength value)
		{
			if (_splitter.ResizeDirection == swc.GridResizeDirection.Columns)
				Control.ColumnDefinitions[panel].Width = value;
			else
				Control.RowDefinitions[panel].Height = value;
		}

		bool HasHiddenPanels
		{
			get { return _panel1 == null || !_panel1.Visible || _panel2 == null || !_panel2.Visible; }
		}

		bool SetHiddenPanels()
		{
			if (!Widget.Loaded)
				return false;
			if (_panel1 == null || !_panel1.Visible)
			{
				SetLength(0, new sw.GridLength(0, sw.GridUnitType.Pixel));
				SetLength(1, new sw.GridLength(0, sw.GridUnitType.Pixel));
				SetLength(2, new sw.GridLength(1, sw.GridUnitType.Star));
				return true;
			}
			if (_panel2 == null || !_panel2.Visible)
			{
				SetLength(0, new sw.GridLength(1, sw.GridUnitType.Star));
				SetLength(1, new sw.GridLength(0, sw.GridUnitType.Pixel));
				SetLength(2, new sw.GridLength(0, sw.GridUnitType.Pixel));
				return true;
			}
			return false;
		}

		void SetPosition(int newPosition)
		{
			if (SetHiddenPanels())
				return;
			_positionChanged = true;
			SetLength(1, sw.GridLength.Auto);
			if (!Control.IsLoaded)
			{
				_position = newPosition;
				_relative = double.NaN;
				SetLength(0, new sw.GridLength(Math.Max(0, newPosition), sw.GridUnitType.Pixel));
				SetLength(2, new sw.GridLength(1, sw.GridUnitType.Star));
				return;
			}

			_position = null;
			var size = GetAvailableSize(false);
			_relative = _fixedPanel == SplitterFixedPanel.Panel1 ? Math.Max(0, newPosition)
				: _fixedPanel == SplitterFixedPanel.Panel2 ? Math.Max(0, size - newPosition)
				: size <= 0 ? 0.5 : Math.Max(0.0, Math.Min(1.0, newPosition / (double)size));
			if (_fixedPanel == SplitterFixedPanel.Panel1)
			{
				SetLength(0, new sw.GridLength(Math.Max(0, newPosition), sw.GridUnitType.Pixel));
				SetLength(2, new sw.GridLength(1, sw.GridUnitType.Star));
			}
			else if (_fixedPanel == SplitterFixedPanel.Panel2)
			{
				SetLength(0, new sw.GridLength(1, sw.GridUnitType.Star));
				SetLength(2, new sw.GridLength(Math.Max(0, size - newPosition), sw.GridUnitType.Pixel));
			}
			else
			{
				SetLength(0, new sw.GridLength(Math.Max(0, newPosition), sw.GridUnitType.Star));
				SetLength(2, new sw.GridLength(Math.Max(0, size - newPosition), sw.GridUnitType.Star));
			}
		}

		void SetRelative(double newRelative)
		{
			if (SetHiddenPanels())
				return;
			_positionChanged = true;
			_position = null;
			_relative = newRelative;
			PositionChangedEnabled++;
			SetLength(1, sw.GridLength.Auto);
			if (_fixedPanel == SplitterFixedPanel.Panel1)
			{
				SetLength(0, new sw.GridLength(Math.Max(0, _relative), sw.GridUnitType.Pixel));
				SetLength(2, new sw.GridLength(1, sw.GridUnitType.Star));
			}
			else if (_fixedPanel == SplitterFixedPanel.Panel2)
			{
				SetLength(0, new sw.GridLength(1, sw.GridUnitType.Star));
				SetLength(2, new sw.GridLength(Math.Max(0, _relative), sw.GridUnitType.Pixel));
			}
			else
			{
				SetLength(0, new sw.GridLength(Math.Max(0, _relative), sw.GridUnitType.Star));
				SetLength(2, new sw.GridLength(Math.Max(0, 1 - _relative), sw.GridUnitType.Star));
			}
			PositionChangedEnabled--;
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			var control = _panel1.GetWpfFrameworkElement();
			if (control != null)
				control.SetScale(true, true);
			control = _panel2.GetWpfFrameworkElement();
			if (control != null)
				control.SetScale(true, true);
		}

		public Control Panel1
		{
			get { return _panel1; }
			set
			{
				_panel1VisibilityNotifier.PropertySource = null;

				_panel1 = value;
				_pane1.Children.Clear();
				if (_panel1 != null)
				{
					var control = _panel1.GetWpfFrameworkElement();
					SetStretch(_panel1);
					if (Widget.Loaded)
						control.SetScale(true, true);

					_pane1.Children.Add(control.ContainerControl);
					_panel1VisibilityNotifier.PropertySource = control.ContainerControl;
					HandlePanelVisibleChanged(ref _panel1Visible, _panel1);
				}
				else
				{
					SetHiddenPanels();
				}
			}
		}
		
		public Control Panel2
		{
			get { return _panel2; }
			set
			{
				_panel2VisibilityNotifier.PropertySource = null;

				_panel2 = value;
				_pane2.Children.Clear();
				if (_panel2 != null)
				{
					var control = _panel2.GetWpfFrameworkElement();
					SetStretch(_panel2);
					if (Widget.Loaded)
						control.SetScale(true, true);
					_pane2.Children.Add(control.ContainerControl);

					_panel2VisibilityNotifier.PropertySource = control.ContainerControl;
					HandlePanelVisibleChanged(ref _panel2Visible, _panel2);
				}
				else
				{
					SetHiddenPanels();
				}
			}
		}

		void HandlePanel2IsVisibleChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
		{
			HandlePanelVisibleChanged(ref _panel2Visible, _panel2);
		}

		void HandlePanel1IsVisibleChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
		{
			HandlePanelVisibleChanged(ref _panel1Visible, _panel1);
		}

		void HandlePanelVisibleChanged(ref bool isVisible, Control panel)
		{
			if ((Control.IsLoaded || WasLoaded) && isVisible != panel.Visible)
			{
				isVisible = panel.Visible;
				if (!panel.Visible)
				{
					_position = null;
					if ((_panel1 != null && _panel1.Visible) || (_panel2 != null && _panel2.Visible))
						UpdateRelative();
					SetHiddenPanels();
				}
				else if (!double.IsNaN(_relative))
				{
					SetRelative(_relative);
				}
			}
		}

		public override void Remove(sw.FrameworkElement child)
		{
			if (_pane1.Children.Contains(child))
			{
				_panel1VisibilityNotifier.PropertySource = null;
				_panel1 = null;
				_pane1.Children.Remove(child);
			}
			else if (_pane2.Children.Contains(child))
			{
				_panel2VisibilityNotifier.PropertySource = null;
				_panel2 = null;
				_pane2.Children.Remove(child);
			}
		}

		static readonly object WasLoaded_Key = new object();

		bool WasLoaded
		{
			get { return Widget.Properties.Get<bool>(WasLoaded_Key); }
			set { Widget.Properties.Set(WasLoaded_Key, value); }
		}
		
		private void ResetMinMax()
		{
			if (Orientation == Orientation.Horizontal)
			{
				Control.ColumnDefinitions[0].MinWidth = _panel1MinimumSize;
				Control.ColumnDefinitions[2].MinWidth = _panel2MinimumSize;
				Control.RowDefinitions[0].MinHeight = 0;
				Control.RowDefinitions[2].MinHeight = 0;
			}
			else
			{
				Control.ColumnDefinitions[0].MinWidth = 0;
				Control.ColumnDefinitions[2].MinWidth = 0;
				Control.RowDefinitions[0].MinHeight = Panel1MinimumSize;
				Control.RowDefinitions[2].MinHeight = _panel2MinimumSize;
			}

			if (Control.IsLoaded)
			{
				var available = this.GetAvailableSize();
				// need to set the max height of resizing panel otherwise it can grow outside of window
				if (Orientation == Orientation.Horizontal)
				{
					Control.ColumnDefinitions[0].MaxWidth = FixedPanel == SplitterFixedPanel.Panel1 ? Math.Max(0, available - Panel2MinimumSize) : double.PositiveInfinity;
					Control.ColumnDefinitions[2].MaxWidth = FixedPanel == SplitterFixedPanel.Panel2 ? Math.Max(0, available - Panel1MinimumSize) : double.PositiveInfinity;
					Control.RowDefinitions[0].MaxHeight = double.PositiveInfinity;
					Control.RowDefinitions[2].MaxHeight = double.PositiveInfinity;
				}
				else
				{
					Control.ColumnDefinitions[0].MaxWidth = double.PositiveInfinity;
					Control.ColumnDefinitions[2].MaxWidth = double.PositiveInfinity;
					Control.RowDefinitions[0].MaxHeight = FixedPanel == SplitterFixedPanel.Panel1 ? Math.Max(0, available - Panel2MinimumSize) : double.PositiveInfinity;
					Control.RowDefinitions[2].MaxHeight = FixedPanel == SplitterFixedPanel.Panel2 ? Math.Max(0, available - Panel1MinimumSize) : double.PositiveInfinity;
				}
			}
			else
			{
				// need to set the max height of panel 2 otherwise it will grow beyond the bounds
				Control.ColumnDefinitions[0].MaxWidth = double.PositiveInfinity;
				Control.ColumnDefinitions[2].MaxWidth = double.PositiveInfinity;
				Control.RowDefinitions[0].MaxHeight = double.PositiveInfinity;
				Control.RowDefinitions[2].MaxHeight = double.PositiveInfinity;
			}
		}

		public int Panel1MinimumSize
		{
			get { return _panel1MinimumSize; }
			set
			{
				_panel1MinimumSize = value;
				ResetMinMax();
			}
		}

		public int Panel2MinimumSize
		{
			get { return _panel2MinimumSize; }
			set
			{
				_panel2MinimumSize = value;
				ResetMinMax();
			}
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			WasLoaded = false;
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			WasLoaded = true;
			if (double.IsNaN(_relative) || !HasHiddenPanels)
				UpdateRelative();
			_position = null;
		}
	}
}