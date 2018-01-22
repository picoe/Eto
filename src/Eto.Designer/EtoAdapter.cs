using System;
using Eto.Forms;
using Eto.Drawing;
using Microsoft.CSharp;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using Eto.Designer;

namespace Eto.Designer
{
	public interface IEtoAdapterHandler
	{
		object ToContract(Control control);

		Control FromContract(object contract);

		void Unload();
	}

	public static class EtoAdapter
	{
		public static object ToContract(this Control control)
		{
			return Platform.Instance.CreateShared<IEtoAdapterHandler>().ToContract(control);
		}

		public static Control ToControl(object contract)
		{
			return Platform.Instance.CreateShared<IEtoAdapterHandler>().FromContract(contract);
		}

		public static void Unload()
		{
			Platform.Instance.CreateShared<IEtoAdapterHandler>().Unload();
		}
	}
}
