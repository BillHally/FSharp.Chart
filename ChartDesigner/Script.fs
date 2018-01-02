module ChartDesigner.Script

open System
open System.Drawing

open FSharp.IO
open FSharp.Chart

let colorToScript (x : Color) =
    if x.IsKnownColor then
        sprintf "Color.%s" x.Name
    else
        sprintf
            "Color.FromArgb(%d, %d, %d, %d)"
            x.A
            x.R
            x.G
            x.B

let floatOptionToScript =
    function
    | Some x -> sprintf "Some %.3f" x
    | None   -> "None"

let textToScript (x : Text) =
    sprintf
        """{ Value = "%s"; Font = Font.Default; Color = %s }"""
        x.Value
        // TODO: x.Font
        (colorToScript x.Color)

let axisToScript (x : Axis) =
    if x = Axis.DefaultX then
        "Axis.DefaultX"
    else if x = Axis.DefaultY then
        "Axis.DefaultY"
    else
        sprintf
            """{
                    AxisPosition = %A
                    Title        = %s
                    TextColor    = %s
                    AxisType     = %A
                    Minimum      = %s
                    Maximum      = %s
                }"""
            x.AxisPosition
            (textToScript x.Title)
            (colorToScript x.TextColor)
            x.AxisType
            (floatOptionToScript x.Minimum)
            (floatOptionToScript x.Maximum)

let seriesToScript (x : Series) =
    sprintf
        """
        {
            SeriesData = %A
            Color      = %s
            XAxisIndex = %d
            YAxisIndex = %d
        }"""

        x.SeriesData
        (colorToScript x.Color)
        x.XAxisIndex
        x.YAxisIndex

let formatArrayContents before after =
    function
    | "" -> "[||]"
    | x  -> sprintf "%s%s%s" before x after

let axesToScript =
    Seq.map axisToScript
    >> String.concat "\r\n"
    >> formatArrayContents "[| " " |]"

let multipleSeriesToScript =
    Array.map seriesToScript
    >> String.concat "; "
    >> formatArrayContents "\r\n    [|" "\r\n    |]"

let toScript (x : Chart) =
    sprintf
        """//#I "<full path to the directory containing ChartDesigner>"
#r "WindowsBase"
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
let seriesData = %s

let chart =
    {
        Title    = %s
        Subtitle = %s

        Background = %s
        PlotAreaBackground = %s

        XAxes = %s

        YAxes = %s

        Series = seriesData
    }

let plotModel = PlotModel.from chart
System.Windows.Window(Content = PlotView(Model = plotModel)).ShowDialog() |> ignore
"""
        (multipleSeriesToScript x.Series)

        (textToScript x.Title)
        (textToScript x.Subtitle)

        (colorToScript x.Background)
        (colorToScript x.PlotAreaBackground)

        (axesToScript x.XAxes)
        (axesToScript x.YAxes)

let export saveFile writeAllText x =
    [|
        { Label = "F# script"; Extensions = [| "*.fsx" |]}
        FileFilter.AllFiles
    |]
    |> saveFile
    |> Option.iter
        (
            fun file ->
                x
                |> toScript
                |> Text
                |> writeAllText file
        )

