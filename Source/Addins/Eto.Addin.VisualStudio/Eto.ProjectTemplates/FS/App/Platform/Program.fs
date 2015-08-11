namespace $safeprojectname$
module Program =

    open System
    open $root.safeprojectname$

    [<EntryPoint>]
    [<STAThread>]
    let Main(args) = 
        let app = new Eto.Forms.Application($EtoPlatform$)
        app.Run(new MainForm())
        0