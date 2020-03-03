using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Designer
{
	public interface IDesignHost : IDisposable
	{
		Action ControlCreating { get; set; }
		Action ControlCreated { get; set; }
		Action<DesignError> Error { get; set; }
		Action ContainerChanged { get; set; }
		string MainAssembly { get; set; }
		IEnumerable<string> References { get; set; }
		Control GetContainer();
		void Update(string code);
		void Invalidate();
		bool SetBuilder(string fileName);
		string GetCodeFile(string fileName);
	}
}
