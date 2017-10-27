module ChartDesigner.Core

open Gjallarhorn
open Gjallarhorn.Bindable
open Gjallarhorn.Bindable.Framework
open Gjallarhorn.Validation
open Gjallarhorn.Validation.Validators

open FSharp.Chart
open FSharp.Chart.OxyPlot

open ChartDesigner.Models

module Program =
    open OxyPlot

    type Model =
        {
            ProgramTitle : string
            Width        : float
            Height       : float
            Chart        : Chart

            Title : string
        }
    with
        static member Default =
            {
                ProgramTitle = "Chart Designer"
                Width        = 1000.0
                Height       = 1000.0
                Chart        = ChartDesigner.Models.Examples.boxplot ()
                Title        = "Boxplot"
            }

    type Msg =
        | Width  of float
        | Height of float
        | Title  of string

    let update msg (model : Model) =
        match msg with
        | Width  x -> { model with Width  = x }
        | Height x -> { model with Height = x }
        | Title  x ->
            {
                model with
                    Title = x
                    Chart = { model.Chart with Title = { model.Chart.Title with Value = x } }
            }

    [<CLIMutable>]
    type ViewModel =
        {
            ProgramTitle : string
            Width        : float
            Height       : float
            PlotModel    : PlotModel

            Title : string
        }

    let d =
        {
            ProgramTitle = ""
            Width        = 1000.0
            Height       = 1000.0
            PlotModel    = PlotModel()

            Title = ""
        }

    let bindToSource =
        Component.fromBindings<Model, Msg> [
            <@ d.ProgramTitle @> |> Bind.oneWay (fun x -> x.ProgramTitle)
            <@ d.Width        @> |> Bind.twoWay (fun x -> x.Width)  Width  //Validated (fun m -> m.FirstName) notNullOrWhitespace FirstName
            <@ d.Height       @> |> Bind.twoWay (fun x -> x.Height) Height //Validated (fun m -> m.LastName) validLast LastName

            <@ d.Title        @> |> Bind.twoWay (fun x -> x.Title) Title

            <@ d.PlotModel    @> |> Bind.oneWay (fun x -> x.Chart |> PlotModel.from)
        ]

    let applicationCore = Framework.basicApplication Model.Default update bindToSource

