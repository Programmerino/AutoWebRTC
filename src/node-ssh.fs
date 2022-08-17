// ts2fable 0.8.0-build.664
module rec ``node-ssh``
open System
open Fable.Core
open Fable.Core.JS
open Node.Buffer

type Error = System.Exception

type ConnectConfig = Ssh2.ConnectConfig
type ClientChannel = Ssh2.ClientChannel
type SFTPWrapper = Ssh2.SFTPWrapper
type ExecOptions = Ssh2.ExecOptions
type PseudoTtyOptions = Ssh2.PseudoTtyOptions
type ShellOptions = Ssh2.ShellOptions
type Prompt = Ssh2.Prompt
type TransferOptions = Ssh2.TransferOptions

type [<AllowNullLiteral>] IExports =
    abstract SSHError: SSHErrorStatic
    abstract NodeSSH: NodeSSHStatic

type [<AllowNullLiteral>] Config =
    interface end

type [<AllowNullLiteral>] SSHExecCommandOptions =
    abstract cwd: string option with get, set
    abstract stdin: U2<string, Node.Stream.Readable<string>> option with get, set
    abstract execOptions: ExecOptions option with get, set
    abstract encoding: Node.Buffer.BufferEncoding option with get, set
    abstract onChannel: (ClientChannel -> unit) option with get, set
    abstract onStdout: (Buffer -> unit) option with get, set
    abstract onStderr: (Buffer -> unit) option with get, set

type [<AllowNullLiteral>] SSHExecCommandResponse =
    abstract stdout: string with get, set
    abstract stderr: string with get, set
    abstract code: float option with get, set
    abstract signal: string option with get, set

type [<AllowNullLiteral>] SSHExecOptions =
    inherit SSHExecCommandOptions
    abstract stream: SSHExecOptionsStream option with get, set

type [<AllowNullLiteral>] SSHPutFilesOptions =
    abstract sftp: SFTPWrapper option with get, set
    abstract concurrency: float option with get, set
    abstract transferOptions: TransferOptions option with get, set

type [<AllowNullLiteral>] SSHGetPutDirectoryOptions =
    inherit SSHPutFilesOptions
    abstract tick: (string -> string -> Error option -> unit) option with get, set
    abstract validate: (string -> bool) option with get, set
    abstract recursive: bool option with get, set

type [<StringEnum>] [<RequireQualifiedAccess>] SSHMkdirMethod =
    | Sftp
    | Exec

type [<AllowNullLiteral>] SSHError =
    abstract code: string option with get, set

type [<AllowNullLiteral>] SSHErrorStatic =
    [<EmitConstructor>] abstract Create: message: string * ?code: string -> SSHError

type [<AllowNullLiteral>] NodeSSH =
    abstract connection: Ssh2.Client option with get, set
    abstract connect: givenConfig: Config -> Promise<NodeSSH>
    abstract isConnected: unit -> bool
    abstract requestShell: ?options: U2<PseudoTtyOptions, ShellOptions> -> Promise<ClientChannel>
    abstract withShell: callback: (ClientChannel -> Promise<unit>) * ?options: U2<PseudoTtyOptions, ShellOptions> -> Promise<unit>
    abstract requestSFTP: unit -> Promise<SFTPWrapper>
    abstract withSFTP: callback: (SFTPWrapper -> Promise<unit>) -> Promise<unit>
    abstract execCommand: givenCommand: string * ?options: SSHExecCommandOptions -> Promise<SSHExecCommandResponse>
    abstract exec: command: string * parameters: ResizeArray<string> * ?options: obj -> Promise<string>
    abstract mkdir: path: string * ?method: SSHMkdirMethod * ?givenSftp: SFTPWrapper -> Promise<unit>
    abstract getFile: localFile: string * remoteFile: string * ?givenSftp: SFTPWrapper * ?transferOptions: TransferOptions -> Promise<unit>
    abstract putFile: localFile: string * remoteFile: string * ?givenSftp: SFTPWrapper * ?transferOptions: TransferOptions -> Promise<unit>
    abstract putFiles: files: ResizeArray<{| local: string; remote: string |}> * ?p1: SSHPutFilesOptions -> Promise<unit>
    abstract putDirectory: localDirectory: string * remoteDirectory: string * ?p2: SSHGetPutDirectoryOptions -> Promise<bool>
    abstract getDirectory: localDirectory: string * remoteDirectory: string * ?p2: SSHGetPutDirectoryOptions -> Promise<bool>
    abstract dispose: unit -> unit

type [<AllowNullLiteral>] NodeSSHStatic =
    [<EmitConstructor>] abstract Create: unit -> NodeSSH

type [<StringEnum>] [<RequireQualifiedAccess>] SSHExecOptionsStream =
    | Stdout
    | Stderr
    | Both
