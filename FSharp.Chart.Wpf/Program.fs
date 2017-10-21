module Main

open System
open System.Drawing

open FSharp.Chart

let foo () =
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
                        SeriesData = FloatSeries [| 1.0..10.0 |]
                        SeriesType = Column 3.0
                        Color      = Color.Blue
                }

                {
                    Series.Default with
                        SeriesData = FloatSeries [| 2.5..5.0..97.5 |]
                        SeriesType = Scatter
                        Color      = Color.Red
                        XAxisIndex = 1
                }
            |]
    }
    |> FSharp.Chart.OxyPlot.Wpf.Chart.plot

[<STAThread>]
[<EntryPoint>]
let main argv =
    foo ()
    0
