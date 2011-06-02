using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms
{
	public partial interface IWindow : IContainer
	{
		MenuBar Menu { get; set; }
		
		Icon Icon { get; set; }
		
		bool Resizable { get; set; }
		
		void Minimize();
	}
	
	public abstract partial class Window : Container
	{
		public virtual MenuBar Menu
		{
			get { return inner.Menu; }
			set { inner.Menu = value; }
		}

		public Icon Icon
		{
			get { return inner.Icon; }
			set { inner.Icon = value; }
		}
		
		
		public bool Resizable
		{
			get { return inner.Resizable; }
			set { inner.Resizable = value; }
		}
		
		public void Minimize()
		{
			inner.Minimize();
		}
	}
}
