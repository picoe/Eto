using System;
using Eto.Forms;
using Eto.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Eto.Addin.Shared
{
	public class ProjectWizardPageModel : INotifyPropertyChanged
	{
		public IParameterSource Source { get; private set; }

		public ProjectWizardPageModel(IParameterSource source)
		{
			Source = source;
			UsePCL = SupportsPCL;
			UseNET = !SupportsPCL;
			UseSAL = false;
			Combined = false;
			Mode = "code";
			IncludeSolution = false;
			if (SupportsBase)
				Base = "Panel";
			if (SupportsFramework)
				SelectedFramework = SupportedFrameworks[0];
		}

		public bool IsValid
		{
			get
			{
				if (SupportsAppName)
				{
					
					if (string.IsNullOrWhiteSpace(AppName) || AppNameInvalid)
						return false;
				}
				if (SupportsFramework && SelectedFramework == null)
					return false;
				return true;
			}
		}

		public bool AppNameInvalid
		{
			get
			{
				if (string.IsNullOrWhiteSpace(AppName)) // not invalid, but can't continue
					return false;
				if (NoDash && AppName.Contains("-"))
					return true;
				if (!Regex.IsMatch(AppName, @"^[a-zA-Z_-][\w\.-_]*$"))
					return true;
				return false;
			}
		}

		public string AppNameValidationText
		{
			get
			{
				if (NoDash)
					return "App name must only be a combination of letters, digits, or one of '_', '.'";
				else
					return "App name must only be a combination of letters, digits, or one of '_', '-', '.'";
			}
		}

		public string AppName
		{
			get { return Source.GetParameter("AppName"); }
			set
			{
				Source.SetParameter("AppName", value);
				OnPropertyChanged(nameof(IsValid));
				OnPropertyChanged(nameof(AppNameInvalid));
			}
		}

		public bool SupportsAppName => Source.IsSupportedParameter("AppName");

		public bool IsLibrary => Source.IsSupportedParameter("IsLibrary");

		public bool SupportsPCL => Source.TargetFrameworkVersion > new Version(4, 0) && Source.IsSupportedParameter("PCL");

		public bool SupportsNetStandard => Source.IsSupportedParameter("NetStandard");

		public bool SupportsSAL => Source.IsSupportedParameter("SAL");

		public bool SupportsFramework => Source.IsSupportedParameter("Framework");

		public bool SupportsProjectType => SupportsPCL || SupportsSAL || SupportsNetStandard;

		public bool SupportsCombined => Source.IsSupportedParameter("Combined");

		public bool SupportsXamMac => Source.IsSupportedParameter("XamMac");

		public bool SupportsXeto => Source.IsSupportedParameter("Xeto");

		public bool SupportsJeto => Source.IsSupportedParameter("Jeto");

		public bool SupportsCodePreview => Source.IsSupportedParameter("Preview");

		public bool SupportsPanelType => SupportsXeto || SupportsJeto || SupportsCodePreview;

		public bool SupportsBase => Source.IsSupportedParameter("Base");

		public bool NoDash => Source.IsSupportedParameter("NoDash");

		public bool Combined
		{
			get { return Source.GetParameter("Combined").ToBool(); }
			set
			{
				Source.SetParameter("Combined", value.ToString());
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

		public string Base
		{
			get { return Source.GetParameter("Base"); }
			set
			{
				Source.SetParameter("Base", value);
				OnPropertyChanged();
				OnPropertyChanged(nameof(Information));
			}
		}

		public string Title => string.Format("Eto.Forms {0} Properties", IsLibrary ? "Library" : "Application");

		public bool RequiresInput => SupportsCombined || SupportsProjectType || SupportsPanelType;

		public class FrameworkInfo
		{
			public string Text { get; set; }
			public string Value { get; set; }
			public string Description { get; set; }
			public bool CanUseCombined { get; set; }
		}

		public FrameworkInfo[] SupportedFrameworks => frameworkInformation;

		static readonly FrameworkInfo[] frameworkInformation =
		{
			new FrameworkInfo { Text = ".NET 5", Value = "net5.0", CanUseCombined = true },
			new FrameworkInfo { Text = ".NET Core 3.1", Value = "netcoreapp3.1", CanUseCombined = true },
			new FrameworkInfo { Text = ".NET Framework 4.8", Value = "net48", CanUseCombined = true },
			new FrameworkInfo { Text = ".NET Framework 4.7.2", Value = "net472", CanUseCombined = true },
			new FrameworkInfo { Text = ".NET Framework 4.6.2", Value = "net462", CanUseCombined = true },
		};

		FrameworkInfo _selectedFramework;

		public FrameworkInfo SelectedFramework
		{
			get => _selectedFramework;
			set
			{
				_selectedFramework = value;

				Source.SetParameter("TargetFrameworkOverride", value.Value);

				if (!AllowCombined)
					Combined = false;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Information));
				OnPropertyChanged(nameof(AllowCombined));
			}
		}

		public bool AllowCombined => _selectedFramework?.CanUseCombined == true;


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
			public bool Combined;
			public bool? IncludeXamMac;
		}

		static CombinedInfo[] combinedInformation = {
			new CombinedInfo { Combined = true, IncludeXamMac = true, Text = "A single combined project that can build for Windows, Linux and Mac, and a separate Xamarin.Mac project to bundle mono with VS for Mac." },
			new CombinedInfo { Combined = true, IncludeXamMac = false, Text = "A single combined project that can build for Windows, Linux, and Mac." },
			new CombinedInfo { Combined = false, Text = "A separate project for each platform." },
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

		struct BaseInfo
		{
			public string Text;
			public string Base;
		}

		static BaseInfo[] baseInformation = {
			new BaseInfo { Base = "Panel", Text = "A panel that can be put onto an existing window" },
			new BaseInfo { Base = "Dialog", Text = "A modal dialog that waits for input" },
			new BaseInfo { Base = "Form", Text = "A modesless form" },
		};

		public string Information
		{
			get
			{
				var text = new List<string>();

				if (SupportsCombined)
				{
					var combinedInfo = from i in combinedInformation
									   where i.Combined == Combined
											  && (i.IncludeXamMac == null || i.IncludeXamMac == IncludeXamMac)
									   select i.Text;
					text.Add(combinedInfo.FirstOrDefault());
				}

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

				if (SupportsBase)
				{

				}

				if (SupportsFramework)
				{
					text.Add($"For {SelectedFramework.Text}.");
				}

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