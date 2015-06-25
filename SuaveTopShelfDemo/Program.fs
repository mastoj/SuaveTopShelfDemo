open Suave
open Suave.Http.Successful
open Suave.Web 
open Topshelf
open System

[<AutoOpen>]
module Topshelf = 
    type ServiceName = ServiceName of string
    type DisplayName = DisplayName of string
    type TopshelfSpec = 
        {
            DisplayName: DisplayName option
            ServiceName: ServiceName option
            Start: (HostControl -> bool) option
            Stop: (HostControl -> bool) option
        }
        static member Zero = {DisplayName = None; ServiceName = None; Start = None; Stop = None}

    let toAction1 f = Action<_>(f)
    let toFunc f = Func<_>(f)

    let withDisplayName name spec = {spec with DisplayName = Some name}
    let withServiceName name spec = {spec with ServiceName = Some name}
    let withStart start spec = {spec with Start = Some start}
    let withStop stop spec = {spec with Stop = Some stop}

    let createHostConfigurator (DisplayName dn) (ServiceName sn) createFun (conf:HostConfigurators.HostConfigurator) =
        let createAction = createFun |> toFunc
        conf.SetDisplayName(dn)
        conf.SetServiceName(sn)
        conf.Service<ServiceControl>(createAction) |> ignore
        
    let service (start : HostControl -> bool) (stop : HostControl -> bool) () =
        { new ServiceControl with
            member x.Start hc =
              start hc
            member x.Stop hc =
              stop hc }

    let run spec = 
        service spec.Start.Value spec.Stop.Value 
        |> createHostConfigurator spec.DisplayName.Value spec.ServiceName.Value
        |> toAction1
        |> HostFactory.Run

[<EntryPoint>]
let main argv = 
    printfn "%A" argv

    let start hc = 
        startWebServerAsync defaultConfig (OK "Hello World from a service") 
        |> snd
        |> Async.StartAsTask 
        |> ignore
        true

    let stop hc = true

    TopshelfSpec.Zero 
    |> withDisplayName (DisplayName "ServiceDisplayName")
    |> withServiceName (ServiceName "ServiceName")
    |> withStart start
    |> withStop stop
    |> run
    |> ignore

    0

