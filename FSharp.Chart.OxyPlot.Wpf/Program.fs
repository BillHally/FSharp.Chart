module Main

open System
open System.Drawing

open FSharp.Chart

let doubleSeriesExample () =
    let text x = { Text.Default with Value = x }

    {
        Title    = text "Title"
        Subtitle = text "Subtitle"

        XAxes =
            [|
                {
                    Axis.DefaultX with
                        Title = text "Categorical x-axis"
                        AxisType = Categorical
                }

                {
                    Axis.DefaultX with
                        Title = text "X axis"
                }
            |]
                    
        YAxes =
            [|
                {
                    Axis.DefaultY with
                        Title = text "Y axis"
                }
            |]

        Series =
            [|
                {
                    Series.Default with
                        SeriesData = SeriesData [| 1.0..10.0 |]
                        SeriesType = Column 3.0
                        Color      = Color.Blue
                }

                {
                    Series.Default with
                        SeriesData = SeriesData [| 2.5..5.0..97.5 |]
                        SeriesType = Scatter
                        Color      = Color.Red
                        XAxisIndex = 1
                }
            |]
    }
    |> FSharp.Chart.OxyPlot.Wpf.Chart.plot

let dateTimeTimeSpanExample () =
    let text x = { Text.Default with Value = x }

    {
        Chart.Default with
            Title = text "DateTime vs. TimeSpan"

            XAxes = [| { Axis.DefaultX with Title = text "X axis"; AxisType = AxisType.DateTime } |]
            YAxes = [| { Axis.DefaultY with Title = text "Y axis"; AxisType = AxisType.TimeSpan } |]

            Series =
                [|
                    {
                        Series.Default with
                            SeriesData =
                                SeriesData
                                    (
                                        ([| 1  ..10   |] |> Array.map (fun x -> System.DateTime(2017, 10, x))),
                                        ([| 1.0..10.0 |] |> Array.map TimeSpan.FromMinutes)
                                    )
                            SeriesType = Scatter
                            Color      = Color.Blue
                            XAxisIndex = 0
                            YAxisIndex = 0
                    }
                |]
    }
    |> FSharp.Chart.OxyPlot.Wpf.Chart.plot

[<STAThread>]
[<EntryPoint>]
let main argv =
    //doubleSeriesExample ()
    dateTimeTimeSpanExample ()
    0
