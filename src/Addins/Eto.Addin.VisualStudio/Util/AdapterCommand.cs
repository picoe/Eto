using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Eto.Addin.VisualStudio.Util
{
	class AdapterCommand : IOleCommandTarget
	{
		IOleCommandTarget nextCommandTarget;
		Guid menuGroup;
		uint cmdID;
		Action commandEvent;
		Func<bool> queryEvent;
		System.IServiceProvider provider;

		public AdapterCommand(IVsTextView adapter, System.IServiceProvider provider, Guid menuGroup, uint cmdID, Action commandEvent, Func<bool> queryEvent = null)
		{
			this.provider = provider;
			this.menuGroup = menuGroup;
			this.cmdID = cmdID;
			this.commandEvent = commandEvent;
			this.queryEvent = queryEvent ?? (() => true);

			var mcs = provider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if (mcs != null)
				mcs.AddCommand(menuGroup, (int)cmdID, commandEvent, cmd => {
					cmd.Visible = cmd.Enabled = this.queryEvent();
				});

			Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
			{
				ErrorHandler.ThrowOnFailure(adapter.AddCommandFilter(this, out nextCommandTarget));
			}), DispatcherPriority.ApplicationIdle, null);
		}

		public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
		{
			if (pguidCmdGroup == menuGroup && nCmdID == cmdID)
			{
				commandEvent();
				return VSConstants.S_OK;
			}
			return nextCommandTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
		}

		public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
		{
			if (pguidCmdGroup == menuGroup)
			{
				for (int i = 0; i < cCmds; i++)
				{
					if (prgCmds[i].cmdID == cmdID && queryEvent())
					{
						prgCmds[i].cmdf = (uint)(OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED);
						return VSConstants.S_OK;
					}
				}
			}
			return nextCommandTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

		}
	}
}
