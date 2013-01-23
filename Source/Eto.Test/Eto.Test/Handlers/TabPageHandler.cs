using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Handlers
{
    class TabPageHandler : ThemedContainerHandler<Panel, TabPage>, ITabPage, IContainer
    {
		public Tab Tab { get; private set; }

        public TabPageHandler()
        {
			this.Tab = new Tab { Tag = this };
            this.Control = new Panel { };
        }

		public string Text 
		{
			get { return this.Tab.Text; }
			set { this.Tab.Text = value; }
		}

		public Image Image
		{
			get;
			set; // TODO
		}
    }
}
