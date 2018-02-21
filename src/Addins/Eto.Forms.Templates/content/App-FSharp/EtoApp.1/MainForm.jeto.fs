#if (UseJeto)
namespace EtoApp._1

open System
open Eto.Forms
open Eto.Drawing
open Eto.Serialization.Json;

type MainForm() as this = 
    inherit Form()

    do 
        JsonReader.Load(this)

#if IsForm
    member this.HandleClickMe(sender:obj, e:EventArgs) =
        MessageBox.Show("Hello!") |> ignore

    member this.HandleAbout(sender:obj, e:EventArgs) =
        let dlg = new AboutDialog()
        dlg.ShowDialog(this) |> ignore

    member this.HandleQuit(sender:obj, e:EventArgs) =
        Application.Instance.Quit()
#endif
#endif