namespace $rootnamespace$

open System
open Eto.Forms
open Eto.Drawing
open Eto.Serialization.Json

type $safeitemname$ () as this =
    inherit $BaseClassName$ ()

    do
        JsonReader.Load(this, "$itemname$.jeto")$Methods$