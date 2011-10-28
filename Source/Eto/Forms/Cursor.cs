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
		ICursor inner;
		
		public Cursor (CursorType cursor)
			: this (Generator.Current, cursor)
		{
		}
		
		public Cursor (Generator generator, CursorType cursor)
			: this (generator)
		{
			inner.Create (cursor);
		}
		
		protected Cursor (Generator generator)
			: base(generator, typeof(ICursor), true)
		{
			inner = (ICursor)Handler;
		}
	}
}

