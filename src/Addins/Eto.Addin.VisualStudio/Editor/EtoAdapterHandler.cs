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
using System.Windows.Threading;
using Eto;
using Eto.Addin.VisualStudio.Editor;

[assembly: ExportHandler(typeof(IEtoAdapterHandler), typeof(EtoAdapterHandler))]

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
			Control = control;
		}

		public override Drawing.Color BackgroundColor
		{
			get { return Drawing.Colors.Transparent; }
			set
			{
			}
		}
	}

	public class EtoAdapterHandler : IEtoAdapterHandler
	{
		public object ToContract(Control control)
		{
			var content = DesignPanel.GetContent(control);
			var view = content.ToNative(true);
			return FrameworkElementAdapters.ViewToContractAdapter(view);
		}

		public Control FromContract(object contract)
		{
			var view = FrameworkElementAdapters.ContractToViewAdapter((INativeHandleContract)contract);
			view.Focusable = false; // otherwise editor loses focus when switching back to its tab.. may be a better way around this.
			return new Control(new NativeControl(view));
		}

		public void Unload()
		{
			Dispatcher.CurrentDispatcher?.InvokeShutdown();
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}

