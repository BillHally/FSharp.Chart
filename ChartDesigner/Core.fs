module ChartDesigner.Core

open System
open System.Drawing

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

            Background : Color
            PlotAreaBackground : Color

            Title : string
            Subtitle : string
        }
    with
        static member Default =
            {
                ProgramTitle = "Chart Designer"
                Width        = 1000.0
                Height       = 1000.0
                Chart        = ChartDesigner.Models.Examples.boxplot ()

                Title    = "Boxplot"
                Subtitle = "Subtitle"

                Background         = Color.Transparent
                PlotAreaBackground = Color.Transparent
            }

    type Msg =
        | Width    of float
        | Height   of float
        | Title    of string
        | Subtitle of string
        | Background         of Color
        | PlotAreaBackground of Color

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
        | Subtitle x ->
            {
                model with
                    Subtitle = x
                    Chart = { model.Chart with Subtitle = { model.Chart.Subtitle with Value = x } }
            }
        | Background x ->
            {
                model with
                    Background = x
                    Chart = { model.Chart with Background = x }
            }
        | PlotAreaBackground x ->
            {
                model with
                    PlotAreaBackground = x
                    Chart = { model.Chart with PlotAreaBackground = x }
            }


    [<CLIMutable>]
    type ViewModel =
        {
            ProgramTitle : string
            Width        : float
            Height       : float
            PlotModel    : PlotModel

            Title    : string
            Subtitle : string
            Background : Color
            PlotAreaBackground : Color
        }

    let d =
        {
            ProgramTitle = ""
            Width        = 1000.0
            Height       = 1000.0
            PlotModel    = PlotModel()

            Title    = ""
            Subtitle = ""
            Background = Color.Transparent
            PlotAreaBackground = Color.Transparent
        }

    let bindToSource =
        Component.fromBindings<Model, Msg> [
            <@ d.ProgramTitle @> |> Bind.oneWay (fun x -> x.ProgramTitle)
            <@ d.Width        @> |> Bind.twoWay (fun x -> x.Width)  Width  //Validated (fun m -> m.FirstName) notNullOrWhitespace FirstName
            <@ d.Height       @> |> Bind.twoWay (fun x -> x.Height) Height //Validated (fun m -> m.LastName) validLast LastName

            <@ d.Title              @> |> Bind.twoWay (fun x -> x.Title             ) Title
            <@ d.Subtitle           @> |> Bind.twoWay (fun x -> x.Subtitle          ) Subtitle
            <@ d.Background         @> |> Bind.twoWay (fun x -> x.Background        ) Background
            <@ d.PlotAreaBackground @> |> Bind.twoWay (fun x -> x.PlotAreaBackground) PlotAreaBackground

            <@ d.PlotModel    @> |> Bind.oneWay (fun x -> x.Chart |> PlotModel.from)
        ]

    let applicationCore = Framework.basicApplication Model.Default update bindToSource

