using System;
using Eto.Forms;
using Eto.Designer;
using Eto.Wpf.Forms;
using System.Windows;
using System.AddIn.Pipeline;
using System.AddIn.Contract;
using System.Collections.Generic;
using Eto.Wpf.Forms.Controls;
using sw = System.Windows;
using swc = System.Windows.Controls;

[assembly: PlatformInitializerAttribute(typeof(Eto.Addin.VisualStudio.Editor.PlatformInitializer))]

namespace Eto.Addin.VisualStudio.Editor
{
	class ControlContract : MarshalByRefObject
	{
		public IntPtr Handle { get; set; }
	}

	class NativeControl : WpfFrameworkElement<FrameworkElement, Control, Control.ICallback>
	{
		public NativeControl(FrameworkElement control)
		{
			this.Control = control;
		}

		public override Drawing.Color BackgroundColor
		{
			get { return Drawing.Colors.Transparent; }
			set
			{
			}
		}
	}

	public class PlatformInitializer : IPlatformInitializer
	{
		public void Initialize(Platform platform)
		{
			platform.Add<IEtoAdapterHandler>(() => new EtoAdapterHandler());
		}
	}

	public class EtoAdapterHandler : IEtoAdapterHandler
	{
		public object ToContract(Control control)
		{
			var view = control.ToNative(true);

			var holder = new swc.ScrollViewer
			{
				HorizontalScrollBarVisibility = swc.ScrollBarVisibility.Disabled,
				VerticalScrollBarVisibility = swc.ScrollBarVisibility.Disabled,
				Content = view
			};

			return FrameworkElementAdapters.ViewToContractAdapter(holder);
		}

		public Control FromContract(object contract)
		{
			var view = FrameworkElementAdapters.ContractToViewAdapter((INativeHandleContract)contract);
			return new Control(new NativeControl(view));
		}
	}
}

