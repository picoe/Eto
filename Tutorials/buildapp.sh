#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

target_dir="$1"
assembly="$2"
project_name="$3"

eto_dir="$DIR/.."
eto_bin_dir="$eto_dir/BuildOutput/Release"

input_app="$eto_dir/Resources/MacAppTemplate.app"
output_app="$target_dir/$project_name.app"

output_res="$output_app/Contents/Resources"
output_macos="$output_app/Contents/MacOS"

# copy Eto files to output for Gtk & Windows platforms
cp "$eto_bin_dir"/Eto.dll "$target_dir"
cp "$eto_bin_dir"/Eto.Platform.GTK.dll "$target_dir"
cp "$eto_bin_dir"/Eto.Platform.Windows.dll "$target_dir"
cp "$eto_bin_dir"/Eto.Platform.Wpf.dll "$target_dir"


# copy MacAppTemplate.app to our new .app bundle
rm -Rf "$output_app"
cp -Rf "$input_app" "$output_app"

# copy assemblies into .app bundle
cp "$target_dir"/$assembly "$output_res"
cp "$target_dir"/*.dll "$output_res"
rm -Rf "$output_res"/Eto.*

# copy Eto Mac platform into .app bundle
cp "$eto_bin_dir"/Eto.dll "$output_res"
cp "$eto_bin_dir"/Eto.Platform.Mac.dll "$output_res"
cp "$eto_bin_dir"/MonoMac.dll "$output_res"

# update launcher to execute our executable
sed -i "" -e "s/\"MyApp.exe\"/\"$assembly\"/" "$output_macos/Launcher"
sed -i "" -e "s/\"MyApp\"/\"$project_name\"/" "$output_macos/Launcher"

# update Info.plist to use our project name as the app name
sed -i "" -e "s/>MyApp</>$project_name</" "$output_app/Contents/Info.plist"

