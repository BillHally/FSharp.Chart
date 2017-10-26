module ChartDesigner.Main

open System
open System.Windows

open FsXaml
open Gjallarhorn.Wpf

open FSharp.Chart.OxyPlot

open ChartDesigner.Models
open ChartDesigner.ViewModels

type MainWindow = XAML<"MainWindow.xaml">

let createVM () =
    Examples.boxPlotExample ()
    |> PlotModel.from
    |> ChartDesignerViewModel

[<STAThread>]
[<EntryPoint>]
let main argv =
    let vm = createVM ()
    MainWindow(Title = "Chart designer", DataContext = vm).ShowDialog() |> ignore
    0
