#if (UseJeto)
namespace EtoApp

open System
open Eto.Forms
open Eto.Drawing
open Eto.Serialization.Json;

type MainForm() as this = 
    inherit Form()

    do 
        JsonReader.Load(this)

    member this.HandleClickMe(sender:obj, e:EventArgs) =
        MessageBox.Show("Hello!") |> ignore

    member this.HandleQuit(sender:obj, e:EventArgs) =
        Application.Instance.Quit()
#endif