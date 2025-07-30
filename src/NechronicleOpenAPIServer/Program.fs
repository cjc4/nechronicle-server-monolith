module NechronicleOpenAPIServer.App

open AppUserEndpointHandlers
open FactionEndpointHandlers
open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Giraffe.EndpointRouting
open System.Text.Json.Serialization

// ---------------------------------
// Routing to endpoint handlers
// ---------------------------------

let endpoints =
    [
        // subRoute "/campaigns" [
        //     GET [
        //         // route "/" HandleListCampaign
        //     ]
        // ]
        subRoute "/factions" [
            DELETE [
                routef "/%s/units/%s" HandleDeleteFighter
            ]
            GET [
                // route "/" HandleListFaction
                // routef "/%s" HandleRetrieveFactionById
                routef "/%s/units" HandleListFighter
                routef "/%s/units/%s" HandleRetrieveFighterById
            ]
            PATCH [
                routef "/%s/units/%s" HandleUpdateFighter
            ]
            POST [
                routef "/%s/units" HandleCreateFighter
            ]
        ]
        subRoute "/users" [
            GET [
                route "/" HandleListUser
                // routef "/%s" HandleRetrieveUserById
            ]
        ]
    ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder
        .WithOrigins(
            "http://localhost:5000",
            "https://localhost:5001")
       .AllowAnyMethod()
       .AllowAnyHeader()
       |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (match env.IsDevelopment() with
    | true  ->
        app.UseDeveloperExceptionPage()
    | false ->
        app .UseGiraffeErrorHandler(errorHandler)
            .UseHttpsRedirection())
        .UseCors(configureCors)
        .UseRouting()
        .UseEndpoints(fun e -> e.MapGiraffeEndpoints(endpoints))
        |> ignore

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore
    let jsonOptions =
        JsonFSharpOptions.Default()
            .WithUnionUntagged()
            .WithSkippableOptionFields()
    services.AddSingleton<Json.ISerializer>(Json.FsharpFriendlySerializer(jsonOptions)) |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.AddConsole()
           .AddDebug() |> ignore

[<EntryPoint>]
let main args =
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .Configure(Action<IApplicationBuilder> configureApp)
                    .ConfigureServices(configureServices)
                    .ConfigureLogging(configureLogging)
                    |> ignore)
        .Build()
        .Run()
    0
