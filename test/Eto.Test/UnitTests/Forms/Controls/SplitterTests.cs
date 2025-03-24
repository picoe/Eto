using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class SplitterTests : TestBase
	{
		// currently not working on Gtk or WPF due to deferred size allocation
		bool ReplayTests => !Platform.Instance.IsGtk && !Platform.Instance.IsWpf;

		static IEnumerable SplitterCases
		{
			get
			{
				foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
					foreach (SplitterFixedPanel fix in Enum.GetValues(typeof(SplitterFixedPanel)))
						yield return new TestCaseData(orientation, fix);
			}
		}

		[Test, TestCaseSource(nameof(SplitterCases))]
		public void PositionShouldNotChange(Orientation orient, SplitterFixedPanel fix)
		{
			bool replay = false;
			Shown(
				form => new Splitter()
				{
					Size = new Size(300, 300),
					Orientation = orient,
					FixedPanel = fix,
					Position = 50,
					Panel1 = new Panel
					{
						BackgroundColor = Colors.White
					},
					Panel2 = new Panel
					{
						BackgroundColor = Colors.Black
					}
				},
				it =>
				{
					Assert.That(it.Position, Is.EqualTo(50), $"Fix: {fix}; {orient} [replay={replay}]");
					if (ReplayTests)
						replay = !replay;
				}, replay: ReplayTests);
		}

		[Test, TestCaseSource(nameof(SplitterCases))]
		public void RelativePositionShouldNotChange(Orientation orient, SplitterFixedPanel fix)
		{
			bool replay = false;
			double pos = fix == SplitterFixedPanel.None ? (1 / 3.0) : 50;
			Shown(
				form => new Splitter()
				{
					Size = new Size(300, 300),
					Orientation = orient,
					FixedPanel = fix,
					RelativePosition = pos,
					Panel1 = new Panel
					{
						BackgroundColor = Colors.White
					},
					Panel2 = new Panel
					{
						BackgroundColor = Colors.Black
					}
				},
				it =>
				{
					Assert.That(it.RelativePosition, Is.EqualTo(pos).Within(1e-2), $"Fix: {fix}; {orient} [replay={replay}]");
					if (ReplayTests)
						replay = !replay;
				},
				replay: ReplayTests);
		}

		[Test, TestCaseSource(nameof(SplitterCases))]
		public void NoPositionShouldAutoSizeBasic(Orientation orient, SplitterFixedPanel fix)
		{
			bool replay = false;
			var sz = new Size(50, 50);
			Shown(
				form => new Splitter
				{
					Orientation = orient,
					FixedPanel = fix,
					Panel1 = new Panel
					{
						Size = sz,
						BackgroundColor = Colors.White
					},
					Panel2 = new Panel
					{
						Size = sz,
						BackgroundColor = Colors.Black
					}
				},
				it =>
				{
					if (orient == Orientation.Horizontal)
						Assert.That(it.Panel1.Height, Is.EqualTo(it.Panel2.Height),
							$"Height! Fix: {fix}; {orient} [replay={replay}]");
					else
						Assert.That(it.Panel1.Width, Is.EqualTo(it.Panel2.Width),
							$"Width! Fix: {fix}; {orient} [replay={replay}]");
					switch (fix)
					{
						case SplitterFixedPanel.Panel1:
							if (orient == Orientation.Horizontal)
								Assert.That(sz.Width, Is.EqualTo(it.Panel1.Width),
									$"P1.Width! Fix: {fix}; {orient} [replay={replay}]");
							else
								Assert.That(sz.Height, Is.EqualTo(it.Panel1.Height),
									$"P1.Height! Fix: {fix}; {orient} [replay={replay}]");
							break;
						case SplitterFixedPanel.Panel2:
							if (orient == Orientation.Horizontal)
								Assert.That(sz.Width, Is.EqualTo(it.Panel2.Width),
									$"P2.Width! Fix: {fix}; {orient} [replay={replay}]");
							else
								Assert.That(sz.Height, Is.EqualTo(it.Panel2.Height),
									$"P2.Height! Fix: {fix}; {orient} [replay={replay}]");
							break;
						case SplitterFixedPanel.None:
							if (orient == Orientation.Horizontal)
								Assert.That(it.Panel1.Width, Is.EqualTo(it.Panel2.Width).Within(1),
									$"Width! Fix: {fix}; {orient} [replay={replay}]");
							else
								Assert.That(it.Panel1.Height, Is.EqualTo(it.Panel2.Height).Within(1),
									$"Height! Fix: {fix}; {orient} [replay={replay}]");
							break;
					}
					if (ReplayTests)
						replay = !replay;
				},
				replay: ReplayTests);
		}

		[Test, TestCaseSource(nameof(SplitterCases))]
		public void NoPositionShouldAutoSizeComplexTest1(Orientation orient, SplitterFixedPanel fix)
		{
			bool replay = false;
			Shown(
				form =>
				{
					form.WindowStyle = WindowStyle.Utility;
					// +====test 1====+ 
					// |        |     | 
					// | tested |     |   Tested splitter is placed inside two other splitters
					// |        |     |   to overcome minimal window/form size problems
					// +--------+     |   ...or non-desktop plaforms
					// |        |     | 
					// +--------+-----+ 
					var it = new Splitter
					{
						ID = "main.panel1.panel1",
						Orientation = orient,
						FixedPanel = fix,
						Panel1 = new Panel
						{
							Size = new Size(40, 40),
							BackgroundColor = Colors.White
						},
						Panel2 = new Panel
						{
							Size = new Size(60, 60),
							BackgroundColor = Colors.Black
						}
					};
					form.Content = new Splitter
					{
						ID = "main",
						Panel1 = new Splitter
						{
							ID = "main.panel1",
							Orientation = Orientation.Vertical,
							Panel1 = it,
							Panel2 = new Panel()
						},
						Panel2 = new Panel()
					};
					return it;
				},
				it =>
				{
					Assert.That(it.Position, Is.EqualTo(40), $"#1 {fix}; {orient}; Replay:{replay}");
					Assert.That(it.RelativePosition, Is.EqualTo(fix == SplitterFixedPanel.Panel1 ? 40 : fix == SplitterFixedPanel.Panel2 ? 60 : 0.4), "{0}; {1}", $"#2 {fix}; {orient}; Replay:{replay}");
					var sz = orient == Orientation.Horizontal ? new Size(100 + it.SplitterWidth, 60) : new Size(60, 100 + it.SplitterWidth);
					Assert.That(it.Size, Is.EqualTo(sz), $"#3 {fix}; {orient}; Replay:{replay}");
					if (ReplayTests)
						replay = !replay;
				}, replay: ReplayTests);
		}

		[Test, TestCaseSource(nameof(SplitterCases))]
		public void NoPositionShouldAutoSizeComplexTest2(Orientation orient, SplitterFixedPanel fix)
		{
			Shown(
				form =>
				{
					// +====test 2====+
					// |              |
					// |-----+--------|  Tested splitter is placed inside two other splitters
					// |     |        |  to overcome minimal window/form size problems
					// |     | tested |  ...or non-desktop plaforms
					// |     |        |
					// +-----+--------+
					var it = new Splitter
					{
						Orientation = orient,
						FixedPanel = fix,
						Panel1 = new Panel
						{
							Size = new Size(40, 40),
							BackgroundColor = Colors.White
						},
						Panel2 = new Panel
						{
							Size = new Size(60, 60),
							BackgroundColor = Colors.Black
						}
					};
					form.Content = new Splitter
					{
						Orientation = Orientation.Vertical,
						FixedPanel = SplitterFixedPanel.Panel2,
						Panel1 = new Panel(),
						Panel2 = new Splitter
						{
							FixedPanel = SplitterFixedPanel.Panel2,
							Panel1 = new Panel(),
							Panel2 = it
						}
					};
					return it;
				},
				it =>
				{
					Assert.That(it.Position, Is.EqualTo(40), $"{fix}; {orient}");
					Assert.That(it.RelativePosition, Is.EqualTo(fix == SplitterFixedPanel.Panel1 ? 40 : fix == SplitterFixedPanel.Panel2 ? 60 : 0.4), $"{fix}; {orient}");
					var sz = orient == Orientation.Horizontal ? new Size(100 + it.SplitterWidth, 60) : new Size(60, 100 + it.SplitterWidth);
					Assert.That(it.Size, Is.EqualTo(sz), $"{fix}; {orient}");
				}, replay: ReplayTests);
		}

		[Test, TestCaseSource(nameof(SplitterCases))]
		// Issue #309
		public void PositionShouldTrackInitialResize(Orientation orient, SplitterFixedPanel fix)
		{
			bool replay = false;
			Shown(
				form =>
				{
					var it = new Splitter()
					{
						Orientation = orient,
						FixedPanel = fix,
						Position = 50,
						Panel1 = new Panel
						{
							BackgroundColor = Colors.White
						},
						Panel2 = new Panel
						{
							BackgroundColor = Colors.Black
						}
					};
					it.Size = new Size(100, 100) + it.SplitterWidth;
					form.ClientSize = new Size(150, 150) + it.SplitterWidth;
					return it;
				},
				it =>
				{
					double pos = fix == SplitterFixedPanel.None ? 0.5 : 50.0;
					Assert.That(it.RelativePosition, Is.EqualTo(pos).Within(1e-2), $"Fix: {fix}; {orient} [replay={replay}]");
					if (ReplayTests)
						replay = !replay;
				},
				replay: ReplayTests);
		}

		[Test, ManualTest]
		public void SplitterShouldRegisterChangeNotifications()
		{
			bool success = false;
			string message = "#1";
			Form(form =>
			{
				var posLabel = new Label();
				var label = new Label
				{
					Text = "Drag the splitter right",
					TextAlignment = TextAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};
				int stage = 0;
				var splitter = new Splitter
				{
					Panel1 = new Panel { BackgroundColor = SystemColors.ControlBackground, Size = new Size(100, 100) },
					Panel2 = new Panel { BackgroundColor = SystemColors.ControlBackground }
				};
				int? startingPosition = null;
				double? startingRelativePosition = null;
				form.Shown += (sender, e) =>
				{
					startingPosition = splitter.Position;
					startingRelativePosition = splitter.RelativePosition;
					posLabel.Text = startingPosition?.ToString();
					if (startingPosition == 0)
					{
						message = "#2 - Initial splitter position not set properly before Shown event";
						form.Close();
					}
				};
				splitter.PositionChanged += (sender, e) =>
				{
					posLabel.Text = splitter.Position.ToString();
					if (success || startingPosition == null)
						return;

					switch (stage)
					{
						case 0:
							if (splitter.Position > startingPosition.Value)
							{
								if (splitter.RelativePosition <= startingRelativePosition.Value)
								{
									success = false;
									message = "Relative position was not updated, it should be greater than the starting position";
									form.Close();
									return;
								}
								label.Text = "Now, slide to the left";
								startingPosition = splitter.Position;
								stage++;
							}
							break;
						case 1:
							if (splitter.Position < startingPosition.Value)
							{
								if (splitter.RelativePosition >= startingRelativePosition.Value)
								{
									success = false;
									message = "Relative position was not updated, it should be less than the starting position";
								}
								else
									success = true;
								form.Close();
							}
							break;
					}
				};

				form.Size = new Size(300, 300);
				form.Content = new StackLayout
				{
					HorizontalContentAlignment = HorizontalAlignment.Stretch,
					Items =
					{
						new StackLayout
						{
							Orientation = Orientation.Horizontal,
							Padding = 10,
							Items = { new StackLayoutItem(label, true), posLabel }
						},
						new StackLayoutItem(splitter, true)
					}
				};
			}, -1);
			Assert.That(success, Is.True, message);
		}

		[Test, ManualTest]
		public void SplitterInTabControlShouldKeepPosition()
		{
			ManualForm("Move the splitter then switch tabs and then back again. The splitter should be at the same position as you left it", form =>
			{
				var splitter = new Splitter
				{
					FixedPanel = SplitterFixedPanel.Panel1,
					Orientation = Orientation.Vertical,
					Panel1 = new Panel { Content = "Panel1" },
					Panel2 = new Panel { Content = "Panel2" }
				};
				var tabs = new TabControl
				{
					Size = new Size(300, 300),
					Pages =
					{
						new TabPage { Text = "Tab with splitter", Content = splitter },
						new TabPage { Text = "Tab 2", Content = "Some content" }
					}
				};
				return tabs;
			});
		}

		[Test, ManualTest]
		public void SplitterChangingShouldAllowRestrictingWithoutArtifacts()
		{
			int? outOfBounds = null;
			ManualForm("Splitter should be restricted between 100 and 200, and start at 300", form =>
			{
				form.ClientSize = new Size(600, 300);
				var splitter = new Splitter
				{

					Panel1 = new Panel { BackgroundColor = Colors.Blue, Size = new Size(300, 200) },
					Panel2 = new Panel { BackgroundColor = Colors.Red, Size = new Size(300, 200) }
				};

				splitter.PositionChanging += (sender, e) =>
				{
					System.Diagnostics.Debug.WriteLine($"PositionChanging, Position {splitter.Position}, NewPosition: {e.NewPosition}");
					if (e.NewPosition < 100)
					{
						splitter.Position = 100;
						e.Cancel = true;
					}
					if (e.NewPosition > 200)
					{
						splitter.Position = 200;
						e.Cancel = true;
					}
				};
				splitter.PositionChanged += (sender, e) =>
				{
					var position = splitter.Position;
					System.Diagnostics.Debug.WriteLine($"PositionChanged, Position: {position}");
					if (position > 200 || position < 100)
					{
						outOfBounds = position;
						form.Close();
					}
				};

				return splitter;
			});
			Assert.That(outOfBounds, Is.Null, $"#1 - Position went out of bounds 100-200, was {outOfBounds}");
		}

		[Test, ManualTest]
		public void SplitterShouldNotMoveWhenChangingCancelled()
		{
			int positionChanged = 0;
			ManualForm("Splitter should not be able to move", form =>
			{
				form.ClientSize = new Size(600, 300);
				var splitter = new Splitter
				{

					Panel1 = new Panel { BackgroundColor = Colors.Blue, Size = new Size(300, 200) },
					Panel2 = new Panel { BackgroundColor = Colors.Red, Size = new Size(300, 200) }
				};

				splitter.PositionChanging += (sender, e) =>
				{
					System.Diagnostics.Debug.WriteLine($"PositionChanging, Position {splitter.Position}, NewPosition: {e.NewPosition}");
					e.Cancel = true;
				};
				splitter.PositionChanged += (sender, e) =>
				{
					positionChanged++;
				};
				return splitter;
			});
			Assert.That(positionChanged, Is.EqualTo(0), $"#1 - PositionChanged should not fire");
		}

		[TestCase(Orientation.Horizontal)]
		[TestCase(Orientation.Vertical)]
		public void ZeroRelativePositionShouldNotCrash(Orientation orientation)
		{
			Shown(form =>
			{
				return new Splitter
				{
					Orientation = orientation,
					Panel1 = new Panel { Size = new Size(200, 200) },
					Panel2 = new Panel { Size = new Size(200, 200) },
					FixedPanel = SplitterFixedPanel.None,
					RelativePosition = 0
				};
			},
			c =>
			{
				// if we got here it was successful
			});
		}

		[Test]
		public void SplitterShouldGetAutoSizedAppropriately()
		{
			// ManualForm("blah", form =>
			ShownAsync(form =>
			{
				var splitter = new Splitter
				{
					FixedPanel = SplitterFixedPanel.Panel2,
					Orientation = Orientation.Horizontal,
					Panel1 = new Panel { Content = "Something that should be shown" },
					Panel2 = new Panel { Size = new Size(200, 200), Content = "Panel2" },
				};
				form.Content = splitter;
				return splitter;
			},
			async splitter =>
			{
				await Task.Delay(200);
				Assert.That(splitter.Panel1.Width, Is.GreaterThan(100), "#1");
				Assert.That(splitter.Panel2.Width, Is.EqualTo(200), "#2");
			});
		}
	}
}
