module FSharp.Chart.OxyPlot.Wpf.Chart

open System

open OxyPlot.Wpf

open FSharp.Chart
open FSharp.Chart.OxyPlot

let plot (chart : Chart) =
    let plotModel = PlotModel.from chart
    System.Windows.Window(Content = PlotView(Model = plotModel)).ShowDialog() |> ignore
