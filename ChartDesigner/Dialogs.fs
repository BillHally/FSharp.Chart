module FSharp.IO.Dialogs

open Microsoft.Win32

open MahApps.Metro.Controls.Dialogs

open FSharp.IO

let showMessage context title message =

    printfn "%s: %s" title message

    async {
        let! __ =
            DialogCoordinator.Instance.ShowMessageAsync(context, title, message)
            |> Async.AwaitTask
        return ()
    }
    |> Async.Start

let saveFile filters =
    let dialog = SaveFileDialog(Filter = FileFilter.toString filters)

    match dialog.ShowDialog() |> Option.ofNullable with
    | Some true  -> Some (File dialog.FileName)
    | Some false
    | None -> None
