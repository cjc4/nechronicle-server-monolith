module NechronicleOpenAPIServer.Fighter

// Third party dependencies
open FSharp.Data

let create id factionID name attributes =
    {
        ID = id
        FactionID = factionID
        Name = name
        Attributes = attributes
    }

let getDefaultAttributes = Attributes(JsonValue.Parse("{}"))
