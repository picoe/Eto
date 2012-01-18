using System;
using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;
using System.Linq;
using System.Windows.Markup;

namespace Eto.Forms
{
	public partial interface IContainer : IControl
	{
		Size ClientSize { get; set; }

		object ContainerObject { get; }

		void SetLayout (Layout layout);
	}
	
	[ContentProperty("Layout")]
	public partial class Container : Control
	{
		IContainer inner;
		Layout layout;

		public IEnumerable<Control> Controls {
			get { 
				if (Layout != null)
					return Layout.Controls;
				else
					return Enumerable.Empty<Control> ();
			}
		}
		
		public IEnumerable<Control> Children {
			get {
				if (Layout != null) {
					foreach (var control in Layout.Controls) {
						yield return control;
						var container = control as Container;
						if (container != null) {
							foreach (var child in container.Children)
								yield return child;
						}
					}
				}
			}
		}
		
		public override void OnPreLoad (EventArgs e)
		{
			base.OnPreLoad (e);
			
			if (Layout != null)
				Layout.OnPreLoad (e);
			
			foreach (Control control in Controls) {
				control.OnPreLoad (e);
			}
		}
		
		public override void OnLoad (EventArgs e)
		{
			foreach (Control control in Controls) {
				control.OnLoad (e);
			}
			
			base.OnLoad (e);
			
			if (Layout != null)
				Layout.OnLoad (e);
		}

		public override void OnLoadComplete (EventArgs e)
		{
			foreach (Control control in Controls) {
				control.OnLoadComplete (e);
			}
			
			base.OnLoadComplete (e);
			
			if (Layout != null)
				Layout.OnLoadComplete (e);
		}
		
		protected Container (Generator g, Type type, bool initialize = true) : base(g, type, initialize)
		{
			inner = (IContainer)base.Handler;
		}
		
		public object ContainerObject {
			get { return inner.ContainerObject; }
		}
		
		public Layout Layout 
		{
			get { return layout; }
			set {
				layout = value;
				layout.Container = this;
				SetInnerLayout ();
			}
		}

		public void SetInnerLayout ()
		{
			var innerLayout = layout.InnerLayout;
			if (innerLayout != null) {
				innerLayout.Container = this;
				inner.SetLayout (innerLayout);
				if (Loaded) {
					layout.OnPreLoad (EventArgs.Empty);
					layout.OnLoad (EventArgs.Empty);
					layout.OnLoadComplete (EventArgs.Empty);
				}
			}
		}
		
		public Size ClientSize {
			get { return inner.ClientSize; }
			set { inner.ClientSize = value; }
		}
	}
}
