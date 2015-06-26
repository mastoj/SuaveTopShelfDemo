open Suave
open Suave.Http.Successful
open Suave.Web 
open Topshelf
open System

[<EntryPoint>]
let main argv = 
    printfn "%A" argv

    let start hc = 
        startWebServerAsync defaultConfig (OK "Hello World from a service") 
        |> snd
        |> Async.StartAsTask 
        |> ignore
        true

    let stop hc = 
        true

    Service.Default 
    |> display_name "ServiceDisplayName"
    |> instance_name "ServiceName"
    |> with_start start
    |> with_stop stop
    |> with_topshelf

