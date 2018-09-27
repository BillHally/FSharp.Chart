namespace ChartDesigner

open System
open System.Drawing

open FSharp.Chart

module Examples =
    let bar () =
        let text x = { Text.Default with Value = x }

        {
            Chart.Default with
                Title    = text "Bar"
                Subtitle = text ""

                XAxes = [| { Axis.DefaultX with Title = text "X-axis" } |]

                YAxes =
                    [|
                        {
                            Axis.DefaultY with
                                Title = text "Categorical y-axis"
                                AxisType = Categorical
                        }
                    |]

                Series =
                    [|
                        {
                            Series.Bar 2.0 (FloatData [| 1.0..10.0 |]) with
                                Color = Some Color.Blue
                        }
                    |]
        }

    let boxplot () =
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

                Series = [| { Series.BoxPlot data with Color = Some Color.AliceBlue } |]

                XAxes =
                    [|
                        {
                            Axis.DefaultX with
                                Minimum = Some  0.5
                                Maximum = Some 10.5
                        }
                    |]

                YAxes =
                    [|
                        {
                            Axis.DefaultY with
                                Minimum = Some    0.0
                                Maximum = Some 1275.0
                        }
                    |]
        }

    let column () =
        let text x = { Text.Default with Value = x }

        {
            Chart.Default with
                Title    = text "Column"
                Subtitle = text ""

                XAxes =
                    [|
                        {
                            Axis.DefaultX with
                                Title = text "Categorical x-axis"
                                AxisType = Categorical
                        }
                    |]

                YAxes = [| { Axis.DefaultY with Title = text "Y axis" } |]

                Series =
                    [|
                        {
                            Series.Column 1.0 (ColumnData [| for x in 1..10 do yield (if x % 2 = 0 then "Even" else "Odd"), float x |]) with
                                Color = Some Color.Blue
                        }
                    |]
        }

    let errorColumn () =
        let text x = { Text.Default with Value = x }

        {
            Chart.Default with
                Title    = text "Error column"
                Subtitle = text ""

                XAxes =
                    [|
                        {
                            Axis.DefaultX with
                                Title = text "Categorical x-axis"
                                AxisType = Categorical
                        }
                    |]

                YAxes = [| { Axis.DefaultY with Title = text "Y axis" } |]

                Series =
                    [|
                        {
                            Series.ErrorColumn 3.0 (ErrorData ([| 20.0..20.0..200.0 |], [| 2.0..2.0..20.0 |])) with
                                Color = Some Color.Green
                        }
                    |]
        }

    let scatter () =
        let text x = { Text.Default with Value = x }

        {
            Chart.Default with
                Title    = text "Title"
                Subtitle = text "Subtitle"

                XAxes = [| { Axis.DefaultX with Title = text "X axis" } |]
                YAxes = [| { Axis.DefaultY with Title = text "Y axis" } |]

                Series =
                    [|
                        {
                            Series.Scatter (ScatterData [| 2.5..5.0..97.5 |]) with
                                Color = Some Color.Red
                        }
                    |]
        }
