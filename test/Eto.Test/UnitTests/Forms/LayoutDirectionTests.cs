using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class LayoutDirectionTests : TestBase
	{
		static Panel Probe(int width = 40, int height = 20) => new Panel { Size = new Size(width, height), BackgroundColor = Colors.Red };

		static RectangleF BoundsIn(Control control, Control container)
		{
			var location = container.PointFromScreen(control.PointToScreen(PointF.Empty));
			return new RectangleF(location, control.Size);
		}

		/// <summary>
		/// Builds the same content LTR and RTL side by side and checks each probe's RTL bounds mirror its LTR bounds,
		/// both for content shown in RTL and for content switched to RTL (and back) after it was shown.
		/// </summary>
		static void AssertMirrored(Func<Control[], Control> create, int probeCount, Action<Control, Control> extraCheck = null)
		{
			Panel ltrHost = null, rtlHost = null, changedHost = null;
			Control[] ltrProbes = null, rtlProbes = null, changedProbes = null;
			Control ltrContent = null, rtlContent = null, changedContent = null;
			ShownAsync(form =>
			{
				Control[] CreateProbes() => Enumerable.Range(0, probeCount).Select(i => (Control)Probe()).ToArray();
				ltrProbes = CreateProbes();
				rtlProbes = CreateProbes();
				changedProbes = CreateProbes();
				ltrContent = create(ltrProbes);
				rtlContent = create(rtlProbes);
				changedContent = create(changedProbes);
				ltrHost = new Panel { LayoutDirection = LayoutDirection.LeftToRight, Size = new Size(300, 120), Content = ltrContent };
				rtlHost = new Panel { LayoutDirection = LayoutDirection.RightToLeft, Size = new Size(300, 120), Content = rtlContent };
				changedHost = new Panel { LayoutDirection = LayoutDirection.LeftToRight, Size = new Size(300, 120), Content = changedContent };
				form.Content = new TableLayout(new TableRow(ltrHost), new TableRow(rtlHost), new TableRow(changedHost));
			}, async () =>
			{
				void Check(string name, Panel host, Control[] probes, Control content)
				{
					Assert.That(host.Width, Is.EqualTo(ltrHost.Width), $"{name}: hosts should be the same width");
					var width = ltrHost.Width;
					for (int i = 0; i < probeCount; i++)
					{
						var ltr = BoundsIn(ltrProbes[i], ltrHost);
						var rtl = BoundsIn(probes[i], host);
						Assert.That(rtl.Width, Is.EqualTo(ltr.Width).Within(1), $"{name}: probe {i} width");
						Assert.That(rtl.Y, Is.EqualTo(ltr.Y).Within(1), $"{name}: probe {i} y");
						Assert.That(rtl.X, Is.EqualTo(width - ltr.Right).Within(1), $"{name}: probe {i} x should be mirrored (ltr: {ltr}, rtl: {rtl}, width: {width})");
					}
					extraCheck?.Invoke(ltrContent, content);
				}
				Check("Shown in right-to-left", rtlHost, rtlProbes, rtlContent);

				changedHost.LayoutDirection = LayoutDirection.RightToLeft;
				await Task.Delay(200);
				Check("Changed to right-to-left", changedHost, changedProbes, changedContent);

				changedHost.LayoutDirection = LayoutDirection.LeftToRight;
				await Task.Delay(200);
				for (int i = 0; i < probeCount; i++)
				{
					var ltr = BoundsIn(ltrProbes[i], ltrHost);
					var changed = BoundsIn(changedProbes[i], changedHost);
					Assert.That(changed.X, Is.EqualTo(ltr.X).Within(1), $"Changed back to left-to-right: probe {i} x (ltr: {ltr}, changed: {changed})");
					Assert.That(changed.Width, Is.EqualTo(ltr.Width).Within(1), $"Changed back to left-to-right: probe {i} width");
				}
			});
		}

		[Test]
		public void DefaultShouldInheritFromApplication()
		{
			Invoke(() =>
			{
				var panel = new Panel();
				Assert.That(panel.LayoutDirection, Is.EqualTo(LayoutDirection.Inherit));
				Assert.That(panel.EffectiveLayoutDirection, Is.EqualTo(Application.Instance.LayoutDirection));
				Assert.That(Application.Instance.LayoutDirection, Is.Not.EqualTo(LayoutDirection.Inherit));
			});
		}

		[Test]
		public void ChildShouldInheritFromParentUntilOverridden()
		{
			Invoke(() =>
			{
				var child = new Panel();
				var inner = new Label();
				child.Content = inner;
				var parent = new Panel { LayoutDirection = LayoutDirection.RightToLeft, Content = child };

				Assert.That(child.EffectiveLayoutDirection, Is.EqualTo(LayoutDirection.RightToLeft));
				Assert.That(inner.EffectiveLayoutDirection, Is.EqualTo(LayoutDirection.RightToLeft));

				child.LayoutDirection = LayoutDirection.LeftToRight;
				Assert.That(child.EffectiveLayoutDirection, Is.EqualTo(LayoutDirection.LeftToRight));
				Assert.That(inner.EffectiveLayoutDirection, Is.EqualTo(LayoutDirection.LeftToRight));

				child.LayoutDirection = LayoutDirection.Inherit;
				Assert.That(inner.EffectiveLayoutDirection, Is.EqualTo(LayoutDirection.RightToLeft));

				parent.LayoutDirection = LayoutDirection.LeftToRight;
				Assert.That(inner.EffectiveLayoutDirection, Is.EqualTo(LayoutDirection.LeftToRight));
			});
		}

		[Test]
		public void ChildrenOfCompositeLayoutsShouldInherit()
		{
			Invoke(() =>
			{
				var stackChild = new Label();
				var dynamicChild = new Label();
				var stack = new StackLayout { Orientation = Orientation.Horizontal, Items = { stackChild } };
				var dynamic = new DynamicLayout();
				dynamic.AddRow(dynamicChild);
				var parent = new Panel { LayoutDirection = LayoutDirection.RightToLeft, Content = new TableLayout(stack, dynamic) };

				// loading creates the inner layouts these use
				parent.AttachNative();
				Assert.That(stackChild.EffectiveLayoutDirection, Is.EqualTo(LayoutDirection.RightToLeft));
				Assert.That(dynamicChild.EffectiveLayoutDirection, Is.EqualTo(LayoutDirection.RightToLeft));
				parent.DetachNative();
			});
		}

		[Test]
		public void EffectiveLayoutDirectionChangedShouldFireOncePerChange()
		{
			Panel parent = null, middle = null;
			Label child = null;
			int childChanged = 0, middleChanged = 0;
			Shown(form =>
			{
				child = new Label { Text = "Hello" };
				middle = new Panel { Content = child };
				parent = new Panel { Content = middle };
				form.Content = parent;
			}, () =>
			{
				var initial = child.EffectiveLayoutDirection;
				var other = initial == LayoutDirection.RightToLeft ? LayoutDirection.LeftToRight : LayoutDirection.RightToLeft;
				child.EffectiveLayoutDirectionChanged += (sender, e) => childChanged++;
				middle.EffectiveLayoutDirectionChanged += (sender, e) => middleChanged++;

				parent.LayoutDirection = other;
				Assert.That(childChanged, Is.EqualTo(1));
				Assert.That(middleChanged, Is.EqualTo(1));
				Assert.That(child.EffectiveLayoutDirection, Is.EqualTo(other));

				// explicitly setting the same effective value isn't a change
				middle.LayoutDirection = other;
				Assert.That(childChanged, Is.EqualTo(1));
				Assert.That(middleChanged, Is.EqualTo(1));

				// child is shielded by middle's explicit value
				parent.LayoutDirection = initial;
				Assert.That(childChanged, Is.EqualTo(1));

				middle.LayoutDirection = LayoutDirection.Inherit;
				Assert.That(childChanged, Is.EqualTo(2));
				Assert.That(middleChanged, Is.EqualTo(2));
				Assert.That(child.EffectiveLayoutDirection, Is.EqualTo(initial));
			});
		}

		[Test]
		public void ChangingApplicationLayoutDirectionShouldUpdateOpenWindows()
		{
			Label child = null;
			int changed = 0;
			Shown(form =>
			{
				child = new Label { Text = "Hello" };
				form.Content = child;
			}, () =>
			{
				var app = Application.Instance;
				var initial = app.LayoutDirection;
				var other = initial == LayoutDirection.RightToLeft ? LayoutDirection.LeftToRight : LayoutDirection.RightToLeft;
				child.EffectiveLayoutDirectionChanged += (sender, e) => changed++;
				try
				{
					app.LayoutDirection = other;
					Assert.That(app.LayoutDirection, Is.EqualTo(other));
					Assert.That(child.EffectiveLayoutDirection, Is.EqualTo(other));
					Assert.That(changed, Is.EqualTo(1));
				}
				finally
				{
					app.LayoutDirection = LayoutDirection.Inherit;
				}
				Assert.That(app.LayoutDirection, Is.EqualTo(initial), "Setting Inherit should restore the platform default");
				Assert.That(child.EffectiveLayoutDirection, Is.EqualTo(initial));
				Assert.That(changed, Is.EqualTo(2));
			});
		}

		[Test]
		public void TableLayoutShouldMirror() => AssertMirrored(p => new TableLayout
		{
			Padding = new Padding(10, 5, 30, 5),
			Spacing = new Size(7, 7),
			Rows = { new TableRow(p[0], p[1], p[2], null), new TableRow(null, p[3]) }
		}, 4);

		[Test]
		public void HorizontalStackLayoutShouldMirror() => AssertMirrored(p => new StackLayout
		{
			Orientation = Orientation.Horizontal,
			Spacing = 5,
			Padding = new Padding(15, 0, 0, 0),
			Items = { p[0], p[1] }
		}, 2);

		[Test]
		public void VerticalStackLayoutAlignmentShouldMirror() => AssertMirrored(p => new StackLayout
		{
			HorizontalContentAlignment = HorizontalAlignment.Left,
			Items = { p[0], new StackLayoutItem(p[1], HorizontalAlignment.Right) }
		}, 2);

		[Test]
		public void DynamicLayoutShouldMirror() => AssertMirrored(p =>
		{
			var layout = new DynamicLayout { Spacing = new Size(5, 5) };
			layout.BeginHorizontal();
			layout.Add(p[0]);
			layout.Add(p[1]);
			layout.Add(null);
			layout.EndHorizontal();
			return layout;
		}, 2);

		[Test]
		public void PixelLayoutShouldMirror() => AssertMirrored(p =>
		{
			var layout = new PixelLayout();
			layout.Add(p[0], 10, 10);
			layout.Add(p[1], 60, 40);
			return layout;
		}, 2);

		[Test]
		public void PixelLayoutMoveShouldMirror() => AssertMirrored(p =>
		{
			var layout = new PixelLayout();
			layout.Add(p[0], 10, 10);
			layout.Add(p[1], 0, 0);
			layout.LoadComplete += (sender, e) => layout.Move(p[1], 100, 50);
			return layout;
		}, 2);

		[Test]
		public void PanelPaddingShouldMirror() => AssertMirrored(p => new Panel
		{
			Padding = new Padding(25, 5, 5, 5),
			Content = new TableLayout(new TableRow(p[0], null), null)
		}, 1);

		[Test]
		public void SplitterShouldMirror() => AssertMirrored(p => new Splitter
		{
			Orientation = Orientation.Horizontal,
			FixedPanel = SplitterFixedPanel.Panel1,
			Position = 80,
			Panel1 = new TableLayout(new TableRow(p[0], null), null),
			Panel2 = new TableLayout(new TableRow(p[1], null), null)
		}, 2, (ltr, rtl) =>
		{
			Assert.That(((Splitter)rtl).Position, Is.EqualTo(((Splitter)ltr).Position), "Position should be the size of Panel1 in both directions");
		});

		[Test]
		public void GroupBoxContentShouldMirror() => AssertMirrored(p => new GroupBox
		{
			Text = "Group",
			Padding = new Padding(20, 5, 5, 5),
			Content = new TableLayout(new TableRow(p[0], null), null)
		}, 1);

		[Test]
		public void TabControlContentShouldMirror() => AssertMirrored(p => new TabControl
		{
			Pages = { new TabPage { Text = "Page", Padding = new Padding(20, 5, 5, 5), Content = new TableLayout(new TableRow(p[0], null), null) } }
		}, 1);

		[Test]
		public void ScrollableContentShouldStartAtLeadingEdge() => AssertMirrored(p => new Scrollable
		{
			ExpandContentWidth = false,
			Border = BorderType.None,
			Content = p[0]
		}, 1);

		[Test]
		public void LeftToRightOverrideInsideRightToLeftShouldNotMirror()
		{
			Panel host = null;
			Control first = null, second = null;
			Shown(form =>
			{
				first = Probe();
				second = Probe();
				var inner = new TableLayout { LayoutDirection = LayoutDirection.LeftToRight, Rows = { new TableRow(first, second, null) } };
				host = new Panel { LayoutDirection = LayoutDirection.RightToLeft, Size = new Size(300, 50), Content = inner };
				form.Content = host;
			}, () =>
			{
				var firstBounds = BoundsIn(first, host);
				var secondBounds = BoundsIn(second, host);
				Assert.That(firstBounds.X, Is.EqualTo(0).Within(1), "First cell should be on the left");
				Assert.That(secondBounds.X, Is.GreaterThan(firstBounds.X));
			});
		}

		[Test]
		public void LocationShouldBePhysical()
		{
			PixelLayout layout = null;
			Control probe = null;
			Shown(form =>
			{
				probe = Probe();
				layout = new PixelLayout { LayoutDirection = LayoutDirection.RightToLeft, Size = new Size(300, 100) };
				layout.Add(probe, 10, 10);
				form.Content = layout;
			}, () =>
			{
				var bounds = BoundsIn(probe, layout);
				Assert.That(bounds.X, Is.EqualTo(layout.Width - 10 - probe.Width).Within(1), "PixelLayout X is measured from the right in right-to-left");
				Assert.That(probe.Location.X, Is.EqualTo(bounds.X).Within(1), "Location is measured from the left");
				Assert.That(probe.Bounds.X, Is.EqualTo(bounds.X).Within(1), "Bounds is measured from the left");
			});
		}

		[Test]
		public void DrawableMouseLocationShouldBePhysical()
		{
			Drawable drawable = null;
			Shown(form =>
			{
				drawable = new Drawable { Size = new Size(200, 100), LayoutDirection = LayoutDirection.RightToLeft };
				form.Content = new TableLayout(new TableRow(drawable, null), null);
			}, () =>
			{
				// a point near the left edge of the drawable should map back to the left in both directions
				var screen = drawable.PointToScreen(new PointF(10, 10));
				var leftEdge = drawable.PointToScreen(PointF.Empty);
				var rightEdge = drawable.PointToScreen(new PointF(drawable.Width, 0));
				Assert.That(screen.X - leftEdge.X, Is.EqualTo(10).Within(1));
				Assert.That(rightEdge.X, Is.GreaterThan(leftEdge.X), "x should increase to the right");
				Assert.That(drawable.PointFromScreen(screen).X, Is.EqualTo(10).Within(1));
			});
		}

		static IEnumerable<object> TextAlignmentControls()
		{
			yield return new TestCaseData((Func<Control>)(() => new TextBox())).SetName("TextBox");
			yield return new TestCaseData((Func<Control>)(() => new Label { Text = "Label" })).SetName("Label");
			yield return new TestCaseData((Func<Control>)(() => new TextArea())).SetName("TextArea");
			yield return new TestCaseData((Func<Control>)(() => new NumericStepper())).SetName("NumericStepper");
		}

		[TestCaseSource(nameof(TextAlignmentControls))]
		public void TextAlignmentShouldRoundTripInRightToLeft(Func<Control> create)
		{
			Control control = null;
			Shown(form =>
			{
				control = create();
				SetTextAlignment(control, TextAlignment.Left);
				Assert.That(GetTextAlignment(control), Is.EqualTo(TextAlignment.Left), "Before load");
				form.Content = new Panel { LayoutDirection = LayoutDirection.RightToLeft, Content = control };
			}, () =>
			{
				Assert.That(GetTextAlignment(control), Is.EqualTo(TextAlignment.Left), "After load");
				SetTextAlignment(control, TextAlignment.Right);
				Assert.That(GetTextAlignment(control), Is.EqualTo(TextAlignment.Right), "After setting while loaded");
			});
		}

		static void SetTextAlignment(Control control, TextAlignment alignment) => control.GetType().GetProperty(nameof(TextBox.TextAlignment)).SetValue(control, alignment);
		static TextAlignment GetTextAlignment(Control control) => (TextAlignment)control.GetType().GetProperty(nameof(TextBox.TextAlignment)).GetValue(control);

		static readonly int[] ColumnsLeftToRight = { 0, 1, 2 };
		static readonly int[] ColumnsRightToLeft = { 2, 1, 0 };

		static readonly Size GridHostSize = new Size(300, 150);

		static Grid CreateGrid(bool tree)
		{
			Grid grid;
			if (tree)
			{
				var items = new TreeGridItemCollection();
				for (int i = 0; i < 30; i++)
					items.Add(new TreeGridItem("A", "B", "C"));
				grid = new TreeGridView { DataStore = items };
			}
			else
				grid = new GridView { DataStore = Enumerable.Range(0, 30).Select(i => new object[] { "A", "B", "C" }).ToList() };
			for (int i = 0; i < 3; i++)
				grid.Columns.Add(new GridColumn { HeaderText = $"Column {i}", Width = 60, AutoSize = false, DataCell = new TextBoxCell(i) });
			return grid;
		}

		/// <summary>
		/// Hit-tests across a data row to find the order the columns appear in, from left to right.
		/// </summary>
		static int[] ColumnOrder(Grid grid)
		{
			var order = new List<int>();
			var y = grid.Height - 20;
			for (int x = 0; x < grid.Width; x += 4)
			{
				var location = new PointF(x, y);
				var column = grid is TreeGridView tree ? tree.GetCellAt(location)?.Column : ((GridView)grid).GetCellAt(location)?.Column;
				if (column == null)
					continue;
				var index = grid.Columns.IndexOf(column);
				if (order.Count == 0 || order[order.Count - 1] != index)
					order.Add(index);
			}
			return order.ToArray();
		}

		/// <summary>
		/// Waits for the grid to lay out its columns (GTK does so after the window is shown), then checks their order.
		/// </summary>
		static async Task AssertColumnOrder(Grid grid, LayoutDirection direction, string message)
		{
			var expected = direction == LayoutDirection.RightToLeft ? ColumnsRightToLeft : ColumnsLeftToRight;
			var order = ColumnOrder(grid);
			for (int i = 0; i < 20 && !order.SequenceEqual(expected); i++)
			{
				await Task.Delay(100);
				order = ColumnOrder(grid);
			}
			Assert.That(order, Is.EqualTo(expected), message);
		}

		[TestCase(false)]
		[TestCase(true)]
		public void GridColumnsShouldBeReversedInRightToLeft(bool tree)
		{
			Grid ltrGrid = null, rtlGrid = null;
			ShownAsync(form =>
			{
				ltrGrid = CreateGrid(tree);
				rtlGrid = CreateGrid(tree);
				form.Content = new TableLayout(
					new Panel { LayoutDirection = LayoutDirection.LeftToRight, Size = GridHostSize, Content = ltrGrid },
					new Panel { LayoutDirection = LayoutDirection.RightToLeft, Size = GridHostSize, Content = rtlGrid }
				);
			}, async () =>
			{
				await AssertColumnOrder(ltrGrid, LayoutDirection.LeftToRight, "Left-to-right");
				await AssertColumnOrder(rtlGrid, LayoutDirection.RightToLeft, "Right-to-left");
			});
		}

		[TestCase(false)]
		[TestCase(true)]
		public void GridColumnsShouldReverseWhenInheritedDirectionChanges(bool tree)
		{
			Panel host = null;
			Grid grid = null;
			ShownAsync(form =>
			{
				grid = CreateGrid(tree);
				host = new Panel { LayoutDirection = LayoutDirection.LeftToRight, Size = GridHostSize, Content = grid };
				// WinForms doesn't size the window to a sized panel that is its direct content
				form.Content = new TableLayout(host);
			}, async () =>
			{
				await AssertColumnOrder(grid, LayoutDirection.LeftToRight, "Initial");

				host.LayoutDirection = LayoutDirection.RightToLeft;
				await AssertColumnOrder(grid, LayoutDirection.RightToLeft, "After changing the parent to right-to-left");

				host.LayoutDirection = LayoutDirection.LeftToRight;
				await AssertColumnOrder(grid, LayoutDirection.LeftToRight, "After changing the parent back to left-to-right");

				var app = Application.Instance;
				var initial = app.LayoutDirection;
				var other = initial == LayoutDirection.RightToLeft ? LayoutDirection.LeftToRight : LayoutDirection.RightToLeft;
				host.LayoutDirection = LayoutDirection.Inherit;
				try
				{
					app.LayoutDirection = other;
					await AssertColumnOrder(grid, other, "After changing the application direction");
				}
				finally
				{
					app.LayoutDirection = LayoutDirection.Inherit;
				}
				await AssertColumnOrder(grid, initial, "After restoring the application direction");
			});
		}
	}
}
