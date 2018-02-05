using System;
using Eto.Forms;
using Eto.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

namespace Eto.Addin.Shared
{
	public class ProjectWizardPageModel : OptionsPageModel, INotifyPropertyChanged
	{
		public IParameterSource Source { get; private set; }

		public ProjectWizardPageModel(IParameterSource source, XElement optionsElement)
			: base(optionsElement)
		{
			Source = source;
			UsePCL = SupportsPCL;
			UseNET = !SupportsPCL;
			UseSAL = false;
			Separate = false;
			Mode = "code";
			IncludeSolution = false;
		}

		public string AppName
		{
			get { return Source.GetParameter("AppName"); }
			set { Source.SetParameter("AppName", value); }
		}

		public bool ShowAppName => Source.IsSupportedParameter("AppName");

		public bool IsLibrary => Source.IsSupportedParameter("IsLibrary");

		public bool SupportsPCL => Source.TargetFrameworkVersion > new Version(4, 0) && Source.IsSupportedParameter("PCL");

		public bool SupportsNetStandard => Source.IsSupportedParameter("NetStandard");

		public bool SupportsSAL => Source.IsSupportedParameter("SAL");

		public bool SupportsFramework => Source.IsSupportedParameter("Framework");

		public bool SupportsProjectType => SupportsPCL || SupportsSAL || SupportsNetStandard;

		public bool SupportsSeparated => Source.IsSupportedParameter("Separated");

		public bool SupportsXamMac => Source.IsSupportedParameter("XamMac");

		public bool SupportsXeto => Source.IsSupportedParameter("Xeto");

		public bool SupportsJeto => Source.IsSupportedParameter("Jeto");

		public bool SupportsCodePreview => Source.IsSupportedParameter("Preview");

		public bool SupportsPanelType => SupportsXeto || SupportsJeto || SupportsCodePreview;

		public bool Separate
		{
			get { return Source.GetParameter("Separate").ToBool(); }
			set
			{
				Source.SetParameter("Separate", value.ToString());
				OnPropertyChanged();
				OnPropertyChanged(nameof(Information));
			}
		}

		public bool IncludeXamMac
		{
			get { return Source.GetParameter("IncludeXamMac").ToBool(); }
			set
			{
				Source.SetParameter("IncludeXamMac", value.ToString());
				OnPropertyChanged();
				OnPropertyChanged(nameof(Information));
			}
		}

		public bool IncludeSolution
		{
			get { return Source.GetParameter("IncludeSolution").ToBool(); }
			set
			{
				Source.SetParameter("IncludeSolution", value.ToString());
				OnPropertyChanged();
			}
		}

		public bool UsePCL
		{
			get { return Source.GetParameter("UsePCL").ToBool(); }
			set
			{
				Source.SetParameter("UsePCL", value.ToString());
				OnPropertyChanged();
				OnPropertyChanged(nameof(Information));
			}
		}

		public bool UseNetStandard
		{
			get { return Source.GetParameter("UseNetStandard").ToBool(); }
			set
			{
				Source.SetParameter("UseNetStandard", value.ToString());
				OnPropertyChanged();
				OnPropertyChanged(nameof(Information));
			}
		}

		public bool UseSAL
		{
			get { return Source.GetParameter("UseSAL").ToBool(); }
			set
			{
				Source.SetParameter("UseSAL", value.ToString());
				OnPropertyChanged();
				OnPropertyChanged(nameof(Information));
				UpdateCommon();
			}
		}

		public bool UseNET
		{
			get { return Source.GetParameter("UseNET").ToBool(); }
			set
			{
				Source.SetParameter("UseNET", value.ToString());
				OnPropertyChanged();
				OnPropertyChanged(nameof(Information));
			}
		}

		public string Mode
		{
			get { return Source.GetParameter("Mode"); }
			set
			{
				Source.SetParameter("Mode", value);
				OnPropertyChanged();
				OnPropertyChanged(nameof(Information));
			}
		}

		public override string Title
		{
			get { return string.Format("Eto.Forms {0} Properties", IsLibrary ? "Library" : "Application"); }
			set { base.Title = value; }
		}

		public bool RequiresInput => SupportsSeparated || SupportsProjectType;

		struct TypeInfo
		{
			public string Text;
			public bool UsePCL;
			public bool UseSAL;
			public bool UseNET;
		}

		static TypeInfo[] typeInformation = {
			new TypeInfo { UseNET = true, Text = "Use the full desktop .NET Framework for shared code and maximum compatibility with existing frameworks." },
			new TypeInfo { UsePCL = true, Text = "Use a .NET Standard assembly for shared code and maximum portability." },
			new TypeInfo { UseSAL = true, Text = "Use a shared asset library for shared code and maximum flexibility." }
		};

		struct CombinedInfo
		{
			public string Text;
			public bool Separate;
			public bool? IncludeXamMac;
		}

		static CombinedInfo[] combinedInformation = {
			new CombinedInfo { Separate = false, IncludeXamMac = true, Text = "A single combined project that can build for Windows, Linux and Mac, and a separate Xamarin.Mac project to bundle mono with VS for Mac." },
			new CombinedInfo { Separate = false, IncludeXamMac = false, Text = "A single combined project that can build for Windows, Linux, and Mac." },
			new CombinedInfo { Separate = true, Text = "A separate project for each platform." },
		};

		struct FormatInfo
		{
			public string Text;
			public string Mode;
		}

		static FormatInfo[] formatInformation = {
			new FormatInfo { Mode = "xaml", Text = "Use xaml (.xeto) to define the layout of your form, with code behind for logic and event handlers" },
			new FormatInfo { Mode = "json", Text = "Use json (.jeto) to define the layout of your form, with code behind for logic and event handlers" },
			new FormatInfo { Mode = "code", Text = "Write your form entirely in a single source file (no preview)" },
			new FormatInfo { Mode = "preview", Text = "Define your view layout in a partial class with form preview, with logic and event handlers in a separate file." },
		};

		public string Information
		{
			get
			{
				var text = new List<string>();
				var combinedInfo = from i in combinedInformation
								   where i.Separate == Separate
				                          && (i.IncludeXamMac == null || i.IncludeXamMac == IncludeXamMac)
								   select i.Text;
				text.Add(combinedInfo.FirstOrDefault());

				if (SupportsProjectType)
				{
					var typeInfo = from i in typeInformation
								   where i.UseNET == UseNET
								   && i.UsePCL == UsePCL
								   && i.UseSAL == UseSAL
								   select i.Text;
					text.Add(typeInfo.FirstOrDefault());
				}

				var formatInfo = from i in formatInformation
								 where i.Mode == Mode
								 select i.Text;
				text.Add(formatInfo.FirstOrDefault());

				return string.Join("\n\n", text);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		void UpdateCommon()
		{
			//Source.SetParameter("UseSharedXeto", (UseSAL && UseXeto).ToString());
			//Source.SetParameter("UseSharedJeto", (UseSAL && UseJeto).ToString());
		}
	}

}