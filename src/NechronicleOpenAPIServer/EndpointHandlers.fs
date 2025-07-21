module NechronicleOpenAPIServer.EndpointHandlers

// First party dependencies
open AppLogic

// Third party dependencies
open FSharp.Data
open Giraffe
open Microsoft.AspNetCore.Http

let listFighter (factionID : string) : HttpHandler =
    let fighterList = Repository.listFighter
    json fighterList

let listUser : HttpHandler =
    let userList = Repository.listUser
    json userList

let createFighter (factionID : string) : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! body = ctx.ReadBodyFromRequestAsync()
            let bodyValue = JsonValue.Parse(body)
            let submittedName =  bodyValue.TryGetProperty("name")
            // TODO: figure out the below

            // let exists (submittedName) =
            //     match submittedName with
            //     | Some(value) -> let name = value // Why can't I do this?
            //     | None -> return! RequestErrors.BAD_REQUEST "fighter name required" next ctx
            let name =
                match submittedName with
                | Some(name) -> name.AsString()
                | None -> "" // TODO: return an HTTP error instead, a draftFighter must have a name
            let draftFighter =
                {
                    FactionID = FactionID factionID
                    Name = name
                    Attributes = bodyValue.TryGetProperty("attributes")
                }
            let result = createFighter draftFighter
            match result with
            | Ok fighter -> return! Successful.OK fighter next ctx
            | Error message -> return! ServerErrors.INTERNAL_ERROR message next ctx
            //return! Successful.OK fighter next ctx
        }
