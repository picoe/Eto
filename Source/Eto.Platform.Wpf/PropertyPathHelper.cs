using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swk = System.Windows.Markup;
using sw = System.Windows;
using System.Reflection;
using System.ComponentModel;
using Eto.Forms;

namespace Eto.Platform.Wpf
{
    public static class PropertyPathHelper
    {
        static ConstructorInfo propertyPathConstructor = typeof(sw.PropertyPath).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, System.Type.DefaultBinder, new Type[] { typeof(string), typeof(ITypeDescriptorContext) }, null);

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
                    Type[] types = new Type[genericParameters.Length];
                    for (int i = 0; i < types.Length; i++)
                    {
                        types[i] = Type.GetType(genericParameters[i]);
                    }
                    return genericType.MakeGenericType(types);
                }
                else
                    return Type.GetType(qualifiedTypeName);
            }
        }

        public class NameServiceProvider : IServiceProvider, System.ComponentModel.ITypeDescriptorContext
        {
            NameResolver nameResolver = new NameResolver();
            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(swk.IXamlTypeResolver))
                    return nameResolver;
                return null;
            }

            public System.ComponentModel.IContainer Container
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

        static NameServiceProvider nameServiceProvider = new NameServiceProvider();

        public static sw.PropertyPath Create (string path)
        {

            return propertyPathConstructor.Invoke(new object[] { path, nameServiceProvider }) as sw.PropertyPath;
        }

    }
}
