namespace NechronicleOpenAPIServer

open FSharp.Data
open System

[<AutoOpen>]
module DomainTypes =
    // Domain properties.
    type AppUserUUID = AppUserUUID of Guid
    type Attributes = Attributes of JsonValue
    type CampaignID = CampaignID of string
    type CampaignEventID = CampaignEventID of string
    type CampaignEventType = CampaignEventType of string
    type ContactInfoID = ContactInfoID of string
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
