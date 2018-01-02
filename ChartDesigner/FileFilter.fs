namespace FSharp.IO

type FileFilter =
    {
        Label : string
        Extensions : string[]
    }

    override this.ToString() =
        this.Extensions
        |> String.concat ";"
        |> sprintf "%s|%s" this.Label

    static member AllFiles =
        {
            Label = "All files"
            Extensions = [| "*.*" |]
        }

[<RequireQualifiedAccess>]
module FileFilter =
    let toString (xs : FileFilter[]) =
        xs
        |> Array.map (sprintf "%O")
        |> String.concat "|"
