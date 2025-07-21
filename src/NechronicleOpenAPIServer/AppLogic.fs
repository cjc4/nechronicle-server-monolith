module NechronicleOpenAPIServer.AppLogic
// TODO: find a better name and/or layer for this

let factionIDExists (draftFighter : DraftFighter) =
    // TODO: implement call to database to check if a faction with this draftFighter's FactionID exists
    Ok draftFighter

let createFighter (draftFighter : DraftFighter) =
    factionIDExists
    >=> Repository.createFighter
