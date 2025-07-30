module NechronicleOpenAPIServer.FactionOperations

open CommonOperationTypes
open System.Text.Json

let defaultFighterFields =
    [
        "FighterID";
        "FactionID";
        "Name";
        "Attributes"
    ]

let listFighter ( request : RequestContext ) =
    // Assumptions: this function will be called with PathParameters
    let factionID = FactionID(request.PathParameters.Value.Item("FactionID"))
    let queryParameters = request.QueryParameters
    let filterClauses = []
    let returnFields = defaultFighterFields
    match FactionRepository.listFighter(factionID, filterClauses, returnFields) with
    | Ok fighterList -> Ok fighterList

let extractFighterValuesFromJSONBody ( body : JsonElement ) =
    // TODO: figure out how to properly test for the existence of these JSON values with System.Text.Json
    // TODO: figure out how to make this more efficient by iterating over the JSON once instead of searching
    let name = body.GetProperty("name").ToString()
    let attributes = body.GetProperty("attributes")
    let bodyDirtyValues = 
        {
            defaultDirtyValues with
                ResourceName = Some(name)
                ResourceAttributes = Some(attributes)

        }
    bodyDirtyValues

let extractFighterValuesFromRequest (request : RequestContext) =
    // Assumptions: this function will be called with PathParameters
    let pathParameters = request.PathParameters.Value
    let factionID = pathParameters.TryFind "FactionID"
    let fighterID = pathParameters.TryFind "FighterID"
    let contentType =
        match request.RequestBody with
        | Some(requestBody) -> Some(requestBody.ContentType)
        | None -> None
    let dirtyValues =
        match contentType with
        | Some(JSON) ->
            let body = request.RequestBody.Value.ParsedJson.Value
            let bodyDirtyValues = extractFighterValuesFromJSONBody body
            {
                bodyDirtyValues with
                    FighterID = fighterID
                    FactionID = factionID
            }
        | None ->
            {
                defaultDirtyValues with
                    FighterID = fighterID
                    FactionID = factionID
            }
    let updatedRequest = { request with DirtyValues = Some(dirtyValues) }
    Ok updatedRequest

let retrieveFighterById (request : RequestContext) =
    // Assumptions: this function will be called with DirtyValues with a FighterID
    let fighterID = FighterID(request.DirtyValues.Value.FighterID.Value)
    let queryParameters = request.QueryParameters
    let returnFields = defaultFighterFields
    match FactionRepository.retrieveFighterByID(fighterID, returnFields) with
    | Ok (fighter, eTag) -> Ok (fighter, eTag)

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

let createFighter (request : RequestContext) =
    // Assumptions: this function will be called with a CreatableFighter
    let creatableFighter = request.UsableResource.Value.CreatableFighter.Value
    let returnFields = defaultFighterFields
    match FactionRepository.createFighter(creatableFighter) with
    | Ok (fighterID, eTag) -> FactionRepository.retrieveFighterByID(fighterID, returnFields)

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

let updateFighter (request : RequestContext) =
    // Assumptions: this function will be called with an UpdatableFighter
    let updatableFighter = request.UsableResource.Value.UpdatableFighter.Value
    // TODO: use the query parameters to see if client wants the updated Fighter returned
    let queryParameters = request.QueryParameters
    match FactionRepository.updateFighter(updatableFighter) with
    | Ok eTag -> Ok eTag

let deleteFighter (request : RequestContext) =
    // Assumptions: this function will be called with DirtyValues with a FighterID
    let fighterID = FighterID(request.DirtyValues.Value.FighterID.Value)
    match FactionRepository.deleteFighter(fighterID) with
    | Ok () -> Ok ()
