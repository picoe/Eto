namespace $rootnamespace$

open System
open Eto.Forms
open Eto.Drawing
open Eto.Serialization.Xaml

type $safeitemname$ () as this =
    inherit $BaseClassName$ ()

    do
        XamlReader.Load(this, "$itemname$.xeto")$Methods$