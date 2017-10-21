namespace FSharp.Chart.OxyPlot.Wpf

open System

open OxyPlot

open FSharp.Chart


module Chart =
    let plot (chart : Chart) (data : ChartData) =
        printfn "plotting: %A %A" chart data

    let foo () =
        let data = CDFloat [| 1.0..10.0 |]

        let chart =
            {
                Title = { Title = "A chart"; Font = { Name = "Segoe UI"; Size = 15; Style = FontStyle.Normal } }

                Axes =
                    [|
                        {
                            Title = { Title = "X axis"; Font = { Name = "Segoe UI"; Size = 15; Style = FontStyle.Normal } }
                            AxisDirection = AxisDirection.Horizontal
                        }
                            
                        {
                            Title = { Title = "Y axis"; Font = { Name = "Segoe UI"; Size = 15; Style = FontStyle.Normal } }
                            AxisDirection = AxisDirection.Vertical
                        }
                    |]
            }

        plot chart data
