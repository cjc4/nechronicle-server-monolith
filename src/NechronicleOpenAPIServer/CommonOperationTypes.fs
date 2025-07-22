module NechronicleOpenAPIServer.CommonOperationTypes

open FSharp.Data

type OperationErrors =
    | MalformedJson
    | FactionDoesNotExist
    | FighterDoesNotExist
    | NameNotProvided

type ContentType = | JSON

type DirtyValues =
    {
        AppUserFirstName : string option
        AppUserLastName : string option
        AppUserUUID : string option
        CampaignID : string option
        CampaignEventID : string option
        CampaignEventOccurredAt: string option
        CampaignEventType : string option
        CampaignStatus : string option
        ContactInfoID : string option
        EquipmentID : string option
        Email : string option
        FactionID : string option
        FighterID : string option
        ResourceAttributes : JsonValue option
        ResourceName : string option
        Username : string option
    }

let defaultDirtyValues =
    {
        AppUserFirstName = None
        AppUserLastName = None
        AppUserUUID = None
        CampaignID = None
        CampaignEventID = None
        CampaignEventOccurredAt = None
        CampaignEventType = None
        CampaignStatus = None
        ContactInfoID = None
        EquipmentID = None
        Email = None
        FactionID = None
        FighterID = None
        ResourceAttributes = None
        ResourceName = None
        Username = None
    }

type Operation =
    | List
    | Create
    | RetrieveByID
    | Update
    | Delete

type RequestBody =
    {
        Body : string
        ContentType : ContentType
        ParsedJson : JsonValue option
    }

let defaultRequestBody =
    {
        Body = "{}"
        ContentType = JSON
        ParsedJson = None
    }

type Resource =
    | AppUser
    | Campaign
    | CampaignEvent
    | ContactInfo
    | Equipment
    | Faction
    | Fighter

type UsableResource =
    {
        CreatableAppUser : CreatableAppUser option
        CreatableFighter : CreatableFighter option
        UpdatableAppUser : UpdatableAppUser option
        UpdatableFighter : UpdatableFighter option
    }

let defaultUsableResource =
    {
        CreatableAppUser = None
        CreatableFighter = None
        UpdatableAppUser = None
        UpdatableFighter = None
    }

type RequestContext =
    {
        Operation : Operation
        Resource : Resource
        PathParameters : Map<string, string> option
        QueryParameters : Map<string, string> option
        RequestBody : RequestBody option
        DirtyValues : DirtyValues option
        UsableResource : UsableResource option
    }

let defaultRequestContext =
    {
        Operation = List
        Resource = Campaign
        PathParameters = None
        QueryParameters = None
        RequestBody = None
        DirtyValues = None
        UsableResource = None
    }
