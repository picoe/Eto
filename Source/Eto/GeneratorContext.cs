using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto
{
	/// <summary>
	/// Sets the current generator for a block of code,
	/// and verifies that all objects created in that 
	/// block were created using that generator.
	/// </summary>
	internal class GeneratorContext : IDisposable
	{
		Generator previous;
		Generator previousValidate;

		public GeneratorContext(Generator g)
		{
			previous = Generator.Current;
			previousValidate = Generator.ValidateGenerator;
			Generator.Initialize(g);
			Eto.Generator.ValidateGenerator = g;
		}

		public void Dispose()
		{
			Generator.Initialize(previous);
			Generator.ValidateGenerator = previousValidate;
		}
	}
}
