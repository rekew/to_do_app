module frontend.Pages.Page

open Feliz
open ElmishLand
open frontend.Shared
open frontend.Pages

module TodoState = frontend.TodoState
module TodoView = frontend.TodoView

type Model = TodoState.Model

type Msg =
    | TodoMsg of TodoState.Msg
    | LayoutMsg of Layout.Msg

let init () =
    let model, cmd = TodoState.init ()
    model, Command.map TodoMsg id cmd

let update (msg: Msg) (model: Model) =
    match msg with
    | TodoMsg msg' ->
        let model', cmd = TodoState.update msg' model
        model', Command.map TodoMsg id cmd
    | LayoutMsg _ ->
        model, Command.none

let view (model: Model) (dispatch: Msg -> unit) =
    TodoView.view model (TodoMsg >> dispatch)

let page (_shared: SharedModel) (_route: HomeRoute) =
    Page.from init update view () LayoutMsg
