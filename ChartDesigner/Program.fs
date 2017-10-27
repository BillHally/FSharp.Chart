module ChartDesigner.Main

open System
open System.Windows

open FsXaml
open Gjallarhorn.Wpf

open FSharp.Chart.OxyPlot

open ChartDesigner.Models
open ChartDesigner.ViewModels

module WindowsConsole =
    open System.Runtime.InteropServices

    [<DllImport("kernel32.dll", SetLastError=true)>]
    extern bool AttachConsole(uint32 dwProcessId)
    let ATTACH_PARENT_PROCESS = 0xFFFFFFFFu

    let attachToParentConsole () = AttachConsole ATTACH_PARENT_PROCESS

type MainWindow = XAML<"MainWindow.xaml">

let createVM () =
    Examples.boxPlotExample ()
    |> PlotModel.from
    |> ChartDesignerViewModel

[<STAThread>]
[<EntryPoint>]
let main argv =
    WindowsConsole.attachToParentConsole () |> ignore
    try
        let vm = createVM ()
        MainWindow(Title = "Chart designer", DataContext = vm).ShowDialog() |> ignore
        0
    with
    | ex ->
        printfn "%A" ex
        1
