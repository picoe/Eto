using System;

namespace Eto.Platform.Android
{
	public class Generator : Eto.Generator
	{
		public override string ID { get { return "android"; } }

		public Generator()
		{
			AddTo(this);
		}

		public static void AddTo(Generator generator)
		{
		}
	}
}

