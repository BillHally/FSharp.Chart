namespace ChartDesigner.Models

open System
open System.Drawing

open FSharp.Chart

module Examples =
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




