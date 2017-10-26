namespace ChartDesigner.ViewModels

open System
open OxyPlot


type ChartDesignerViewModel(model : PlotModel) =
    member __.Model = model
