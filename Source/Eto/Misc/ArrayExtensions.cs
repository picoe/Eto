using System;
using System.Collections.Generic;

namespace Eto.Misc
{
	public static class ArrayExtensions
	{
		
		public static Dictionary<T, T> ToDictionary<T>(this T[,] array)
		{
			var ret = new Dictionary<T, T>();
			if (array.GetLength(1) != 2) throw new ArgumentException("array must only have 2 elements in the second dimension");
			for (int i=0; i<array.GetLength(0); i++)
			{
				ret[array[i,0]] = array[i,1];
			}
			return ret;
		}
	}
}

