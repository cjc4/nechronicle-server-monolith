module NechronicleOpenAPIServer.AppUserEndpointHandlers

open CommonOperationTypes
open CommonOperations
open AppUserOperations
open Giraffe
open Microsoft.AspNetCore.Http

let HandleListUser : HttpHandler =
    // TODO: figure out how to get query parameters from URL
    let request =
        {
            defaultRequestContext with
                Operation = List
                Resource = AppUser
        }
    let chain = listAppUser
    match chain(request) with
    | Ok appUserList -> Successful.OK appUserList
