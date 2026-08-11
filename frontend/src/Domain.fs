module frontend.Domain

open System

type Todo =
    { Id: Guid
      Title: string
      Completed: bool }
