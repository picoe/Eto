using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Services.Protocols;

using Eto.Collections;

namespace Eto.Web
{
	public class VirtualWebService : VirtualPage
	{
		static MethodInfo coreGetHandler = typeof(WebServiceHandlerFactory).GetMethod("CoreGetHandler", BindingFlags.NonPublic | BindingFlags.Instance);
		
		public VirtualWebService(string path, Type type)
			: base(path, type)
		{
		}
		
		public VirtualWebService(string path, Type type, bool regularExpression)
			: base(path, type, regularExpression)
		{
		}
		
		public override IHttpHandler CreateInstance(IHttpHandlerFactory factory, HttpContext context, string path)
		{
			IHttpHandler handler = coreGetHandler.Invoke(factory, new object[] { Type, context, context.Request, context.Response }) as IHttpHandler;
			RewritePath(context, path);
			return handler;
		}
	}
}

