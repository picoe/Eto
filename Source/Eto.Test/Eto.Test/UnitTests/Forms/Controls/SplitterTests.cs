using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;
using System;
using System.Collections;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture, Category("ui")]
	public class SplitterTests
	{
		// currently working only for WinForms
		bool ReplayTests { get { return true; } }

		static IEnumerable SplitterCases
		{
			get
			{
				foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
					foreach (SplitterFixedPanel fix in Enum.GetValues(typeof(SplitterFixedPanel)))
						yield return new TestCaseData(orientation, fix);
			}
		}

		[Test, TestCaseSource("SplitterCases")]
		public void PositionShouldNotChange(Orientation orient, SplitterFixedPanel fix)
		{
			bool replay = false;
			TestUtils.Shown(
				form => new Splitter()
				{
					Size = new Size(150, 150),
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
					Assert.AreEqual(50, it.Position, "Fix: {0}; {1} [replay={2}]", fix, orient, replay);
					if (ReplayTests)
						replay = !replay;
				}, replay: ReplayTests);
		}

		[Test, TestCaseSource("SplitterCases")]
		public void RelativePositionShouldNotChange(Orientation orient, SplitterFixedPanel fix)
		{
			bool replay = false;
			double pos = fix == SplitterFixedPanel.None ? (1 / 3.0) : 50;
			TestUtils.Shown(
				form => new Splitter()
				{
					Size = new Size(150, 150),
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
					Assert.AreEqual(pos, it.RelativePosition, 1e-2, "Fix: {0}; {1} [replay={2}]", fix, orient, replay);
					if (ReplayTests)
						replay = !replay;
				},
				replay: ReplayTests);
		}

		[Test, TestCaseSource("SplitterCases")]
		public void NoPositionShouldAutoSizeBasic(Orientation orient, SplitterFixedPanel fix)
		{
			bool replay = false;
			var sz = new Size(50, 50);
			TestUtils.Shown(
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
						Assert.AreEqual(it.Panel1.Height, it.Panel2.Height,
							"Height! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
					else
						Assert.AreEqual(it.Panel1.Width, it.Panel2.Width,
							"Width! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
					switch (fix)
					{
						case SplitterFixedPanel.Panel1:
							if (orient == Orientation.Horizontal)
								Assert.AreEqual(sz.Width, it.Panel1.Width,
									"P1.Width! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
							else
								Assert.AreEqual(sz.Height, it.Panel1.Height,
									"P1.Height! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
							break;
						case SplitterFixedPanel.Panel2:
							if (orient == Orientation.Horizontal)
								Assert.AreEqual(sz.Width, it.Panel2.Width,
									"P2.Width! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
							else
								Assert.AreEqual(sz.Height, it.Panel2.Height,
									"P2.Height! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
							break;
						case SplitterFixedPanel.None:
							if (orient == Orientation.Horizontal)
								Assert.AreEqual(it.Panel1.Width, it.Panel2.Width, 1,
									"Width! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
							else
								Assert.AreEqual(it.Panel1.Height, it.Panel2.Height, 1,
									"Height! Fix: {0}; {1} [replay={2}]", fix, orient, replay);
							break;
					}
					if (ReplayTests)
						replay = !replay;
				},
				replay: ReplayTests);
		}

		[Test, TestCaseSource("SplitterCases")]
		public void NoPositionShouldAutoSizeComplexTest1(Orientation orient, SplitterFixedPanel fix)
		{
			TestUtils.Shown(
				form =>
				{
					// +====test 1====+ 
					// |        |     | 
					// | tested |     |   Tested splitter is placed inside two other splitters
					// |        |     |   to overcome minimal window/form size problems
					// +--------+     |   ...or non-desktop plaforms
					// |        |     | 
					// +--------+-----+ 
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
						Panel1 = new Splitter
						{
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
					Assert.AreEqual(40, it.Position, "{0}; {1}", fix, orient);
					Assert.AreEqual(fix == SplitterFixedPanel.Panel1 ? 40 : fix == SplitterFixedPanel.Panel2 ? 60 : 0.4, it.RelativePosition, "{0}; {1}", fix, orient);
					var sz = orient == Orientation.Horizontal ? new Size(100 + it.SplitterWidth, 60) : new Size(60, 100 + it.SplitterWidth);
					Assert.AreEqual(sz, it.Size, "{0}; {1}", fix, orient);
				}, replay: ReplayTests);
		}

		[Test, TestCaseSource("SplitterCases")]
		public void NoPositionShouldAutoSizeComplexTest2(Orientation orient, SplitterFixedPanel fix)
		{
			TestUtils.Shown(
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
					Assert.AreEqual(40, it.Position, "{0}; {1}", fix, orient);
					Assert.AreEqual(fix == SplitterFixedPanel.Panel1 ? 40 : fix == SplitterFixedPanel.Panel2 ? 60 : 0.4, it.RelativePosition, "{0}; {1}", fix, orient);
					var sz = orient == Orientation.Horizontal ? new Size(100 + it.SplitterWidth, 60) : new Size(60, 100 + it.SplitterWidth);
					Assert.AreEqual(sz, it.Size, "{0}; {1}", fix, orient);
				}, replay: ReplayTests);
		}

		[Test, TestCaseSource("SplitterCases")]
		// Issue #309
		public void PositionShouldTrackInitialResize(Orientation orient, SplitterFixedPanel fix)
		{
			bool replay = false;
			TestUtils.Shown(
				form =>
				{
					var it = new Splitter()
					{
						Orientation = orient,
						FixedPanel	= fix,
						Position	= 50,
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
					Assert.AreEqual(pos, it.RelativePosition, 1e-2, "Fix: {0}; {1} [replay={2}]", fix, orient, replay);
					if (ReplayTests)
						replay = !replay;
				},
				replay: ReplayTests);
		}
	}
}
