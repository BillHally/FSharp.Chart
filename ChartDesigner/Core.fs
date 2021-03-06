﻿namespace ChartDesigner

open System
open System.Drawing

open Gjallarhorn
open Gjallarhorn.Bindable
open Gjallarhorn.Bindable.Framework
open Gjallarhorn.Validation
open Gjallarhorn.Validation.Validators

open OxyPlot

open FSharp.Chart
open FSharp.Chart.OxyPlot
open OxyPlot.Wpf

module Chart =
    let setPlotAreaBackground plotAreaBackground x =
        { x with PlotAreaBackground = plotAreaBackground }

    let setBackground background x =
        { x with Background = background }

    let setSubtitle subtitle x =
        { x with Subtitle = { x.Subtitle with Value = subtitle } }

    let setTitle title x : Chart =
        { x with Title = { x.Title with Value = title } }

type SeriesType =
    | BarSeries
    | BoxPlotSeries
    | ColumnSeries
    | ErrorColumnSeries
    | ScatterSeries
    | AreaSeries
    | LineSeries

    static member Items =
        [|
            BarSeries
            BoxPlotSeries
            ColumnSeries
            ErrorColumnSeries
            ScatterSeries
            AreaSeries
            LineSeries
        |]

module Program =

    let getExample =
        function
        | BarSeries         -> Examples.bar         ()
        | BoxPlotSeries     -> Examples.boxplot     ()
        | ColumnSeries      -> Examples.column      ()
        | ErrorColumnSeries -> Examples.errorColumn ()
        | ScatterSeries     -> Examples.scatter     ()
        | AreaSeries        -> Examples.area        ()
        | LineSeries        -> Examples.line        ()

    type Model =
        {
            ProgramTitle : string
            Width        : float
            Height       : float
            Chart        : Chart
            SeriesType   : SeriesType

            Background : Color
            PlotAreaBackground : Color

            Title : string
            Subtitle : string
        }
    with
        static member Default =
            {
                ProgramTitle = "Chart Designer"
                Width        = 1500.0
                Height       = 1000.0
                SeriesType   = BoxPlotSeries

                Chart = ChartDesigner.Examples.boxplot ()

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
        | SeriesType of SeriesType
        | Export

    let update export msg (model : Model) =
        match msg with
        | Export   -> export model.Chart; model
        | Width  x -> { model with Width  = x }
        | Height x -> { model with Height = x }
        | Title  x ->
            {
                model with
                    Title = x
                    Chart = Chart.setTitle x model.Chart
            }
        | Subtitle x ->
            {
                model with
                    Subtitle = x
                    Chart = Chart.setSubtitle x model.Chart
            }
        | Background x ->
            {
                model with
                    Background = x
                    Chart = Chart.setBackground (Some x) model.Chart
            }
        | PlotAreaBackground x ->
            {
                model with
                    PlotAreaBackground = x
                    Chart = Chart.setPlotAreaBackground (Some x) model.Chart
            }
        | SeriesType x ->
            {
                model with
                    SeriesType = x
                    Chart =
                        x
                        |> getExample
                        |> Chart.setTitle              model.Title
                        |> Chart.setSubtitle           model.Subtitle
                        |> Chart.setBackground         (Some model.Background)
                        |> Chart.setPlotAreaBackground (Some model.PlotAreaBackground)
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
            SeriesType : SeriesType

            ExportCommand : VmCmd<Msg>
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
            SeriesType = BoxPlotSeries

            ExportCommand = Vm.cmd Export
        }

    let bindToSource =
        Component.create [
            <@ d.ProgramTitle @> |> Bind.oneWay (fun (x : Model) -> x.ProgramTitle)
            <@ d.Width        @> |> Bind.twoWay (fun x -> x.Width)  Width  //Validated (fun m -> m.FirstName) notNullOrWhitespace FirstName
            <@ d.Height       @> |> Bind.twoWay (fun x -> x.Height) Height //Validated (fun m -> m.LastName) validLast LastName

            <@ d.Title              @> |> Bind.twoWay (fun x -> x.Title             ) Title
            <@ d.Subtitle           @> |> Bind.twoWay (fun x -> x.Subtitle          ) Subtitle
            <@ d.Background         @> |> Bind.twoWay (fun x -> x.Background        ) Background
            <@ d.PlotAreaBackground @> |> Bind.twoWay (fun x -> x.PlotAreaBackground) PlotAreaBackground
            <@ d.SeriesType         @> |> Bind.twoWay (fun x -> x.SeriesType        ) SeriesType

            <@ d.PlotModel    @> |> Bind.oneWay (fun x -> x.Chart |> PlotModel.from)

            <@ d.ExportCommand @> |> Bind.cmd
        ]

    let applicationCore export = Framework.application Model.Default (update export) bindToSource Nav.empty

