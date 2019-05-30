using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Designer
{
	public class InProcessDesignPanel : DesignPanel
	{
		public override void Update(string code)
		{
			var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			using (AssemblyResolver.Register(baseDir))
				base.Update(code);
		}
	}

}
