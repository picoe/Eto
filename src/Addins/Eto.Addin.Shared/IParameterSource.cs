using System;
using Eto.Forms;
using Eto.Drawing;
using System.ComponentModel;

namespace Eto.Addin.Shared
{
	public interface IParameterSource
	{
        Version TargetFrameworkVersion { get; }

		bool SeparateMac { get; }

		bool IsSupportedParameter(string parameter);

		string GetParameter(string parameter);

		void SetParameter(string parameter, string value);
	}
	
}