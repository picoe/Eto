namespace ${Namespace}

open System
open Eto.Forms
open Eto.Drawing
open Eto.Serialization.Json;

type ${EscapedIdentifier} () as this =
    inherit Dialog ()

    do
        JsonReader.Load (this)
