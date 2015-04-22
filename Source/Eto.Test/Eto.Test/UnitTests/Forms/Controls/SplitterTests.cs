using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;
using System;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture, Category("ui"), Category(TestUtils.NoTestPlatformCategory)]
	public class SplitterTests
	{
		// currently working only for WinForms
		bool ReplayTests { get { return Platform.Instance.IsWinForms; } }

		[Test]
		public void PositionShouldNotChange()
		{
			bool replay = false;
			Action<SplitterOrientation, SplitterFixedPanel>
				test = (orient, fix) =>
			{
				TestUtils.Shown(form =>
				{
					return new Splitter()
					{
						Size = new Size(150, 150),
						Orientation  = orient,
						FixedPanel	 = fix,
						Position	 = 50,
						Panel1 = new Panel
						{
							BackgroundColor = Colors.White
						},
						Panel2 = new Panel
						{
							BackgroundColor = Colors.Black
						}
					};
				},
				it =>
				{
					Assert.AreEqual(50, it.Position, "Fix: {0}; {1} [replay={2}]", fix, orient, replay);
					if (ReplayTests) replay = !replay;
				},
				replay: ReplayTests);
			};
			foreach (SplitterOrientation o in Enum.GetValues(typeof(SplitterOrientation)))
				foreach (SplitterFixedPanel p in Enum.GetValues(typeof(SplitterFixedPanel)))
					test(o, p);
		}

		[Test]
		public void RelativePositionShouldNotChange()
		{
			bool replay = false;
			Action<SplitterOrientation, SplitterFixedPanel>
				test = (orient, fix) =>
			{
				double pos = fix == SplitterFixedPanel.None ? (1/3.0) : 50;
				TestUtils.Shown(form =>
				{
					return new Splitter()
					{
						Size = new Size(150, 150),
						Orientation  = orient,
						FixedPanel	 = fix,
						RelativePosition = pos,
						Panel1 = new Panel
						{
							BackgroundColor = Colors.White
						},
						Panel2 = new Panel
						{
							BackgroundColor = Colors.Black
						}
					};
				},
				it =>
				{
					Assert.AreEqual(pos, it.RelativePosition, 1e-6, "Fix: {0}; {1} [replay={2}]", fix, orient, replay);
					if (ReplayTests) replay = !replay;
				},
				replay: ReplayTests);
			};
			foreach (SplitterOrientation o in Enum.GetValues(typeof(SplitterOrientation)))
				foreach (SplitterFixedPanel p in Enum.GetValues(typeof(SplitterFixedPanel)))
					test(o, p);
		}

		[Test]
		public void NoPositionShouldAutoSizeBasic()
		{
			bool replay = false;
			Action<SplitterOrientation, SplitterFixedPanel>
				test = (orient, fix) =>
			{
				var sz = new Size(50, 50);
				TestUtils.Shown(form =>
				{
					return new Splitter
					{
						Orientation  = orient,
						FixedPanel	 = fix,
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
					};
				},
				it =>
				{
					if (orient == SplitterOrientation.Horizontal)
						Assert.AreEqual(it.Panel1.Height, it.Panel2.Height,
							"Height! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
					else
						Assert.AreEqual(it.Panel1.Width, it.Panel2.Width,
							"Width! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
					switch (fix)
					{
						case SplitterFixedPanel.Panel1:
							if (orient == SplitterOrientation.Horizontal)
								Assert.AreEqual(sz.Width, it.Panel1.Width,
									"P1.Width! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
							else
								Assert.AreEqual(sz.Height, it.Panel1.Height,
									"P1.Height! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
							break;
						case SplitterFixedPanel.Panel2:
							if (orient == SplitterOrientation.Horizontal)
								Assert.AreEqual(sz.Width, it.Panel2.Width,
									"P2.Width! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
							else
								Assert.AreEqual(sz.Height, it.Panel2.Height,
									"P2.Height! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
							break;
						case SplitterFixedPanel.None:
							if (orient == SplitterOrientation.Horizontal)
								Assert.AreEqual(it.Panel1.Width, it.Panel2.Width, 1,
									"Width! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
							else
								Assert.AreEqual(it.Panel1.Height, it.Panel2.Height, 1,
									"Height! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
							break;
					}
					if (ReplayTests) replay = !replay;
				},
				replay: ReplayTests);
			};
			foreach (SplitterOrientation o in Enum.GetValues(typeof(SplitterOrientation)))
				foreach (SplitterFixedPanel	p in Enum.GetValues(typeof(SplitterFixedPanel)))
					test(o, p);
		}

		[Test]
		public void NoPositionShouldAutoSizeComplex()
		{
			Action<SplitterOrientation, SplitterFixedPanel, int>
				test = (orient, fix, testN) =>
			{
				// +====test 1====+ +====test 2====+
				// |        |     | |              |
				// | tested |     | |-----+--------|  Tested splitter is placed inside two other splitters
				// |        |     | |     |        |  to overcome minimal window/form size problems
				// +--------+     | |     | tested |  ...or non-desktop plaforms
				// |        |     | |     |        |
				// +--------+-----+ +-----+--------+
				TestUtils.Shown(form =>
				{
					var it = new Splitter
					{
						Orientation  = orient,
						FixedPanel	 = fix,
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
					form.Content = testN == 1 ? new Splitter
					{
						Panel1 = new Splitter
						{
							Orientation = SplitterOrientation.Vertical,
							Panel1 = it,
							Panel2 = new Panel()
						},
						Panel2 = new Panel()
					}
					: new Splitter
					{
						Orientation = SplitterOrientation.Vertical,
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
					Assert.AreEqual(40, it.Position, "{0}; {1}", fix, orient);
					Assert.AreEqual(fix == SplitterFixedPanel.Panel1 ? 40
						: fix == SplitterFixedPanel.Panel2 ? 60 : 0.4,
						it.RelativePosition, "{0}; {1}", fix, orient);
					var sz = orient == SplitterOrientation.Horizontal
						? new Size(100 + it.SplitterWidth, 60)
						: new Size(60, 100 + it.SplitterWidth);
					Assert.AreEqual(sz, it.Size, "{0}; {1}", fix, orient);
				},
				replay: ReplayTests);
			};
			for (int testN = 1; testN < 2; testN++)
				foreach (SplitterOrientation o in Enum.GetValues(typeof(SplitterOrientation)))
					foreach (SplitterFixedPanel	p in Enum.GetValues(typeof(SplitterFixedPanel)))
						test(o, p, testN);
		}

		[Test] // Issue #309
		public void PositionShouldTrackInitialResize()
		{
			bool replay = false;
			Action<SplitterOrientation, SplitterFixedPanel>
				test = (orient, fix) =>
				{
					TestUtils.Shown(form =>
					{
						var it = new Splitter() {
							Orientation = orient,
							FixedPanel	= fix,
							Position	= 50,
							Panel1 = new Panel {
								BackgroundColor = Colors.White
							},
							Panel2 = new Panel {
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
						Assert.AreEqual(pos, it.RelativePosition, 1e-6, "Fix: {0}; {1} [replay={2}]", fix, orient, replay);
						if (ReplayTests)
							replay = !replay;
					},
					replay: ReplayTests);
				};
			foreach (SplitterOrientation o in Enum.GetValues(typeof(SplitterOrientation)))
				foreach (SplitterFixedPanel p in Enum.GetValues(typeof(SplitterFixedPanel)))
					test(o, p);
		}
	}
}
