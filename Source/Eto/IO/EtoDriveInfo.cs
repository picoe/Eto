using System;
using System.IO;
using System.Collections.Generic;

namespace Eto.IO
{
	
	public class EtoDriveInfo : EtoDirectoryInfo
	{
		DriveInfo drive;
		DiskDirectoryInfo directory;
		
		public EtoDriveInfo()
		{
		}

		public EtoDriveInfo(DriveInfo drive)
		{
			this.drive = drive;
			this.directory = new DiskDirectoryInfo(drive.RootDirectory);
		}
		
		public override string Name {
			get {
				if (drive != null)
				{
					if (drive.IsReady) return (string.IsNullOrWhiteSpace(drive.VolumeLabel)) ? drive.Name : drive.VolumeLabel;
					else return drive.Name;
				}
				return string.Empty;
			}
		}
		
		public override string FullName {
			get {
				return Name;
			}
		}
		
		public override IEnumerable<EtoFileInfo> GetFiles (IEnumerable<string> patterns)
		{
			yield break;
		}
		
		protected override IEnumerable<EtoDirectoryInfo> GetPathDirectories ()
		{
			if (drive != null) return directory.GetDirectories();
			
			var list = new List<EtoDirectoryInfo>();
			var drives = DriveInfo.GetDrives();
			foreach (var childDrive in drives)
			{
				if (childDrive.IsReady) list.Add(new EtoDriveInfo(childDrive));
			}
			return list;
		}
		
		public override EtoDirectoryInfo GetSubDirectory (string subDirectory)
		{
			if (drive != null) return directory.GetSubDirectory(subDirectory);

			var drives = DriveInfo.GetDrives();
			foreach (var childDrive in drives)
			{
				return new EtoDriveInfo(childDrive);
			}
			return null;
		}
		
		public override IEnumerable<EtoFileInfo> GetFiles ()
		{
			if (drive != null) return directory.GetFiles();
			return new List<EtoFileInfo>();
		}
		
		public override EtoDirectoryInfo Parent {
			get {
				if (drive != null) return new EtoDriveInfo();
				return null;
			}
		}
		
	}
}

