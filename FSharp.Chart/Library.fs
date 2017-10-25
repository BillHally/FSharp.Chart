namespace FSharp.Chart

open System
open System.Drawing

type Data1D =
    | FloatData    of float    []
    | DateTimeData of DateTime []
    | TimeSpanData of TimeSpan []

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

type SeriesDataWrapper =
    | Data1D   of Data1D
    | Data1x1D of Data1D * Data1D
    | Data2D   of Data2D

type SeriesData private(data : SeriesDataWrapper) =

    new(ys)     = SeriesData(Data1D   (FloatData    ys))
    new(ys)     = SeriesData(Data1D   (DateTimeData ys))
    new(ys)     = SeriesData(Data1D   (TimeSpanData ys))

    new(xs, ys) = SeriesData(Data1x1D (FloatData    xs, FloatData    ys))
    new(xs, ys) = SeriesData(Data1x1D (FloatData    xs, DateTimeData ys))
    new(xs, ys) = SeriesData(Data1x1D (FloatData    xs, TimeSpanData ys))
    new(xs, ys) = SeriesData(Data1x1D (DateTimeData xs, FloatData    ys))
    new(xs, ys) = SeriesData(Data1x1D (DateTimeData xs, DateTimeData ys))
    new(xs, ys) = SeriesData(Data1x1D (DateTimeData xs, TimeSpanData ys))
    new(xs, ys) = SeriesData(Data1x1D (TimeSpanData xs, FloatData    ys))
    new(xs, ys) = SeriesData(Data1x1D (TimeSpanData xs, DateTimeData ys))
    new(xs, ys) = SeriesData(Data1x1D (TimeSpanData xs, TimeSpanData ys))

    new(xys)    = SeriesData(Data2D   (FloatFloat       xys))
    new(xys)    = SeriesData(Data2D   (FloatDateTime    xys))
    new(xys)    = SeriesData(Data2D   (FloatTimeSpan    xys))
    new(xys)    = SeriesData(Data2D   (DateTimeFloat    xys))
    new(xys)    = SeriesData(Data2D   (DateTimeDateTime xys))
    new(xys)    = SeriesData(Data2D   (DateTimeTimeSpan xys))
    new(xys)    = SeriesData(Data2D   (TimeSpanFloat    xys))
    new(xys)    = SeriesData(Data2D   (TimeSpanDateTime xys))
    new(xys)    = SeriesData(Data2D   (TimeSpanTimeSpan xys))

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
    }

    static member DefaultX =
        {
            AxisPosition = Bottom
            Title        = Text.Default
            TitleColor   = Color.Black
            TextColor    = Color.Black
            AxisType     = Linear
        }

    static member DefaultY =
        {
            AxisPosition = Left
            Title        = Text.Default
            TitleColor   = Color.Black
            TextColor    = Color.Black
            AxisType     = Linear
        }

type SeriesType =
    | Bar
    | BoxPlot
    | Column of width : float
    | ErrorColumn
    | Scatter

type Series =
    {
        SeriesData : SeriesData
        SeriesType : SeriesType
        Color      : Color
        XAxisIndex : int
        YAxisIndex : int
    }

    static member Default =
        {
            SeriesData = SeriesData([||] : float[])
            SeriesType = Scatter
            Color      = Color.Black
            XAxisIndex = -1
            YAxisIndex = -1
        }

type Chart =
    {
        Title    : Text
        Subtitle : Text

        XAxes : Axis []
        YAxes : Axis []

        Series : Series []
    }

    static member Default =
        {
            Title    = Text.Default
            Subtitle = Text.Default

            XAxes  = [||]
            YAxes  = [||]
            Series = [||]
        }
