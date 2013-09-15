using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms
{
	public abstract class WpfDockContainer<T, W> : WpfContainer<T, W>, IDockContainer
		where T : sw.FrameworkElement
		where W : DockContainer
	{
		Control content;
		swc.Border border;
		Size? clientSize;

		protected virtual bool UseContentSize { get { return true; } }

		public override Size ClientSize
		{
			get
			{
				if (!Control.IsLoaded && clientSize != null) return clientSize.Value;
				else return Conversions.GetSize(border);
			}
			set
			{
				clientSize = value;
				Conversions.SetSize(border, value);
			}
		}

		public override sw.Size GetPreferredSize(sw.Size? constraint)
		{
			var size = constraint ?? new sw.Size(double.PositiveInfinity, double.PositiveInfinity);
			size = new sw.Size(Math.Max(0, size.Width - Padding.Horizontal), Math.Max(0, size.Height - Padding.Vertical));
			var baseSize = base.GetPreferredSize(size);
			if (UseContentSize)
			{
				var preferredSize = content.GetPreferredSize(size);
				baseSize = new sw.Size(Math.Max(baseSize.Width, preferredSize.Width + Padding.Horizontal), Math.Max(baseSize.Height, preferredSize.Height + Padding.Vertical));
			}
			return new sw.Size(Math.Max(0, baseSize.Width), Math.Max(0, baseSize.Height));
		}

		public WpfDockContainer()
		{
			border = new swc.Border();
			border.SizeChanged += (sender, e) =>
			{
				var element = content.GetContainerControl();
				if (element != null)
				{
					if (!double.IsNaN(element.Width))
						element.Width = Math.Max(0, e.NewSize.Width - Padding.Horizontal);
					if (!double.IsNaN(element.Height))
						element.Height = Math.Max(0, e.NewSize.Height - Padding.Vertical);
				}
			};
		}

		protected override void Initialize()
		{
			base.Initialize();
			SetContainerContent(border);
		}

		public Padding Padding
		{
			get { return border.Padding.ToEto(); }
			set { border.Padding = value.ToWpf(); }
		}

		public Control Content
		{
			get { return content; }
			set
			{
				content = value;
				if (content != null)
				{
					var element = content.GetContainerControl();
					element.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
					element.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
					border.Child = element;
				}
				else
					border.Child = null;
				SetContent();
			}
		}

		public abstract void SetContainerContent(sw.FrameworkElement content);

		public virtual void SetContent()
		{
		}

		public override void Remove(sw.FrameworkElement child)
		{
			if (border.Child == child)
			{
				content = null;
				border.Child = null;
			}
		}
	}
}
