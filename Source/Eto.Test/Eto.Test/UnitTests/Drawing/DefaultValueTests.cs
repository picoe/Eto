using NUnit.Framework;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Eto.Drawing;
using System.Linq.Expressions;

namespace Eto.Test.UnitTests.Drawing
{
	[TestFixture, Category(TestUtils.NoTestPlatformCategory)]
	public class DefaultValueTests
	{
		class TypeTest
		{
			public Type Type { get; set; }
			public Func<object> Create { get; set; }
			public List<string> Properties { get; set; }
		}

		static TypeTest Test<T>(Func<T> create, params Expression<Func<T, object>>[] param)
		{
			return new TypeTest
			{
				Type = typeof(T),
				Create = () => create(),
				Properties = param.Select(r => r.GetMemberInfo().Member.Name).ToList()
			};
		}

		static TypeTest Test<T>(params Expression<Func<T, object>>[] param)
			where T: new()
		{
			return new TypeTest
			{
				Type = typeof(T),
				Create = () => new T(),
				Properties = param.Select(r => r.GetMemberInfo().Member.Name).ToList()
			};
		}

		static IEnumerable<TypeTest> GetTests()
		{ 
			yield return Test(() => new Eto.Drawing.LinearGradientBrush(Colors.Black, Colors.White, PointF.Empty, new PointF(10, 10)), r => r.Wrap);
			yield return Test(() => new Eto.Drawing.RadialGradientBrush(Colors.Black, Colors.White, PointF.Empty, new PointF(1, 1), new SizeF(10, 10)), r => r.Wrap);
		}

		[Test]
		public void DefaultPropertyValuesShouldBeCorrect()
		{
			TestUtils.Invoke(() =>
			{
				foreach (var test in GetTests())
				{
					var obj = test.Create != null ? test.Create() : Activator.CreateInstance(test.Type);
					foreach (var propertyName in test.Properties)
					{
						var propertyInfo = obj.GetType().GetTypeInfo().GetDeclaredProperty(propertyName);
						var defValAttr = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();
						var defaultValue = defValAttr != null ? defValAttr.Value : propertyInfo.PropertyType.GetTypeInfo().IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
						var val = propertyInfo.GetValue(obj);
						Assert.AreEqual(defaultValue, val, string.Format("Property '{0}' of type '{1}' is expected to be '{2}'", propertyName, test.Type.Name, defaultValue));
					}
				}

			});
		}
	}
}
