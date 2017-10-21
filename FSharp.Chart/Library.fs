namespace FSharp.Chart

open System
open System.Drawing

type SeriesData =
    | FloatSeries    of float    []
    | DateTimeSeries of DateTime []
    | TimeSpanSeries of TimeSpan []

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
    | Linear
    | Categorical

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
            SeriesData = FloatSeries [||]
            SeriesType = Scatter
            Color      = Color.Blue
            XAxisIndex = 0
            YAxisIndex = 0
        }

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
