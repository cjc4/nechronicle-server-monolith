module NechronicleOpenAPIServer.CommonRepositoryTypes

exception RepositoryInputException of string

type RepositoryErrors =
    | FighterDoesNotExist
    | FactionDoesNotExist
    | NoMatchesFound

// type whereClause =
//     {
//         column : string
//         operator : string
//         value : string
//     }
