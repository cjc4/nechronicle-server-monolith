module NechronicleOpenAPIServer.FactionRepository

open CommonRepositoryTypes
open FSharp.Data
open Microsoft.FSharp.Core

let convertFieldToColumn field =
    match field with
    | "FighterID" -> "id"
    | "FactionID" -> "faction_id"
    | "Name" -> "name"
    | "Attributes" -> "attributes"
    | _ -> raise (RepositoryInputError("Invalid field name provided: " + field))

let createFighter (input : CreatableFighter) =
    let fighterID = FighterID "100"
    let eTag = "33a64df551425fcc33e4d42a148795d9f25f89d4"
    Ok (fighterID, eTag)

let deleteFighter (id : FighterID) =
    Ok ()

let listFighter ( factionID, filterClauses, returnFields) =
    let mockSnood =
        {
            ID = Some(FighterID "100")
            FactionID = Some(factionID)
            Name = Some("Snood the lesser")
            Attributes = Some(Attributes(JsonValue.Parse("""{"category":"Juve","Type":"Bonepicker","XP":"1"}""")))
        } : FighterView
    let mockHauberk =
        {
            ID = Some(FighterID "101")
            FactionID = Some(factionID)
            Name = Some("Hauberk 'Heavy-hand'")
            Attributes = Some(Attributes(JsonValue.Parse("""{"category":"Leader","Type":"Cawdor Word-Keeper","XP":"7"}""")))
        } : FighterView
    let mockList = [mockSnood; mockHauberk]
    Ok mockList

let retrieveFighterByID (fighterID, returnFields) =
    let mockSnood =
        {
            ID = Some(fighterID)
            FactionID = Some(FactionID "100")
            Name = Some("Snood the lesser")
            Attributes = Some(Attributes(JsonValue.Parse("""{"category":"Juve","Type":"Bonepicker","XP":"1"}""")))
        } : FighterView
    let eTag = "33a64df551425fcc55e4d42a148795d9f25f89d4"
    Ok (mockSnood, eTag)

let updateFighter (input : UpdatableFighter) =
    let eTag = HttpEtag "33a64df551425fcc44e4d42a148795d9f25f89d4"
    Ok eTag
