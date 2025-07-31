module NechronicleOpenAPIServer.CommonRepositoryFunctions

open System

let createConnectionString =
    let dbHost = Environment.GetEnvironmentVariable "NECHRONICLE_POSTGRES_HOST"
    // TODO: catch null exception from below and provide default value
    let dbPort =
        match Environment.GetEnvironmentVariable "NECHRONICLE_POSTGRES_PORT" with
        | value -> value
        | _ -> "5432" // Default PostgreSQL port
    let dbUsername = Environment.GetEnvironmentVariable "PGUSER"
    let dbPassFile = Environment.GetEnvironmentVariable "PGPASSFILE"
    let dbDatabase = Environment.GetEnvironmentVariable "NECHRONICLE_POSTGRES_DATABASE"
    "Host=" + dbHost + ";Port=" + dbPort + ";Username=" + dbUsername +
        ";Passfile=" + dbPassFile + ";Database=" + dbDatabase

let filterToFieldList (filter : FilterByField, fieldList : string list) =
    let field = filter.Field
    field :: fieldList

let filterToOperatorList (filter : FilterByField, operatorList : string list) =
    let operator = filter.Operator
    operator :: operatorList
