module frontend.TodoView

open Browser.Types
open Feliz
open frontend.Domain
open frontend.TodoState

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
