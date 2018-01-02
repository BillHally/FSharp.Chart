module ChartDesigner.Tests.TestScript

open System
open System.Drawing

open FSharp.Chart
open ChartDesigner

open NUnit.Framework
open FsUnitTyped

module String =
    let split (separator : string) (s : string) = s.Split([| separator |], StringSplitOptions.None)

[<Test>]
let ``colorToScript when passed a KnownColor returns the expected String`` () =
    Color.AliceBlue
    |> Script.colorToScript
    |> shouldEqual "Color.AliceBlue"

[<Test>]
let ``colorToScript when passed a non-KnownColor returns the expected String`` () =
    Color.FromArgb(1, 2, 3, 4)
    |> Script.colorToScript
    |> shouldEqual "Color.FromArgb(1, 2, 3, 4)"

[<Test>]
let ``axisToScript when passed Axis.DefaultX returns the expected String`` () =
    Axis.DefaultX
    |> Script.axisToScript
    |> shouldEqual "Axis.DefaultX"

[<Test>]
let ``axisToScript when passed Axis.DefaultY returns the expected String`` () =
    Axis.DefaultY
    |> Script.axisToScript
    |> shouldEqual "Axis.DefaultY"

[<Test>]
let ``axisToScript when passed a non-default axis returns the expected String`` () =
    {
        Title =
            {
                Text.Value = "Axis title"
                Font = Font.Default
                Color = Color.Black
            }

        AxisPosition = Top
        TextColor = Color.Orange
        AxisType = Categorical
        Minimum = Some 0.1
        Maximum = Some 5.0
    }
    |> Script.axisToScript
    |> shouldEqual """{
                    AxisPosition = Top
                    Title        = { Value = "Axis title"; Font = Font.Default; Color = Color.Black }
                    TextColor    = Color.Orange
                    AxisType     = Categorical
                    Minimum      = Some 0.100
                    Maximum      = Some 5.000
                }"""

[<Test>]
let ``floatOptionToScript when passed none returns the expected String`` () =
    None
    |> Script.floatOptionToScript
    |> shouldEqual "None"

[<Test>]
let ``floatOptionToScript when passed "Some x" returns the expected String`` () =
    Some 0.123
    |> Script.floatOptionToScript
    |> shouldEqual "Some 0.123"

[<Test>]
let ``toScript when passed a Chart returns the expected String`` () =
    {
        Title = { Value = "The title"; Font = Font.Default; Color = Color.Black }
        Subtitle = { Value = "The subtitle"; Font = Font.Default; Color = Color.Black }
        Background = Color.FromArgb(1, 2, 3, 4)
        PlotAreaBackground = Color.FromArgb(5, 6, 7, 8)

        XAxes =
            [|
                {
                    Title =
                        {
                            Text.Value = "Axis title"
                            Font = Font.Default
                            Color = Color.Black
                        }

                    AxisPosition = Top
                    TextColor = Color.Orange
                    AxisType = Categorical
                    Minimum = Some 0.1
                    Maximum = Some 5.0
                }
            |]

        YAxes = [| Axis.DefaultY |]

        Series = [||] // TODO
    }
    |> Script.toScript
    |> String.split Environment.NewLine
    |> Seq.skip 1 // Skip over the line which adds the app's directory to the library include path
    |> String.concat Environment.NewLine
    |> shouldEqual
        """#r "WindowsBase"
#r "PresentationCore"
#r "PresentationFramework"

#r "OxyPlot.dll"
#r "OxyPlot.Wpf.dll"

#r "FSharp.Chart.dll"
#r "FSharp.Chart.OxyPlot.dll"

open System
open System.Drawing

open OxyPlot.Wpf

open FSharp.Chart
open FSharp.Chart.OxyPlot

// TODO: Replace this hard-coded data
let seriesData = [||]

let chart =
    {
        Title    = { Value = "The title"; Font = Font.Default; Color = Color.Black }
        Subtitle = { Value = "The subtitle"; Font = Font.Default; Color = Color.Black }

        Background = Color.FromArgb(1, 2, 3, 4)
        PlotAreaBackground = Color.FromArgb(5, 6, 7, 8)

        XAxes = [| {
                    AxisPosition = Top
                    Title        = { Value = "Axis title"; Font = Font.Default; Color = Color.Black }
                    TextColor    = Color.Orange
                    AxisType     = Categorical
                    Minimum      = Some 0.100
                    Maximum      = Some 5.000
                } |]

        YAxes = [| Axis.DefaultY |]

        Series = seriesData
    }

let plotModel = PlotModel.from chart
System.Windows.Window(Content = PlotView(Model = plotModel)).ShowDialog() |> ignore
"""
