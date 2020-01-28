using System;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Eto.Designer
{
	public class PreviewEditorView : Panel
	{
		IDesignHost designPanel;
		Panel errorPanel;
		UITimer timer;
		UITimer errorTimer;
		int processingCount;
		Func<string> getCode;
		Control errorContent;
		Panel designPanelHolder;

		static double lastPosition = 0.4;

		public double RefreshTime { get; set; } = 0.5;

		public double ErrorDisplayTime { get; set; } = 2.0;

		public string MainAssemblyPath { get; set; }

		public static bool EnableAppDomains { get; set; } = Platform.Instance.Supports<IEtoAdapterHandler>();

		public PreviewEditorView(string mainAssembly, IEnumerable<string> references, Func<string> getCode)
		{
			//Size = new Size (200, 200);
			MainAssemblyPath = mainAssembly;
			this.getCode = getCode;

			if (EnableAppDomains)
				designPanel = new AppDomainDesignHost();
			else
				designPanel = new InProcessDesignPanel();

			designPanel.MainAssembly = mainAssembly;
			designPanel.References = references;

			designPanel.ControlCreating = () => FinishProcessing(null);
			designPanel.Error = FinishProcessing;

			designPanelHolder = new Panel();
			designPanelHolder.Content = designPanel.GetContainer();

			designPanel.ContainerChanged = () => designPanelHolder.Content = designPanel.GetContainer();

			errorPanel = new Panel { Padding = new Padding(5), Visible = false, BackgroundColor = new Color(Colors.Red, .4f) };

			Content = new StackLayout
			{
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Items =
				{
					new StackLayoutItem(designPanelHolder, expand: true),
					errorPanel
				}
			};

			timer = new UITimer { Interval = RefreshTime };
			timer.Elapsed += Timer_Elapsed;

			errorTimer = new UITimer { Interval = ErrorDisplayTime };
			errorTimer.Elapsed += ErrorTimer_Elapsed;
		}

		void ErrorTimer_Elapsed(object sender, EventArgs e)
		{
			errorTimer.Stop();
			if (errorContent != null)
			{
				errorPanel.Content = errorContent;
				errorPanel.Visible = true;
				errorContent = null;
			}
		}

		void Timer_Elapsed(object sender, EventArgs e)
		{
			timer.Stop();
			processingCount = 1; // only check for additional changes AFTER this point to restart the timer
			var code = getCode();
			if (!string.IsNullOrEmpty(code))
			{
				designPanel?.Update(code);
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			designPanel?.Invalidate();
		}

		void FinishProcessing(DesignError error)
		{
			if (error != null)
			{
				var errorLabel = new Label { Text = error.Message, ToolTip = error.Details.ToString() };
				if (errorPanel.Visible)
				{
					errorPanel.Content = errorLabel;
				}
				else
				{
					errorContent = errorLabel;
					errorTimer.Start();
				}
			}
			else
			{
				errorPanel.Visible = false;
				errorContent = null;
			}

			if (processingCount > 1)
			{
				// process was requested while we were processing the last one, so redo
				processingCount = 1;
				timer.Start();
			}
			else
				processingCount = 0;
		}

		void Stop()
		{
			timer.Stop();
		}

		/// <summary>
		/// Call to update the preview with the current code
		/// </summary>
		public void Update()
		{
			processingCount++;
			// only start if we aren't already compiling the UI
			if (processingCount == 1)
				timer.Start();
		}

		/// <summary>
		/// Call to set the builder based on the file name
		/// </summary>
		/// <param name="fileName"></param>
		public bool SetBuilder(string fileName)
		{
			Stop();
			return designPanel.SetBuilder(fileName);
		}

		public string GetCodeFile(string fileName)
		{
			return designPanel.GetCodeFile(fileName);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				designPanelHolder.Content = null;
				designPanel.Dispose();
				designPanel = null;
			}
			base.Dispose(disposing);
		}
	}


	public class PreviewEditorViewSplitter : Splitter
	{
		IDesignHost designPanel;
		Panel errorPanel;
		UITimer timer;
		UITimer errorTimer;
		int processingCount;
		Func<string> getCode;
		Control errorContent;
		Panel designPanelHolder;

		static double lastPosition = 0.4;

		public Control Editor { get; }

		public double RefreshTime { get; set; } = 0.5;

		public double ErrorDisplayTime { get; set; } = 2.0;

		public string MainAssemblyPath { get; set; }

		public static bool EnableAppDomains { get; set; } = Platform.Instance.Supports<IEtoAdapterHandler>();

		public PreviewEditorViewSplitter(Control editor, string mainAssembly, IEnumerable<string> references, Func<string> getCode)
		{
			//Size = new Size (200, 200);
			Editor = editor;
			MainAssemblyPath = mainAssembly;
			this.getCode = getCode;

			if (EnableAppDomains)
				designPanel = new AppDomainDesignHost();
			else
				designPanel = new InProcessDesignPanel();

			designPanel.MainAssembly = mainAssembly;
			designPanel.References = references;

			designPanel.ControlCreated = () => FinishProcessing(null);
			designPanel.Error = FinishProcessing;

			designPanelHolder = new Panel();
			designPanelHolder.Content = designPanel.GetContainer();

			designPanel.ContainerChanged = () => designPanelHolder.Content = designPanel.GetContainer();

			Orientation = Orientation.Vertical;
			FixedPanel = SplitterFixedPanel.None;

			errorPanel = new Panel { Padding = new Padding(5), Visible = false, BackgroundColor = new Color(Colors.Red, .4f) };

			Panel1 = new StackLayout
			{
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Items =
				{
					new StackLayoutItem(designPanelHolder, expand: true),
					errorPanel
				}
			};
			Panel2 = editor;
			RelativePosition = lastPosition;

			timer = new UITimer { Interval = RefreshTime };
			timer.Elapsed += Timer_Elapsed;

			errorTimer = new UITimer { Interval = ErrorDisplayTime };
			errorTimer.Elapsed += ErrorTimer_Elapsed;
		}

		void ErrorTimer_Elapsed(object sender, EventArgs e)
		{
			errorTimer.Stop();
			if (errorContent != null)
			{
				errorPanel.Content = errorContent;
				errorPanel.Visible = true;
				errorContent = null;
			}
		}

		void Timer_Elapsed(object sender, EventArgs e)
		{
			timer.Stop ();
			processingCount = 1; // only check for additional changes AFTER this point to restart the timer
			var code = getCode();
			if (!string.IsNullOrEmpty(code))
			{
				designPanel?.Update(code);
			}
		}

		protected override void OnPositionChanged(EventArgs e)
		{
			base.OnPositionChanged(e);
			lastPosition = RelativePosition;
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			designPanel?.Invalidate();
		}

		void FinishProcessing(DesignError error)
		{
			if (error != null)
			{
				var errorLabel = new Label { Text = error.Message, ToolTip = error.Details.ToString() };
				if (errorPanel.Visible)
				{
					errorPanel.Content = errorLabel;
				}
				else
				{
					errorContent = errorLabel;
					errorTimer.Start();
				}
			}
			else
			{
				errorPanel.Visible = false;
				errorContent = null;
			}

			if (processingCount > 1)
			{
				// process was requested while we were processing the last one, so redo
				processingCount = 1;
				timer.Start();
			}
			else
				processingCount = 0;
		}

		void Stop()
		{
			timer.Stop();
		}

		/// <summary>
		/// Call to update the preview with the current code
		/// </summary>
		public void Update()
		{
			processingCount++;
			// only start if we aren't already compiling the UI
			if (processingCount == 1)
				timer.Start();
		}

		/// <summary>
		/// Call to set the builder based on the file name
		/// </summary>
		/// <param name="fileName"></param>
		public bool SetBuilder(string fileName)
		{
			Stop();
			return designPanel.SetBuilder(fileName);
		}

		public string GetCodeFile(string fileName)
		{
			return designPanel.GetCodeFile(fileName);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				designPanelHolder.Content = null;
				designPanel.Dispose();
				designPanel = null;
			}
			base.Dispose(disposing);
		}
	}
}

