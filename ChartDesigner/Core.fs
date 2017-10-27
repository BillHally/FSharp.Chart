module ChartDesigner.Core

open Gjallarhorn
open Gjallarhorn.Bindable
open Gjallarhorn.Bindable.Framework
open Gjallarhorn.Validation
open Gjallarhorn.Validation.Validators

open FSharp.Chart
open FSharp.Chart.OxyPlot

open ChartDesigner.Models

// Note that this program is defined in a PCL, and is completely platform neutral.
// It will work unchanged on WPF, Xamarin Forms, etc

module Program =
    open OxyPlot

    // ----------------------------------     Model     ----------------------------------
    // Model contains our first and last name
    type Model =
        {
            Width  : float
            Height : float
            Title  : string
            Chart  : Chart
        }
    with
        static member Default =
            {
                Width  = 1000.0
                Height = 1000.0
                Title  = "Chart Designer"
                Chart  = ChartDesigner.Models.Examples.boxPlotExample ()
            }

    // ----------------------------------    Update     ----------------------------------
    // We define a union type for each possible message
    type Msg =
        | Width  of float
        | Height of float

    // Create a function that updates the model given a message
    let update msg (model : Model) =
        printfn "update: %A" msg
        match msg with
        | Width  x -> { model with Width  = x }
        | Height x -> { model with Height = x }

    // Our "ViewModel". This is optional, but allows you to avoid using "magic strings", as well as enables design time XAML in C# projects
    [<CLIMutable>] // CLIMutable is required by XAML tooling if we have 2-way bindings
    type ViewModel =
        {
            Title     : string
            Width     : float
            Height    : float
            PlotModel : PlotModel
        }

    let d =
        {
            Title     = ""
            Width     = 1000.0
            Height    = 1000.0
            PlotModel = PlotModel()
        }

    // ----------------------------------    Binding    ----------------------------------
    // Create a function that binds a model to a source, and outputs messages
    let bindToSource =
        Component.fromBindings<Model, Msg> [
            <@ d.Width     @> |> Bind.twoWay (fun x -> x.Width)  Width //Validated (fun m -> m.FirstName) notNullOrWhitespace FirstName
            <@ d.Height    @> |> Bind.twoWay (fun x -> x.Height) Height //Validated (fun m -> m.LastName) validLast LastName
            <@ d.PlotModel @> |> Bind.oneWay (fun x -> x.Chart |> PlotModel.from)
            <@ d.Title     @> |> Bind.oneWay (fun x -> x.Title)
        ]

    let applicationCore = Framework.basicApplication Model.Default update bindToSource

