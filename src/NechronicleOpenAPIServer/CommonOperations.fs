module NechronicleOpenAPIServer.CommonOperations

open CommonOperationTypes
open System.Text.Json

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
    let factionID = FactionID(request.DirtyValues.Value.FactionID.Value)
    let fields = ["id"]
    // TODO: implement call to database to check if a faction with this FactionID exists
    Ok request

let checkFighterExists (request: RequestContext) =
    // Assumptions: this function will be called with DirtyValues with a FighterID
    let fighterID = FighterID(request.DirtyValues.Value.FighterID.Value)
    let fields = ["id"]
    match FactionRepository.retrieveFighterByID(fighterID, fields) with
    | Ok _ -> Ok request
    | Error FighterDoesNotExist -> Error FighterDoesNotExist

let checkNameNotEmpty (request : RequestContext) =
    let name = request.DirtyValues.Value.ResourceName
    match name with
    | Some(name) ->
        match name with
        | "" -> Error NameNotProvided
        | _ -> Ok request
    | None -> Error NameNotProvided
