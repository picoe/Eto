using System;
using Eto.Forms;
using Eto.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

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
			Combined = true;
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

		public bool Combined
		{
			get { return Source.GetParameter("Combined").ToBool(); }
			set
			{
				Source.SetParameter("Combined", value.ToString());
				OnPropertyChanged(nameof(Combined));
				OnPropertyChanged(nameof(Information));
			}
		}

		public bool UsePCL
		{
			get { return Source.GetParameter("UsePCL").ToBool(); }
			set
			{
				Source.SetParameter("UsePCL", value.ToString());
				OnPropertyChanged(nameof(Information));
			}
		}

		public bool UseSAL
		{
			get { return Source.GetParameter("UseSAL").ToBool(); }
			set
			{
				Source.SetParameter("UseSAL", value.ToString());
				OnPropertyChanged(nameof(Information));
			}
		}

		public bool UseNET
		{
			get { return Source.GetParameter("UseNET").ToBool(); }
			set
			{
				Source.SetParameter("UseNET", value.ToString());
				OnPropertyChanged(nameof(UseNET));
				OnPropertyChanged(nameof(Information));
			}
		}

		public string Title
		{
			get { return string.Format("Eto.Forms {0} Properties", IsLibrary ? "Library" : "Application"); }
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

				return string.Join("\n\n", combinedText, typeText);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}