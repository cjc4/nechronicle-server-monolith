module NechronicleOpenAPIServer.FighterEndpointHandlers

open CommonOperationTypes
open FighterOperations
open Giraffe
open Microsoft.AspNetCore.Http

let HandleListFighter factionID : HttpHandler =
    // TODO: figure out how to get query parameters from URL
    let pathParameters = Map.empty.Add("factionID", factionID)
    let request =
        {
            defaultRequestContext with
                Operation = List
                Resource = Fighter
                PathParameters = Some(pathParameters)
        }
    let chain = listFighter
    match chain(request) with
    | Ok fighterList -> Successful.OK fighterList

let HandleCreateFighter (factionID : string) : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let pathParameters = Map.empty.Add("factionID", factionID)
            let! body = ctx.ReadBodyFromRequestAsync()
            let requestBody = { defaultRequestBody with Body = body }
            let request =
                {
                    defaultRequestContext with
                        Operation = Create
                        Resource = Fighter
                        PathParameters = Some(pathParameters)
                        RequestBody = Some(requestBody)
                }
            let chain = parseRequestBody
                        >==> extractFighterValuesFromRequest
                        >==> checkFactionIDExists
                        >==> makeCreatableFighter
                        >==> createFighter
            match chain(request) with
            | Ok fighter -> return! Successful.OK fighter next ctx
            | Error MalformedJson -> return! RequestErrors.BAD_REQUEST "Request body has malformed JSON" next ctx
            | Error FactionDoesNotExist -> return! RequestErrors.BAD_REQUEST "No faction with that ID exists" next ctx
            | Error NameNotProvided -> return! RequestErrors.BAD_REQUEST "A value for the name property is required" next ctx
            | _ -> return! ServerErrors.INTERNAL_ERROR "An unexpected error occurred" next ctx
        }

let HandleUpdateFighter (factionID : string, fighterID: string) : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let pathParameters =
                Map.empty.
                    Add("factionID", factionID).
                    Add("fighterID", fighterID)
            let! body = ctx.ReadBodyFromRequestAsync()
            let requestBody = { defaultRequestBody with Body = body }
            let request =
                {
                    defaultRequestContext with
                        Operation = Update
                        Resource = Fighter
                        PathParameters = Some(pathParameters)
                        RequestBody = Some(requestBody)
                }
            let chain = parseRequestBody
                        >==> extractFighterValuesFromRequest
                        >==> checkFactionIDExists
                        >==> checkFighterExists
                        >==> makeUpdatableFighter
                        >==> updateFighter
            match chain(request) with
            | Ok fighter -> return! Successful.OK fighter next ctx
            | Error MalformedJson -> return! RequestErrors.BAD_REQUEST "Request body has malformed JSON" next ctx
            | Error FactionDoesNotExist -> return! RequestErrors.BAD_REQUEST "No faction with that ID exists" next ctx
            | Error FighterDoesNotExist -> return! RequestErrors.NOT_FOUND "Fighter not found" next ctx
        }
