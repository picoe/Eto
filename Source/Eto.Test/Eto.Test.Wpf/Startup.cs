using System;
using Eto;
using Eto.Misc;
using Eto.Forms;
using Eto.Drawing;
using System.Diagnostics;
using Eto.Platform.Wpf.Drawing;
using System.Threading;
using System.Collections.Generic;

namespace Eto.Test.Wpf
{
	class Startup
	{
		[STAThread]
		static void Main (string[] args)
		{
			var generator = new Eto.Platform.Wpf.Generator ();

			var app = new TestApplication (generator);

			var sw = new Stopwatch ();
			var count = 10000000;

			// pre-load
			generator.CreateHandler (typeof (IMatrixHandler), null);

			var list = new List<object> (count);

			var activator = Matrix.Activator ();

			sw.Start ();
			for (var i = 0; i < count; ++i) {
				/**
				var matrix = generator.CreateHandler<IMatrixHandler> ();
				matrix.Create ();
				/**
				var matrix = Matrix.Create ();
				/**
				var handler = new MatrixHandler ();
				/**
				var matrix = new Matrix ();
				/**/
				var matrix = activator ();
				/**/
				matrix.Scale (10, 10);
				//list.Add (matrix);
				/**/
			}

			var e1 = sw.Elapsed;

			var list2 = new List<System.Windows.Media.Matrix> ();
			sw.Restart ();

			for (var i = 0; i < count; ++i) {
				/**
				var m = new MatrixHandler ();
				m.Create ();
				/**/
				//var m = new System.Windows.Media.Matrix ();
				var matrix = System.Windows.Media.Matrix.Identity;
				matrix.Scale (10, 10);
				//list2.Add (matrix);
				/**/
			}

			var e2 = sw.Elapsed;

			Trace.WriteLine (string.Format ("Time: eto: {0}, direct: {1}, Diff: {2}", e1.TotalMilliseconds, e2.TotalMilliseconds, (e1.TotalSeconds / e2.TotalSeconds)));

			//app.Run (args);
		}

	}
}

