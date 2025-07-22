module NechronicleOpenAPIServer.FighterOperations

open CommonOperationTypes
open FSharp.Data

type FighterPathParameters =
    {
        FighterID : string option
        FactionID : string
    }

type DirtyFighterValues =
    {
        ID : string option
        FactionID : string
        Name : string option
        Attributes : JsonValue option
    }

let listFighter ( request : RequestContext ) =
    // Assumptions: this function will be called with PathParameters
    let factionID = FactionID(request.PathParameters.Value.Item("FactionID"))
    let queryParameters = request.QueryParameters
    match Repository.listFighter(factionID, queryParameters) with
    | Ok fighterList -> Ok fighterList

let parseRequestBody ( request : RequestContext ) =
    // Assumptions: this function will be called with a RequestBody
    let contentType = request.RequestBody.Value.ContentType
    match contentType with
    | JSON ->
        match JsonValue.TryParse(request.RequestBody.Value.Body) with
        | Some(body) ->
            let updatedReqBody = { request.RequestBody.Value with ParsedJson = Some(body) }
            let updatedRequest = { request with RequestBody = Some(updatedReqBody) }
            Ok updatedRequest
        | None -> Error MalformedJson

let extractFighterValuesFromRequest (request : RequestContext) =
    // Assumptions: this function will be called with PathParameters and a RequestBody
    let pathParameters = request.PathParameters.Value
    let factionID = pathParameters.TryFind "factionID"
    let fighterID = pathParameters.TryFind "fighterID"
    let contentType = request.RequestBody.Value.ContentType
    let name =
        match contentType with
        | JSON ->
            match request.RequestBody.Value.ParsedJson.Value.TryGetProperty "name" with
            | Some(value) -> Some(value.AsString())
            | None -> None
    let attributes =
        match contentType with
        | JSON ->
            match request.RequestBody.Value.ParsedJson.Value.TryGetProperty "attributes" with
            | Some(value) -> Some(value)
            | None -> None
    let dirtyValues =
        {
            defaultDirtyValues with
                FighterID = fighterID
                FactionID = factionID
                ResourceName = name
                ResourceAttributes = attributes
        }
    let updatedRequest = { request with DirtyValues = Some(dirtyValues) }
    Ok updatedRequest

let checkFactionIDExists (request : RequestContext) =
    // Assumptions: this function will be called with DirtyValues with a FactionID
    let factionID = FactionID(request.DirtyValues.Value.FactionID.Value)
    // TODO: implement call to database to check if a faction with this FactionID exists
    Ok request

let checkFighterExists (request: RequestContext) =
    // Assumptions: this function will be called with DirtyValues with a FighterID
    let fighterID = FactionID(request.DirtyValues.Value.FighterID.Value)
    // TODO: implement call to database to check if a fighter with this FighterID exists
    Ok request

// let checkNameNotEmpty (request : RequestContext) =
//     let name = request.Name
//     match name with
//     | Some(name) ->
//         match name with
//         | "" -> Error NameNotProvided
//         | _ -> Ok request
//     | None -> Error NameNotProvided

let makeCreatableFighter (request : RequestContext) =
    // Assumptions: this function will be called with DirtyValues
    // In the future, implement functions for creating each value type that validates
    let values = request.DirtyValues.Value
    let attributes = values.ResourceAttributes
                     |> Option.map Attributes
    let creatableFighter =
        {
            FactionID = FactionID values.FactionID.Value
            Name = values.ResourceName.Value
            Attributes = attributes
        } : CreatableFighter
    let usableResource = { defaultUsableResource with CreatableFighter = Some(creatableFighter) }
    let updatedRequest = { request with UsableResource = Some(usableResource) }
    Ok updatedRequest

let makeUpdatableFighter (request: RequestContext) =
    // Assumptions: this function will be called with DirtyValues
    // In the future, implement functions for creating each value type that validates
    let values = request.DirtyValues.Value
    let attributes = values.ResourceAttributes
                     |> Option.map Attributes
    let updatableFighter =
        {
            ID = FighterID values.FighterID.Value
            FactionID = FactionID values.FactionID.Value
            Name = values.ResourceName
            Attributes = attributes
        } : UpdatableFighter
    let usableResource = { defaultUsableResource with UpdatableFighter = Some(updatableFighter) }
    let updatedRequest = { request with UsableResource = Some(usableResource) }
    Ok updatedRequest

let createFighter (request : RequestContext) =
    // Assumptions: this function will be called with a CreatableFighter
    let creatableFighter = request.UsableResource.Value.CreatableFighter.Value
    match Repository.createFighter(creatableFighter) with
    | Ok fighter -> Ok fighter

let updateFighter (request : RequestContext) =
    // Assumptions: this function will be called with an UpdatableFighter
    let updatableFighter = request.UsableResource.Value.UpdatableFighter.Value
    match Repository.updateFighter(updatableFighter) with
    | Ok fighter -> Ok fighter
