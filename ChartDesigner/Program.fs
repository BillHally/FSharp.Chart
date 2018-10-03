module ChartDesigner.Main

open System
open System.Windows

open FsXaml
open Gjallarhorn.Wpf
open Gjallarhorn.Bindable

open FSharp.Chart.OxyPlot

open ChartDesigner
open FSharp.IO

module WindowsConsole =
    open System.Runtime.InteropServices

    [<DllImport("kernel32.dll", SetLastError=true)>]
    extern bool AttachConsole(uint32 dwProcessId)
    let ATTACH_PARENT_PROCESS = 0xFFFFFFFFu

    let attachToParentConsole () = AttachConsole ATTACH_PARENT_PROCESS

type Application = XAML<"App.xaml">
type MainWindowBase = XAML<"Views/MainWindow.xaml">

type MainWindow() as this =
    inherit MainWindowBase()

    do
        MainWindow.Instance <- this
        MahApps.Metro.Controls.Dialogs.DialogParticipation.SetRegister(this, this)

    static member val Instance = Unchecked.defaultof<MainWindow> with get, set

let showMessage title text =
    Dialogs.showMessage MainWindow.Instance title text

let writeAllText = File.writeAllText showMessage

[<STAThread>]
[<EntryPoint>]
let main argv =
    let attachedToParentConsole = WindowsConsole.attachToParentConsole ()

    let __ = Application()
    try
        Framework.RunApplication (Navigation.singleViewFromWindow MainWindow, Program.applicationCore (Script.export Dialogs.saveFile writeAllText))
        0
    with
    | ex ->
        if attachedToParentConsole then
            printfn "%A" ex
            1
        else
            reraise ()
