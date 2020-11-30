namespace EtoApp.1.Wpf
module Program =

    open System
    open EtoApp.1

    [<EntryPoint>]
    [<STAThread>]
    let Main(args) = 
        let app = new Eto.Forms.Application(Eto.Platforms.Wpf)
        app.Run(new MainForm())
        0