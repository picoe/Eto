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
using Eto.Designer.Builders;

namespace Eto.Designer
{
	/// <summary>
	/// Used to instantiate the control from an app domain
	/// </summary>
	public class ControlLoader : MarshalByRefObject
	{
		static object controlHolder;

		public object Execute(string platformType, string testAssembly, string initializeAssembly = null)
		{
			try
			{
				if (Platform.Instance == null)
				{
					var plat = Activator.CreateInstance(Type.GetType(platformType)) as Platform;
					Platform.Initialize(plat);
					if (!string.IsNullOrEmpty(initializeAssembly))
					{
						plat.LoadAssembly(initializeAssembly);
					}
					new Application().Attach();
				}
				var asm = Assembly.Load(testAssembly);
				var type = CodeInterfaceBuilder.FindControlType(asm);
				if (type != null)
				{
					var control = CodeInterfaceBuilder.InstantiateControl(type);
					if (control != null)
					{
						controlHolder = control;
						return control.ToContract();
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
			return null;
		}
	}
	
}
