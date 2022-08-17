module App

open System
open Fable.Core
open Fable.Core.JsInterop
open ``node-ssh``
open Fable.RegexProvider
open System.IO
open FSharp.Control
open Node
open Node.Api
open timers.__timers_promises

[<ImportMember("node-ssh")>]
let NodeSSH: NodeSSHStatic = jsNative

let inline promiseToAsync x = x |> Async.AwaitPromise |> AsyncRx.ofAsync

let ssh = NodeSSH.Create()

let DEFAULT_SCPFLAGS = "-p"
let DEFAULT_CLIENTFLAGS = "--autocall --autoconnect"

let Verbose = true
let ScpFlags = ""
let RunningDirectory = "/tmp"
let LogDirectory = "./out"
let ClientFlags = DEFAULT_CLIENTFLAGS
let ServerFlags = ""
let InitScript = "envInitBundle"
let Runtime = None

let log x = if Verbose then printfn $"{x}" else ()

let parseNode x =
    if x <> "disabled" then
        let results: string[] = x?``match``(@"([^@]+)@([^\n]+)")
        Some(results[1], results[2])
    else
        None

let executeLocalCommand command =
    match childProcess.execSync(command) with U2.Case1 x -> x | U2.Case2 _ -> failwith "Not a string"

let resolveExecutable name =
    if fs.existsSync (U2.Case1 name) then
        path.resolve [|name|]
    else
        executeLocalCommand $"which {name}"

let makeNode privKey user host =
    ssh.connect(!!{|
        host = host;
        username = user;
        privateKey = privKey
    |})

let startNode (node : NodeSSH) command =
    let disOut, stdout = AsyncRx.subject<string> ()
    
    let stdin = stream.PassThrough.Create()

    let command = node.exec($"%s{command}", ResizeArray<string> [||], jsOptions<SSHExecOptions>(fun x ->
        let handler = Some (fun (x: Buffer.Buffer) ->
            (disOut.OnNextAsync (x.toString(Buffer.BufferEncoding.Utf8))) |> Async.StartImmediate
        )
        x.onStdout <- handler
        x.onStderr <- handler
        x.stdin <- Some (U2.Case2 stdin)
        x.execOptions <- Some (jsOptions<ExecOptions>(fun x ->
            x.pty <- Some (U2.Case2 true)
        ))
    ))

    command, stdout, (fun _ -> stdin.``end``("\x03"))

[<Emit("Promise.race($0)")>]
let race (pr: seq<JS.Promise<'T>>): JS.Promise<'T> = jsNative

let out path =
    let createPathString name =
        let rec go n =
            let name = $"{name}{n}"
            if fs.existsSync (U2.Case1 name) then go (n + 1) else name

        go 0

    let fd = fs.openSync(U2.Case1 (createPathString path), U2.Case1 "a+")

    (fun x ->
        printfn "%s" x

        fs.writeSync(fd, $"{x}\n") |> ignore)

type CLIArguments = {
    privateKey: string
    serverPath: string
    clientPath: string
    server: string
    firstnode: string
    secondnode: string
}

let hashFile x =
    crypto.createHash("sha256").update(fs.readFileSync(x)).digest("hex") :?> string

let copyFile (node: NodeSSH) local target options setExec =
    promise {
        let realHash = hashFile local
        printfn "%A" realHash
        let! resp = node.execCommand($"sha256sum {target}")
        let hash = (resp.stdout.Split ' ')[0]
        if hash <> realHash then
            Console.WriteLine($"Mismatch between local file and target file, copying... Expected: {realHash} Actual: {hash}")
            do! (node.execCommand($"rm {target}") |> Promise.map(fun x -> printfn $"{x.stdout}"; ()))
            do! node.putFile(local,target, ?transferOptions=options)
        if setExec then
            let! resp = node.execCommand($"chmod +x {target}")
            Console.WriteLine(resp.stderr + resp.stdout)
    }

let handleInput (x: CLIArguments) =
    promise {
        let privateKey = x.privateKey
        let (serverUser, serverHost) = (x.server |> parseNode).Value
        let first = x.firstnode |> parseNode
        let second = x.secondnode |> parseNode

        let clients =
            [|
                first
                second
            |]
            |> Array.choose id

        log "Verbose logging enabled"

        log "Searching for server binary..."

        let serverBinary = resolveExecutable x.serverPath

        log $"Found at {serverBinary}"

        log "Searching for client binary..."

        let clientBinary = resolveExecutable x.clientPath

        log $"Found at {clientBinary}"


        let scpFlags = ScpFlags + DEFAULT_SCPFLAGS

        let runningDir = RunningDirectory

        let logDir = LogDirectory

        try 
            fs.mkdirSync logDir
        with
        | _ -> ()

        let clientFlags = ClientFlags + $" --server {serverHost}"

        let serverFlags = ServerFlags

        let runtime = Runtime

        let serverPath = $"{runningDir}/peerconnection_server"
        let clientPath = $"{runningDir}/peerconnection_client"

        log $"Input information: {serverHost} {serverUser} {Verbose} {serverBinary} {clientBinary} {scpFlags} {runningDir} {logDir} {clientFlags} {serverFlags} {runtime}"

        log "Starting server..."

        let! server = makeNode privateKey serverUser serverHost

        log "Server started!"

        let serverLogPath = $"{logDir}/server_{serverHost}.log"

        log "Copying server binary to target location..."

        do! copyFile server serverBinary serverPath None true

        log "Copy complete!"

        log "Starting server process and logging..."

        let (_, serverOut, serverKill) = startNode server $"{serverPath} {serverFlags}"

        serverOut
        |> AsyncRx.toObservable
        |> Observable.subscribe (out serverLogPath) |> ignore

        log "Server and process logging initialized!"

        log "Starting clients..."

        let! clients =
            clients
            |> Array.map(fun (user, host) ->
                promise {
                    let! clientProcess = makeNode privateKey user host
                    let clientLogPath = $"{logDir}/client_{host}.log"
                    return (host, clientProcess, clientLogPath)
                }
            )
            |> Promise.all

        log "Clients started!"

        log "Copying client binaries to targets..."

        do! clients
            |> Array.distinctBy(fun (host, _, _) -> host)
            |> Array.collect(fun (_, node, _) ->
                [|
                    copyFile node clientBinary clientPath (Some(jsOptions<TransferOptions>(fun x ->
                        x.mode <- Some(U2.Case2 "500")
                    ))) true
                |]
            )
            |> Promise.all
            |> Promise.map(fun _ -> ())

        log "Finished copying!"

        log "Starting client processes and logging..."
 
        let kills =
            clients
            |> Array.groupBy(fun (host, _, _) -> host)
            |> Array.map(fun (_, xs) ->
                let (_, node, log) = xs[0]
                let _, nodeOut, nodeKill = startNode node $"{clientPath} {Array.length xs} {clientFlags}"
                nodeOut
                |> AsyncRx.toObservable
                |> Observable.subscribe (out log) |> ignore
                nodeKill
            )

        log "Clients and logging initialized!"

        let timer =
            match runtime with
            | Some x -> ``timers/promises``.setTimeout (x * 1000.0) |> Promise.map(fun _ -> ())
            | None -> Promise.create(fun _ _ -> ())

        let sigObs =
            Promise.create (fun succ err ->
                ``process``.on("SIGINT", (fun _ ->
                    log "Caught interrupt"
                    succ ()
                )) |> ignore
            )

        log "Waiting for exit or timer to elapse..."

        do! race([|timer; sigObs|])

        log "Shutting down..."

        log "Shutting down server..."
        serverKill()
        log "Server shut down!"

        log "Shutting down clients..."
        kills |> Array.iter(fun x -> x())
        log "Clients shut down!"

        log "Exiting..."

        ``process``.exit()

        return ()
    }

let args = commander.program
            .name("AutoWebRTC")
            .description("Automatically spawns WebRTC receivers and a WebRTC signalling server with SSH")
            .version("0.0.1")
            // .option("--runningdir <path>", "Directory to copy binaries to on target nodes (defaults to /tmp)", U3.Case1 "/tmp")
            // .option("--scpflags <flags>", "Additional flags to pass to scp (defaults to %s{DEFAULT_SCPFLAGS})")
            .option("--serverPath <path>", "Path to the peerconnection_server binary. Defaults to current directory and then PATH")
            .option("--clientPath <path>", "Path to the peerconnection_client binary. Defaults to current directory and then PATH")
            // .option("--logdir <path>", "Directory to write logs to. Defaults to ./out")
            // .option("--clientflags <flags>", $"Additional flags to pass to the client (defaults to %s{DEFAULT_CLIENTFLAGS})")
            // .option("--serverflags <flags>", "Flags to pass to the server")
            // .option("--server <user@host>", "user@host of the server node. The localhost@localhost machine is default") 
            // .option("--verbose", "If true, will print out information about what the application is doing")
            .requiredOption("--privateKey <path>", "Path to the private key to use for ssh (defaults to ~/.ssh/id_rsa)")
            .requiredOption("--server <user@host>", "")
            .requiredOption("--firstnode <user@host>", "Pass disabled to not manage this node")
            .requiredOption("--secondnode <user@host>", "Pass disabled to not manage this node")
            .parse()

let options: CLIArguments = !!(args.opts())

handleInput(options) |> Promise.start 
