namespace EtoApp._1.Desktop
module Program =

    open System
    open EtoApp._1

    [<EntryPoint>]
    [<STAThread>]
    let Main(args) = 
        let app = new Eto.Forms.Application(Eto.Platform.Detect)
        app.Run(new MainForm())
        0