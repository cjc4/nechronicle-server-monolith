module NechronicleOpenAPIServer.FactionRepository

open CommonRepositoryTypes
open Microsoft.FSharp.Core
open System
open System.Text.Json
open Npgsql
open Npgsql.FSharp

let convertFighterFieldToColumn field =
    match field with
    | "ETag" -> "http_etag"
    | "FighterID" -> "id"
    | "FactionID" -> "faction_id"
    | "Name" -> "name"
    | "Attributes" -> "attributes"
    | _ -> raise (RepositoryInputException("Invalid field name provided: " + field))

type FighterRow =
    {
        ETag : Guid
        ID : int option
        FactionID : int option
        Name : string option
        Attributes : JsonElement option
    }

let createFighter (input : CreatableFighter, dataSource : NpgsqlDataSource) =
    let fighterID = FighterID 100
    let eTag = "33a64df551425fcc33e4d42a148795d9f25f89d4"
    Ok (fighterID, eTag)

let deleteFighter (id : FighterID, dataSource : NpgsqlDataSource) =
    Ok ()

//let listFighter (returnFields : string list, filters: FilterByField list, dataSource : NpgsqlDataSource) =
    // selectColumnList = List.map convertFighterFieldToColumn returnFields
    //let selectString = String.Join(", ", selectColumnList)

    // DRAFT idea, to review
    // let whereFields = (filters, []) ||> List.foldBack filterToFieldList
    // let whereColumns = List.map convertFighterFieldToColumn whereFields
    // let whereOperators = (filters, []) ||> List.foldBack filterToOperatorList
    // let command  = dataSource.CreateCommand("SELECT " + selectString + " FROM faction.unit WHERE " + whereString)

    // MOCK list, always returning all fighters WHERE faction_id=1
    //let command = dataSource.CreateCommand("SELECT " + selectString + " FROM faction.unit WHERE faction_id=$1")

let retrieveFighterByID (fighterID : FighterID, returnFields : string list, dataSource : NpgsqlDataSource) =
    let unwrapFighterID (FighterID a) = a // Is this really necessary?
    let id = unwrapFighterID fighterID
    let fieldList = "ETag" :: returnFields
    let columnList = List.map convertFighterFieldToColumn fieldList
    let selectString = String.Join(", ", columnList)

    use connection = dataSource.CreateConnection()
    connection.Open()
    let fighterRow =
        dataSource
        |> Sql.fromDataSource
        |> Sql.query("SELECT " + selectString + " FROM faction.unit WHERE id=@id")
        |> Sql.parameters [ "@id", Sql.int id ] // Is there a way to use positional parameters, instead of named?
        |> Sql.executeRow (fun read ->
            {
                ETag = read.uuid "http_etag"
                ID = read.intOrNone "id"
                FactionID = read.intOrNone "faction_id"
                Name = read.textOrNone "name"
                Attributes =
                    match read.fieldValueOrNone<JsonDocument>("attributes") with
                    | Some(document) -> Some(document.RootElement)
                    | None -> None
            } : FighterRow)
    let fighterView =
        {
            ID = fighterRow.ID |> Option.map FighterID
            FactionID = fighterRow.FactionID |> Option.map FactionID
            Name = fighterRow.Name
            Attributes = fighterRow.Attributes |> Option.map Attributes
        } : FighterView
    let eTag = fighterRow.ETag.ToString()
    // TODO: figure out how to test for failure to return any results
    Ok (fighterView, eTag)

    // ORIGINAL code below
    // task {
    //     let fieldList = "ETag" :: returnFields
    //     let columnList = List.map convertFighterFieldToColumn returnFields
    //     let selectString = String.Join(", ", columnList)
    //     let command = dataSource.CreateCommand("SELECT " + selectString + " FROM faction.unit WHERE id=$1")
    //     do command.Parameters.AddWithValue(fighterID) |> ignore
    //     let! reader = command.ExecuteReaderAsync()
    //     match reader.HasRows with
    //     | true ->
    //         let eTag = reader.GetString 0 // ETag will always be the first column selected, see above.
    //         let fighterID =
    //             match fieldList |> List.tryFindIndex (fun str -> String.Equals (str, "FighterID")) with
    //             | Some(index) -> reader.GetInt32 index |> FighterID |> Some
    //             | None -> None
    //         let factionID =
    //             match fieldList |> List.tryFindIndex (fun str -> String.Equals (str, "FactionID")) with
    //             | Some(index) -> reader.GetInt32 index |> FactionID |> Some
    //             | None -> None
    //         let name =
    //             match fieldList |> List.tryFindIndex (fun str -> String.Equals (str, "Name")) with
    //             | Some(index) -> reader.GetString index |> Some
    //             | None -> None
    //         let attributes =
    //             match fieldList |> List.tryFindIndex (fun str -> String.Equals (str, "Attributes")) with
    //             | Some(index) -> reader.GetFieldValue<JsonDocument>(index).RootElement |> Attributes |> Some
    //             | None -> None
    //         let fighterView =
    //             {
    //                 ID = fighterID
    //                 FactionID = factionID
    //                 Name = name
    //                 Attributes = attributes
    //             } : FighterView
    //         return Ok (fighterView, eTag)
    //     | false -> return Error FighterDoesNotExist
    // }

let updateFighter (input : UpdatableFighter, dataSource : NpgsqlDataSource) =
    let eTag = "33a64df551425fcc44e4d42a148795d9f25f89d4"
    Ok eTag
