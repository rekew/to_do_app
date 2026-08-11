module frontend.Pages.Page

open System
open Browser.Types
open Feliz
open ElmishLand
open frontend.Shared
open frontend.Pages

type Todo =
    { Id: Guid
      Title: string
      Completed: bool }

type Model =
    { Todos: Todo list
      Input: string
      EditingId: Guid option
      EditInput: string }

type Msg =
    | InputChanged of string
    | AddTodo
    | ToggleTodo of Guid
    | StartEditing of Guid
    | EditInputChanged of string
    | SaveEdit of Guid
    | CancelEdit
    | DeleteTodo of Guid
    | LayoutMsg of Layout.Msg

let init () =
    { Todos = []
      Input = ""
      EditingId = None
      EditInput = "" },
    Command.none

let update (msg: Msg) (model: Model) =
    match msg with
    | InputChanged text ->
        { model with Input = text }, Command.none

    | AddTodo ->
        let title = model.Input.Trim()
        if title = "" then
            model, Command.none
        else
            let todo =
                { Id = Guid.NewGuid()
                  Title = title
                  Completed = false }

            { model with
                Todos = todo :: model.Todos
                Input = "" },
            Command.none

    | ToggleTodo id ->
        let todos =
            model.Todos
            |> List.map (fun todo ->
                if todo.Id = id then
                    { todo with Completed = not todo.Completed }
                else
                    todo)

        { model with Todos = todos }, Command.none

    | StartEditing id ->
        let editInput =
            model.Todos
            |> List.tryFind (fun todo -> todo.Id = id)
            |> Option.map (fun todo -> todo.Title)
            |> Option.defaultValue ""

        { model with
            EditingId = Some id
            EditInput = editInput },
        Command.none

    | EditInputChanged text ->
        { model with EditInput = text }, Command.none

    | SaveEdit id ->
        let title = model.EditInput.Trim()
        let todos =
            model.Todos
            |> List.map (fun todo ->
                if todo.Id = id && title <> "" then
                    { todo with Title = title }
                else
                    todo)

        { model with
            Todos = todos
            EditingId = None
            EditInput = "" },
        Command.none

    | CancelEdit ->
        { model with
            EditingId = None
            EditInput = "" },
        Command.none

    | DeleteTodo id ->
        let todos = model.Todos |> List.filter (fun todo -> todo.Id <> id)
        { model with Todos = todos }, Command.none

    | LayoutMsg _ ->
        model, Command.none

let view (model: Model) (dispatch: Msg -> unit) =
    let todoRow (todo: Todo) =
        let isEditing = model.EditingId = Some todo.Id

        Html.li [
            prop.key (todo.Id.ToString())
            prop.style [
                style.display.flex
                style.alignItems.center
                style.gap 8
            ]
            prop.children [
                Html.input [
                    prop.type'.checkbox
                    prop.isChecked todo.Completed
                    prop.onChange (fun (_: bool) -> dispatch (ToggleTodo todo.Id))
                ]
                if isEditing then
                    Html.input [
                        prop.type'.text
                        prop.autoFocus true
                        prop.value model.EditInput
                        prop.onChange (fun (text: string) -> dispatch (EditInputChanged text))
                        prop.onKeyDown (fun (e: KeyboardEvent) ->
                            if e.key = "Enter" then
                                dispatch (SaveEdit todo.Id))
                    ]
                    Html.button [
                        prop.text "Save"
                        prop.onClick (fun _ -> dispatch (SaveEdit todo.Id))
                    ]
                    Html.button [
                        prop.text "Cancel"
                        prop.onClick (fun _ -> dispatch CancelEdit)
                    ]
                else
                    Html.span [
                        prop.style [
                            if todo.Completed then
                                style.textDecorationLine.lineThrough
                        ]
                        prop.text todo.Title
                    ]
                    Html.button [
                        prop.text "Edit"
                        prop.onClick (fun _ -> dispatch (StartEditing todo.Id))
                    ]
                    Html.button [
                        prop.text "Delete"
                        prop.onClick (fun _ -> dispatch (DeleteTodo todo.Id))
                    ]
            ]
        ]

    Html.div [
        prop.children [
            Html.h1 "To-Do"
            Html.div [
                prop.style [
                    style.display.flex
                    style.gap 8
                ]
                prop.children [
                    Html.input [
                        prop.type'.text
                        prop.placeholder "What needs to be done?"
                        prop.value model.Input
                        prop.onChange (fun (text: string) -> dispatch (InputChanged text))
                        prop.onKeyDown (fun (e: KeyboardEvent) ->
                            if e.key = "Enter" then
                                dispatch AddTodo)
                    ]
                    Html.button [
                        prop.text "Add"
                        prop.onClick (fun _ -> dispatch AddTodo)
                    ]
                ]
            ]
            if model.Todos.IsEmpty then
                Html.p "No tasks yet. Add one above."
            else
                Html.ul [
                    prop.children (model.Todos |> List.map todoRow)
                ]
        ]
    ]

let page (_shared: SharedModel) (_route: HomeRoute) =
    Page.from init update view () LayoutMsg
