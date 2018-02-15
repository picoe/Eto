using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Json;

namespace ${Namespace}
{	
	public class ${EscapedIdentifier} : Panel
	{	
		public ${EscapedIdentifier}()
		{
			JsonReader.Load(this);
		}
	}
}
