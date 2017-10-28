module Main

open System
open System.Drawing

open FSharp.Chart

let doubleSeriesExample () =
    let text x = { Text.Default with Value = x }

    {
        Chart.Default with
            Title    = text "Title"
            Subtitle = text "Subtitle"

            XAxes =
                [|
                    {
                        Axis.DefaultX with
                            Title = text "Categorical x-axis"
                            AxisType = Categorical
                    }

                    { Axis.DefaultX with Title = text "X axis" }
                |]

            YAxes = [| { Axis.DefaultY with Title = text "Y axis" } |]

            Series =
                [|
                    {
                        Series.Column 3.0 (BasicData [| 1.0..10.0 |]) with
                            Color = Color.Blue
                    }

                    {
                        Series.Scatter (BasicData [| 2.5..5.0..97.5 |]) with
                            Color = Color.Red
                            XAxisIndex = 1
                    }
                |]
    }
    |> FSharp.Chart.OxyPlot.Wpf.Chart.plot

let dateTimeTimeSpanExample () =
    let text x = { Text.Default with Value = x }

    let data =
        BasicData
            (
                ([| 1  ..10   |] |> Array.map (fun x -> System.DateTime(2017, 10, x))),
                ([| 1.0..10.0 |] |> Array.map TimeSpan.FromMinutes)
            )

    {
        Chart.Default with
            Title = text "DateTime vs. TimeSpan"

            XAxes = [| { Axis.DefaultX with Title = text "X axis"; AxisType = AxisType.DateTime } |]
            YAxes = [| { Axis.DefaultY with Title = text "Y axis"; AxisType = AxisType.TimeSpan } |]

            Series = [| { Series.Scatter data with Color = Color.Blue } |]
    }
    |> FSharp.Chart.OxyPlot.Wpf.Chart.plot

let boxPlotExample () =
    let text x = { Text.Default with Value = x }

    let data =
        [| 100.0..100.0..1000.0 |]
        |> Array.map
            (
                fun x ->
                    {
                        UpperWhisker = x
                        BoxTop       = x * 0.9
                        Median       = x * 0.6
                        Mean         = x * 0.5
                        BoxBottom    = x * 0.4
                        LowerWhisker = x * 0.3
                        Outliers     = [| 0.2 * x; 0.25 * x; 1.2 * x; 1.25 * x |]
                    }
            )


    {
        Chart.Default with
            Title = text "Boxplot"

            Series = [| { Series.BoxPlot data with Color = Color.AliceBlue } |]
    }
    |> FSharp.Chart.OxyPlot.Wpf.Chart.plot

[<STAThread>]
[<EntryPoint>]
let main argv =
    doubleSeriesExample ()
    dateTimeTimeSpanExample ()
    boxPlotExample ()

    0
