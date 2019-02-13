using System;
using System.Reflection;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Dialog for displaying information about the application.
	/// </summary>
	[Handler(typeof(AboutDialog.IHandler))]
	public class AboutDialog : CommonDialog
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.AboutDialog"/> class.
		/// </summary>
		public AboutDialog() : this(TypeHelper.GetCallingAssembly?.Invoke(null, null) as Assembly)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.AboutDialog"/> class.
		/// </summary>
		/// <param name="assembly">
		/// Assembly file from which it'll try to load <see cref="ProgramName"/>, 
		/// <see cref="ProgramDescription"/>, <see cref="Version"/> and <see cref="Copyright"/> properties.
		/// </param>
		public AboutDialog(Assembly assembly)
		{
			Title = Application.Instance.Localize(this, "About");

			if (assembly != null)
			{
				Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "";
				ProgramDescription = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "";
				ProgramName = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "";
				Version = assembly.GetName()?.Version?.ToString() ?? "";
			}
		}

		/// <summary>
		/// Gets or sets the copyright text.
		/// </summary>
		/// <value>The copyright text.</value>
		public string Copyright
		{
			get { return Handler.Copyright; }
			set { Handler.Copyright = value; }
		}

		/// <summary>
		/// Gets or sets the list of graphical designers.
		/// </summary>
		/// <value>The list of graphical designers.</value>
		/// <seealso cref="Developers"/>
		/// <seealso cref="Documenters"/>
		public string[] Designers
		{
			get { return Handler.Designers; }
			set { Handler.Designers = value; }
		}

		/// <summary>
		/// Gets or sets the list of developers.
		/// </summary>
		/// <value>The list of developers.</value>
		/// <seealso cref="Designers"/>
		/// <seealso cref="Documenters"/>
		public string[] Developers
		{
			get { return Handler.Developers; }
			set { Handler.Developers = value; }
		}

		/// <summary>
		/// Gets or sets the list of documenters.
		/// </summary>
		/// <value>The list of documenters.</value>
		/// <seealso cref="Designers"/>
		/// <seealso cref="Developers"/>
		public string[] Documenters
		{
			get { return Handler.Documenters; }
			set { Handler.Documenters = value; }
		}

		/// <summary>
		/// Gets or sets the license.
		/// </summary>
		/// <value>The license.</value>
		public string License
		{
			get { return Handler.License; }
			set { Handler.License = value; }
		}

		/// <summary>
		/// Gets or sets the application logo.
		/// </summary>
		/// <value>The application logo.</value>
		public Image Logo
		{
			get { return Handler.Logo; }
			set { Handler.Logo = value; }
		}

		/// <summary>
		/// Gets or sets the program description.
		/// </summary>
		/// <value>The program description.</value>
		public string ProgramDescription
		{
			get { return Handler.ProgramDescription; }
			set { Handler.ProgramDescription = value; }
		}

		/// <summary>
		/// Gets or sets the name of the program.
		/// </summary>
		/// <value>The name of the program.</value>
		public string ProgramName
		{
			get { return Handler.ProgramName; }
			set { Handler.ProgramName = value; }
		}

		/// <summary>
		/// Gets or sets the window title.
		/// </summary>
		/// <value>The window title.</value>
		public string Title
		{
			get { return Handler.Title; }
			set { Handler.Title = value; }
		}

		/// <summary>
		/// Gets or sets the application version.
		/// </summary>
		/// <value>The application version.</value>
		public string Version
		{
			get { return Handler.Version; }
			set { Handler.Version = value; }
		}

		/// <summary>
		/// Gets or sets the application website.
		/// </summary>
		/// <value>The application website.</value>
		public Uri Website
		{
			get { return Handler.Website; }
			set { Handler.Website = value; }
		}

		/// <summary>
		/// Gets or sets the application website label.
		/// </summary>
		/// <value>The application website label.</value>
		public string WebsiteLabel
		{
			get { return Handler.WebsiteLabel; }
			set { Handler.WebsiteLabel = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="AboutDialog"/>
		/// </summary>
		public new interface IHandler : CommonDialog.IHandler
		{
			/// <summary>
			/// Gets or sets the copyright text.
			/// </summary>
			/// <value>The copyright text.</value>
			string Copyright { get; set; }

			/// <summary>
			/// Gets or sets the list of graphical designers.
			/// </summary>
			/// <value>The list of graphical designers.</value>
			/// <seealso cref="Developers"/>
			/// <seealso cref="Documenters"/>
			string[] Designers { get; set; }

			/// <summary>
			/// Gets or sets the list of developers.
			/// </summary>
			/// <value>The list of developers.</value>
			/// <seealso cref="Designers"/>
			/// <seealso cref="Documenters"/>
			string[] Developers { get; set; }

			/// <summary>
			/// Gets or sets the list of documenters.
			/// </summary>
			/// <value>The list of documenters.</value>
			/// <seealso cref="Designers"/>
			/// <seealso cref="Developers"/>
			string[] Documenters { get; set; }

			/// <summary>
			/// Gets or sets the license.
			/// </summary>
			/// <value>The license.</value>
			string License { get; set; }

			/// <summary>
			/// Gets or sets the application logo.
			/// </summary>
			/// <value>The application logo.</value>
			Image Logo { get; set; }

			/// <summary>
			/// Gets or sets the program description.
			/// </summary>
			/// <value>The program description.</value>
			string ProgramDescription { get; set; }

			/// <summary>
			/// Gets or sets the name of the program.
			/// </summary>
			/// <value>The name of the program.</value>
			string ProgramName { get; set; }

			/// <summary>
			/// Gets or sets the window title.
			/// </summary>
			/// <value>The window title.</value>
			string Title { get; set; }

			/// <summary>
			/// Gets or sets the application version.
			/// </summary>
			/// <value>The application version.</value>
			string Version { get; set; }

			/// <summary>
			/// Gets or sets the application website.
			/// </summary>
			/// <value>The application website.</value>
			Uri Website { get; set; }

			/// <summary>
			/// Gets or sets the application website label.
			/// </summary>
			/// <value>The application website label.</value>
			string WebsiteLabel { get; set; }
		}
	}
}
