using System;

namespace Eto.Forms
{
	public enum CursorType
	{
		Default,
		Arrow,
		Crosshair,
		Pointer,
		Move,
		IBeam,
		VerticalSplit,
		HorizontalSplit
	}
	
	public interface ICursor : IInstanceWidget
	{
		void Create(CursorType cursor);
	}
	
	public class Cursor : InstanceWidget
	{
		ICursor handler;
		
		public Cursor (CursorType cursor)
			: this (Generator.Current, cursor)
		{
		}
		
		public Cursor (Generator generator, CursorType cursor)
			: this (generator)
		{
			handler.Create (cursor);
		}
		
		protected Cursor (Generator generator)
			: this (generator, typeof(ICursor))
		{
		}
		
		protected Cursor (Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
			handler = (ICursor)Handler;
		}
		
	}
}

