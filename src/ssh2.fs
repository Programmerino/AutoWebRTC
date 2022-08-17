// ts2fable 0.8.0-build.664
module rec Ssh2

#nowarn "3390" // disable warnings for invalid XML comments
open Fable.Core
open Node.Stream
open System
open Node.Events
open Node.Buffer
open Node.Fs

type [<AllowNullLiteral>] VerifyCallback =
    [<Emit("$0($1...)")>] abstract Invoke: valid: bool -> unit

type [<AllowNullLiteral>] HostVerifier =
    [<Emit("$0($1...)")>] abstract Invoke: key: Buffer * verify: VerifyCallback -> unit

type [<AllowNullLiteral>] SyncHostVerifier =
    [<Emit("$0($1...)")>] abstract Invoke: key: Buffer -> bool

type [<AllowNullLiteral>] HostFingerprintVerifier =
    [<Emit("$0($1...)")>] abstract Invoke: fingerprint: string * verify: VerifyCallback -> bool

type [<AllowNullLiteral>] SyncHostFingerprintVerifier =
    [<Emit("$0($1...)")>] abstract Invoke: fingerprint: string -> bool

type Error = System.Exception

type [<StringEnum>] [<RequireQualifiedAccess>] KeyType =
    | [<CompiledName("ssh-rsa")>] SshRsa
    | [<CompiledName("ssh-dss")>] SshDss
    | [<CompiledName("ssh-ed25519")>] SshEd25519
    | [<CompiledName("ecdsa-sha2-nistp256")>] EcdsaSha2Nistp256
    | [<CompiledName("ecdsa-sha2-nistp384")>] EcdsaSha2Nistp384
    | [<CompiledName("ecdsa-sha2-nistp521")>] EcdsaSha2Nistp521

type [<AllowNullLiteral>] ParsedKey =
    abstract ``type``: KeyType with get, set
    abstract comment: string with get, set
    abstract sign: data: U2<Buffer, string> * ?algo: string -> Buffer
    abstract verify: data: U2<Buffer, string> * signature: Buffer * ?algo: string -> bool
    abstract isPrivateKey: unit -> bool
    abstract getPrivatePEM: unit -> string
    abstract getPublicPEM: unit -> string
    abstract getPublicSSH: unit -> Buffer
    abstract equals: key: U3<Buffer, string, ParsedKey> -> bool

type [<AllowNullLiteral>] PublicKeyEntry =
    abstract pubKey: U2<ParsedKey, {| pubKey: U3<ParsedKey, Buffer, string>; comment: string option |}> with get, set

type KnownPublicKeys<'T> =
    ResizeArray<U2<'T, PublicKeyEntry>>

type [<AllowNullLiteral>] IdentityCallback<'T> =
    [<Emit("$0($1...)")>] abstract Invoke: ?err: Error * ?keys: KnownPublicKeys<'T> -> unit

type [<StringEnum>] [<RequireQualifiedAccess>] SigningRequestOptionsHash =
    | Sha256
    | Sha512

type [<AllowNullLiteral>] SigningRequestOptions =
    abstract hash: SigningRequestOptionsHash with get, set

type [<AllowNullLiteral>] SignCallback =
    [<Emit("$0($1...)")>] abstract Invoke: ?err: Error * ?signature: Buffer -> unit

type [<AllowNullLiteral>] GetStreamCallback =
    [<Emit("$0($1...)")>] abstract Invoke: ?err: Error * ?stream: Duplex<obj, obj> -> unit

type [<AllowNullLiteral>] BaseAgent<'TPublicKey> =
    /// <summary>
    /// Retrieves user identities, where <c>keys</c> is a possible array of public
    /// keys for authentication.
    /// </summary>
    abstract getIdentities: cb: IdentityCallback<'TPublicKey> -> unit
    /// Signs the datawith the given public key, and calls back with its signature.
    /// Note that, in the current implementation, "options" is always an empty object.
    abstract sign: pubKey: 'TPublicKey * data: Buffer * options: SigningRequestOptions * ?cb: SignCallback -> unit
    abstract sign: pubKey: 'TPublicKey * data: Buffer * cb: SignCallback -> unit
    /// <summary>
    /// Optional method that may be implemented to support agent forwarding. Callback
    /// should be invoked with a Duplex stream to be used to communicate with your agent/
    /// You will probably want to utilize <c>AgentProtocol</c> as agent forwarding is an
    /// OpenSSH feature, so the <c>stream</c> needs to be able to
    /// transmit/receive OpenSSH agent protocol packets.
    /// </summary>
    abstract getStream: cb: GetStreamCallback -> unit

type BaseAgent =
    BaseAgent<U3<string, Buffer, ParsedKey>>

type [<AllowNullLiteral>] Record<'K, 'T> =
    interface end

type [<StringEnum>] [<RequireQualifiedAccess>] AlgorithmListRecord =
    | Append
    | Prepend
    | Remove

type AlgorithmList<'T> =
    U2<ResizeArray<'T>, Record<AlgorithmListRecord, U2<'T, ResizeArray<'T>>>>

type [<StringEnum>] [<RequireQualifiedAccess>] KexAlgorithm =
    | [<CompiledName("curve25519-sha256")>] Curve25519Sha256
    | [<CompiledName("curve25519-sha256@libssh.org")>] ``Curve25519Sha256Atlibssh_org``
    | [<CompiledName("ecdh-sha2-nistp256")>] EcdhSha2Nistp256
    | [<CompiledName("ecdh-sha2-nistp384")>] EcdhSha2Nistp384
    | [<CompiledName("ecdh-sha2-nistp521")>] EcdhSha2Nistp521
    | [<CompiledName("diffie-hellman-group-exchange-sha256")>] DiffieHellmanGroupExchangeSha256
    | [<CompiledName("diffie-hellman-group14-sha256")>] DiffieHellmanGroup14Sha256
    | [<CompiledName("diffie-hellman-group15-sha512")>] DiffieHellmanGroup15Sha512
    | [<CompiledName("diffie-hellman-group16-sha512")>] DiffieHellmanGroup16Sha512
    | [<CompiledName("diffie-hellman-group17-sha512")>] DiffieHellmanGroup17Sha512
    | [<CompiledName("diffie-hellman-group18-sha512")>] DiffieHellmanGroup18Sha512
    | [<CompiledName("diffie-hellman-group-exchange-sha1")>] DiffieHellmanGroupExchangeSha1
    | [<CompiledName("diffie-hellman-group14-sha1")>] DiffieHellmanGroup14Sha1
    | [<CompiledName("diffie-hellman-group1-sha1")>] DiffieHellmanGroup1Sha1

type [<StringEnum>] [<RequireQualifiedAccess>] CipherAlgorithm =
    | [<CompiledName("chacha20-poly1305@openssh.com")>] ``Chacha20Poly1305Atopenssh_com``
    | [<CompiledName("aes128-gcm@openssh.com")>] ``Aes128GcmAtopenssh_com``
    | [<CompiledName("aes256-gcm@openssh.com")>] ``Aes256GcmAtopenssh_com``
    | [<CompiledName("aes128-ctr")>] Aes128Ctr
    | [<CompiledName("aes192-ctr")>] Aes192Ctr
    | [<CompiledName("aes256-ctr")>] Aes256Ctr
    | [<CompiledName("aes256-cbc")>] Aes256Cbc
    | [<CompiledName("aes192-cbc")>] Aes192Cbc
    | [<CompiledName("aes128-cbc")>] Aes128Cbc
    | [<CompiledName("blowfish-cbc")>] BlowfishCbc
    | [<CompiledName("3des-cbc")>] N3desCbc
    | Arcfour256
    | Arcfour128
    | [<CompiledName("cast128-cbc")>] Cast128Cbc
    | Arcfour

type [<StringEnum>] [<RequireQualifiedAccess>] ServerHostKeyAlgorithm =
    | [<CompiledName("ssh-ed25519")>] SshEd25519
    | [<CompiledName("ecdsa-sha2-nistp256")>] EcdsaSha2Nistp256
    | [<CompiledName("ecdsa-sha2-nistp384")>] EcdsaSha2Nistp384
    | [<CompiledName("ecdsa-sha2-nistp521")>] EcdsaSha2Nistp521
    | [<CompiledName("rsa-sha2-512")>] RsaSha2512
    | [<CompiledName("rsa-sha2-256")>] RsaSha2256
    | [<CompiledName("ssh-rsa")>] SshRsa
    | [<CompiledName("ssh-dss")>] SshDss

type [<StringEnum>] [<RequireQualifiedAccess>] MacAlgorithm =
    | [<CompiledName("hmac-sha2-256-etm@openssh.com")>] ``HmacSha2256EtmAtopenssh_com``
    | [<CompiledName("hmac-sha2-512-etm@openssh.com")>] ``HmacSha2512EtmAtopenssh_com``
    | [<CompiledName("hmac-sha1-etm@openssh.com")>] ``HmacSha1EtmAtopenssh_com``
    | [<CompiledName("hmac-sha2-256")>] HmacSha2256
    | [<CompiledName("hmac-sha2-512")>] HmacSha2512
    | [<CompiledName("hmac-sha1")>] HmacSha1
    | [<CompiledName("hmac-md5")>] HmacMd5
    | [<CompiledName("hmac-sha2-256-96")>] HmacSha225696
    | [<CompiledName("hmac-sha2-512-96")>] HmacSha251296
    | [<CompiledName("hmac-ripemd160")>] HmacRipemd160
    | [<CompiledName("hmac-sha1-96")>] HmacSha196
    | [<CompiledName("hmac-md5-96")>] HmacMd596

type [<StringEnum>] [<RequireQualifiedAccess>] CompressionAlgorithm =
    | None
    | Zlib
    | [<CompiledName("zlib@openssh.com")>] ``ZlibAtopenssh_com``

type [<AllowNullLiteral>] Algorithms =
    abstract kex: AlgorithmList<KexAlgorithm> option with get, set
    abstract cipher: AlgorithmList<CipherAlgorithm> option with get, set
    abstract serverHostKey: AlgorithmList<ServerHostKeyAlgorithm> option with get, set
    abstract hmac: AlgorithmList<MacAlgorithm> option with get, set
    abstract compress: AlgorithmList<CompressionAlgorithm> option with get, set

type [<AllowNullLiteral>] DebugFunction =
    [<Emit("$0($1...)")>] abstract Invoke: message: string -> unit

type [<StringEnum>] [<RequireQualifiedAccess>] AuthenticationType =
    | Password
    | Publickey
    | Hostbased
    | Agent
    | [<CompiledName("keyboard-interactive")>] KeyboardInteractive
    | None

type [<AllowNullLiteral>] AuthMethod =
    abstract ``type``: AuthenticationType with get, set
    abstract username: string with get, set

/// <summary>Strategy returned from the <see cref="ConnectConfig.authHandler" /> to connect with an agent.</summary>
type [<AllowNullLiteral>] AgentAuthMethod =
    inherit AuthMethod
    abstract ``type``: string with get, set
    /// <summary>
    /// Can be a string that is interpreted exactly like the <c>agent</c> connection config
    /// option or can be a custom agent object/instance that extends and implements <c>BaseAgent</c>
    /// </summary>
    abstract agent: U2<BaseAgent, string> with get, set

/// <summary>Strategy returned from the <see cref="ConnectConfig.authHandler" /> to connect with host-based authentication.</summary>
type [<AllowNullLiteral>] HostBasedAuthMethod =
    inherit AuthMethod
    abstract ``type``: string with get, set
    abstract localHostname: string with get, set
    abstract localUsername: string with get, set
    /// Can be a string, Buffer, or parsed key containing a private key
    abstract key: U3<ParsedKey, Buffer, string> with get, set
    /// <summary><c>passphrase</c> only required for encrypted keys</summary>
    abstract passphrase: U2<Buffer, string> option with get, set

type [<AllowNullLiteral>] Prompt =
    abstract prompt: string with get, set
    abstract echo: bool option with get, set

type [<AllowNullLiteral>] KeyboardInteractiveCallback =
    [<Emit("$0($1...)")>] abstract Invoke: answers: ResizeArray<string> -> unit

/// <summary>Strategy returned from the <see cref="ConnectConfig.authHandler" /> to connect with an agent.</summary>
type [<AllowNullLiteral>] KeyboardInteractiveAuthMethod =
    inherit AuthMethod
    abstract ``type``: string with get, set
    /// This works exactly the same way as a 'keyboard-interactive' client event handler
    abstract prompt: name: string * instructions: string * lang: string * prompts: ResizeArray<Prompt> * finish: KeyboardInteractiveCallback -> unit

/// <summary>Strategy returned from the <see cref="ConnectConfig.authHandler" /> to connect without authentication.</summary>
type [<AllowNullLiteral>] NoAuthMethod =
    inherit AuthMethod
    abstract ``type``: string with get, set

/// <summary>Strategy returned from the <see cref="ConnectConfig.authHandler" /> to connect with a password.</summary>
type [<AllowNullLiteral>] PasswordAuthMethod =
    inherit AuthMethod
    abstract ``type``: string with get, set
    abstract password: string with get, set

/// <summary>Strategy returned from the <see cref="ConnectConfig.authHandler" /> to connect with a public key.</summary>
type [<AllowNullLiteral>] PublicKeyAuthMethod =
    inherit AuthMethod
    abstract ``type``: string with get, set
    abstract key: U3<ParsedKey, Buffer, string> with get, set
    abstract passphrase: U2<Buffer, string> option with get, set

type [<TypeScriptTaggedUnion("type")>] [<RequireQualifiedAccess>] AnyAuthMethod =
    | [<CompiledName("agent")>] AgentAuthMethod of AgentAuthMethod
    | [<CompiledName("hostbased")>] HostBasedAuthMethod of HostBasedAuthMethod
    | [<CompiledName("keyboard-interactive")>] KeyboardInteractiveAuthMethod of KeyboardInteractiveAuthMethod
    | [<CompiledName("none")>] NoAuthMethod of NoAuthMethod
    | [<CompiledName("password")>] PasswordAuthMethod of PasswordAuthMethod
    | [<CompiledName("publickey")>] PublicKeyAuthMethod of PublicKeyAuthMethod
    static member inline op_ErasedCast(x: AgentAuthMethod) = AgentAuthMethod x
    static member inline op_ErasedCast(x: HostBasedAuthMethod) = HostBasedAuthMethod x
    static member inline op_ErasedCast(x: KeyboardInteractiveAuthMethod) = KeyboardInteractiveAuthMethod x
    static member inline op_ErasedCast(x: NoAuthMethod) = NoAuthMethod x
    static member inline op_ErasedCast(x: PasswordAuthMethod) = PasswordAuthMethod x
    static member inline op_ErasedCast(x: PublicKeyAuthMethod) = PublicKeyAuthMethod x

type [<AllowNullLiteral>] NextAuthHandler =
    [<Emit("$0($1...)")>] abstract Invoke: authName: U2<AuthenticationType, AnyAuthMethod> -> unit

type [<AllowNullLiteral>] AuthHandlerMiddleware =
    [<Emit("$0($1...)")>] abstract Invoke: authsLeft: ResizeArray<AuthenticationType> * partialSuccess: bool * next: NextAuthHandler -> unit

type [<AllowNullLiteral>] ConnectConfig =
    /// Hostname or IP address of the server.
    abstract host: string option with get, set
    /// Port number of the server.
    abstract port: float option with get, set
    /// <summary>Only connect via resolved IPv4 address for <c>host</c>.</summary>
    abstract forceIPv4: bool option with get, set
    /// <summary>Only connect via resolved IPv6 address for <c>host</c>.</summary>
    abstract forceIPv6: bool option with get, set
    /// <summary>The host's key is hashed using this method and passed to <c>hostVerifier</c>.</summary>
    abstract hostHash: string option with get, set
    /// Verifies a hexadecimal hash of the host's key.
    abstract hostVerifier: U4<HostVerifier, SyncHostVerifier, HostFingerprintVerifier, SyncHostFingerprintVerifier> option with get, set
    /// Username for authentication.
    abstract username: string option with get, set
    /// Password for password-based user authentication.
    abstract password: string option with get, set
    /// Path to ssh-agent's UNIX socket for ssh-agent-based user authentication (or 'pageant' when using Pagent on Windows).
    abstract agent: U2<BaseAgent, string> option with get, set
    /// Buffer or string that contains a private key for either key-based or hostbased user authentication (OpenSSH format).
    abstract privateKey: U2<Buffer, string> option with get, set
    /// For an encrypted private key, this is the passphrase used to decrypt it.
    abstract passphrase: U2<Buffer, string> option with get, set
    /// <summary>Along with <c>localUsername</c> and <c>privateKey</c>, set this to a non-empty string for hostbased user authentication.</summary>
    abstract localHostname: string option with get, set
    /// <summary>Along with <c>localHostname</c> and <c>privateKey</c>, set this to a non-empty string for hostbased user authentication.</summary>
    abstract localUsername: string option with get, set
    /// Try keyboard-interactive user authentication if primary user authentication method fails.
    abstract tryKeyboard: bool option with get, set
    /// How often (in milliseconds) to send SSH-level keepalive packets to the server. Set to 0 to disable.
    abstract keepaliveInterval: float option with get, set
    /// How many consecutive, unanswered SSH-level keepalive packets that can be sent to the server before disconnection.
    abstract keepaliveCountMax: float option with get, set
    /// * How long (in milliseconds) to wait for the SSH handshake to complete.
    abstract readyTimeout: float option with get, set
    /// Performs a strict server vendor check before sending vendor-specific requests.
    abstract strictVendor: bool option with get, set
    /// <summary>A <c>ReadableStream</c> to use for communicating with the server instead of creating and using a new TCP connection (useful for connection hopping).</summary>
    abstract sock: Readable<obj> option with get, set
    /// <summary>Set to <c>true</c> to use OpenSSH agent forwarding (<c>auth-agent@openssh.com</c>) for the life of the connection.</summary>
    abstract agentForward: bool option with get, set
    /// Explicit overrides for the default transport layer algorithms used for the connection.
    abstract algorithms: Algorithms option with get, set
    /// A function that receives a single string argument to get detailed (local) debug information.
    abstract debug: DebugFunction option with get, set
    /// Function with parameters (methodsLeft, partialSuccess, callback) where methodsLeft and partialSuccess are null on the first authentication attempt, otherwise are an array and boolean respectively. Return or call callback() with the name of the authentication method to try next (pass false to signal no more methods to try). Valid method names are: 'none', 'password', 'publickey', 'agent', 'keyboard-interactive', 'hostbased'. Default: function that follows a set method order: None -> Password -> Private Key -> Agent (-> keyboard-interactive if tryKeyboard is true) -> Hostbased.
    abstract authHandler: U3<ResizeArray<AuthenticationType>, AuthHandlerMiddleware, ResizeArray<AuthMethod>> option with get, set
    /// IP address of the network interface to use to connect to the server. Default: (none -- determined by OS)
    abstract localAddress: string option with get, set
    /// The local port number to connect from. Default: (none -- determined by OS)
    abstract localPort: float option with get, set
    /// The underlying socket timeout in ms. Default: none)
    abstract timeout: float option with get, set
    /// A custom server software name/version identifier. Default: 'ssh2js' + moduleVersion + 'srv'
    abstract ident: U2<Buffer, string> option with get, set

type [<StringEnum>] [<RequireQualifiedAccess>] ChannelType =
    | Session
    | Sftp
    | [<CompiledName("direct-tcpip")>] DirectTcpip
    | [<CompiledName("direct-streamlocal@openssh.com")>] ``DirectStreamlocalAtopenssh_com``

type [<StringEnum>] [<RequireQualifiedAccess>] ChannelSubType =
    | Exec
    | Shell

type Symbol = obj
type Function = System.Action

type [<AllowNullLiteral>] Channel =
    inherit Duplex<obj, obj>
    /// Standard input for the Channel.
    abstract stdin: Channel with get, set
    /// Standard output for the Channel.
    abstract stdout: Channel with get, set
    /// Standard error for the Channel.
    abstract stderr: U2<Writable<obj>, Readable<obj>> with get, set
    /// Indicates whether this is a server or client channel.
    abstract server: bool with get, set
    /// The channel type, usually "session".
    abstract ``type``: ChannelType with get, set
    /// The channel subtype, usually "exec", "shell", or undefined.
    abstract subtype: ChannelSubType option with get, set
    abstract incoming: obj with get, set
    abstract outgoing: obj with get, set
    /// Sends EOF to the remote side.
    abstract eof: unit -> unit
    /// Closes the channel on both sides.
    abstract close: [<ParamArray>] args: obj option[] -> unit
    /// Shuts down the channel on this side.
    abstract destroy: unit -> Channel
    /// Session type-specific methods
    abstract setWindow: rows: string * cols: string * height: string * width: string -> unit
    abstract signal: signalName: string -> unit
    abstract exit: status: float -> unit
    abstract exit: signalName: string * ?coreDumped: bool * ?msg: string -> unit
    /// Emitted once the channel is completely closed on both the client and the server.
    [<Emit("$0.on('close',$1)")>] abstract on_close: listener: (unit -> unit) -> Channel
    [<Emit("$0.on('eof',$1)")>] abstract on_eof: listener: (unit -> unit) -> Channel
    [<Emit("$0.on('end',$1)")>] abstract on_end: listener: (unit -> unit) -> Channel
    abstract on: ``event``: U2<string, Symbol> * listener: Function -> Channel

type [<AllowNullLiteral>] ClientChannel =
    inherit Channel
    /// Standard error for the Channel.
    abstract stderr: Readable<obj> with get, set
    /// Indicates whether this is a server or client channel.
    abstract server: bool with get, set
    /// <summary>
    /// An <c>exit</c> event *may* (the SSH2 spec says it is optional) be emitted when the process
    /// finishes. If the process finished normally, the process's return value is passed to
    /// the <c>exit</c> callback.
    /// </summary>
    [<Emit("$0.on('exit',$1)")>] abstract on_exit: listener: (string -> unit) -> ClientChannel
    /// Emitted once the channel is completely closed on both the client and the server.
    [<Emit("$0.on('exit',$1)")>] abstract on_exit: listener: (obj -> string -> string -> string -> unit) -> ClientChannel
    /// Emitted once the channel is completely closed on both the client and the server.
    abstract on: ``event``: U2<string, Symbol> * listener: Function -> ClientChannel

type [<AllowNullLiteral>] TransferOptions =
    abstract concurrency: float option with get, set
    abstract chunkSize: float option with get, set
    abstract fileSize: float option with get, set
    abstract step: (float -> float -> float -> unit) option with get, set
    abstract mode: U2<float, string> option with get, set

type [<AllowNullLiteral>] Callback =
    [<Emit("$0($1...)")>] abstract Invoke: ?err: Error -> unit

type [<AllowNullLiteral>] ReadFileOptions =
    abstract encoding: BufferEncoding option with get, set
    abstract flag: string option with get, set

type [<AllowNullLiteral>] WriteFileOptions =
    abstract encoding: BufferEncoding option with get, set
    abstract mode: float option with get, set
    abstract flag: string option with get, set

type [<StringEnum>] [<RequireQualifiedAccess>] OpenMode =
    | R
    | [<CompiledName("r+")>] ``RPlus``
    | W
    | Wx
    | Xw
    | [<CompiledName("w+")>] ``WPlus``
    | [<CompiledName("xw+")>] ``XwPlus``
    | A
    | Ax
    | Xa
    | [<CompiledName("a+")>] ``APlus``
    | [<CompiledName("ax+")>] ``AxPlus``
    | [<CompiledName("xa+")>] ``XaPlus``

type [<AllowNullLiteral>] InputAttributes =
    abstract mode: U2<float, string> option with get, set
    abstract uid: float option with get, set
    abstract gid: float option with get, set
    abstract size: float option with get, set
    abstract atime: U2<float, DateTime> option with get, set
    abstract mtime: U2<float, DateTime> option with get, set

type [<AllowNullLiteral>] Attributes =
    abstract mode: float with get, set
    abstract uid: float with get, set
    abstract gid: float with get, set
    abstract size: float with get, set
    abstract atime: float with get, set
    abstract mtime: float with get, set

type [<AllowNullLiteral>] FileEntry =
    abstract filename: string with get, set
    abstract longname: string with get, set
    abstract attrs: Attributes with get, set

type [<AllowNullLiteral>] SFTPWrapper =
    inherit EventEmitter
    /// <summary>
    /// (Client-only)
    /// Downloads a file at <c>remotePath</c> to <c>localPath</c> using parallel reads for faster throughput.
    /// </summary>
    abstract fastGet: remotePath: string * localPath: string * options: TransferOptions * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Downloads a file at <c>remotePath</c> to <c>localPath</c> using parallel reads for faster throughput.
    /// </summary>
    abstract fastGet: remotePath: string * localPath: string * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Uploads a file from <c>localPath</c> to <c>remotePath</c> using parallel reads for faster throughput.
    /// </summary>
    abstract fastPut: localPath: string * remotePath: string * options: TransferOptions * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Uploads a file from <c>localPath</c> to <c>remotePath</c> using parallel reads for faster throughput.
    /// </summary>
    abstract fastPut: localPath: string * remotePath: string * callback: Callback -> unit
    /// (Client-only)
    /// Reads a file in memory and returns its contents
    abstract readFile: remotePath: string * options: ReadFileOptions * callback: (Error option -> Buffer -> unit) -> unit
    /// (Client-only)
    /// Reads a file in memory and returns its contents
    abstract readFile: remotePath: string * encoding: BufferEncoding * callback: (Error option -> Buffer -> unit) -> unit
    /// (Client-only)
    /// Reads a file in memory and returns its contents
    abstract readFile: remotePath: string * callback: (Error option -> Buffer -> unit) -> unit
    /// <summary>
    /// (Client-only)
    /// Returns a new readable stream for <c>path</c>.
    /// </summary>
    abstract createReadStream: path: string * ?options: ReadStreamOptions -> ReadStream<obj>
    /// (Client-only)
    /// Writes data to a file
    abstract writeFile: remotePath: string * data: U2<string, Buffer> * options: WriteFileOptions * ?callback: Callback -> unit
    /// (Client-only)
    /// Writes data to a file
    abstract writeFile: remotePath: string * data: U2<string, Buffer> * encoding: string * ?callback: Callback -> unit
    /// (Client-only)
    /// Writes data to a file
    abstract writeFile: remotePath: string * data: U2<string, Buffer> * ?callback: Callback -> unit
    /// (Client-only)
    /// Appends data to a file
    abstract appendFile: remotePath: string * data: U2<string, Buffer> * options: WriteFileOptions * ?callback: Callback -> unit
    /// (Client-only)
    /// Appends data to a file
    abstract appendFile: remotePath: string * data: U2<string, Buffer> * ?callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Returns a new writable stream for <c>path</c>.
    /// </summary>
    abstract createWriteStream: path: string * ?options: WriteStreamOptions -> WriteStream<obj>
    /// <summary>
    /// (Client-only)
    /// Opens a file <c>filename</c> for <c>mode</c> with optional <c>attributes</c>.
    /// </summary>
    abstract ``open``: filename: string * mode: U2<float, OpenMode> * attributes: InputAttributes * callback: (Error option -> Buffer -> unit) -> unit
    abstract ``open``: filename: string * mode: U2<float, OpenMode> * attributes: U2<string, float> * callback: (Error option -> Buffer -> unit) -> unit
    /// <summary>
    /// (Client-only)
    /// Opens a file <c>filename</c> for <c>mode</c>.
    /// </summary>
    abstract ``open``: filename: string * mode: U2<float, OpenMode> * callback: (Error option -> Buffer -> unit) -> unit
    /// <summary>
    /// (Client-only)
    /// Closes the resource associated with <c>handle</c> given by <c>open()</c> or <c>opendir()</c>.
    /// </summary>
    abstract close: handle: Buffer * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Reads <c>length</c> bytes from the resource associated with <c>handle</c> starting at <c>position</c>
    /// and stores the bytes in <c>buffer</c> starting at <c>offset</c>.
    /// </summary>
    abstract read: handle: Buffer * buffer: Buffer * offset: float * length: float * position: float * callback: (Error option -> float -> Buffer -> float -> unit) -> unit
    /// (Client-only)
    abstract write: handle: Buffer * buffer: Buffer * offset: float * length: float * position: float * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Retrieves attributes for the resource associated with <c>handle</c>.
    /// </summary>
    abstract fstat: handle: Buffer * callback: (Error option -> Stats -> unit) -> unit
    /// <summary>
    /// (Client-only)
    /// Sets the attributes defined in <c>attributes</c> for the resource associated with <c>handle</c>.
    /// </summary>
    abstract fsetstat: handle: Buffer * attributes: InputAttributes * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Sets the access time and modified time for the resource associated with <c>handle</c>.
    /// </summary>
    abstract futimes: handle: Buffer * atime: U2<float, DateTime> * mtime: U2<float, DateTime> * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Sets the owner for the resource associated with <c>handle</c>.
    /// </summary>
    abstract fchown: handle: Buffer * uid: float * gid: float * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Sets the mode for the resource associated with <c>handle</c>.
    /// </summary>
    abstract fchmod: handle: Buffer * mode: U2<float, string> * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Opens a directory <c>path</c>.
    /// </summary>
    abstract opendir: path: string * callback: (Error option -> Buffer -> unit) -> unit
    /// (Client-only)
    /// Retrieves a directory listing.
    abstract readdir: location: U2<string, Buffer> * callback: (Error option -> ResizeArray<FileEntry> -> unit) -> unit
    /// <summary>
    /// (Client-only)
    /// Removes the file/symlink at <c>path</c>.
    /// </summary>
    abstract unlink: path: string * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Renames/moves <c>srcPath</c> to <c>destPath</c>.
    /// </summary>
    abstract rename: srcPath: string * destPath: string * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Creates a new directory <c>path</c>.
    /// </summary>
    abstract mkdir: path: string * attributes: InputAttributes * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Creates a new directory <c>path</c>.
    /// </summary>
    abstract mkdir: path: string * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Removes the directory at <c>path</c>.
    /// </summary>
    abstract rmdir: path: string * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Retrieves attributes for <c>path</c>.
    /// </summary>
    abstract stat: path: string * callback: (Error option -> Stats -> unit) -> unit
    /// <summary>
    /// (Client-only)
    /// <c>path</c> exists.
    /// </summary>
    abstract exists: path: string * callback: (bool -> unit) -> unit
    /// <summary>
    /// (Client-only)
    /// Retrieves attributes for <c>path</c>. If <c>path</c> is a symlink, the link itself is stat'ed
    /// instead of the resource it refers to.
    /// </summary>
    abstract lstat: path: string * callback: (Error option -> Stats -> unit) -> unit
    /// <summary>
    /// (Client-only)
    /// Sets the attributes defined in <c>attributes</c> for <c>path</c>.
    /// </summary>
    abstract setstat: path: string * attributes: InputAttributes * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Sets the access time and modified time for <c>path</c>.
    /// </summary>
    abstract utimes: path: string * atime: U2<float, DateTime> * mtime: U2<float, DateTime> * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Sets the owner for <c>path</c>.
    /// </summary>
    abstract chown: path: string * uid: float * gid: float * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Sets the mode for <c>path</c>.
    /// </summary>
    abstract chmod: path: string * mode: U2<float, string> * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Retrieves the target for a symlink at <c>path</c>.
    /// </summary>
    abstract readlink: path: string * callback: (Error option -> string -> unit) -> unit
    /// <summary>
    /// (Client-only)
    /// Creates a symlink at <c>linkPath</c> to <c>targetPath</c>.
    /// </summary>
    abstract symlink: targetPath: string * linkPath: string * callback: Callback -> unit
    /// <summary>
    /// (Client-only)
    /// Resolves <c>path</c> to an absolute path.
    /// </summary>
    abstract realpath: path: string * callback: (Error option -> string -> unit) -> unit
    /// <summary>
    /// (Client-only, OpenSSH extension)
    /// Performs POSIX rename(3) from <c>srcPath</c> to <c>destPath</c>.
    /// </summary>
    abstract ext_openssh_rename: srcPath: string * destPath: string * callback: Callback -> unit
    /// <summary>
    /// (Client-only, OpenSSH extension)
    /// Performs POSIX statvfs(2) on <c>path</c>.
    /// </summary>
    abstract ext_openssh_statvfs: path: string * callback: (Error option -> obj option -> unit) -> unit
    /// <summary>
    /// (Client-only, OpenSSH extension)
    /// Performs POSIX fstatvfs(2) on open handle <c>handle</c>.
    /// </summary>
    abstract ext_openssh_fstatvfs: handle: Buffer * callback: (Error option -> obj option -> unit) -> unit
    /// <summary>
    /// (Client-only, OpenSSH extension)
    /// Performs POSIX link(2) to create a hard link to <c>targetPath</c> at <c>linkPath</c>.
    /// </summary>
    abstract ext_openssh_hardlink: targetPath: string * linkPath: string * callback: Callback -> unit
    /// <summary>
    /// (Client-only, OpenSSH extension)
    /// Performs POSIX fsync(3) on the open handle <c>handle</c>.
    /// </summary>
    abstract ext_openssh_fsync: handle: Buffer * callback: (Error option -> obj option -> unit) -> unit
    /// (Client-only, OpenSSH extension)
    /// Similar to setstat(), but instead sets attributes on symlinks.
    abstract ext_openssh_lsetstat: path: string * attrs: InputAttributes * callback: Callback -> unit
    abstract ext_openssh_lsetstat: path: string * callback: Callback -> unit
    /// (Client-only, OpenSSH extension)
    /// Similar to realpath(), but supports tilde-expansion, i.e. "~", "~/..." and "~user/...". These paths are expanded using shell-like rules.
    abstract ext_openssh_expandPath: path: string * callback: (Error option -> string -> unit) -> unit
    /// (Client-only)
    /// Performs a remote file copy. If length is 0, then the server will read from srcHandle until EOF is reached.
    abstract ext_copy_data: handle: Buffer * srcOffset: float * len: float * dstHandle: Buffer * dstOffset: float * callback: Callback -> unit
    /// Emitted after initial protocol version check has passed
    [<Emit("$0.on('ready',$1)")>] abstract on_ready: listener: (unit -> unit) -> SFTPWrapper
    [<Emit("$0.on('OPEN',$1)")>] abstract on_OPEN: listener: (float -> string -> float -> Attributes -> unit) -> SFTPWrapper
    [<Emit("$0.on('READ',$1)")>] abstract on_READ: listener: (float -> Buffer -> float -> float -> unit) -> SFTPWrapper
    [<Emit("$0.on('WRITE',$1)")>] abstract on_WRITE: listener: (float -> Buffer -> float -> Buffer -> unit) -> SFTPWrapper
    [<Emit("$0.on('FSTAT',$1)")>] abstract on_FSTAT: listener: (float -> Buffer -> unit) -> SFTPWrapper
    [<Emit("$0.on('FSETSTAT',$1)")>] abstract on_FSETSTAT: listener: (float -> Buffer -> Attributes -> unit) -> SFTPWrapper
    [<Emit("$0.on('CLOSE',$1)")>] abstract on_CLOSE: listener: (float -> Buffer -> unit) -> SFTPWrapper
    [<Emit("$0.on('OPENDIR',$1)")>] abstract on_OPENDIR: listener: (float -> string -> unit) -> SFTPWrapper
    [<Emit("$0.on('READDIR',$1)")>] abstract on_READDIR: listener: (float -> Buffer -> unit) -> SFTPWrapper
    [<Emit("$0.on('LSTAT',$1)")>] abstract on_LSTAT: listener: (float -> string -> unit) -> SFTPWrapper
    [<Emit("$0.on('STAT',$1)")>] abstract on_STAT: listener: (float -> string -> unit) -> SFTPWrapper
    [<Emit("$0.on('REMOVE',$1)")>] abstract on_REMOVE: listener: (float -> string -> unit) -> SFTPWrapper
    [<Emit("$0.on('RMDIR',$1)")>] abstract on_RMDIR: listener: (float -> string -> unit) -> SFTPWrapper
    [<Emit("$0.on('REALPATH',$1)")>] abstract on_REALPATH: listener: (float -> string -> unit) -> SFTPWrapper
    [<Emit("$0.on('READLINK',$1)")>] abstract on_READLINK: listener: (float -> string -> unit) -> SFTPWrapper
    [<Emit("$0.on('SETSTAT',$1)")>] abstract on_SETSTAT: listener: (float -> string -> Attributes -> unit) -> SFTPWrapper
    [<Emit("$0.on('MKDIR',$1)")>] abstract on_MKDIR: listener: (float -> string -> Attributes -> unit) -> SFTPWrapper
    [<Emit("$0.on('RENAME',$1)")>] abstract on_RENAME: listener: (float -> string -> string -> unit) -> SFTPWrapper
    [<Emit("$0.on('SYMLINK',$1)")>] abstract on_SYMLINK: listener: (float -> string -> string -> unit) -> SFTPWrapper
    [<Emit("$0.on('EXTENDED',$1)")>] abstract on_EXTENDED: listener: (float -> string -> Buffer -> unit) -> SFTPWrapper
    abstract on: ``event``: U2<string, Symbol> * listener: Function -> SFTPWrapper
    /// Sends a status response for the request identified by id.
    abstract status: reqId: float * code: float * ?message: string -> unit
    /// Sends a handle response for the request identified by id.
    /// handle must be less than 256 bytes and is an opaque value that could merely contain the value of a
    /// backing file descriptor or some other unique, custom value.
    abstract handle: reqId: float * handle: Buffer -> unit
    /// Sends a data response for the request identified by id. data can be a Buffer or string.
    /// If data is a string, encoding is the encoding of data.
    abstract data: reqId: float * data: U2<Buffer, string> * ?encoding: BufferEncoding -> unit
    /// Sends a name response for the request identified by id.
    abstract name: reqId: float * names: ResizeArray<FileEntry> -> unit
    /// Sends an attrs response for the request identified by id.
    abstract attrs: reqId: float * attrs: Attributes -> unit

type [<AllowNullLiteral>] Dict<'T> =
    [<EmitIndexer>] abstract Item: key: string -> 'T option with get, set

type [<AllowNullLiteral>] ProcessEnv =
    inherit Dict<string>
    /// Can be used to change the default timezone at runtime
    abstract TZ: string option with get, set

type [<RequireQualifiedAccess>] TerminalModesIGNPAR =
    | N0 = 0
    | N1 = 1

type [<AllowNullLiteral>] TerminalModes =
    /// <summary>Interrupt character; <c>255</c> if none. Not all of these characters are supported on all systems.</summary>
    abstract VINTR: float option with get, set
    /// <summary>The quit character (sends <c>SIGQUIT</c> signal on POSIX systems).</summary>
    abstract VQUIT: float option with get, set
    /// Erase the character to left of the cursor.
    abstract VERASE: float option with get, set
    /// Kill the current input line.
    abstract VKILL: float option with get, set
    /// <summary>End-of-file character (sends <c>EOF</c> from the terminal).</summary>
    abstract VEOF: float option with get, set
    /// End-of-line character in addition to carriage return and/or linefeed.
    abstract VEOL: float option with get, set
    /// Additional end-of-line character.
    abstract VEOL2: float option with get, set
    /// Continues paused output (normally control-Q).
    abstract VSTART: float option with get, set
    /// Pauses output (normally control-S).
    abstract VSTOP: float option with get, set
    /// Suspends the current program.
    abstract VSUSP: float option with get, set
    /// Another suspend character.
    abstract VDSUSP: float option with get, set
    /// Reprints the current input line.
    abstract VREPRINT: float option with get, set
    /// Erases a word left of cursor.
    abstract VWERASE: float option with get, set
    /// Enter the next character typed literally, even if it is a special character
    abstract VLNEXT: float option with get, set
    /// Character to flush output.
    abstract VFLUSH: float option with get, set
    /// Switch to a different shell layer.
    abstract VSWTCH: float option with get, set
    /// Prints system status line (load, command, pid, etc).
    abstract VSTATUS: float option with get, set
    /// Toggles the flushing of terminal output.
    abstract VDISCARD: float option with get, set
    /// <summary>The ignore parity flag.  The parameter SHOULD be <c>0</c> if this flag is FALSE, and <c>1</c> if it is TRUE.</summary>
    abstract IGNPAR: TerminalModesIGNPAR option with get, set
    /// Mark parity and framing errors.
    abstract PARMRK: TerminalModesIGNPAR option with get, set
    /// Enable checking of parity errors.
    abstract INPCK: TerminalModesIGNPAR option with get, set
    /// Strip 8th bit off characters.
    abstract ISTRIP: TerminalModesIGNPAR option with get, set
    /// Map NL into CR on input.
    abstract INLCR: TerminalModesIGNPAR option with get, set
    /// Ignore CR on input.
    abstract IGNCR: TerminalModesIGNPAR option with get, set
    /// Map CR to NL on input.
    abstract ICRNL: TerminalModesIGNPAR option with get, set
    /// Translate uppercase characters to lowercase.
    abstract IUCLC: TerminalModesIGNPAR option with get, set
    /// Enable output flow control.
    abstract IXON: TerminalModesIGNPAR option with get, set
    /// Any char will restart after stop.
    abstract IXANY: TerminalModesIGNPAR option with get, set
    /// Enable input flow control.
    abstract IXOFF: TerminalModesIGNPAR option with get, set
    /// Ring bell on input queue full.
    abstract IMAXBEL: TerminalModesIGNPAR option with get, set
    /// Enable signals INTR, QUIT, [D]SUSP.
    abstract ISIG: TerminalModesIGNPAR option with get, set
    /// Canonicalize input lines.
    abstract ICANON: TerminalModesIGNPAR option with get, set
    /// <summary>Enable input and output of uppercase characters by preceding their lowercase equivalents with <c>\</c>.</summary>
    abstract XCASE: TerminalModesIGNPAR option with get, set
    /// Enable echoing.
    abstract ECHO: TerminalModesIGNPAR option with get, set
    /// Visually erase chars.
    abstract ECHOE: TerminalModesIGNPAR option with get, set
    /// Kill character discards current line.
    abstract ECHOK: TerminalModesIGNPAR option with get, set
    /// Echo NL even if ECHO is off.
    abstract ECHONL: TerminalModesIGNPAR option with get, set
    /// Don't flush after interrupt.
    abstract NOFLSH: TerminalModesIGNPAR option with get, set
    /// Stop background jobs from output.
    abstract TOSTOP: TerminalModesIGNPAR option with get, set
    /// Enable extensions.
    abstract IEXTEN: TerminalModesIGNPAR option with get, set
    /// Echo control characters as ^(Char).
    abstract ECHOCTL: TerminalModesIGNPAR option with get, set
    /// Visual erase for line kill.
    abstract ECHOKE: TerminalModesIGNPAR option with get, set
    /// Retype pending input.
    abstract PENDIN: TerminalModesIGNPAR option with get, set
    /// Enable output processing.
    abstract OPOST: TerminalModesIGNPAR option with get, set
    /// Convert lowercase to uppercase.
    abstract OLCUC: TerminalModesIGNPAR option with get, set
    /// Map NL to CR-NL.
    abstract ONLCR: TerminalModesIGNPAR option with get, set
    /// Translate carriage return to newline (output).
    abstract OCRNL: TerminalModesIGNPAR option with get, set
    /// Translate newline to carriage return-newline (output).
    abstract ONOCR: TerminalModesIGNPAR option with get, set
    /// Newline performs a carriage return (output).
    abstract ONLRET: TerminalModesIGNPAR option with get, set
    /// 7 bit mode.
    abstract CS7: TerminalModesIGNPAR option with get, set
    /// 8 bit mode.
    abstract CS8: TerminalModesIGNPAR option with get, set
    /// Parity enable.
    abstract PARENB: TerminalModesIGNPAR option with get, set
    /// Odd parity, else even.
    abstract PARODD: TerminalModesIGNPAR option with get, set
    /// Specifies the input baud rate in bits per second.
    abstract TTY_OP_ISPEED: float option with get, set
    /// Specifies the output baud rate in bits per second.
    abstract TTY_OP_OSPEED: float option with get, set

type [<AllowNullLiteral>] PseudoTtyOptions =
    /// <summary>The number of rows (default: <c>24</c>).</summary>
    abstract rows: float option with get, set
    /// <summary>The number of columns (default: <c>80</c>).</summary>
    abstract cols: float option with get, set
    /// <summary>The height in pixels (default: <c>480</c>).</summary>
    abstract height: float option with get, set
    /// <summary>The width in pixels (default: <c>640</c>).</summary>
    abstract width: float option with get, set
    /// <summary>The value to use for $TERM (default: <c>'vt100'</c>)</summary>
    abstract term: string option with get, set
    /// An object containing Terminal Modes as keys, with each value set to each mode argument. Default: null
    abstract modes: TerminalModes option with get, set

type [<AllowNullLiteral>] X11Options =
    /// <summary>Whether to allow just a single connection (default: <c>false</c>).</summary>
    abstract single: bool option with get, set
    /// <summary>The Screen number to use (default: <c>0</c>).</summary>
    abstract screen: float option with get, set
    /// The authentication protocol name. Default: 'MIT-MAGIC-COOKIE-1'
    abstract protocol: string option with get, set
    /// The authentication cookie. Can be a hex string or a Buffer containing the raw cookie value (which will be converted to a hex string). Default: (random 16 byte value)
    abstract cookie: U2<Buffer, string> option with get, set

type [<AllowNullLiteral>] ExecOptions =
    /// An environment to use for the execution of the command.
    abstract env: ProcessEnv option with get, set
    /// <summary>Set to <c>true</c> to allocate a pseudo-tty with defaults, or an object containing specific pseudo-tty settings.</summary>
    abstract pty: U2<PseudoTtyOptions, bool> option with get, set
    /// <summary>Set either to <c>true</c> to use defaults, a number to specify a specific screen number, or an object containing x11 settings.</summary>
    abstract x11: U3<X11Options, float, bool> option with get, set
    abstract allowHalfOpen: bool option with get, set

type [<AllowNullLiteral>] ShellOptions =
    /// An environment to use for the execution of the shell.
    abstract env: ProcessEnv option with get, set
    /// <summary>Set either to <c>true</c> to use defaults, a number to specify a specific screen number, or an object containing x11 settings.</summary>
    abstract x11: U3<X11Options, float, bool> option with get, set

type [<AllowNullLiteral>] TcpConnectionDetails =
    /// The originating IP of the connection.
    abstract srcIP: string with get, set
    /// The originating port of the connection.
    abstract srcPort: float with get, set
    /// <summary>The remote IP the connection was received on (given in earlier call to <c>forwardIn()</c>).</summary>
    abstract destIP: string with get, set
    /// <summary>The remote port the connection was received on (given in earlier call to <c>forwardIn()</c>).</summary>
    abstract destPort: float with get, set

type [<AllowNullLiteral>] AcceptConnection<'T when 'T :> Channel> =
    [<Emit("$0($1...)")>] abstract Invoke: unit -> 'T

type [<AllowNullLiteral>] RejectConnection =
    [<Emit("$0($1...)")>] abstract Invoke: unit -> unit

type [<AllowNullLiteral>] X11Details =
    /// The originating IP of the connection.
    abstract srcIP: string with get, set
    /// The originating port of the connection.
    abstract srcPort: float with get, set

type [<AllowNullLiteral>] ChangePasswordCallback =
    [<Emit("$0($1...)")>] abstract Invoke: newPassword: string -> unit

type [<AllowNullLiteral>] NegotiatedAlgorithms =
    abstract kex: KexAlgorithm with get, set
    abstract serverHostKey: ServerHostKeyAlgorithm with get, set
    abstract cs: {| cipher: CipherAlgorithm; mac: U2<MacAlgorithm, string>; compress: CompressionAlgorithm; lang: string |} with get, set
    abstract sc: {| cipher: CipherAlgorithm; mac: U2<MacAlgorithm, string>; compress: CompressionAlgorithm; lang: string |} with get, set

type [<AllowNullLiteral>] UNIXConnectionDetails =
    abstract socketPath: string with get, set

type AcceptConnection =
    AcceptConnection<Channel>

type [<AllowNullLiteral>] ClientCallback =
    [<Emit("$0($1...)")>] abstract Invoke: err: Error option * channel: ClientChannel -> unit

type [<AllowNullLiteral>] ClientForwardCallback =
    [<Emit("$0($1...)")>] abstract Invoke: err: Error option * port: float -> unit

type [<AllowNullLiteral>] ClientSFTPCallback =
    [<Emit("$0($1...)")>] abstract Invoke: err: Error option * sftp: SFTPWrapper -> unit

type [<AllowNullLiteral>] Client =
    inherit EventEmitter
    /// Emitted when a notice was sent by the server upon connection.
    [<Emit("$0.on('banner',$1)")>] abstract on_banner: listener: (string -> unit) -> Client
    /// Emitted when authentication was successful.
    [<Emit("$0.on('ready',$1)")>] abstract on_ready: listener: (unit -> unit) -> Client
    /// <summary>
    /// Emitted when an incoming forwarded TCP connection is being requested.
    /// 
    /// Calling <c>accept()</c> accepts the connection and returns a <c>Channel</c> object.
    /// Calling <c>reject()</c> rejects the connection and no further action is needed.
    /// </summary>
    [<Emit("$0.on('tcp connection',$1)")>] abstract on_tcp_connection: listener: (TcpConnectionDetails -> AcceptConnection<ClientChannel> -> RejectConnection -> unit) -> Client
    /// <summary>
    /// Emitted when an incoming X11 connection is being requested.
    /// 
    /// Calling <c>accept()</c> accepts the connection and returns a <c>Channel</c> object.
    /// Calling <c>reject()</c> rejects the connection and no further action is needed.
    /// </summary>
    [<Emit("$0.on('x11',$1)")>] abstract on_x11: listener: (X11Details -> AcceptConnection<ClientChannel> -> RejectConnection -> unit) -> Client
    /// <summary>
    /// Emitted when the server is asking for replies to the given <c>prompts</c> for keyboard-
    /// interactive user authentication.
    /// 
    /// * <c>name</c> is generally what you'd use as a window title (for GUI apps).
    /// * <c>prompts</c> is an array of <c>Prompt</c> objects.
    /// 
    /// The answers for all prompts must be provided as an array of strings and passed to
    /// <c>finish</c> when you are ready to continue.
    /// 
    /// NOTE: It's possible for the server to come back and ask more questions.
    /// </summary>
    [<Emit("$0.on('keyboard-interactive',$1)")>] abstract ``on_keyboard-interactive``: listener: (string -> string -> string -> ResizeArray<Prompt> -> KeyboardInteractiveCallback -> unit) -> Client
    /// <summary>
    /// Emitted when the server has requested that the user's password be changed, if using
    /// password-based user authentication.
    /// 
    /// Call <c>done</c> with the new password.
    /// </summary>
    [<Emit("$0.on('change password',$1)")>] abstract on_change_password: listener: (string -> ChangePasswordCallback -> unit) -> Client
    /// Emitted when an error occurred.
    [<Emit("$0.on('error',$1)")>] abstract on_error: listener: (obj -> unit) -> Client
    /// Emitted when the socket was disconnected.
    [<Emit("$0.on('end',$1)")>] abstract on_end: listener: (unit -> unit) -> Client
    /// Emitted when the socket was closed.
    [<Emit("$0.on('close',$1)")>] abstract on_close: listener: (unit -> unit) -> Client
    /// Emitted when the socket has timed out.
    [<Emit("$0.on('timeout',$1)")>] abstract on_timeout: listener: (unit -> unit) -> Client
    /// Emitted when the socket has connected.
    [<Emit("$0.on('connect',$1)")>] abstract on_connect: listener: (unit -> unit) -> Client
    /// Emitted when the server responds with a greeting message.
    [<Emit("$0.on('greeting',$1)")>] abstract on_greeting: listener: (string -> unit) -> Client
    /// Emitted when a handshake has completed (either initial or rekey).
    [<Emit("$0.on('handshake',$1)")>] abstract on_handshake: listener: (NegotiatedAlgorithms -> unit) -> Client
    /// Emitted when the server announces its available host keys.
    [<Emit("$0.on('hostkeys',$1)")>] abstract on_hostkeys: listener: (ResizeArray<ParsedKey> -> unit) -> Client
    /// An incoming forwarded UNIX socket connection is being requested.
    [<Emit("$0.on('unix connection',$1)")>] abstract on_unix_connection: listener: (UNIXConnectionDetails -> AcceptConnection -> RejectConnection -> unit) -> Client
    /// Attempts a connection to a server.
    abstract connect: config: ConnectConfig -> Client
    /// <summary>Executes a command on the server.</summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="options">Options for the command.</param>
    /// <param name="callback">The callback to execute when the command has completed.</param>
    abstract exec: command: string * options: ExecOptions * callback: ClientCallback -> Client
    /// <summary>Executes a command on the server.</summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="callback">The callback to execute when the command has completed.</param>
    abstract exec: command: string * callback: ClientCallback -> Client
    /// <summary>Starts an interactive shell session on the server.</summary>
    /// <param name="window">Either an object containing pseudo-tty settings, <c>false</c> to suppress creation of a pseudo-tty.</param>
    /// <param name="options">Options for the command.</param>
    /// <param name="callback">The callback to execute when the channel has been created.</param>
    abstract shell: window: PseudoTtyOptions * options: ShellOptions * callback: ClientCallback -> Client
    /// <summary>Starts an interactive shell session on the server.</summary>
    /// <param name="window">Either an object containing pseudo-tty settings, <c>false</c> to suppress creation of a pseudo-tty.</param>
    /// <param name="callback">The callback to execute when the channel has been created.</param>
    abstract shell: window: PseudoTtyOptions * callback: ClientCallback -> Client
    /// <summary>Starts an interactive shell session on the server.</summary>
    /// <param name="options">Options for the command.</param>
    /// <param name="callback">The callback to execute when the channel has been created.</param>
    abstract shell: options: ShellOptions * callback: ClientCallback -> Client
    /// <summary>Starts an interactive shell session on the server.</summary>
    /// <param name="callback">The callback to execute when the channel has been created.</param>
    abstract shell: callback: ClientCallback -> Client
    /// <summary>Bind to <c>remoteAddr</c> on <c>remotePort</c> on the server and forward incoming TCP connections.</summary>
    /// <param name="remoteAddr">
    /// The remote address to bind on the server. The following lists several special values for <c>remoteAddr</c> and their respective bindings:
    /// 
    /// | address       | description
    /// |:--------------|:-----------
    /// | <c>''</c>          | Listen on all protocol families supported by the server
    /// | <c>'0.0.0.0'</c>   | Listen on all IPv4 addresses
    /// | <c>'::'</c>        | Listen on all IPv6 addresses
    /// | <c>'localhost'</c> | Listen on the loopback interface for all protocol families
    /// | <c>'127.0.0.1'</c> | Listen on the loopback interfaces for IPv4
    /// | <c>'::1'</c>       | Listen on the loopback interfaces for IPv6
    /// </param>
    /// <param name="remotePort">The remote port to bind on the server. If this value is <c>0</c>, the actual bound port is provided to <c>callback</c>.</param>
    /// <param name="callback">An optional callback that is invoked when the remote address is bound.</param>
    abstract forwardIn: remoteAddr: string * remotePort: float * ?callback: ClientForwardCallback -> Client
    /// <summary>
    /// Unbind from <c>remoteAddr</c> on <c>remotePort</c> on the server and stop forwarding incoming TCP
    /// connections. Until <c>callback</c> is called, more connections may still come in.
    /// </summary>
    /// <param name="remoteAddr">The remote address to unbind on the server.</param>
    /// <param name="remotePort">The remote port to unbind on the server.</param>
    /// <param name="callback">An optional callback that is invoked when the remote address is unbound.</param>
    abstract unforwardIn: remoteAddr: string * remotePort: float * ?callback: Callback -> Client
    /// <summary>
    /// Open a connection with <c>srcIP</c> and <c>srcPort</c> as the originating address and port and
    /// <c>dstIP</c> and <c>dstPort</c> as the remote destination address and port.
    /// </summary>
    /// <param name="srcIP">The originating address.</param>
    /// <param name="srcPort">The originating port.</param>
    /// <param name="dstIP">The destination address.</param>
    /// <param name="dstPort">The destination port.</param>
    /// <param name="callback">The callback that is invoked when the address is bound.</param>
    abstract forwardOut: srcIP: string * srcPort: float * dstIP: string * dstPort: float * ?callback: ClientCallback -> Client
    /// <summary>Starts an SFTP session.</summary>
    /// <param name="callback">The callback that is invoked when the SFTP session has started.</param>
    abstract sftp: callback: ClientSFTPCallback -> Client
    /// <summary>Invokes <c>subsystem</c> on the server.</summary>
    /// <param name="subsystem">The subsystem to start on the server.</param>
    /// <param name="callback">The callback that is invoked when the subsystem has started.</param>
    abstract subsys: subsystem: string * callback: ClientCallback -> Client
    /// Disconnects the socket.
    abstract ``end``: unit -> Client
    /// Destroys the socket.
    abstract destroy: unit -> Client
    /// OpenSSH extension that sends a request to reject any new sessions (e.g. exec, shell,
    /// sftp, subsys) for this connection.
    abstract openssh_noMoreSessions: cb: Callback -> Client
    /// <summary>
    /// OpenSSH extension that binds to a UNIX domain socket at <c>socketPath</c> on the server and
    /// forwards incoming connections.
    /// </summary>
    abstract openssh_forwardInStreamLocal: socketPath: string * cb: Callback -> Client
    /// <summary>
    /// OpenSSH extension that unbinds from a UNIX domain socket at <c>socketPath</c> on the server
    /// and stops forwarding incoming connections.
    /// </summary>
    abstract openssh_unforwardInStreamLocal: socketPath: string * cb: Callback -> Client
    /// <summary>
    /// OpenSSH extension that opens a connection to a UNIX domain socket at <c>socketPath</c> on
    /// the server.
    /// </summary>
    abstract openssh_forwardOutStreamLocal: socketPath: string * cb: ClientCallback -> Client