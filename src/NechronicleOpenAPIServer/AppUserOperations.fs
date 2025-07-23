module NechronicleOpenAPIServer.AppUserOperations

open CommonOperationTypes

let listAppUser ( request : RequestContext ) =
    let queryParameters = request.QueryParameters
    match AppUserRepository.listAppUser(queryParameters) with
    | Ok appUserList -> Ok appUserList
