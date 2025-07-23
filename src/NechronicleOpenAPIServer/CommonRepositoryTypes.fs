module NechronicleOpenAPIServer.CommonRepositoryTypes

exception RepositoryInputError of string

type HttpEtag = HttpEtag of string

type whereClause =
    {
        column : string
        operator : string
        value : string
    }
