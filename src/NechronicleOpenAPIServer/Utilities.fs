namespace NechronicleOpenAPIServer

[<AutoOpen>]
module Utilities =

    let (>==>) switch1 switch2 x =
        match switch1 x with
        | Ok s -> switch2 s
        | Error f -> Error f
