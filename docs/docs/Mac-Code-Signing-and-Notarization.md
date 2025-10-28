# Mac Code Signing and Notarization

In order for you to distribute your Mac applications, you need to code sign and notarize them.  More information about this can be found [here](https://developer.apple.com/documentation/security/notarizing_macos_software_before_distribution).  Code signing and notarization require Xcode tools, so this can only be done on a Mac.

Eto.Forms provides msbuild targets that will automatically code sign and notarize your software during the build process for the Mac64 platform target.  These are the steps to follow to get it going:

1. Get an [Apple developer account](https://developer.apple.com/programs/enroll/).
2. Import your code signing and notarization certificates.  For GitHub Actions, [`apple-actions/import-codesign-certs`](https://github.com/apple-actions/import-codesign-certs) has been used with success.
3. (Optional) create an app-specific password for your [apple developer account](https://appleid.apple.com/account/manage) to use for notarization
4. Store your apple notarization credentials in the keychain using altool with the name `AC_PASSWORD` (replace `${{ secrets.* }}` with the appropriate values, or add the secrets to your GitHub Actions project or organization):

    ```sh
    xcrun altool --store-password-in-keychain-item "AC_PASSWORD" -u "${{ secrets.AC_USERNAME }}" -p "${{ secrets.AC_PASSWORD }}"
    ```

5. Set these properties on your .csproj:

    ```xml
    <PropertyGroup Condition="$([MSBuild]::IsOsPlatform(OSX)) AND $(Configuration) == 'Release'">
      <EnableCodeSigning>True</EnableCodeSigning>
      <EnableNotarization>True</EnableNotarization>
      <EnableDmgBuild>True</EnableDmgBuild>
    </PropertyGroup>
    ```

6. Build:

    ```sh
    dotnet build -c Release MyProject.Mac/MyProject.Mac.csproj
    ```

Note that notarization can take a while to get results for stapling the .dmg.  If your app is not stapled it will still run on users machines when connected to the internet. You can disable stapling by setting the `EnableNotarizationStapler` property.  E.g. `<EnableNotarizationStapler>False</EnableNotarizationStapler>`
