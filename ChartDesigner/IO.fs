namespace FSharp.IO

open System
open System.IO

type File = File of string
type Text = Text of string

module File =
    let writeAllText showMessage (File file) (Text text) =
        try
            IO.File.WriteAllText(file, text)
        with
        | :? IOException as ex -> showMessage "Error" ex.Message
