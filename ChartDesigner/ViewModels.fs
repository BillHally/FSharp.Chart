namespace ChartDesigner.ViewModels

open System
open OxyPlot

type ChartDesignerViewModel(title : string, model : PlotModel) =
    member __.Title = title
    member __.Model = model
