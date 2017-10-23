module FSharp.Chart.OxyPlot.Wpf.Chart

open System

open OxyPlot.Wpf

open FSharp.Chart
open FSharp.Chart.OxyPlot

let plot (chart : Chart) =
    printfn "plot: %A" chart.Series
    let plotModel = PlotModel.from chart
    System.Windows.Window(Content = PlotView(Model = plotModel)).ShowDialog() |> ignore
