module ChartDesigner.Script

open System
open System.Drawing
open System.IO

open FSharp.IO
open FSharp.Chart

let formatArrayContents empty before after =
    function
    | "" -> empty
    | x  -> sprintf "%s%s%s" before x after

let formatArrayContentsOnSingleLine = formatArrayContents "[||]" "[| " " |]"

let formatArrayContentsOnMultipleLines =
    formatArrayContents
        "[||]"
        "\r\n                [|\r\n                    "
        "\r\n                |]"

let simpleDataToScript sep formatArrayContents =
    function
    | FloatData xs ->
        xs
        |> Seq.map (sprintf "%.2f")
        |> String.concat sep
        |> formatArrayContents
        |> sprintf "FloatData%s"
    | DateTimeData xs ->
        xs
        |> Seq.map (sprintf "%A")
        |> String.concat sep
        |> formatArrayContents
        |> sprintf "DateTimeData%s"
    | TimeSpanData xs ->
        xs
        |> Seq.map (sprintf "%A")
        |> String.concat sep
        |> formatArrayContents
        |> sprintf "TimeSpanData%s"

let simpleDataToScriptOnSingleLine = simpleDataToScript "; " formatArrayContentsOnSingleLine

let simpleDataToScriptOnMultipleLines =
    simpleDataToScript
        "\r\n                    "
        formatArrayContentsOnMultipleLines

let colorToScript (x : Option<Color>) =
    match x with
    | Some x ->
        if x.IsKnownColor then
            sprintf "Some Color.%s" x.Name
        else
            sprintf
                "Some (Color.FromArgb(%d, %d, %d, %d))"
                x.A
                x.R
                x.G
                x.B
    | None -> "None"

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

let boxPlotItemToScript x =
    sprintf
        """
            {
                UpperWhisker = %.3f
                BoxTop       = %.3f
                Median       = %.3f
                Mean         = %.3f
                BoxBottom    = %.3f
                LowerWhisker = %.3f

                Outliers = %s
            }"""
        x.UpperWhisker
        x.BoxTop
        x.Median
        x.Mean
        x.BoxBottom
        x.LowerWhisker

        (
            x.Outliers
            |> Seq.map (sprintf "%.2f")
            |> String.concat "; "
            |> formatArrayContents " [||]" "[| " " |]"
        )

let seriesDataToScript n seriesData =
    match seriesData with
    | Bar (simpleData, width) ->
        sprintf """Bar
        (
            %s,
            %.2f
        )"""
            (simpleDataToScriptOnMultipleLines simpleData)
            width

    | BoxPlot     (boxPlotItems     ) ->
        sprintf
            """BoxPlot%s"""
            (
                boxPlotItems
                |> Seq.map boxPlotItemToScript
                |> String.concat "\r\n"
                |> formatArrayContents "[||]" "\r\n        [|" "\r\n        |]"
            )

    | Column      (simpleData, width) -> sprintf "Column (%A, %.2f)" simpleData width
    | ErrorColumn (errorData,  width) -> sprintf "ErrorColumn (%A, %.2f)" errorData width
    | Scatter     (scatterData      ) -> sprintf "Scatter %A" scatterData
    | Area        (areaData         ) -> sprintf "Area %A" areaData
    | Line        (lineData         ) -> sprintf "Line %A" lineData
    |> sprintf
        """let seriesData%d =
    %s"""
        n

let multipleSeriesDataToScript =
    Seq.mapi (fun i x -> seriesDataToScript i x)
    >> String.concat "\r\n"

let seriesToScript n (x : Series) =
    sprintf
        """{
            Name       = "%s"
            SeriesData = seriesData%d
            Color      = %s
            XAxisIndex = %d
            YAxisIndex = %d
        }"""

        x.Name
        n
        (colorToScript x.Color)
        x.XAxisIndex
        x.YAxisIndex

let axesToScript =
    Seq.map axisToScript
    >> String.concat "\r\n"
    >> formatArrayContents "[||]" "[| " " |]"

let multipleSeriesToScript =
    Array.mapi (fun i x -> seriesToScript i x)
    >> String.concat "; "
    >> formatArrayContents "[||]" "\r\n        [|" "\r\n        |]"

let toScript (x : Chart) =
    sprintf
        """#I @"%s"
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
%s

let chart =
    {
        Title    = %s
        Subtitle = %s

        Background = %s
        PlotAreaBackground = %s

        XAxes = %s

        YAxes = %s

        Series = %s
    }

let plotModel = PlotModel.from chart
System.Windows.Window(Content = PlotView(Model = plotModel)).ShowDialog() |> ignore
"""
        (Path.GetDirectoryName (Reflection.Assembly.GetExecutingAssembly().Location))

        (
            x.Series
            |> Array.map (fun x -> x.SeriesData)
            |> multipleSeriesDataToScript
        )

        (textToScript x.Title)
        (textToScript x.Subtitle)

        (colorToScript x.Background)
        (colorToScript x.PlotAreaBackground)

        (axesToScript x.XAxes)
        (axesToScript x.YAxes)

        (multipleSeriesToScript x.Series)

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

