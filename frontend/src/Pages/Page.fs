module frontend.Pages.Page

open Feliz
open ElmishLand
open frontend.Shared
open frontend.Pages

type Model = unit

type Msg =
    | LayoutMsg of Layout.Msg

let init () =
    (),
    Command.none

let update (msg: Msg) (model: Model) =
    match msg with
    | LayoutMsg _ -> model, Command.none

let view (_model: Model) (_dispatch: Msg -> unit) =
    Html.text " Page"

let page (_shared: SharedModel) (_route: HomeRoute) =
    Page.from init update view () LayoutMsg
