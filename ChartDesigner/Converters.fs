namespace ChartDesigner.Converters

open System
open System.Windows.Data
open System.Windows

type DrawingColorToMediaColorConverter() =

    interface IValueConverter with
        member __.Convert(value, _, _, _) =
            match value with
            | :? Drawing.Color as x ->
                Media.Color.FromArgb(x.A, x.R, x.G, x.B) |> box
            | _ ->
                failwithf "Convert: Can't convert: %A (%s)" value (value.GetType().FullName)

        member __.ConvertBack(value, _, _, _) =
            match value with
            | :? Media.Color as x ->
                let value = Drawing.Color.FromArgb(int x.A, int x.R, int x.G, int x.B)
                Nullable(value) :> obj
            | :? string as x ->
                let value = Media.ColorConverter.ConvertFromString x :?> Media.Color
                Nullable(value) :> obj
            | _ ->
                failwithf "ConvertBack: Can't convert back: %A (%s)" value (value.GetType().FullName)
