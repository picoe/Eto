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
	public interface IInterfaceBuilder
	{
		void Create(string text, Action<Control> controlCreated, Action<Exception> error);
	}
	
}
