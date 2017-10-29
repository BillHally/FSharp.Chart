module ChartDesigner.Main

open System
open System.Windows

open FsXaml
open Gjallarhorn.Wpf
open Gjallarhorn.Bindable

open FSharp.Chart.OxyPlot

open ChartDesigner
open ChartDesigner.ViewModels

module WindowsConsole =
    open System.Runtime.InteropServices

    [<DllImport("kernel32.dll", SetLastError=true)>]
    extern bool AttachConsole(uint32 dwProcessId)
    let ATTACH_PARENT_PROCESS = 0xFFFFFFFFu

    let attachToParentConsole () = AttachConsole ATTACH_PARENT_PROCESS

type Application = XAML<"App.xaml">
type MainWindow = XAML<"MainWindow.xaml">

[<STAThread>]
[<EntryPoint>]
let main argv =
    WindowsConsole.attachToParentConsole () |> ignore
    try
        Framework.RunApplication (Application, MainWindow, Program.applicationCore)
        0
    with
    | ex ->
        printfn "%A" ex
        1
