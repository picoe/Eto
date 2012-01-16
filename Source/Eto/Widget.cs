using System;
using System.Xaml;
using System.Collections.Generic;

namespace Eto
{
	public interface IWidget
	{
		IWidget Handler { get; set; }

		void Initialize ();
	}
	
	public abstract class Widget : IWidget, IDisposable //, IAttachedPropertyStore
	{

		/*
  		#region IAttachedPropertyStore Members
		
		IDictionary<AttachableMemberIdentifier, object> attachedProperties = new Dictionary<AttachableMemberIdentifier, object> ();
 
		public void CopyPropertiesTo (KeyValuePair<AttachableMemberIdentifier, object>[] array, int index)
		{
			attachedProperties.CopyTo (array, index);
		}
 
		public int PropertyCount { get { return attachedProperties.Count; } }
 
		public bool RemoveProperty (AttachableMemberIdentifier member)
		{
			return attachedProperties.Remove (member);
		}
		
		public void SetProperty (AttachableMemberIdentifier member, object value)
		{
			attachedProperties [member] = value;
		}
 
		public bool TryGetProperty (AttachableMemberIdentifier member, out object value)
		{
			return attachedProperties.TryGetValue (member, out value);
		}
		
		#endregion
		*/

		public Generator Generator { get; private set; }

		public object Tag { get; set; }

		public IWidget Handler { get; set; }

		~Widget ()
		{
			//Console.WriteLine ("GC: {0}", this.GetType ().FullName);
			Dispose (false);
		}
		
		protected Widget (Generator generator, IWidget handler, bool initialize = true)
		{
			this.Generator = generator;
			this.Handler = handler;
			this.Handler.Handler = this; // tell the handler who we are
			if (initialize)
				Initialize ();
		}

		protected Widget (Generator generator, Type type, bool initialize = true)
		{
			this.Generator = generator;
			this.Handler = generator.CreateControl (type, this);
			if (initialize)
				Initialize ();
		}
		
		public void Initialize ()
		{
			Handler.Initialize ();
		}
		
		#region IDisposable Members

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}
		
		#endregion
		
		protected virtual void Dispose (bool disposing)
		{
			if (disposing) {
				var handler = this.Handler as IDisposable;
				if (handler != null)
					handler.Dispose ();
				Handler = null;
			}
		}		
		
	}
}

