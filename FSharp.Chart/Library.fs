namespace FSharp.Chart

open System

type ChartData =
    | CDFloat    of float[]
    | CDInt      of int[]
    | CDDateTime of DateTime[]
    | CDTimeSpan of TimeSpan[]

type AxisDirection =
    | Horizontal
    | Vertical

type FontStyle =
    | Normal
    | Bold
    | Italic

type Font =
    {
        Name : string
        Size : int
        Style : FontStyle
    }

type Title =
    {
        Title : string
        Font : Font
    }

type Axis =
    {
        AxisDirection : AxisDirection
        Title : Title
    }

type Chart =
    {
        Title : Title

        Axes : Axis[]
    }
        