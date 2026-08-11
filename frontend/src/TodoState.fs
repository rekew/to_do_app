module frontend.TodoState

open System
open ElmishLand
open frontend.Domain

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
