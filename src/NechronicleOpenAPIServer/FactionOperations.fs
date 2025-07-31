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

let extractFighterFiltersFromQuery (request : RequestContext) =
    let filterByName =
        match request.QueryParameters with
        | Some(collection) ->
            match collection.TryGetValue "name" with
            | true, value -> Some({ Field = "Name"; Operator = "="; Value = value.ToString() })
            | _ -> None
        | None -> None
    let filters =
        match filterByName with
        | Some(filter) -> Some([filter])
        | None -> None
    let updatedRequest = { request with Filters = filters }
    Ok updatedRequest

// let listFighter (request : RequestContext) =
//     // Assumptions: this function will be called with PathParameters
//     // TODO: figure out how to handle user-provided FactionIDs that aren't ints
//     let factionID = request.PathParameters.Value.Item("FactionID")
//                     |> int
//                     |> FactionID
//     let filterByFactionID = { Field = "FactionID"; Operator = "="; Value = factionID.ToString() }
//     let filters =
//         match request.Filters with
//         | Some(filters) -> filterByFactionID :: filters
//         | None -> [filterByFactionID]
//     let returnFields =
//         match request.ReturnFields with
//         | Some(fields) -> fields
//         | None -> defaultFighterFields
//     match FactionRepository.listFighter(returnFields, filters, request.DataSource.Value) with
//     | Ok fighterList -> Ok fighterList

let extractFighterValuesFromJSONBody (body : JsonElement) =
    // TODO: figure out how to properly test for the existence of these JSON values with System.Text.Json

    // In the future, figure out how to make this more efficient by iterating over the JSON once instead of searching
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
    // TODO: figure out how to handle user-provided FighterIDs that aren't ints
    let fighterID = request.DirtyValues.Value.FighterID.Value
                    |> int
                    |> FighterID
    let returnFields =
        match request.ReturnFields with
        | Some(fields) -> fields
        | None -> defaultFighterFields
    let result = FactionRepository.retrieveFighterByID(fighterID, returnFields, request.DataSource.Value)
    match result with
    | Ok (fighter, eTag) -> Ok (fighter, eTag)
    | Error CommonRepositoryTypes.FighterDoesNotExist -> Error FighterDoesNotExist

let makeCreatableFighter (request : RequestContext) =
    // Assumptions: this function will be called with DirtyValues
    // In the future, implement functions for creating each value type that validates
    let values = request.DirtyValues.Value
    let attributes = values.ResourceAttributes
                     |> Option.map Attributes
    let creatableFighter =
        {
            FactionID = values.FactionID.Value |> int |> FactionID
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
    match FactionRepository.createFighter(creatableFighter, request.DataSource.Value) with
    | Ok (fighterID, eTag) -> FactionRepository.retrieveFighterByID(fighterID, returnFields, request.DataSource.Value)

let makeUpdatableFighter (request: RequestContext) =
    // Assumptions: this function will be called with DirtyValues
    // In the future, implement functions for creating each value type that validates
    let values = request.DirtyValues.Value
    let attributes = values.ResourceAttributes
                     |> Option.map Attributes
    let updatableFighter =
        {
            ID = values.FighterID.Value |> int |> FighterID
            FactionID = values.FactionID.Value |> int |> FactionID
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
    match FactionRepository.updateFighter(updatableFighter, request.DataSource.Value) with
    | Ok eTag -> Ok eTag

let deleteFighter (request : RequestContext) =
    // Assumptions: this function will be called with DirtyValues with a FighterID
    let fighterID = request.DirtyValues.Value.FighterID.Value |> int |> FighterID
    match FactionRepository.deleteFighter(fighterID, request.DataSource.Value) with
    | Ok () -> Ok ()
