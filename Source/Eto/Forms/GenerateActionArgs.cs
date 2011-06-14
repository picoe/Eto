using System;
using System.Collections;

namespace Eto.Forms
{
	public interface IActionGenerator
	{
		void Generate(GenerateActionArgs args);
	}
			
	public class GenerateActionArgs : EventArgs
	{
		#region Members
		
		ActionCollection actions;
		ActionItemCollection menu;
		ActionItemCollection toolBar;
		Hashtable arguments = new Hashtable();
		
		#endregion
		
		#region Properties

		public Generator Generator
		{
			get { return actions.Generator; }
		}
		
		public ActionCollection Actions
		{
			get { return actions; }
		}
		
		public ActionItemCollection Menu
		{
			get { return menu; }
		}
		
		public ActionItemCollection ToolBar
		{
			get { return toolBar; }
		}
		
		public Hashtable Arguments
		{
			get { return arguments; }
		}
		
		#endregion

		public GenerateActionArgs()
		: this(Generator.Current)
		{
		}
		
		public GenerateActionArgs(Generator generator)
		: this(generator, null)
		{
		}
		
		public GenerateActionArgs(Control control)
		: this(control.Generator, control)
		{
		}
		
		public GenerateActionArgs(Generator g, Control control)
		{
			this.actions = new ActionCollection(g, control);
			this.menu = new ActionItemCollection(actions);
			this.toolBar = new ActionItemCollection(actions);
		}
		
		public GenerateActionArgs(ActionCollection actions, ActionItemCollection menu, ActionItemCollection toolBar)
		{
			this.actions = actions;
			this.menu = menu;
			this.toolBar = toolBar;
		}

		public void CopyArguments(GenerateActionArgs args)
		{
			foreach (DictionaryEntry de in args.arguments)
			{
				arguments[de.Key] = de.Value;
			}
		}
		
		public void Merge(GenerateActionArgs args)
		{
			toolBar.Merge(args.toolBar);
			menu.Merge(args.Menu);
		}
		
		public object GetArgument(object key, object defaultValue)
		{
			if (!arguments.ContainsKey(key)) return defaultValue;
			return arguments[key];
		}

		public void Clear()
		{
			actions.Clear();
			menu.Clear();
			toolBar.Clear();
		}
	}
}
