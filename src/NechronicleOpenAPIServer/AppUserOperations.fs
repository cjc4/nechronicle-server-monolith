module NechronicleOpenAPIServer.AppUserOperations

open CommonOperationTypes

let listAppUser ( request : RequestContext ) =
    let queryParameters = request.QueryParameters
    match Repository.listAppUser(queryParameters) with
    | Ok appUserList -> Ok appUserList
