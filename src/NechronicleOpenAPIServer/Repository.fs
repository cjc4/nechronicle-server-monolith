module NechronicleOpenAPIServer.Repository

// Third party dependencies
open System
open FSharp.Data
// TODO: figure out if I can make a generic "retrieve <T> by ID"

let createFighter draftFighter =
    let attributes =
        match draftFighter.Attributes with
        | Some(attributes) -> attributes
        | None -> JsonValue.Parse("{}")
    let fighter =
        {
            ID = FighterID "102"
            FactionID = draftFighter.FactionID
            Name = draftFighter.Name
            Attributes = Attributes attributes
        }
    Ok fighter

let listFighter =
    let mockSnood =
        {
            ID = FighterID "100"
            FactionID = FactionID "100"
            Name = "Snood the lesser"
            Attributes = Attributes(JsonValue.Parse("{'category':'Juve','Type':'Bonepicker','XP':'1'}"))
        }
    let mockHauberk =
        {
            ID = FighterID "101"
            FactionID = FactionID "100"
            Name = "Hauberk 'Heavy-hand'"
            Attributes = Attributes(JsonValue.Parse("{'category':'Leader','Type':'Cawdor Word-Keeper','XP':'7'}"))
        }
    let mockList = [mockSnood; mockHauberk]
    mockList

let retrieveFighterByID fighterID =
    let mockSnood =
        {
            ID = FighterID "100"
            FactionID = FactionID "100"
            Name = "Snood the lesser"
            Attributes = Attributes(JsonValue.Parse("{'category':'Juve','Type':'Bonepicker','XP':'1'}"))
        }
    Ok mockSnood

let listUser =
    let mockBob =
        {
            ID = AppUserUUID(Guid.Parse("01982ea1-6de2-7195-8100-2c967a64288b"))
            Username = "burgermaster"
            Email = Email "bob@example.com"
            FirstName = Some("Bob")
            LastName = Some("Belcher")
        }
    let mockGandalf =
        {
            ID = AppUserUUID(Guid.Parse("01982ea2-f46d-7dd4-bb53-1ba39d458052"))
            Username = "wise1"
            Email = Email "gandalf@example.com"
            FirstName = Some("Gandalf")
            LastName = Some("Greyhame")
        }
    let mockList = [mockBob; mockGandalf]
    mockList
