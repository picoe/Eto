using System;
using System.IO;
using json = Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Eto.Json
{
	public class DefaultNamespaceManager : NamespaceManager
	{
		public DefaultNamespaceManager ()
		{
			var asm = typeof(Eto.Forms.Application).Assembly;
			DefaultNamespace = new NamespaceInfo("Eto.Forms", asm);
			Namespaces.Add ("drawing", new NamespaceInfo("Eto.Drawing", asm));
		}
	}
	
}
