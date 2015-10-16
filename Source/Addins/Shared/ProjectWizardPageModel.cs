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
	public class ProjectWizardPageModel : OptionsPageModel,  INotifyPropertyChanged
	{
		public IParameterSource Source { get; private set; }

		public ProjectWizardPageModel(IParameterSource source, XElement optionsElement)
			: base(optionsElement)
		{
			Source = source;
			UsePCL = SupportsPCL;
			UseNET = !SupportsPCL;
			UseSAL = false;
			Combined = true;
			UseCode = true;
		}

		public string AppName
		{
			get { return Source.GetParameter("AppName"); }
			set { Source.SetParameter("AppName", value); }
		}

		public bool ShowAppName { get { return Source.IsSupportedParameter("AppName"); } }

		public bool IsLibrary { get { return Source.IsSupportedParameter("IsLibrary"); } }

		public bool SupportsPCL { get { return Source.TargetFrameworkVersion > new Version(4, 0) && Source.IsSupportedParameter("SupportsPCL"); } }

		public bool SupportsSAL { get { return Source.IsSupportedParameter("SupportsSAL"); } }

		public bool SupportsCombined { get { return Source.IsSupportedParameter("SupportsCombined"); } }

		public bool SupportsProjectType { get { return SupportsPCL || SupportsSAL; } }

		public bool SupportsXeto { get { return Source.IsSupportedParameter("SupportsXeto"); } }

		public bool SupportsJeto { get { return Source.IsSupportedParameter("SupportsJeto"); } }

		public bool SupportsCodePreview { get { return Source.IsSupportedParameter("SupportsCodePreview"); } }

		public bool SupportsPanelType { get { return SupportsXeto || SupportsJeto || SupportsCodePreview; } }

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

		public bool UseCode
		{
			get { return Source.GetParameter("UseCode").ToBool(); }
			set
			{
				Source.SetParameter("UseCode", value.ToString());
				OnPropertyChanged();
				OnPropertyChanged(nameof(Information));
			}
		}

		public bool UseXeto
		{
			get { return Source.GetParameter("UseXeto").ToBool(); }
			set
			{
				Source.SetParameter("UseXeto", value.ToString());
				OnPropertyChanged();
				OnPropertyChanged(nameof(Information));
				UpdateCommon();
			}
		}

		public bool UseJeto
		{
			get { return Source.GetParameter("UseJeto").ToBool(); }
			set
			{
				Source.SetParameter("UseJeto", value.ToString());
				OnPropertyChanged();
				OnPropertyChanged(nameof(Information));
				UpdateCommon();
			}
		}

		public bool UseCodePreview
		{
			get { return Source.GetParameter("UseCodePreview").ToBool(); }
			set
			{
				Source.SetParameter("UseCodePreview", value.ToString());
				OnPropertyChanged();
				OnPropertyChanged(nameof(Information));
			}
		}

		public override string Title
		{
			get { return string.Format("Eto.Forms {0} Properties", IsLibrary ? "Library" : "Application"); }
			set { base.Title = value; }
		}

		public bool RequiresInput
		{
			get { return SupportsCombined || SupportsProjectType; }
		}

		struct TypeInfo
		{
			public string Text;
			public bool UsePCL;
			public bool UseSAL;
			public bool UseNET;
		}

		static TypeInfo[] typeInformation = {
			new TypeInfo { UseNET = true, Text = "Use the full desktop .NET Framework for shared code and maximum compatibility with existing frameworks." },
			new TypeInfo { UsePCL = true, Text = "Use a PCL (Profile 259) assembly for shared code and maximum portability." },
			new TypeInfo { UseSAL = true, Text = "Use a shared asset library for shared code and maximum flexibility." }
		};

		struct CombinedInfo
		{
			public string Text;
			public bool Combined;
			public bool? SeparateMac;
		}

		static CombinedInfo[] combinedInformation = {
			new CombinedInfo { Combined = true, SeparateMac = true, Text = "A single combined .exe that can run on Windows and Linux and a separate Mac app." },
			new CombinedInfo { Combined = true, SeparateMac = false, Text = "A single combined .exe that can run on Windows and Linux and builds an .app bundle for Mac." },
			new CombinedInfo { Text = "A separate .exe assembly for each platform." },
		};

		struct FormatInfo
		{
			public string Text;
			public bool UseXeto;
			public bool UseJeto;
			public bool UseCode;
			public bool UseCodePreview;
		}

		static FormatInfo[] formatInformation = {
			new FormatInfo { UseXeto = true, Text = "Use xaml (.xeto) to define the layout of your form, with code behind for logic and event handlers" },
			new FormatInfo { UseJeto = true, Text = "Use json (.jeto) to define the layout of your form, with code behind for logic and event handlers" },
			new FormatInfo { UseCode = true, Text = "Write your form entirely in a single source file (no preview)" },
			new FormatInfo { UseCodePreview = true, Text = "Define your view layout in a partial class with form preview, with logic and event handlers in a separate file." },
		};

		public string Information
		{
			get
			{
				var combinedInfo = from i in combinedInformation
								   where i.Combined == Combined
								   && (i.SeparateMac == null || i.SeparateMac == Source.SeparateMac)
								   select i.Text;
				var combinedText = combinedInfo.FirstOrDefault();

				var typeInfo = from i in typeInformation
							   where i.UseNET == UseNET
							   && i.UsePCL == UsePCL
							   && i.UseSAL == UseSAL
							   select i.Text;
				var typeText = typeInfo.FirstOrDefault();

				var formatInfo = from i in formatInformation
						where i.UseCode == UseCode
					&& i.UseCodePreview == UseCodePreview
					&& i.UseXeto == UseXeto
					&& i.UseJeto == UseJeto
					select i.Text;
				var formatText = formatInfo.FirstOrDefault();

				return string.Join("\n\n", combinedText, typeText, formatText);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		void UpdateCommon()
		{
			Source.SetParameter("UseSharedXeto", (UseSAL && UseXeto).ToString());
			Source.SetParameter("UseSharedJeto", (UseSAL && UseJeto).ToString());
		}
	}

}