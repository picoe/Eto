
Next Steps:

1. Copy your .exe, .dll's, and any external resources here

  - Make sure to include Eto.dll, Eto.Mac.dll, and MonoMac.dll
	
2. Modify Contents/Info.plist and update these properties:
	
  - MonoBundleExecutable:       Name of your exe to launch
  - CFBundleName:               Short name of your application to display on the menu bar
  - CFBundleIdentifier:         An identifier string that specifies the app type of the
                                bundle. The string should be in reverse DNS format using
                                only the Roman alphabet in upper and lower case
                                (A–Z, a–z), the dot (“.”), and the hyphen (“-”).
  - CFBundleShortVersionString: The release-version-number string for the bundle. 
  - CFBundleVersion:            The build-version-number string for the bundle
  - NSHumanReadableCopyright:   Copyright of your app
  - CFBundleIconFile:           Name of your application's icns file

Notes:

  - The MacOS/Launcher must have the executable bit set.  Simply zipping the .app folder
    and unzipping on OS X will set things up properly.