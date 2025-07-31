module NechronicleOpenAPIServer.CommonOperations

open CommonOperationTypes
open System.Text.Json

let extractReturnFieldsFromQuery (request : RequestContext) =
    let fields =
        match request.QueryParameters with
        | Some(collection) ->
            match collection.TryGetValue "field" with
            | true, value -> value.ToArray() |> Array.toList |> Some
            | _ -> None
        | None -> None
    let updatedRequest = { request with ReturnFields = fields }
    Ok updatedRequest

let parseRequestBody (request : RequestContext) =
    // Assumptions: this function will be called with a RequestBody
    let contentType = request.RequestBody.Value.ContentType
    match contentType with
    | JSON ->
        // TODO: handle the possible exception thrown by System.Text.Json for invalid json
        let root = JsonDocument.Parse(request.RequestBody.Value.Body).RootElement
        let updatedRequestBody = { request.RequestBody.Value with ParsedJson = Some(root) }
        let updatedRequest = { request with RequestBody = Some(updatedRequestBody) }
        Ok updatedRequest

let checkFactionExists (request : RequestContext) =
    // Assumptions: this function will be called with DirtyValues with a FactionID
    // TODO: figure out how to handle user-provided FactionIDs that aren't ints
    let factionID = request.DirtyValues.Value.FactionID.Value
                    |> int
                    |> FactionID
    let fields = ["FactionID"]
    // TODO: implement call to database to check if a faction with this FactionID exists
    Ok request

let checkFighterExists (request: RequestContext) =
    // Assumptions: this function will be called with DirtyValues with a FighterID
    // TODO: figure out how to handle user-provided FighterIDs that aren't ints
    let fighterID = request.DirtyValues.Value.FighterID.Value
                    |> int
                    |> FighterID
    let returnFields = ["FighterID"]
    // In the future, write an optimized Repository function just for confirming a matching fighter exists
    let result = FactionRepository.retrieveFighterByID(fighterID, returnFields, request.DataSource.Value)
    match result with
    | Ok _ -> Ok request
    | Error CommonRepositoryTypes.FighterDoesNotExist -> Error FighterDoesNotExist

let checkNameNotEmpty (request : RequestContext) =
    let name = request.DirtyValues.Value.ResourceName
    match name with
    | Some(name) ->
        match name with
        | "" -> Error NameNotProvided
        | _ -> Ok request
    | None -> Error NameNotProvided
