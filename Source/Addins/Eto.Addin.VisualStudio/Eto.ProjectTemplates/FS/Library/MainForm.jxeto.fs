namespace $safeprojectname$

open System
open Eto.Forms
open Eto.Drawing
open Eto.Serialization.$SerializationType$

type $safeitemname$ () as this =
    inherit Form ()

    do
        $SerializationType$Reader.Load(this, "$itemname$.$ext$")

    member this.HandleClickMe(sender: Object, e: EventArgs) =
        MessageBox.Show("I was clicked!")
        |> ignore

    member this.HandleQuit(sender: Object, e: EventArgs) =
        Application.Instance.Quit()
        |> ignore