namespace FSharp.Chart

open System
open System.Drawing

type Data1D =
    | FloatData    of float   []
    | DateTimeData of DateTime[]
    | TimeSpanData of TimeSpan[]

type Data2D =
    | FloatFloat       of (float    *    float)[]
    | FloatDateTime    of (float    * DateTime)[]
    | FloatTimeSpan    of (float    * TimeSpan)[]
    | DateTimeFloat    of (DateTime *    float)[]
    | DateTimeDateTime of (DateTime * DateTime)[]
    | DateTimeTimeSpan of (DateTime * TimeSpan)[]
    | TimeSpanFloat    of (TimeSpan *    float)[]
    | TimeSpanDateTime of (TimeSpan * DateTime)[]
    | TimeSpanTimeSpan of (TimeSpan * TimeSpan)[]

type Data1DOr2D =
    | Data1D   of Data1D
    | Data1x1D of Data1D * Data1D
    | Data2D   of Data2D

type BasicData private(data : Data1DOr2D) =

    new(ys)     = BasicData(Data1D   (FloatData    ys))
    new(ys)     = BasicData(Data1D   (DateTimeData ys))
    new(ys)     = BasicData(Data1D   (TimeSpanData ys))

    new(xs, ys) = BasicData(Data1x1D (FloatData    xs, FloatData    ys))
    new(xs, ys) = BasicData(Data1x1D (FloatData    xs, DateTimeData ys))
    new(xs, ys) = BasicData(Data1x1D (FloatData    xs, TimeSpanData ys))
    new(xs, ys) = BasicData(Data1x1D (DateTimeData xs, FloatData    ys))
    new(xs, ys) = BasicData(Data1x1D (DateTimeData xs, DateTimeData ys))
    new(xs, ys) = BasicData(Data1x1D (DateTimeData xs, TimeSpanData ys))
    new(xs, ys) = BasicData(Data1x1D (TimeSpanData xs, FloatData    ys))
    new(xs, ys) = BasicData(Data1x1D (TimeSpanData xs, DateTimeData ys))
    new(xs, ys) = BasicData(Data1x1D (TimeSpanData xs, TimeSpanData ys))

    new(xys)    = BasicData(Data2D   (FloatFloat       xys))
    new(xys)    = BasicData(Data2D   (FloatDateTime    xys))
    new(xys)    = BasicData(Data2D   (FloatTimeSpan    xys))
    new(xys)    = BasicData(Data2D   (DateTimeFloat    xys))
    new(xys)    = BasicData(Data2D   (DateTimeDateTime xys))
    new(xys)    = BasicData(Data2D   (DateTimeTimeSpan xys))
    new(xys)    = BasicData(Data2D   (TimeSpanFloat    xys))
    new(xys)    = BasicData(Data2D   (TimeSpanDateTime xys))
    new(xys)    = BasicData(Data2D   (TimeSpanTimeSpan xys))

    member __.Data = data

type BoxPlotItem =
    {
        UpperWhisker : float
        BoxTop       : float
        Median       : float
        Mean         : float
        BoxBottom    : float
        LowerWhisker : float

        Outliers : float[]
    }

type BoxPlotData(data : BoxPlotItem[]) =
    member __.Data = data

type AxisPosition =
    | Left
    | Right
    | Bottom
    | Top

type FontStyle =
    | Normal
    | Bold
    | Italic

type Font =
    {
        Name  : string
        Size  : int
        Style : FontStyle
    }

    static member Default =
        {
            Name  = ""
            Size  = 0
            Style = Normal
        }

type Text =
    {
        Value : string
        Font  : Font
        Color : Color
    }

    static member Default =
        {
            Value = ""
            Font  = Font.Default
            Color = Color.Black
        }

type AxisType =
    | Categorical
    | DateTime
    | Linear
    | TimeSpan

type Axis =
    {
        AxisPosition : AxisPosition
        Title        : Text
        TitleColor   : Color
        TextColor    : Color
        AxisType     : AxisType
        Minimum      : float
        Maximum      : float
    }

    static member DefaultX =
        {
            AxisPosition = Bottom
            Title        = Text.Default
            TitleColor   = Color.Black
            TextColor    = Color.Black
            AxisType     = Linear
            Minimum      = nan
            Maximum      = nan
        }

    static member DefaultY =
        {
            AxisPosition = Left
            Title        = Text.Default
            TitleColor   = Color.Black
            TextColor    = Color.Black
            AxisType     = Linear
            Minimum      = nan
            Maximum      = nan
        }

type SeriesData =
    | Bar         of data : BasicData
    | BoxPlot     of data : BoxPlotData
    | Column      of data : BasicData * width : float
    | ErrorColumn of data : BasicData
    | Scatter     of data : BasicData

type Series =
    {
        SeriesData : SeriesData
        Color      : Color
        XAxisIndex : int
        YAxisIndex : int
    }

    static member Default =
        {
            SeriesData = Scatter (BasicData([||] : float[]))
            Color      = Color.Black
            XAxisIndex = -1
            YAxisIndex = -1
        }

    static member Bar          x = { Series.Default with SeriesData = (Bar x)                   }
    static member BoxPlot      x = { Series.Default with SeriesData = (BoxPlot (BoxPlotData x)) }
    static member Column width x = { Series.Default with SeriesData = (Column (x, width))       }
    static member ErrorColumn  x = { Series.Default with SeriesData = (ErrorColumn x)           }
    static member Scatter      x = { Series.Default with SeriesData = (Scatter x)               }

type Chart =
    {
        Title    : Text
        Subtitle : Text

        XAxes : Axis[]
        YAxes : Axis[]

        Series : Series[]
    }

    static member Default =
        {
            Title    = Text.Default
            Subtitle = Text.Default

            XAxes  = [||]
            YAxes  = [||]
            Series = [||]
        }
