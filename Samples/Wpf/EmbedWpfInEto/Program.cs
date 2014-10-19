using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbedWpfInEto
{
	static class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			new Application(new Eto.Wpf.Platform()).Run(new MainForm());
		}
	}
}
