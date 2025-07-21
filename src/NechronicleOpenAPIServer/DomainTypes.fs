namespace NechronicleOpenAPIServer

open System
open FSharp.Data

[<AutoOpen>]
module DomainTypes =
    // Domain properties.
    type AppUserUUID = AppUserUUID of Guid
    type Attributes = Attributes of JsonValue
    type CampaignID = CampaignID of string
    type EquipmentID = EquipmentID of string
    type Email = Email of string
    type FactionID = FactionID of string
    type FighterID = FighterID of string

    // Domain entities.
    type AppUser =
        {
            ID : AppUserUUID
            Username : string
            Email : Email
            FirstName : string option
            LastName : string option
        }

    type Campaign =
        {
            ID : CampaignID
            Name : string
            Status : string
            Attributes : Attributes
        }

    type Equipment =
        {
            ID : EquipmentID
            FactionID : FactionID
            FighterID : FighterID option
            Name:string
            Attributes:Attributes
        }

    type Faction =
        {
            ID : FactionID
            CampaignID : CampaignID
            AppUserID : AppUserUUID
            Name:string
            Attributes:Attributes
        }

    type Fighter =
        {
            ID : FighterID
            FactionID : FactionID
            Name : string
            Attributes : Attributes
        }

    type DraftFighter =
        {
            FactionID : FactionID
            Name : string
            Attributes : JsonValue option
        }
