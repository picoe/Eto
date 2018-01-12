
This package will create a macOS application that runs on OS X using Eto.Forms.

If you are using Xamarin Studio on OS X, you can use a Xamarin.Mac project to embed
the mono runtime with your app.  This template creates an app that will require the user
to install the mono runtime.

The nuget package automatically adds a targets file to your project, which will
package up your application into $(TargetName).app in the output directory.

Next Steps:

Add Mac/Icon.icns to an icon for your app

Add Mac/Info.plist and set these properties:
	
  - MonoBundleExecutable:       Name of your exe to launch (set automatically to your .exe)
  - CFBundleName:               Short name of your application to display on the menu bar (set automatically to the project name)
  - CFBundleIdentifier:         An identifier string that specifies the app type of the
                                bundle. The string should be in reverse DNS format using
                                only the Roman alphabet in upper and lower case
                                (A–Z, a–z), the dot (“.”), and the hyphen (“-”).
  - CFBundleShortVersionString: The release-version-number string for the bundle. 
  - CFBundleVersion:            The build-version-number string for the bundle
  - NSHumanReadableCopyright:   Copyright of your app
  - CFBundleIconFile:           Name of your application's icns file

Notes:

  - The MacOS/MyProject must have the executable bit set.  Simply zipping the .app folder
    and unzipping on OS X will set things up properly.