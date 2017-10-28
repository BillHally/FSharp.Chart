namespace ChartDesigner.Converters

open System
open System.Windows.Data
open System.Windows

type NullableMediaColorToDrawingColorConverter() =

    let mutable defaultValue = Drawing.Color.Transparent

    member __.DefaultValue with get () = defaultValue and set v = defaultValue <- v

    interface IValueConverter with
        member __.Convert(value, _, _, _) =
            printfn "Convert: %A" value
            match value with
            //| :? Nullable<Media.Color> as x ->
            //    if x.HasValue then
            //        Drawing.Color.FromArgb(int x.Value.A, int x.Value.R, int x.Value.G, int x.Value.B) |> box
            //    else
            //        box defaultValue
            | :? Drawing.Color as x ->
                box x
            | _ ->
                failwithf "Convert: Can't convert: %A (%s)" value (value.GetType().FullName)

        member __.ConvertBack(value, _, _, _) =
            printfn "ConvertBack: %A" value
            match value with
            | :? Media.Color as x ->
                let value = Drawing.Color.FromArgb(int x.A, int x.R, int x.G, int x.B)
                Nullable(value) :> obj
            | :? string as x ->
                let value = Media.ColorConverter.ConvertFromString x :?> Media.Color
                Nullable(value) :> obj
            | _ ->
                failwithf "ConvertBack: Can't convert back: %A (%s)" value (value.GetType().FullName)
