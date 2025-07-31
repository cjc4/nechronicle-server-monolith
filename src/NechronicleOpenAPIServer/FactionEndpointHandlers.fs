module NechronicleOpenAPIServer.FactionEndpointHandlers

open CommonOperationTypes
open CommonOperations
open FactionOperations
open Giraffe
open Microsoft.AspNetCore.Http
open Npgsql

// let HandleListFighter factionID : HttpHandler =
//     fun (next : HttpFunc) (ctx : HttpContext) ->
//         let pathParameters = Map.empty.Add("FactionID", factionID)
//         let queryParameters =
//             match ctx.Request.Query.Count with
//             | 0 -> None
//             | _ -> Some(ctx.Request.Query)
//         use dataSource = NpgsqlDataSource.Create(CommonRepositoryFunctions.createConnectionString)
//         let request =
//             {
//                 defaultRequestContext with
//                     DataSource = Some(dataSource)
//                     Operation = List
//                     Resource = Fighter
//                     PathParameters = Some(pathParameters)
//                     QueryParameters = queryParameters
//             }
//         let chain = extractFighterFiltersFromQuery
//                     >==> extractReturnFieldsFromQuery
//                     >==> listFighter
//         match chain(request) with
//         | Ok fighterViewList -> Successful.OK fighterViewList next ctx
//         | _ -> ServerErrors.INTERNAL_ERROR "An unexpected error occurred" next ctx

let HandleCreateFighter factionID : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let pathParameters = Map.empty.Add("FactionID", factionID)
            let! body = ctx.ReadBodyFromRequestAsync()
            let requestBody = { defaultRequestBody with Body = body }
            use dataSource = NpgsqlDataSource.Create(CommonRepositoryFunctions.createConnectionString)
            let request =
                {
                    defaultRequestContext with
                        DataSource = Some(dataSource)
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
            | Ok (fighterView, eTag) ->
                do ctx.SetHttpHeader("ETag", eTag)
                return! Successful.OK fighterView next ctx
            | Error MalformedJson -> return! RequestErrors.BAD_REQUEST "Request body has malformed JSON" next ctx
            | Error FactionDoesNotExist -> return! RequestErrors.BAD_REQUEST "No faction with that ID exists" next ctx
            | Error NameNotProvided -> return! RequestErrors.BAD_REQUEST "A value for the name property is required" next ctx
            | _ -> return! ServerErrors.INTERNAL_ERROR "An unexpected error occurred" next ctx
        }

let HandleRetrieveFighterById (factionID, fighterID) : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let pathParameters =
                Map.empty.
                    Add("FactionID", factionID).
                    Add("FighterID", fighterID)
        let queryParameters =
            match ctx.Request.Query.Count with
            | 0 -> None
            | _ -> Some(ctx.Request.Query)
        use dataSource = NpgsqlDataSource.Create(CommonRepositoryFunctions.createConnectionString)
        let request =
            {
                defaultRequestContext with
                    DataSource = Some(dataSource)
                    Operation = RetrieveByID
                    Resource = Fighter
                    PathParameters = Some(pathParameters)
                    QueryParameters = queryParameters
            }
        let chain = extractReturnFieldsFromQuery
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
                        Add("FactionID", factionID).
                        Add("FighterID", fighterID)
            let queryParameters =
                match ctx.Request.Query.Count with
                | 0 -> None
                | _ -> Some(ctx.Request.Query)
            let! body = ctx.ReadBodyFromRequestAsync()
            let requestBody = { defaultRequestBody with Body = body }
            use dataSource = NpgsqlDataSource.Create(CommonRepositoryFunctions.createConnectionString)
            let request =
                {
                    defaultRequestContext with
                        DataSource = Some(dataSource)
                        Operation = Update
                        Resource = Fighter
                        PathParameters = Some(pathParameters)
                        QueryParameters = queryParameters
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
            //| Ok (fighterView, eTag) ->
            //    do ctx.SetHttpHeader("ETag", eTag)
            //    return! Successful.OK fighterView next
            | Error MalformedJson -> return! RequestErrors.BAD_REQUEST "Request body has malformed JSON" next ctx
            | Error FactionDoesNotExist -> return! RequestErrors.BAD_REQUEST "No faction with that ID exists" next ctx
            | Error FighterDoesNotExist -> return! RequestErrors.NOT_FOUND "Fighter not found" next ctx
            | _ -> return! ServerErrors.INTERNAL_ERROR "An unexpected error occurred" next ctx
        }

let HandleDeleteFighter (factionID, fighterID) : HttpHandler =
    let pathParameters =
            Map.empty.
                Add("FactionID", factionID).
                Add("FighterID", fighterID)
    use dataSource = NpgsqlDataSource.Create(CommonRepositoryFunctions.createConnectionString)
    let request =
        {
            defaultRequestContext with
                DataSource = Some(dataSource)
                Operation = Delete
                Resource = Fighter
                PathParameters = Some(pathParameters)
        }
    let chain = extractFighterValuesFromRequest
                >==> checkFactionExists
                >==> checkFighterExists
                >==> deleteFighter
    match chain(request) with
    | Ok () -> Successful.NO_CONTENT
    | Error FactionDoesNotExist -> RequestErrors.BAD_REQUEST "Not deleted - no faction with that ID exists"
    | Error FighterDoesNotExist -> RequestErrors.NOT_FOUND "Not deleted - no fighter with that ID exists"
    | _ -> ServerErrors.INTERNAL_ERROR "An unexpected error occurred"
