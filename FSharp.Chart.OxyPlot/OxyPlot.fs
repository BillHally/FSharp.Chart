module FSharp.Chart.OxyPlot

open System
open System.Drawing

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
        | Categorical ->
            Axes.CategoryAxis
                (
                    Title      = x.Title.Value,
                    TitleColor = Color.from x.TitleColor,
                    TextColor  = Color.from x.TextColor,
                    Position   = AxisPosition.from x.AxisPosition
                )
            :> Axes.Axis
        | DateTime ->
            Axes.DateTimeAxis
                (
                    Title        = x.Title.Value,
                    TitleColor   = Color.from x.TitleColor,
                    TextColor    = Color.from x.TextColor,
                    Position     = AxisPosition.from x.AxisPosition
                )
            :> Axes.Axis
        | Linear ->
            Axes.LinearAxis
                (
                    Title      = x.Title.Value,
                    TitleColor = Color.from x.TitleColor,
                    TextColor  = Color.from x.TextColor,
                    Position   = AxisPosition.from x.AxisPosition
                )
            :> Axes.Axis
        | TimeSpan ->
            Axes.TimeSpanAxis
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
        | FloatData    xs -> xs
        | DateTimeData xs -> xs |> Array.map Axes.DateTimeAxis.ToDouble
        | TimeSpanData xs -> xs |> Array.map Axes.TimeSpanAxis.ToDouble

    let private toDataPoints =
        function
        | Data1D        ys  -> ys |> toFloats |> Array.mapi (fun i y -> OxyPlot.DataPoint(float i, y))
        | Data1x1D (xs, ys) ->
            let xs = xs |> toFloats
            let ys = ys |> toFloats
            xs |> Array.mapi (fun i x -> OxyPlot.DataPoint(x, ys.[i]))
        | Data2D xys ->
            match xys with
            | FloatFloat       xys -> xys |> Array.map (fun (x, y) -> OxyPlot.DataPoint(                           x,                            y))
            | FloatDateTime    xys -> xys |> Array.map (fun (x, y) -> OxyPlot.DataPoint(                           x, Axes.DateTimeAxis.ToDouble y)) 
            | FloatTimeSpan    xys -> xys |> Array.map (fun (x, y) -> OxyPlot.DataPoint(                           x, Axes.TimeSpanAxis.ToDouble y)) 
            | DateTimeFloat    xys -> xys |> Array.map (fun (x, y) -> OxyPlot.DataPoint(Axes.DateTimeAxis.ToDouble x,                            y)) 
            | DateTimeDateTime xys -> xys |> Array.map (fun (x, y) -> OxyPlot.DataPoint(Axes.DateTimeAxis.ToDouble x, Axes.DateTimeAxis.ToDouble y)) 
            | DateTimeTimeSpan xys -> xys |> Array.map (fun (x, y) -> OxyPlot.DataPoint(Axes.DateTimeAxis.ToDouble x, Axes.TimeSpanAxis.ToDouble y)) 
            | TimeSpanFloat    xys -> xys |> Array.map (fun (x, y) -> OxyPlot.DataPoint(Axes.TimeSpanAxis.ToDouble x,                            y)) 
            | TimeSpanDateTime xys -> xys |> Array.map (fun (x, y) -> OxyPlot.DataPoint(Axes.TimeSpanAxis.ToDouble x, Axes.DateTimeAxis.ToDouble y)) 
            | TimeSpanTimeSpan xys -> xys |> Array.map (fun (x, y) -> OxyPlot.DataPoint(Axes.TimeSpanAxis.ToDouble x, Axes.TimeSpanAxis.ToDouble y)) 

    let private bar color xs =
        let s = OxyPlot.Series.BarSeries(FillColor = color, ItemsSource = xs)
//        s.Items.AddRange (xs |> Array.map (fun d -> OxyPlot.Series.BarItem(d)))
        s :> OxyPlot.Series.XYAxisSeries

    let private column width color xs =
        let s = OxyPlot.Series.ColumnSeries(FillColor = color, ColumnWidth = width, ItemsSource = xs)
//        s.Items.AddRange (xs |> Array.map (fun d -> OxyPlot.Series.ColumnItem(d)))
        s:> OxyPlot.Series.XYAxisSeries

    let private scatter color (xs : DataPoint[]) =
        let s = OxyPlot.Series.ScatterSeries(MarkerFill = color, ItemsSource = xs, DataFieldX = "X", DataFieldY = "Y")
//        s.Points.AddRange (xs |> Array.map (fun dp -> OxyPlot.Series.ScatterPoint(dp.X, dp.Y)))
        s :> OxyPlot.Series.XYAxisSeries

    let ofType x =
        match x with
        | Bar          -> bar
        | BoxPlot      -> failwithf "nyi: %A" x
        | Column width -> column width
        | ErrorColumn  -> failwithf "nyi: %A" x
        | Scatter      -> scatter

    let from (xAxes : Axes.Axis[]) (yAxes : Axes.Axis[]) (x : Series) =

        let ensureAxisKeys baseKey (axes : Axes.Axis[]) =
            axes |> Array.iteri (fun n axis -> if axis.Key = null then axis.Key <- sprintf "%s.%d" baseKey n)
        
        ensureAxisKeys "X" xAxes
        ensureAxisKeys "Y" yAxes

        let series = x.SeriesData.Data |> toDataPoints |> (ofType x.SeriesType) (Color.from x.Color)
        if x.XAxisIndex >= 0 then series.XAxisKey <- xAxes.[x.XAxisIndex].Key
        if x.YAxisIndex >= 0 then series.YAxisKey <- yAxes.[x.YAxisIndex].Key

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
