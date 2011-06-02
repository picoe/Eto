using System;

namespace Eto.Misc
{
	public static class Url
	{
		public static string Join(string path, string relative)
		{
			bool pathEnds = path.EndsWith("/");
			bool relativeStarts = relative.StartsWith("/");
			if (pathEnds == relativeStarts)
			{
				if (pathEnds) path = path.TrimEnd('/');
				else path = path + "/";
			}
			return path + relative;
		}
	}
}
