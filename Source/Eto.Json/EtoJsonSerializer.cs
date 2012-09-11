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
	public class EtoJsonSerializer : JsonSerializer
	{
		public object Instance { get; set; }
	}
	
}
