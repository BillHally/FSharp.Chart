namespace FSharp.Chart

open System
open System.Drawing

type SimpleData =
    | FloatData    of float   []
    | DateTimeData of DateTime[]
    | TimeSpanData of TimeSpan[]

type NamedData =
    | NamedFloats of (string * float)[]
    // TODO: add others...

type TupleData =
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
    | Data1D   of SimpleData
    | Data1x1D of SimpleData * SimpleData
    | Data2D   of TupleData

type DataWithErrors =
    | TupleOfArrays of SimpleData * SimpleData
    | ArrayOfTuples of TupleData

type ScatterData private(data : Data1DOr2D) =

    new(ys)     = ScatterData(Data1D   (FloatData    ys))
    new(ys)     = ScatterData(Data1D   (DateTimeData ys))
    new(ys)     = ScatterData(Data1D   (TimeSpanData ys))

    new(xs, ys) = ScatterData(Data1x1D (FloatData    xs, FloatData    ys))
    new(xs, ys) = ScatterData(Data1x1D (FloatData    xs, DateTimeData ys))
    new(xs, ys) = ScatterData(Data1x1D (FloatData    xs, TimeSpanData ys))
    new(xs, ys) = ScatterData(Data1x1D (DateTimeData xs, FloatData    ys))
    new(xs, ys) = ScatterData(Data1x1D (DateTimeData xs, DateTimeData ys))
    new(xs, ys) = ScatterData(Data1x1D (DateTimeData xs, TimeSpanData ys))
    new(xs, ys) = ScatterData(Data1x1D (TimeSpanData xs, FloatData    ys))
    new(xs, ys) = ScatterData(Data1x1D (TimeSpanData xs, DateTimeData ys))
    new(xs, ys) = ScatterData(Data1x1D (TimeSpanData xs, TimeSpanData ys))

    new(xys)    = ScatterData(Data2D   (FloatFloat       xys))
    new(xys)    = ScatterData(Data2D   (FloatDateTime    xys))
    new(xys)    = ScatterData(Data2D   (FloatTimeSpan    xys))
    new(xys)    = ScatterData(Data2D   (DateTimeFloat    xys))
    new(xys)    = ScatterData(Data2D   (DateTimeDateTime xys))
    new(xys)    = ScatterData(Data2D   (DateTimeTimeSpan xys))
    new(xys)    = ScatterData(Data2D   (TimeSpanFloat    xys))
    new(xys)    = ScatterData(Data2D   (TimeSpanDateTime xys))
    new(xys)    = ScatterData(Data2D   (TimeSpanTimeSpan xys))

    member __.Data = data

type ErrorData private(data : DataWithErrors) =

    new(xs, ys) = ErrorData(TupleOfArrays (FloatData    xs, FloatData    ys))
    new(xs, ys) = ErrorData(TupleOfArrays (FloatData    xs, DateTimeData ys))
    new(xs, ys) = ErrorData(TupleOfArrays (FloatData    xs, TimeSpanData ys))
    new(xs, ys) = ErrorData(TupleOfArrays (DateTimeData xs, FloatData    ys))
    new(xs, ys) = ErrorData(TupleOfArrays (DateTimeData xs, DateTimeData ys))
    new(xs, ys) = ErrorData(TupleOfArrays (DateTimeData xs, TimeSpanData ys))
    new(xs, ys) = ErrorData(TupleOfArrays (TimeSpanData xs, FloatData    ys))
    new(xs, ys) = ErrorData(TupleOfArrays (TimeSpanData xs, DateTimeData ys))
    new(xs, ys) = ErrorData(TupleOfArrays (TimeSpanData xs, TimeSpanData ys))

    new(xys)    = ErrorData(ArrayOfTuples (FloatFloat       xys))
    new(xys)    = ErrorData(ArrayOfTuples (FloatDateTime    xys))
    new(xys)    = ErrorData(ArrayOfTuples (FloatTimeSpan    xys))
    new(xys)    = ErrorData(ArrayOfTuples (DateTimeFloat    xys))
    new(xys)    = ErrorData(ArrayOfTuples (DateTimeDateTime xys))
    new(xys)    = ErrorData(ArrayOfTuples (DateTimeTimeSpan xys))
    new(xys)    = ErrorData(ArrayOfTuples (TimeSpanFloat    xys))
    new(xys)    = ErrorData(ArrayOfTuples (TimeSpanDateTime xys))
    new(xys)    = ErrorData(ArrayOfTuples (TimeSpanTimeSpan xys))

    member __.Data = data

type ColumnData private(data : NamedData) =
    new(xs) = ColumnData(NamedFloats(xs))

    member __.Data = data

type BoxPlotItem =
    {
        Category     : Option<string>

        UpperWhisker : float
        BoxTop       : float
        Median       : float
        Mean         : float
        BoxBottom    : float
        LowerWhisker : float

        Outliers : float[]
    }

type AreaData =
    {
        AreaTop    : TupleData
        AreaBottom : TupleData
    }

type AxisPosition =
    | LeftAxis
    | RightAxis
    | BottomAxis
    | TopAxis

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
        Color : Option<Color>
    }

    static member Default =
        {
            Value = ""
            Font  = Font.Default
            Color = None
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
        TextColor    : Option<Color>
        AxisType     : AxisType
        Minimum      : Option<float>
        Maximum      : Option<float>
    }

    static member DefaultX =
        {
            AxisPosition = BottomAxis
            Title        = Text.Default
            TextColor    = None
            AxisType     = Linear
            Minimum      = None
            Maximum      = None
        }

    static member DefaultY =
        {
            AxisPosition = LeftAxis
            Title        = Text.Default
            TextColor    = None
            AxisType     = Linear
            Minimum      = None
            Maximum      = None
        }

type SeriesData =
    | Bar         of data : SimpleData * width : float
    | BoxPlot     of data : BoxPlotItem[]
    | Column      of data : ColumnData * width : float
    | ErrorColumn of data : ErrorData  * width : float
    | Scatter     of data : ScatterData
    | Area        of data : AreaData
    | Line        of data : TupleData

type Series =
    {
        Name       : string
        SeriesData : SeriesData
        Color      : Option<Color>
        XAxisIndex : int
        YAxisIndex : int
    }

    static member Default =
        {
            Name       = ""
            SeriesData = Scatter (ScatterData([||] : float[]))
            Color      = None
            XAxisIndex = -1
            YAxisIndex = -1
        }

    static member Bar         width x = { Series.Default with SeriesData = (Bar          (x, width)) }
    static member BoxPlot           x = { Series.Default with SeriesData = (BoxPlot      (x       )) }
    static member Column      width x = { Series.Default with SeriesData = (Column       (x, width)) }
    static member ErrorColumn width x = { Series.Default with SeriesData = (ErrorColumn  (x, width)) }
    static member Scatter           x = { Series.Default with SeriesData = (Scatter       x        ) }
    static member Area              x = { Series.Default with SeriesData = (Area          x        ) }
    static member Line              x = { Series.Default with SeriesData = (Line          x        ) }

type Position =
    | LeftTop
    | CenterTop
    | RightTop
    | RightCenter
    | RightBottom
    | CenterBottom
    | LeftBottom
    | LeftCenter

type Location =
    | Inside
    | Outside

type Legend =
    {
        Title    : Text
        Position : Position
        Location : Location

        Font : Font
    }

    static member Default =
        {
            Title    = Text.Default
            Position = RightCenter
            Location = Outside
            Font     = Font.Default
        }

type Chart =
    {
        Title    : Text
        Subtitle : Text

        Background         : Option<Color>
        PlotAreaBackground : Option<Color>

        XAxes : Axis[]
        YAxes : Axis[]

        Series : Series[]

        Legend : Legend
    }

    static member Default =
        {
            Title    = Text.Default
            Subtitle = Text.Default

            Background         = Some Color.Transparent
            PlotAreaBackground = Some Color.Transparent

            XAxes  = [||]
            YAxes  = [||]
            Series = [||]

            Legend = Legend.Default
        }

module NamedData =
    let getCategories =
        function
        | NamedFloats xs ->
            xs
            |> Seq.map fst
