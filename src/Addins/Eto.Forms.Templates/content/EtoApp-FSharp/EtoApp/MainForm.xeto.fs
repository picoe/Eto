#if (UseXeto)
namespace EtoApp

open System
open Eto.Forms
open Eto.Drawing
open Eto.Serialization.Xaml;

type MainForm() as this = 
    inherit Form()

    do 
        XamlReader.Load(this)

    member this.HandleClickMe(sender:obj, e:EventArgs) =
        MessageBox.Show("Hello!") |> ignore

    member this.HandleQuit(sender:obj, e:EventArgs) =
        Application.Instance.Quit()
#endif