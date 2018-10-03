module FSharp.Chart.OxyPlot

open System
open System.Drawing

open OxyPlot

open FSharp.Chart

module Color =
    let from (x : Option<System.Drawing.Color>) =
        match x with
        | Some x -> OxyColor.FromArgb(x.A, x.R, x.G, x.B)
        | None   -> OxyColors.Automatic

module AxisPosition =
    let from =
        function
        | BottomAxis -> Axes.AxisPosition.Bottom
        | LeftAxis   -> Axes.AxisPosition.Left
        | TopAxis    -> Axes.AxisPosition.Top
        | RightAxis  -> Axes.AxisPosition.Right

module FontWeight =
    let from =
        function
        | Bold   -> FontWeights.Bold
        | Normal -> FontWeights.Normal
        | Italic -> FontWeights.Normal // Italic is not supported by Oxyplot

module LegendPosition =
    let from =
        function
        | LeftTop      -> LegendPosition.TopLeft
        | CenterTop    -> LegendPosition.TopCenter
        | RightTop     -> LegendPosition.TopRight
        | RightCenter  -> LegendPosition.RightMiddle
        | RightBottom  -> LegendPosition.BottomRight
        | CenterBottom -> LegendPosition.BottomCenter
        | LeftBottom   -> LegendPosition.BottomLeft
        | LeftCenter   -> LegendPosition.LeftMiddle

module LegendPlacement =
    let from =
        function
        | Inside  -> LegendPlacement.Inside
        | Outside -> LegendPlacement.Outside

module Legend =
    let set (legend : Legend) (plotModel : PlotModel) =
        plotModel.LegendTitle           <- legend.Title.Value
        plotModel.LegendTitleFont       <- legend.Title.Font.Name
        plotModel.LegendTitleFontSize   <- float legend.Title.Font.Size
        plotModel.LegendTitleFontWeight <- FontWeight.from legend.Title.Font.Style

        plotModel.LegendPosition  <- LegendPosition.from  legend.Position
        plotModel.LegendPlacement <- LegendPlacement.from legend.Location

        plotModel.LegendSymbolMargin <-  2.0
        plotModel.LegendSymbolLength <- float legend.Font.Size * 2.0

        plotModel.LegendLineSpacing  <- float legend.Font.Size * 0.75

        plotModel.LegendFont         <- legend.Font.Name
        plotModel.LegendFontSize     <- float legend.Font.Size
        plotModel.LegendFontWeight   <- FontWeight.from legend.Font.Style

        plotModel.LegendPadding <- float legend.Font.Size

module Axis =
    let from categories numberOfSeries (x : Axis) : Axes.Axis =
        let axis =
            match x.AxisType with
            | Categorical ->
                // TODO: group the categories in some way, on order to differentiate between
                //       different categorical axes
                let axis = Axes.CategoryAxis()
                categories |> Array.iter axis.ActualLabels.Add
                axis.GapWidth <- 1.0 / float numberOfSeries
                axis.IsTickCentered <- true
                axis :> Axes.Axis
            | DateTime    -> Axes.DateTimeAxis() :> Axes.Axis
            | Linear      -> Axes.LinearAxis  () :> Axes.Axis
            | TimeSpan    -> Axes.TimeSpanAxis() :> Axes.Axis

        axis.Title           <- x.Title.Value
        axis.TitleFontSize   <- x.Title.Font.Size  |> float
        axis.TitleFontWeight <- x.Title.Font.Style |> FontWeight.from
        axis.TitleColor      <- x.Title.Color      |> Color.from
        axis.TextColor       <- x.TextColor        |> Color.from
        axis.Position        <- x.AxisPosition     |> AxisPosition.from

        x.Minimum |> Option.iter (fun x -> axis.Minimum <- x)
        x.Maximum |> Option.iter (fun x -> axis.Maximum <- x)

        axis

module Series =
    let private toFloats =
        function
        | FloatData    xs -> xs
        | DateTimeData xs -> xs |> Array.map Axes.DateTimeAxis.ToDouble
        | TimeSpanData xs -> xs |> Array.map Axes.TimeSpanAxis.ToDouble

    let private toNamedFloats =
        function
        | NamedFloats xs -> xs

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

    let private toErrorItems =
        function
        | TupleOfArrays (xs, ys) ->
            let xs = xs |> toFloats
            let ys = ys |> toFloats
            xs |> Array.mapi (fun i x -> OxyPlot.Series.ErrorColumnItem(x, ys.[i]))
        | ArrayOfTuples xys ->
            match xys with
            | FloatFloat       xys -> xys |> Array.map (fun (x, y) -> OxyPlot.Series.ErrorColumnItem(                           x,                            y))
            | FloatDateTime    xys -> xys |> Array.map (fun (x, y) -> OxyPlot.Series.ErrorColumnItem(                           x, Axes.DateTimeAxis.ToDouble y))
            | FloatTimeSpan    xys -> xys |> Array.map (fun (x, y) -> OxyPlot.Series.ErrorColumnItem(                           x, Axes.TimeSpanAxis.ToDouble y))
            | DateTimeFloat    xys -> xys |> Array.map (fun (x, y) -> OxyPlot.Series.ErrorColumnItem(Axes.DateTimeAxis.ToDouble x,                            y))
            | DateTimeDateTime xys -> xys |> Array.map (fun (x, y) -> OxyPlot.Series.ErrorColumnItem(Axes.DateTimeAxis.ToDouble x, Axes.DateTimeAxis.ToDouble y))
            | DateTimeTimeSpan xys -> xys |> Array.map (fun (x, y) -> OxyPlot.Series.ErrorColumnItem(Axes.DateTimeAxis.ToDouble x, Axes.TimeSpanAxis.ToDouble y))
            | TimeSpanFloat    xys -> xys |> Array.map (fun (x, y) -> OxyPlot.Series.ErrorColumnItem(Axes.TimeSpanAxis.ToDouble x,                            y))
            | TimeSpanDateTime xys -> xys |> Array.map (fun (x, y) -> OxyPlot.Series.ErrorColumnItem(Axes.TimeSpanAxis.ToDouble x, Axes.DateTimeAxis.ToDouble y))
            | TimeSpanTimeSpan xys -> xys |> Array.map (fun (x, y) -> OxyPlot.Series.ErrorColumnItem(Axes.TimeSpanAxis.ToDouble x, Axes.TimeSpanAxis.ToDouble y))

    let private toBoxPlotItems (categories : string[]) numberOfSeries seriesIndex (xs : BoxPlotItem[]) =
        let offset = - 0.5 + 1.0 / float numberOfSeries * (float seriesIndex + 0.5)

        xs
        |> Array.mapi
            (
                fun i x ->
                    let i =
                        if categories |> Array.isEmpty then
                            i
                        else
                            match x.Category with
                            | Some category ->
                                match categories |> Array.tryFindIndex ((=) category) with
                                | Some n -> n
                                | None   -> failwithf "ERROR: failed to find category \%A in known categories: %A" category categories
                            | None -> failwithf "ERROR: item has no category, but the following categories were specified: %A" categories

                    OxyPlot.Series.BoxPlotItem
                        (
                            float i + offset,

                            x.LowerWhisker,
                            x.BoxBottom,
                            x.Median,
                            x.BoxTop,
                            x.UpperWhisker,

                            Mean     = x.Mean,
                            Outliers = x.Outliers
                        )
            )

    let private bar width color xs =
        let xs = xs |> Array.map (fun value -> OxyPlot.Series.BarItem(value))
        let s = OxyPlot.Series.BarSeries(FillColor = color, BarWidth = width, ItemsSource = xs)
        s :> OxyPlot.Series.XYAxisSeries

    let private boxplot numberOfSeries color xs =
        let boxWidth = 0.8 * (1.0 / float numberOfSeries)
        OxyPlot.Series.BoxPlotSeries(Fill = color, ItemsSource = xs, BoxWidth = boxWidth)
        :> OxyPlot.Series.XYAxisSeries

    let private column width color categories xs =
        let xs =
            xs
            |> Array.map
                (
                    fun (category, value) ->
                        match categories |> Array.tryFindIndex ((=) category) with
                        | None -> OxyPlot.Series.ColumnItem(value)
                        | Some categoryIndex -> OxyPlot.Series.ColumnItem(value, categoryIndex)
                )

        let s = OxyPlot.Series.ColumnSeries(FillColor = color, ColumnWidth = width, ItemsSource = xs)
//        s.Items.AddRange (xs |> Array.map (fun d -> OxyPlot.Series.ColumnItem(d)))
        s :> OxyPlot.Series.XYAxisSeries

    let private errorColumn width color (xs : OxyPlot.Series.ErrorColumnItem[]) =
        let s = OxyPlot.Series.ErrorColumnSeries(FillColor = color, ColumnWidth = width, ItemsSource = xs)
//        s.Items.AddRange (xs |> Array.map (fun d -> OxyPlot.Series.ErrorColumnItem(d)))
        s :> OxyPlot.Series.XYAxisSeries

    let private scatter color (xs : DataPoint[]) =
        OxyPlot.Series.ScatterSeries(MarkerFill = color, ItemsSource = xs, DataFieldX = "X", DataFieldY = "Y")
        :> OxyPlot.Series.XYAxisSeries

    let convert categories color numberOfSeries seriesIndex x =
        match x with
        | Bar         (data, width) -> bar         width          color            (toFloats                                             data     )
        | BoxPlot      data         -> boxplot     numberOfSeries color            (toBoxPlotItems categories numberOfSeries seriesIndex data     )
        | ErrorColumn (data, width) -> errorColumn width          color            (toErrorItems                                         data.Data)
        | Scatter      data         -> scatter                    color            (toDataPoints                                         data.Data)
        | Column      (data, width) -> column      width          color categories (toNamedFloats                                        data.Data)

    let from categories (xAxes : Axes.Axis[]) (yAxes : Axes.Axis[]) numberOfSeries seriesIndex (x : Series) =

        let ensureAxisKeys baseKey (axes : Axes.Axis[]) =
            axes |> Array.iteri (fun n axis -> if axis.Key = null then axis.Key <- sprintf "%s.%d" baseKey n)

        ensureAxisKeys "X" xAxes
        ensureAxisKeys "Y" yAxes

        let series = x.SeriesData |> convert categories (Color.from x.Color) numberOfSeries seriesIndex
        series.Title <- x.Name

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

        let plotModel =
            PlotModel
                (
                    Title = chart.Title.Value,
                    Subtitle = chart.Subtitle.Value,
                    Background = Color.from chart.Background,
                    PlotAreaBackground = Color.from chart.PlotAreaBackground
                )

        plotModel |> Legend.set chart.Legend

        let categories =
            chart.Series
            |> Seq.collect
                (
                    fun series ->
                        match series.SeriesData with
                        | Column (data, _) -> data.Data |> NamedData.getCategories
                        | BoxPlot xs -> xs |> Seq.choose (fun x -> x.Category)
                        | _ -> Seq.empty
                )
            |> Seq.sort
            |> Seq.distinct
            |> Array.ofSeq

        let numberOfSeries = chart.Series.Length

        let xAxes = chart.XAxes |> Array.map (Axis.from categories numberOfSeries) |> setPositionTiers
        xAxes |> Array.iter plotModel.Axes.Add

        let yAxes = chart.YAxes |> Array.map (Axis.from categories numberOfSeries) |> setPositionTiers
        yAxes |> Array.iter plotModel.Axes.Add

        chart.Series
        |> Array.iteri
            (
                fun i x ->
                    x
                    |> Series.from categories xAxes yAxes numberOfSeries i
                    |> plotModel.Series.Add
            )

        plotModel
