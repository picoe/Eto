using System;
using System.Collections.ObjectModel;

namespace Eto.Forms.Controls
{
	public interface IMdiContainer : IControl
	{
		void InsertChild (int index, MdiChild child);
		
		void RemoveChild (int index, MdiChild child);
		
		void ClearChildren ();
	}
	
	public abstract class MdiContainer : Control
	{
		MdiChildCollection children;
		
		public MdiChildCollection Children {
			get { return children; }
		}
		
		protected MdiContainer (Generator generator, Type type, bool initialize)
			: base (generator, type, initialize)
		{
			children = new MdiChildCollection { 
				Handler = this.Handler as IMdiContainer
			};
		}
	}
	
	public class MdiChildCollection : Collection<MdiChild>
	{
		internal IMdiContainer Handler { get; set; }
		
		protected override void ClearItems ()
		{
			base.ClearItems ();
			Handler.ClearChildren ();
		}
		
		protected override void InsertItem (int index, MdiChild item)
		{
			base.InsertItem (index, item);
			Handler.InsertChild (index, item);
		}
		
		protected override void RemoveItem (int index)
		{
			var item = this [index];
			base.RemoveItem (index);
			Handler.RemoveChild (index, item);
		}
	}
}
