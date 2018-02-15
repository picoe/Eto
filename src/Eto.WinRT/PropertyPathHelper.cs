#if TODO_XAML
using System;
using swk = Windows.UI.Xaml.Markup;
using sw = Windows.UI.Xaml;
using System.Reflection;
using System.ComponentModel;

namespace Eto.WinRT
{
    public static class PropertyPathHelper
    {
        static readonly ConstructorInfo propertyPathConstructor = typeof(sw.PropertyPath).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new Type[] { typeof(string), typeof(ITypeDescriptorContext) }, null);

        public class NameResolver : swk.IXamlTypeResolver
        {
            public Type Resolve(string qualifiedTypeName)
			{
				var genericIndex = qualifiedTypeName.IndexOf('<');
				if (genericIndex > 0)
				{
					var endIndex = qualifiedTypeName.IndexOf('>', genericIndex + 1);
					var genericParameter = qualifiedTypeName.Substring(genericIndex + 1, endIndex - genericIndex - 1).Trim();
					var baseType = qualifiedTypeName.Substring(0, genericIndex).Trim();
					var genericType = Type.GetType(baseType);
					string[] genericParameters = genericParameter.Split('|');
					var types = new Type[genericParameters.Length];
					for (int i = 0; i < types.Length; i++)
					{
						types[i] = Type.GetType(genericParameters[i]);
					}
					return genericType.MakeGenericType(types);
				}
				return Type.GetType(qualifiedTypeName);
			}
        }

        public class NameServiceProvider : IServiceProvider, ITypeDescriptorContext
        {
            readonly NameResolver nameResolver = new NameResolver();
            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(swk.IXamlTypeResolver))
                    return nameResolver;
                return null;
            }

            public IContainer Container
            {
                get { throw new NotImplementedException(); }
            }

            public object Instance
            {
                get { throw new NotImplementedException(); }
            }

            public void OnComponentChanged()
            {
                throw new NotImplementedException();
            }

            public bool OnComponentChanging()
            {
                throw new NotImplementedException();
            }

            public PropertyDescriptor PropertyDescriptor
            {
                get { throw new NotImplementedException(); }
            }
        }

        static readonly NameServiceProvider nameServiceProvider = new NameServiceProvider();

        public static sw.PropertyPath Create (string path)
        {

            return propertyPathConstructor.Invoke(new object[] { path, nameServiceProvider }) as sw.PropertyPath;
        }

    }
}
#endif