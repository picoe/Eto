namespace EtoApp._1.macOS
module Program =

    open System
    open EtoApp._1

    [<EntryPoint>]
    [<STAThread>]
    let Main(args) = 
        let app = new Eto.Forms.Application(Eto.Platforms.macOS)
        app.Run(new MainForm())
        0