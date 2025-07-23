module NechronicleOpenAPIServer.FactionEndpointHandlers

open CommonOperationTypes
open CommonOperations
open FactionOperations
open Giraffe
open Microsoft.AspNetCore.Http

let HandleListFighter factionID : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let queryParameters = ctx.Request.Query
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
        | Ok fighterViewList -> Successful.OK fighterViewList next ctx
        | _ -> ServerErrors.INTERNAL_ERROR "An unexpected error occurred" next ctx

let HandleCreateFighter factionID : HttpHandler =
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
                        >==> checkFactionExists
                        >==> makeCreatableFighter
                        >==> createFighter
            match chain(request) with
            | Ok fighter -> return! Successful.OK fighter next ctx
            | Error MalformedJson -> return! RequestErrors.BAD_REQUEST "Request body has malformed JSON" next ctx
            | Error FactionDoesNotExist -> return! RequestErrors.BAD_REQUEST "No faction with that ID exists" next ctx
            | Error NameNotProvided -> return! RequestErrors.BAD_REQUEST "A value for the name property is required" next ctx
            | _ -> return! ServerErrors.INTERNAL_ERROR "An unexpected error occurred" next ctx
        }

let HandleRetrieveFighterById (factionID, fighterID) : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let pathParameters =
                Map.empty.
                    Add("factionID", factionID).
                    Add("fighterID", fighterID)
        let request =
            {
                defaultRequestContext with
                    Operation = RetrieveByID
                    Resource = Fighter
                    PathParameters = Some(pathParameters)
            }
        let chain = parseRequestBody
                    >==> extractFighterValuesFromRequest
                    >==> retrieveFighterById
        match chain(request) with
        | Ok (fighterView, eTag) ->
            do ctx.SetHttpHeader("ETag", eTag)
            Successful.OK fighterView next ctx
        | Error FighterDoesNotExist -> RequestErrors.NOT_FOUND "Fighter not found" next ctx
        | _ -> ServerErrors.INTERNAL_ERROR "An unexpected error occurred" next ctx

let HandleUpdateFighter (factionID , fighterID) : HttpHandler =
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
                        >==> checkFactionExists
                        >==> checkFighterExists
                        >==> makeUpdatableFighter
                        >==> updateFighter
            match chain(request) with
            | Ok eTag ->
                do ctx.SetHttpHeader("ETag", eTag)
                return! Successful.NO_CONTENT next ctx
            // not implemented yet
            //| Ok (fighter, eTag) ->
            //    do ctx.SetHttpHeader("ETag", eTag)
            //    return! Successful.OK fighter next
            | Error MalformedJson -> return! RequestErrors.BAD_REQUEST "Request body has malformed JSON" next ctx
            | Error FactionDoesNotExist -> return! RequestErrors.BAD_REQUEST "No faction with that ID exists" next ctx
            | Error FighterDoesNotExist -> return! RequestErrors.NOT_FOUND "Fighter not found" next ctx
            | _ -> return! ServerErrors.INTERNAL_ERROR "An unexpected error occurred" next ctx
        }

let HandleDeleteFighter (factionID, fighterID) : HttpHandler =
    let pathParameters =
            Map.empty.
                Add("factionID", factionID).
                Add("fighterID", fighterID)
    let request =
        {
            defaultRequestContext with
                Operation = Delete
                Resource = Fighter
                PathParameters = Some(pathParameters)
        }
    let chain = parseRequestBody
                >==> extractFighterValuesFromRequest
                >==> checkFactionExists
                >==> checkFighterExists
                >==> deleteFighter
    match chain(request) with
    | Ok () -> Successful.NO_CONTENT
    | Error MalformedJson -> RequestErrors.BAD_REQUEST "Request body has malformed JSON"
    | Error FactionDoesNotExist -> RequestErrors.BAD_REQUEST "No faction with that ID exists"
    | Error FighterDoesNotExist -> RequestErrors.NOT_FOUND "Not deleted - no fighter with that ID exists"
    | _ -> ServerErrors.INTERNAL_ERROR "An unexpected error occurred"
