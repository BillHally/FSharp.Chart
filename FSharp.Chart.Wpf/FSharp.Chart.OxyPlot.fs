module FSharp.Chart.OxyPlot

open System
open System.Drawing
open System.Windows.Controls

open OxyPlot

open FSharp.Chart

module Color =
    let from (x : System.Drawing.Color) = OxyColor.FromArgb(x.A, x.R, x.G, x.B)

module AxisPosition =
    let from =
        function
        | AxisPosition.Bottom -> Axes.AxisPosition.Bottom
        | AxisPosition.Left   -> Axes.AxisPosition.Left
        | AxisPosition.Top    -> Axes.AxisPosition.Top
        | AxisPosition.Right  -> Axes.AxisPosition.Right

module Axis =
    let from (x : Axis) : Axes.Axis =
        match x.AxisType with
        | Linear ->
            Axes.LinearAxis
                (
                    Title      = x.Title.Value,
                    TitleColor = Color.from x.TitleColor,
                    TextColor  = Color.from x.TextColor,
                    Position   = AxisPosition.from x.AxisPosition
                )
            :> Axes.Axis
        | Categorical ->
            Axes.CategoryAxis
                (
                    Title      = x.Title.Value,
                    TitleColor = Color.from x.TitleColor,
                    TextColor  = Color.from x.TextColor,
                    Position   = AxisPosition.from x.AxisPosition
                )
            :> Axes.Axis

module Series =
    let private toFloats =
        function
        | FloatSeries    xs -> xs
        | DateTimeSeries xs -> xs |> Array.map Axes.DateTimeAxis.ToDouble
        | TimeSpanSeries xs -> xs |> Array.map Axes.TimeSpanAxis.ToDouble

    let private bar color xs =
        let s = OxyPlot.Series.BarSeries(FillColor = color)
        s.Items.AddRange (xs |> Array.map (fun d -> OxyPlot.Series.BarItem(d)))
        s :> OxyPlot.Series.XYAxisSeries

    let private column width color xs =
        let s = OxyPlot.Series.ColumnSeries(FillColor = color, ColumnWidth = width)
        s.Items.AddRange (xs |> Array.map (fun d -> OxyPlot.Series.ColumnItem(d)))
        s:> OxyPlot.Series.XYAxisSeries

    let private scatter color xs =
        let s = OxyPlot.Series.ScatterSeries(MarkerFill = color)
        s.Points.AddRange (xs |> Array.mapi (fun i d -> OxyPlot.Series.ScatterPoint(float i, d)))
        s :> OxyPlot.Series.XYAxisSeries

    let ofType x =
        match x with
        | Bar          -> bar
        | BoxPlot      -> failwithf "nyi: %A" x
        | Column width -> column width
        | ErrorColumn  -> failwithf "nyi: %A" x
        | Scatter      -> scatter

    let from (xAxes : Axes.Axis[]) (yAxes : Axes.Axis[]) (x : Series) =
        let series = x.SeriesData |> toFloats |> (ofType x.SeriesType) (Color.from x.Color)
        series.XAxisKey <- xAxes.[x.XAxisIndex].Key
        series.YAxisKey <- yAxes.[x.YAxisIndex].Key
        series

module PlotModel =
    let private setPositionTiers (xs : Axes.Axis[]) =

        let filterByPosition (p : Axes.AxisPosition) (xs : Axes.Axis[]) =
            xs |> Array.filter (fun x -> x.Position = p)
        
        let setPositionTier (xs : Axes.Axis[]) =
            xs |> Array.iteri (fun n x -> x.PositionTier <- n)

        xs |> filterByPosition Axes.AxisPosition.Top    |> setPositionTier
        xs |> filterByPosition Axes.AxisPosition.Left   |> setPositionTier
        xs |> filterByPosition Axes.AxisPosition.Bottom |> setPositionTier
        xs |> filterByPosition Axes.AxisPosition.Right  |> setPositionTier

        xs

    let from (chart : Chart) =
        let plotModel = PlotModel(Title = chart.Title.Value, Subtitle = chart.Subtitle.Value)

        let xAxes = chart.XAxes |> Array.map Axis.from |> setPositionTiers
        xAxes |> Array.iter plotModel.Axes.Add

        let yAxes = chart.YAxes |> Array.map Axis.from |> setPositionTiers
        yAxes |> Array.iter plotModel.Axes.Add

        chart.Series |> Array.iter (Series.from xAxes yAxes >> plotModel.Series.Add)

        plotModel
