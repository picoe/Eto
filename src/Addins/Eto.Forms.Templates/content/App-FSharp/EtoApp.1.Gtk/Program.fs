namespace EtoApp._1.Gtk
module Program =

    open System
    open EtoApp._1

    [<EntryPoint>]
    [<STAThread>]
    let Main(args) = 
        let app = new Eto.Forms.Application(Eto.Platforms.Gtk)
        app.Run(new MainForm())
        0