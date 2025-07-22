module NechronicleOpenAPIServer.Repository

open FSharp.Data
open System

let createFighter (input : CreatableFighter) =
    let attributes =
        match input.Attributes with
        | Some(attributes) -> attributes
        | None -> Attributes(JsonValue.Parse("{}"))
    let fighter =
        {
            ID = FighterID "102"
            FactionID = input.FactionID
            Name = input.Name
            Attributes = attributes
        } : Fighter
    Ok fighter

let deleteFighter (id : FighterID) =
    Ok

let listFighter ( factionID : FactionID, queryParameters) =
    let mockSnood =
        {
            ID = FighterID "100"
            FactionID = factionID
            Name = "Snood the lesser"
            Attributes = Attributes(JsonValue.Parse("""{"category":"Juve","Type":"Bonepicker","XP":"1"}"""))
        } : Fighter
    let mockHauberk =
        {
            ID = FighterID "101"
            FactionID = factionID
            Name = "Hauberk 'Heavy-hand'"
            Attributes = Attributes(JsonValue.Parse("""{"category":"Leader","Type":"Cawdor Word-Keeper","XP":"7"}"""))
        } : Fighter
    let mockList = [mockSnood; mockHauberk]
    Ok mockList

let listAppUser queryParameters =
    let mockBob =
        {
            ID = AppUserUUID(Guid.Parse("01982ea1-6de2-7195-8100-2c967a64288b"))
            Username = "burgermaster"
            Email = Email "bob@example.com"
            FirstName = Some("Bob")
            LastName = Some("Belcher")
        } : AppUser
    let mockGandalf =
        {
            ID = AppUserUUID(Guid.Parse("01982ea2-f46d-7dd4-bb53-1ba39d458052"))
            Username = "wise1"
            Email = Email "gandalf@example.com"
            FirstName = Some("Gandalf")
            LastName = Some("Greyhame")
        } : AppUser
    let mockList = [mockBob; mockGandalf]
    Ok mockList

let retrieveFighterByID (id : FighterID) =
    let mockSnood =
        {
            ID = id
            FactionID = FactionID "100"
            Name = "Snood the lesser"
            Attributes = Attributes(JsonValue.Parse("""{"category":"Juve","Type":"Bonepicker","XP":"1"}"""))
        } : Fighter
    Ok mockSnood

let updateFighter (input : UpdatableFighter) =
    let name =
        match input.Name with
        | Some(name) -> name
        | None -> "Snood the lesser"
    let attributes =
        match input.Attributes with
        | Some(attributes) -> attributes
        | None -> Attributes(JsonValue.Parse("""{"category":"Juve","Type":"Bonepicker","XP":"1"}"""))
    let mockFighter =
        {
            ID = input.ID
            FactionID = input.FactionID
            Name = name
            Attributes = attributes
        } : Fighter
    Ok mockFighter
