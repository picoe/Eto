using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Web.UI;
using System.Web;
using Eto.Collections;
using Eto.Misc;

namespace Eto.Web
{
	public interface IVirtualPage
	{
		IHttpHandler CreateInstance(IHttpHandlerFactory factory, HttpContext context, string path);
		bool MatchesPath(string path);
	}
	
	public class VirtualPage : IVirtualPage
	{
		Type type;
		string path;
		bool regularExpression = false;
		string[] groupNames;
		
		public VirtualPage(string path, Type type)
		{
			this.type = type;
			this.path = path;
		}

		public VirtualPage(string path, Type type, bool regularExpression)
		{
			this.type = type;
			this.path = path;
			this.regularExpression = regularExpression;
		}
		
		public Type Type
		{
			get { return type; }
		}
		
		public string Path
		{
			get { return path; }
		}
		
		public bool RegularExpression
		{
			get { return regularExpression; }
			set { regularExpression = value; }
		}
		
		public virtual IHttpHandler CreateInstance(IHttpHandlerFactory factory, HttpContext context, string path)
		{
			IHttpHandler handler = Activator.CreateInstance(type) as IHttpHandler;
			RewritePath(context, path);
			return handler;
		}
		
		protected void RewritePath(HttpContext context, string path)
		{
			if (RegularExpression)
			{
				if (groupNames == null)
				{
					// find named captures in regex expression
					MatchCollection matches = Regex.Matches(this.path, "[(][?][<](?<group>\\w+)[>]", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
					List<string> groups = new List<string>();
					foreach (Match groupMatch in matches)
					{
						Group group = groupMatch.Groups["group"];
						if (group.Success) groups.Add(group.Value);
						//Console.WriteLine(group.Value);
					}
					groupNames = groups.ToArray();
				}
				Match match = Regex.Match(path, this.path, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
				if (match.Success)
				{
					string newPath = path;
					foreach (string groupName in groupNames)
					{
						Group group = match.Groups[groupName];
						if (group.Success)
						{
							if (newPath.IndexOf('?') == -1) newPath += '?';
							else newPath += "&";
							newPath += groupName + "=" + group.Value;
						}
					}
					context.RewritePath(Url.Join(context.Request.ApplicationPath, newPath));
				}
			}
		}
		
		public virtual bool MatchesPath(string path)
		{
			if (regularExpression) return Regex.IsMatch(path, this.path, RegexOptions.Compiled | RegexOptions.IgnoreCase);
			else return string.Compare(this.path, path, true) == 0;
		}
	}

	public class VirtualPageCollection : List<IVirtualPage>
	{

		public VirtualPage FindMap(string path)
		{
			foreach (VirtualPage map in this)
			{
				if (map.MatchesPath(path)) return map;
			}
			return null;
		}
	}
}

