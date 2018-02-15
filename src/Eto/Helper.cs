using System;
using System.Linq.Expressions;

namespace Eto
{
	/// <summary>
	/// Helper functions specific to Eto
	/// </summary>
	static class Helper
	{
		/// <summary>
		/// Constant to convert degrees to radians for angle parameters
		/// </summary>
		public const float DegreesToRadians = (float)(Math.PI / 180);

		public static Tuple<int,int> IndexOf(this object[,] target, object value)
		{
			var rowLowerLimit = target.GetLowerBound(0);
			var rowUpperLimit = target.GetUpperBound(0);

			var colLowerLimit = target.GetLowerBound(1);
			var colUpperLimit = target.GetUpperBound(1);

			for (int row = rowLowerLimit; row <= rowUpperLimit; row++)
			{
				for (int col = colLowerLimit; col <= colUpperLimit; col++)
				{
					if (object.ReferenceEquals(target[row, col], value))
						return new Tuple<int, int>(row, col);
				}
			}

			return null;
		}

		internal static MemberExpression GetMemberInfo(this Expression method)
		{
			var lambda = method as LambdaExpression;
			if (lambda == null)
				return null;

			if (lambda.Body.NodeType == ExpressionType.Convert)
			{
				return ((UnaryExpression)lambda.Body).Operand as MemberExpression;
			}
			if (lambda.Body.NodeType == ExpressionType.MemberAccess)
			{
				return lambda.Body as MemberExpression;
			}

			return null;
		}
	}
}
