// Guids.cs
// MUST match guids.h
using System;

namespace Eto.Addin.VisualStudio
{
    static class Constants
    {
		public const string VersionString = "2.0";

		public const string EtoPreviewPackagePkg_string = "BBAB498D-6FDF-44A3-83EE-A17F909DBF14";
		public const string EtoPreviewEditorFactory_string = "ADBAE4CA-4087-4C6A-8AF3-330CF3C2E78D";
		public const string EtoPreviewEditorFactoryLogicalView_string = "BA7DFDDD-BE36-4F4C-9D7B-3B864BD1C76A";

		public static readonly Guid EtoPreviewEditorFactoryLogicalView_guid = new Guid(EtoPreviewEditorFactoryLogicalView_string);

        public static readonly Guid EtoPreviewEditorFactory_guid = new Guid(EtoPreviewEditorFactory_string);

    };
}