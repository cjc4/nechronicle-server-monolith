namespace NechronicleOpenAPIServer

open System
open System.Text.Json

[<AutoOpen>]
module DomainTypes =
    // Domain properties.
    type AppUserUUID = AppUserUUID of Guid
    type Attributes = Attributes of JsonElement
    type CampaignID = CampaignID of int
    type CampaignEventID = CampaignEventID of int
    type CampaignEventType = CampaignEventType of string
    type ContactInfoID = ContactInfoID of int
    type EquipmentID = EquipmentID of int
    type Email = Email of string
    type FactionID = FactionID of int
    type FighterID = FighterID of int

    // Domain entities.
    type AppUser =
        {
            ID : AppUserUUID
            Username : string
            Email : Email
            FirstName : string option
            LastName : string option
        }

    type CreatableAppUser =
        {
            Username : string
            Email : Email
            FirstName : string option
            LastName : string option
        }

    type UpdatableAppUser =
        {
            ID : AppUserUUID
            Username : string option
            Email : Email option
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

    type CampaignEvent =
        {
            ID : CampaignEventID
            CampaignID : CampaignID
            OccurredAt : string // TODO: implement an ISO 8601 datetime type
            Type : CampaignEventType
            Attributes : Attributes
        }

    type ContactInfo =
        {
            ID : ContactInfoID
            AppUserUUID : AppUserUUID
            Service : string
            Username : string
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

    // type FighterValue =
    //     | ID of FighterID
    //     | FactionID of FactionID
    //     | Name of string
    //     | Attributes of Attributes

    type FighterView =
        {
            ID : FighterID option
            FactionID : FactionID option
            Name : string option
            Attributes : Attributes option
        }

    type CreatableFighter =
        {
            FactionID : FactionID
            Name : string
            Attributes : Attributes option
        }

    type UpdatableFighter =
        {
            ID : FighterID
            FactionID : FactionID
            Name : string option
            Attributes : Attributes option
        }

    // Can I use this to allow the FilterByField records to container Values of many types?
    // type FilterValue =
    //     | FighterValue of FighterValue

    type FilterByField =
        {
            Field : string
            Operator : string
            Value : string
        }
