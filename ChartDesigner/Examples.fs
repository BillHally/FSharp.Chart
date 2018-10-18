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

    let colors =
        [|
            Color.AliceBlue
            Color.LightBlue
            Color.LightGreen
            Color.LightPink
            Color.LightCoral
        |]

    let boxplot () =
        let text x = { Text.Default with Value = x }

        let series =
            [| 100.0..100.0..400.0 |]
            |> Array.mapi
                (
                    fun i x ->
                        let data =
                            [|
                                {
                                    Category     = Some "Category A"
                                    UpperWhisker = x
                                    BoxTop       = x * 0.9
                                    Median       = x * 0.6
                                    Mean         = x * 0.5
                                    BoxBottom    = x * 0.4
                                    LowerWhisker = x * 0.3
                                    Outliers     = [| 0.2 * x; 0.25 * x; 1.2 * x; 1.25 * x |]
                                }

                                (
                                    let x = x * 1.1

                                    {
                                        Category     = Some "Category B"
                                        UpperWhisker = x
                                        BoxTop       = x * 0.9
                                        Median       = x * 0.6
                                        Mean         = x * 0.5
                                        BoxBottom    = x * 0.4
                                        LowerWhisker = x * 0.3
                                        Outliers     = [| 0.2 * x; 0.25 * x; 1.2 * x; 1.25 * x |]
                                    }
                                )
                            |]

                        { Series.BoxPlot data with Name = sprintf "Series %d" i; Color = Some colors.[i] }
                )

        {
            Chart.Default with
                Title = text "Boxplot"

                Series = series

                XAxes =
                    [|
                        {
                            Axis.DefaultX with
                                AxisType = Categorical
                        }
                    |]

                YAxes =
                    [|
                        {
                            Axis.DefaultY with
                                Minimum = Some   0.0
                                Maximum = Some 500.0
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
                        { Series.Column 1.0 (ColumnData [| "Category A", 1.0 ; "Category B", 2.1   |]) with Name = "Series 1" }
                        { Series.Column 1.0 (ColumnData [| "Category A", 1.05; "Category B", 1.9   |]) with Name = "Series 2" }
                        { Series.Column 1.0 (ColumnData [| "Category A", 1.1 ; "Category B", 1.95  |]) with Name = "Series 3" }
                        { Series.Column 1.0 (ColumnData [| "Category A", 1.2 ; "Category B", 2.15  |]) with Name = "Series 4" }
                        { Series.Column 1.0 (ColumnData [| "Category A", 1.13; "Category B", 2.18  |]) with Name = "Series 5" }
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
                                Name = "Some scatter points"
                                Color = Some Color.Red
                        }
                    |]
        }

    let area () =
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
                            Series.Area
                                {
                                    AreaTop    = FloatFloat [| 1.0, 1.0; 2.0, 3.0; 3.0, 2.5 |]
                                    AreaBottom = FloatFloat [| 1.0, 1.0; 2.5, 1.0; 2.8, 2.5 |]
                                } with
                                Name  = "An area"
                                Color = Some Color.LightGreen
                        }
                    |]
        }

    let line () =
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
                            Series.Line (FloatFloat [| 1.0, 2.0; 2.0, 3.0; 3.0, 2.5 |]) with
                                Name  = "A line"
                                Color = Some Color.PaleGoldenrod
                        }
                    |]
        }
