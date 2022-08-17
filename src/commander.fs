// ts2fable 0.8.0-build.664
module rec commander

#nowarn "3390" // disable warnings for invalid XML comments
#nowarn "0044" // disable warnings for `Obsolete` usage

open System
open Fable.Core
open Fable.Core.JS

type Error = System.Exception
type RegExp = System.Text.RegularExpressions.Regex
type Symbol = obj

let [<Import("program","commander")>] program: Command = jsNative

type [<AllowNullLiteral>] IExports =
    abstract CommanderError: CommanderErrorStatic
    abstract InvalidArgumentError: InvalidArgumentErrorStatic
    abstract Argument: ArgumentStatic
    abstract Option: OptionStatic
    abstract Help: HelpStatic
    abstract Command: CommandStatic
    abstract createCommand: ?name: string -> Command
    abstract createOption: flags: string * ?description: string -> Option
    abstract createArgument: name: string * ?description: string -> Argument

type [<AllowNullLiteral>] CommanderError =
    abstract code: string with get, set
    abstract exitCode: float with get, set
    abstract message: string with get, set
    abstract nestedError: string option with get, set

type [<AllowNullLiteral>] CommanderErrorStatic =
    /// <summary>Constructs the CommanderError class</summary>
    /// <param name="exitCode">suggested exit code which could be used with process.exit</param>
    /// <param name="code">an id string representing the error</param>
    /// <param name="message">human-readable description of the error</param>
    [<EmitConstructor>] abstract Create: exitCode: float * code: string * message: string -> CommanderError

type [<AllowNullLiteral>] InvalidArgumentError =
    inherit CommanderError

type [<AllowNullLiteral>] InvalidArgumentErrorStatic =
    /// <summary>Constructs the InvalidArgumentError class</summary>
    /// <param name="message">explanation of why argument is invalid</param>
    [<EmitConstructor>] abstract Create: message: string -> InvalidArgumentError

type [<AllowNullLiteral>] ErrorOptions =
    /// an id string representing the error
    abstract code: string option with get, set
    /// suggested exit code which could be used with process.exit
    abstract exitCode: float option with get, set

type [<AllowNullLiteral>] Argument =
    abstract description: string with get, set
    abstract required: bool with get, set
    abstract variadic: bool with get, set
    /// Return argument name.
    abstract name: unit -> string
    /// Set the default value, and optionally supply the description to be displayed in the help.
    abstract ``default``: value: obj * ?description: string -> Argument
    /// Set the custom handler for processing CLI command arguments into argument values.
    abstract argParser: fn: (string -> 'T -> 'T) -> Argument
    /// Only allow argument value to be one of choices.
    abstract choices: values: ResizeArray<string> -> Argument
    /// Make argument required.
    abstract argRequired: unit -> Argument
    /// Make argument optional.
    abstract argOptional: unit -> Argument

type [<AllowNullLiteral>] ArgumentStatic =
    /// Initialize a new command argument with the given name and description.
    /// The default is that the argument is required, and you can explicitly
    /// indicate this with <> around the name. Put [] around the name for an optional argument.
    [<EmitConstructor>] abstract Create: arg: string * ?description: string -> Argument

type [<AllowNullLiteral>] Option =
    abstract flags: string with get, set
    abstract description: string with get, set
    abstract required: bool with get, set
    abstract optional: bool with get, set
    abstract variadic: bool with get, set
    abstract mandatory: bool with get, set
    abstract optionFlags: string with get, set
    abstract short: string option with get, set
    abstract long: string option with get, set
    abstract negate: bool with get, set
    abstract defaultValue: obj option with get, set
    abstract defaultValueDescription: string option with get, set
    abstract parseArg: (string -> 'T -> 'T) option with get, set
    abstract hidden: bool with get, set
    abstract argChoices: ResizeArray<string> option with get, set
    /// Set the default value, and optionally supply the description to be displayed in the help.
    abstract ``default``: value: obj * ?description: string -> Option
    /// <summary>
    /// Preset to use when option used without option-argument, especially optional but also boolean and negated.
    /// The custom processing (parseArg) is called.
    /// </summary>
    /// <example>
    /// <code lang="ts">
    /// new Option('--color').default('GREYSCALE').preset('RGB');
    /// new Option('--donate [amount]').preset('20').argParser(parseFloat);
    /// </code>
    /// </example>
    abstract preset: arg: obj -> Option
    /// <summary>
    /// Add option name(s) that conflict with this option.
    /// An error will be displayed if conflicting options are found during parsing.
    /// </summary>
    /// <example>
    /// <code lang="ts">
    /// new Option('--rgb').conflicts('cmyk');
    /// new Option('--js').conflicts(['ts', 'jsx']);
    /// </code>
    /// </example>
    abstract conflicts: names: U2<string, ResizeArray<string>> -> Option
    /// <summary>
    /// Specify implied option values for when this option is set and the implied options are not.
    /// 
    /// The custom processing (parseArg) is not called on the implied values.
    /// </summary>
    /// <example>
    /// program
    ///   .addOption(new Option('--log', 'write logging information to file'))
    ///   .addOption(new Option('--trace', 'log extra details').implies({ log: 'trace.txt' }));
    /// </example>
    abstract implies: optionValues: OptionValues -> Option
    /// Set environment variable to check for option value.
    /// Priority order of option values is default < env < cli
    abstract env: name: string -> Option
    /// Calculate the full description, including defaultValue etc.
    abstract fullDescription: unit -> string
    /// Set the custom handler for processing CLI option arguments into option values.
    abstract argParser: fn: (string -> 'T -> 'T) -> Option
    /// Whether the option is mandatory and must have a value after parsing.
    abstract makeOptionMandatory: ?mandatory: bool -> Option
    /// Hide option in help.
    abstract hideHelp: ?hide: bool -> Option
    /// Only allow option value to be one of choices.
    abstract choices: values: ResizeArray<string> -> Option
    /// Return option name.
    abstract name: unit -> string
    /// Return option name, in a camelcase format that can be used
    /// as a object attribute key.
    abstract attributeName: unit -> string
    /// Return whether a boolean option.
    /// 
    /// Options are one of boolean, negated, required argument, or optional argument.
    abstract isBoolean: unit -> bool

type [<AllowNullLiteral>] OptionStatic =
    [<EmitConstructor>] abstract Create: flags: string * ?description: string -> Option

type [<AllowNullLiteral>] Help =
    /// output helpWidth, long lines are wrapped to fit
    abstract helpWidth: float option with get, set
    abstract sortSubcommands: bool with get, set
    abstract sortOptions: bool with get, set
    /// Get the command term to show in the list of subcommands.
    abstract subcommandTerm: cmd: Command -> string
    /// Get the command summary to show in the list of subcommands.
    abstract subcommandDescription: cmd: Command -> string
    /// Get the option term to show in the list of options.
    abstract optionTerm: option: Option -> string
    /// Get the option description to show in the list of options.
    abstract optionDescription: option: Option -> string
    /// Get the argument term to show in the list of arguments.
    abstract argumentTerm: argument: Argument -> string
    /// Get the argument description to show in the list of arguments.
    abstract argumentDescription: argument: Argument -> string
    /// Get the command usage to be displayed at the top of the built-in help.
    abstract commandUsage: cmd: Command -> string
    /// Get the description for the command.
    abstract commandDescription: cmd: Command -> string
    /// Get an array of the visible subcommands. Includes a placeholder for the implicit help command, if there is one.
    abstract visibleCommands: cmd: Command -> ResizeArray<Command>
    /// Get an array of the visible options. Includes a placeholder for the implicit help option, if there is one.
    abstract visibleOptions: cmd: Command -> ResizeArray<Option>
    /// Get an array of the arguments which have descriptions.
    abstract visibleArguments: cmd: Command -> ResizeArray<Argument>
    /// Get the longest command term length.
    abstract longestSubcommandTermLength: cmd: Command * helper: Help -> float
    /// Get the longest option term length.
    abstract longestOptionTermLength: cmd: Command * helper: Help -> float
    /// Get the longest argument term length.
    abstract longestArgumentTermLength: cmd: Command * helper: Help -> float
    /// Calculate the pad width from the maximum term length.
    abstract padWidth: cmd: Command * helper: Help -> float
    /// Wrap the given string to width characters per line, with lines after the first indented.
    /// Do not wrap if insufficient room for wrapping (minColumnWidth), or string is manually formatted.
    abstract wrap: str: string * width: float * indent: float * ?minColumnWidth: float -> string
    /// Generate the built-in help text.
    abstract formatHelp: cmd: Command * helper: Help -> string

type [<AllowNullLiteral>] HelpStatic =
    [<EmitConstructor>] abstract Create: unit -> Help

type HelpConfiguration =
    obj

type [<AllowNullLiteral>] ParseOptions =
    abstract from: ParseOptionsFrom with get, set

type [<AllowNullLiteral>] HelpContext =
    abstract error: bool with get, set

type [<AllowNullLiteral>] AddHelpTextContext =
    abstract error: bool with get, set
    abstract command: Command with get, set

type [<AllowNullLiteral>] OutputConfiguration =
    abstract writeOut: str: string -> unit
    abstract writeErr: str: string -> unit
    abstract getOutHelpWidth: unit -> float
    abstract getErrHelpWidth: unit -> float
    abstract outputError: str: string * write: (string -> unit) -> unit

type [<StringEnum>] [<RequireQualifiedAccess>] AddHelpTextPosition =
    | BeforeAll
    | Before
    | After
    | AfterAll

type [<StringEnum>] [<RequireQualifiedAccess>] HookEvent =
    | PreSubcommand
    | PreAction
    | PostAction

type [<StringEnum>] [<RequireQualifiedAccess>] OptionValueSource =
    | Default
    | Env
    | Config
    | Cli

type [<AllowNullLiteral>] OptionValues =
    [<EmitIndexer>] abstract Item: key: string -> obj option with get, set

type ReturnType = obj

type [<AllowNullLiteral>] Command =
    abstract args: ResizeArray<string> with get, set
    abstract processedArgs: ResizeArray<obj option> with get, set
    abstract commands: ResizeArray<Command> with get, set
    abstract parent: Command option with get, set
    /// <summary>
    /// Set the program version to <c>str</c>.
    /// 
    /// This method auto-registers the "-V, --version" flag
    /// which will print the version number when passed.
    /// 
    /// You can optionally supply the  flags and description to override the defaults.
    /// </summary>
    abstract version: str: string * ?flags: string * ?description: string -> Command
    /// <summary>Define a command, implemented using an action handler.</summary>
    /// <remarks>The command description is supplied using <c>.description</c>, not as a parameter to <c>.command</c>.</remarks>
    /// <example>
    /// <code lang="ts">
    /// program
    ///   .command('clone &lt;source&gt; [destination]')
    ///   .description('clone a repository into a newly created directory')
    ///   .action((source, destination) =&gt; {
    ///     console.log('clone command called');
    ///   });
    /// </code>
    /// </example>
    /// <param name="nameAndArgs">command name and arguments, args are  <c>&lt;required&gt;</c> or <c>[optional]</c> and last may also be <c>variadic...</c></param>
    /// <param name="opts">configuration options</param>
    /// <returns>new command</returns>
    abstract command: nameAndArgs: string * ?opts: CommandOptions -> ReturnType
    /// <summary>Define a command, implemented in a separate executable file.</summary>
    /// <remarks>The command description is supplied as the second parameter to <c>.command</c>.</remarks>
    /// <example>
    /// <code lang="ts">
    ///  program
    ///    .command('start &lt;service&gt;', 'start named service')
    ///    .command('stop [service]', 'stop named service, or all if no name supplied');
    /// </code>
    /// </example>
    /// <param name="nameAndArgs">command name and arguments, args are  <c>&lt;required&gt;</c> or <c>[optional]</c> and last may also be <c>variadic...</c></param>
    /// <param name="description">description of executable command</param>
    /// <param name="opts">configuration options</param>
    /// <returns><c>this</c> command for chaining</returns>
    abstract command: nameAndArgs: string * description: string * ?opts: ExecutableCommandOptions -> Command
    /// Factory routine to create a new unattached command.
    /// 
    /// See .command() for creating an attached subcommand, which uses this routine to
    /// create the command. You can override createCommand to customise subcommands.
    abstract createCommand: ?name: string -> Command
    /// <summary>
    /// Add a prepared subcommand.
    /// 
    /// See .command() for creating an attached subcommand which inherits settings from its parent.
    /// </summary>
    /// <returns><c>this</c> command for chaining</returns>
    abstract addCommand: cmd: Command * ?opts: CommandOptions -> Command
    /// Factory routine to create a new unattached argument.
    /// 
    /// See .argument() for creating an attached argument, which uses this routine to
    /// create the argument. You can override createArgument to return a custom argument.
    abstract createArgument: name: string * ?description: string -> Argument
    /// <summary>
    /// Define argument syntax for command.
    /// 
    /// The default is that the argument is required, and you can explicitly
    /// indicate this with &lt;&gt; around the name. Put [] around the name for an optional argument.
    /// </summary>
    /// <example>
    /// <code>
    /// program.argument('&lt;input-file&gt;');
    /// program.argument('[output-file]');
    /// </code>
    /// </example>
    /// <returns><c>this</c> command for chaining</returns>
    abstract argument: flags: string * description: string * fn: (string -> 'T -> 'T) * ?defaultValue: 'T -> Command
    abstract argument: name: string * ?description: string * ?defaultValue: obj -> Command
    /// <summary>Define argument syntax for command, adding a prepared argument.</summary>
    /// <returns><c>this</c> command for chaining</returns>
    abstract addArgument: arg: Argument -> Command
    /// <summary>
    /// Define argument syntax for command, adding multiple at once (without descriptions).
    /// 
    /// See also .argument().
    /// </summary>
    /// <example>
    /// <code>
    /// program.arguments('&lt;cmd&gt; [env]');
    /// </code>
    /// </example>
    /// <returns><c>this</c> command for chaining</returns>
    abstract arguments: names: string -> Command
    /// <summary>Override default decision whether to add implicit help command.</summary>
    /// <example>
    /// <code>
    /// addHelpCommand() // force on
    /// addHelpCommand(false); // force off
    /// addHelpCommand('help [cmd]', 'display help for [cmd]'); // force on with custom details
    /// </code>
    /// </example>
    /// <returns><c>this</c> command for chaining</returns>
    abstract addHelpCommand: ?enableOrNameAndArgs: U2<string, bool> * ?description: string -> Command
    /// Add hook for life cycle event.
    abstract hook: ``event``: HookEvent * listener: (Command -> Command -> U2<unit, Promise<unit>>) -> Command
    /// Register callback to use as replacement for calling process.exit.
    abstract exitOverride: ?callback: (CommanderError -> U2<obj, unit>) -> Command
    /// Display error message and exit (or call exitOverride).
    abstract error: message: string * ?errorOptions: ErrorOptions -> obj
    /// You can customise the help with a subclass of Help by overriding createHelp,
    /// or by overriding Help properties using configureHelp().
    abstract createHelp: unit -> Help
    /// You can customise the help by overriding Help properties using configureHelp(),
    /// or with a subclass of Help by overriding createHelp().
    abstract configureHelp: configuration: HelpConfiguration -> Command
    /// Get configuration
    abstract configureHelp: unit -> HelpConfiguration
    /// <summary>
    /// The default output goes to stdout and stderr. You can customise this for special
    /// applications. You can also customise the display of errors by overriding outputError.
    /// 
    /// The configuration properties are all functions:
    /// <code>
    /// // functions to change where being written, stdout and stderr
    /// writeOut(str)
    /// writeErr(str)
    /// // matching functions to specify width for wrapping help
    /// getOutHelpWidth()
    /// getErrHelpWidth()
    /// // functions based on what is being written out
    /// outputError(str, write) // used for displaying errors, and not used for displaying help
    /// </code>
    /// </summary>
    abstract configureOutput: configuration: OutputConfiguration -> Command
    /// Get configuration
    abstract configureOutput: unit -> OutputConfiguration
    /// <summary>
    /// Copy settings that are useful to have in common across root command and subcommands.
    /// 
    /// (Used internally when adding a command using <c>.command()</c> so subcommands inherit parent settings.)
    /// </summary>
    abstract copyInheritedSettings: sourceCommand: Command -> Command
    /// Display the help or a custom message after an error occurs.
    abstract showHelpAfterError: ?displayHelp: U2<bool, string> -> Command
    /// Display suggestion of similar commands for unknown commands, or options for unknown options.
    abstract showSuggestionAfterError: ?displaySuggestion: bool -> Command
    /// <summary>Register callback <c>fn</c> for the command.</summary>
    /// <example>
    /// <code>
    /// program
    ///   .command('serve')
    ///   .description('start service')
    ///   .action(function() {
    ///     // do work here
    ///   });
    /// </code>
    /// </example>
    /// <returns><c>this</c> command for chaining</returns>
    abstract action: fn: (ResizeArray<obj option> -> U2<unit, Promise<unit>>) -> Command
    /// <summary>
    /// Define option with <c>flags</c>, <c>description</c> and optional
    /// coercion <c>fn</c>.
    /// 
    /// The <c>flags</c> string contains the short and/or long flags,
    /// separated by comma, a pipe or space. The following are all valid
    /// all will output this way when <c>--help</c> is used.
    /// 
    ///      "-p, --pepper"
    ///      "-p|--pepper"
    ///      "-p --pepper"
    /// </summary>
    /// <example>
    /// <code>
    /// // simple boolean defaulting to false
    ///  program.option('-p, --pepper', 'add pepper');
    /// 
    ///  --pepper
    ///  program.pepper
    ///  // =&gt; Boolean
    /// 
    ///  // simple boolean defaulting to true
    ///  program.option('-C, --no-cheese', 'remove cheese');
    /// 
    ///  program.cheese
    ///  // =&gt; true
    /// 
    ///  --no-cheese
    ///  program.cheese
    ///  // =&gt; false
    /// 
    ///  // required argument
    ///  program.option('-C, --chdir &lt;path&gt;', 'change the working directory');
    /// 
    ///  --chdir /tmp
    ///  program.chdir
    ///  // =&gt; "/tmp"
    /// 
    ///  // optional argument
    ///  program.option('-c, --cheese [type]', 'add cheese [marble]');
    /// </code>
    /// </example>
    /// <returns><c>this</c> command for chaining</returns>
    abstract option: flags: string * ?description: string * ?defaultValue: U3<string, bool, ResizeArray<string>> -> Command
    abstract option: flags: string * description: string * fn: (string -> 'T -> 'T) * ?defaultValue: 'T -> Command
    [<Obsolete("since v7, instead use choices or a custom function")>]
    abstract option: flags: string * description: string * regexp: RegExp * ?defaultValue: U3<string, bool, ResizeArray<string>> -> Command
    /// <summary>
    /// Define a required option, which must have a value after parsing. This usually means
    /// the option must be specified on the command line. (Otherwise the same as .option().)
    /// 
    /// The <c>flags</c> string contains the short and/or long flags, separated by comma, a pipe or space.
    /// </summary>
    abstract requiredOption: flags: string * ?description: string * ?defaultValue: U3<string, bool, ResizeArray<string>> -> Command
    abstract requiredOption: flags: string * description: string * fn: (string -> 'T -> 'T) * ?defaultValue: 'T -> Command
    [<Obsolete("since v7, instead use choices or a custom function")>]
    abstract requiredOption: flags: string * description: string * regexp: RegExp * ?defaultValue: U3<string, bool, ResizeArray<string>> -> Command
    /// Factory routine to create a new unattached option.
    /// 
    /// See .option() for creating an attached option, which uses this routine to
    /// create the option. You can override createOption to return a custom option.
    abstract createOption: flags: string * ?description: string -> Option
    /// Add a prepared Option.
    /// 
    /// See .option() and .requiredOption() for creating and attaching an option in a single call.
    abstract addOption: option: Option -> Command
    /// <summary>
    /// Whether to store option values as properties on command object,
    /// or store separately (specify false). In both cases the option values can be accessed using .opts().
    /// </summary>
    /// <returns><c>this</c> command for chaining</returns>
    abstract storeOptionsAsProperties: unit -> obj when 'T :> OptionValues
    abstract storeOptionsAsProperties: storeAsProperties: bool -> obj when 'T :> OptionValues
    abstract storeOptionsAsProperties: ?storeAsProperties: bool -> Command
    /// Retrieve option value.
    abstract getOptionValue: key: string -> obj option
    /// Store option value.
    abstract setOptionValue: key: string * value: obj -> Command
    /// Store option value and where the value came from.
    abstract setOptionValueWithSource: key: string * value: obj * source: OptionValueSource -> Command
    /// Retrieve option value source.
    abstract getOptionValueSource: key: string -> OptionValueSource
    /// <summary>Alter parsing of short flags with optional values.</summary>
    /// <example>
    /// <code>
    /// // for `.option('-f,--flag [value]'):
    /// .combineFlagAndOptionalValue(true)  // `-f80` is treated like `--flag=80`, this is the default behaviour
    /// .combineFlagAndOptionalValue(false) // `-fb` is treated like `-f -b`
    /// </code>
    /// </example>
    /// <returns><c>this</c> command for chaining</returns>
    abstract combineFlagAndOptionalValue: ?combine: bool -> Command
    /// <summary>Allow unknown options on the command line.</summary>
    /// <returns><c>this</c> command for chaining</returns>
    abstract allowUnknownOption: ?allowUnknown: bool -> Command
    /// <summary>Allow excess command-arguments on the command line. Pass false to make excess arguments an error.</summary>
    /// <returns><c>this</c> command for chaining</returns>
    abstract allowExcessArguments: ?allowExcess: bool -> Command
    /// <summary>
    /// Enable positional options. Positional means global options are specified before subcommands which lets
    /// subcommands reuse the same option names, and also enables subcommands to turn on passThroughOptions.
    /// 
    /// The default behaviour is non-positional and global options may appear anywhere on the command line.
    /// </summary>
    /// <returns><c>this</c> command for chaining</returns>
    abstract enablePositionalOptions: ?positional: bool -> Command
    /// <summary>
    /// Pass through options that come after command-arguments rather than treat them as command-options,
    /// so actual command-options come before command-arguments. Turning this on for a subcommand requires
    /// positional options to have been enabled on the program (parent commands).
    /// 
    /// The default behaviour is non-positional and options may appear before or after command-arguments.
    /// </summary>
    /// <returns><c>this</c> command for chaining</returns>
    abstract passThroughOptions: ?passThrough: bool -> Command
    /// <summary>
    /// Parse <c>argv</c>, setting options and invoking commands when defined.
    /// 
    /// The default expectation is that the arguments are from node and have the application as argv[0]
    /// and the script being run in argv[1], with user parameters after that.
    /// </summary>
    /// <example>
    /// <code>
    /// program.parse(process.argv);
    /// program.parse(); // implicitly use process.argv and auto-detect node vs electron conventions
    /// program.parse(my-args, { from: 'user' }); // just user supplied arguments, nothing special about argv[0]
    /// </code>
    /// </example>
    /// <returns><c>this</c> command for chaining</returns>
    abstract parse: ?argv: ResizeArray<string> * ?options: ParseOptions -> Command
    /// <summary>
    /// Parse <c>argv</c>, setting options and invoking commands when defined.
    /// 
    /// Use parseAsync instead of parse if any of your action handlers are async. Returns a Promise.
    /// 
    /// The default expectation is that the arguments are from node and have the application as argv[0]
    /// and the script being run in argv[1], with user parameters after that.
    /// </summary>
    /// <example>
    /// <code>
    /// program.parseAsync(process.argv);
    /// program.parseAsync(); // implicitly use process.argv and auto-detect node vs electron conventions
    /// program.parseAsync(my-args, { from: 'user' }); // just user supplied arguments, nothing special about argv[0]
    /// </code>
    /// </example>
    /// <returns>Promise</returns>
    abstract parseAsync: ?argv: ResizeArray<string> * ?options: ParseOptions -> Promise<Command>
    /// <summary>
    /// Parse options from <c>argv</c> removing known options,
    /// and return argv split into operands and unknown arguments.
    /// 
    ///      argv =&gt; operands, unknown
    ///      --known kkk op =&gt; [op], []
    ///      op --known kkk =&gt; [op], []
    ///      sub --unknown uuu op =&gt; [sub], [--unknown uuu op]
    ///      sub -- --unknown uuu op =&gt; [sub --unknown uuu op], []
    /// </summary>
    abstract parseOptions: argv: ResizeArray<string> -> ParseOptionsResult
    /// Return an object containing local option values as key-value pairs
    abstract opts: unit -> 'T when 'T :> OptionValues
    /// Return an object containing merged local and global option values as key-value pairs.
    abstract optsWithGlobals: unit -> 'T when 'T :> OptionValues
    /// <summary>Set the description.</summary>
    /// <returns><c>this</c> command for chaining</returns>
    abstract description: str: string -> Command
    [<Obsolete("since v8, instead use .argument to add command argument with description")>]
    abstract description: str: string * argsDescription: CommandDescriptionArgsDescription -> Command
    /// Get the description.
    abstract description: unit -> string
    /// <summary>Set the summary. Used when listed as subcommand of parent.</summary>
    /// <returns><c>this</c> command for chaining</returns>
    abstract summary: str: string -> Command
    /// Get the summary.
    abstract summary: unit -> string
    /// <summary>
    /// Set an alias for the command.
    /// 
    /// You may call more than once to add multiple aliases. Only the first alias is shown in the auto-generated help.
    /// </summary>
    /// <returns><c>this</c> command for chaining</returns>
    abstract alias: alias: string -> Command
    /// Get alias for the command.
    abstract alias: unit -> string
    /// <summary>
    /// Set aliases for the command.
    /// 
    /// Only the first alias is shown in the auto-generated help.
    /// </summary>
    /// <returns><c>this</c> command for chaining</returns>
    abstract aliases: aliases: ResizeArray<string> -> Command
    /// Get aliases for the command.
    abstract aliases: unit -> ResizeArray<string>
    /// <summary>Set the command usage.</summary>
    /// <returns><c>this</c> command for chaining</returns>
    abstract usage: str: string -> Command
    /// Get the command usage.
    abstract usage: unit -> string
    /// <summary>Set the name of the command.</summary>
    /// <returns><c>this</c> command for chaining</returns>
    abstract name: str: string -> Command
    /// Get the name of the command.
    abstract name: unit -> string
    /// <summary>
    /// Set the name of the command from script filename, such as process.argv[1],
    /// or require.main.filename, or __filename.
    /// 
    /// (Used internally and public although not documented in README.)
    /// </summary>
    /// <example>
    /// <code lang="ts">
    /// program.nameFromFilename(require.main.filename);
    /// </code>
    /// </example>
    /// <returns><c>this</c> command for chaining</returns>
    abstract nameFromFilename: filename: string -> Command
    /// <summary>Set the directory for searching for executable subcommands of this command.</summary>
    /// <example>
    /// <code lang="ts">
    /// program.executableDir(__dirname);
    /// // or
    /// program.executableDir('subcommands');
    /// </code>
    /// </example>
    /// <returns><c>this</c> command for chaining</returns>
    abstract executableDir: path: string -> Command
    /// Get the executable search directory.
    abstract executableDir: unit -> string
    /// <summary>
    /// Output help information for this command.
    /// 
    /// Outputs built-in help, and custom text added using <c>.addHelpText()</c>.
    /// </summary>
    abstract outputHelp: ?context: HelpContext -> unit
    [<Obsolete("since v7")>]
    abstract outputHelp: ?cb: (string -> string) -> unit
    /// Return command help documentation.
    abstract helpInformation: ?context: HelpContext -> string
    /// You can pass in flags and a description to override the help
    /// flags and help description for your command. Pass in false
    /// to disable the built-in help option.
    abstract helpOption: ?flags: U2<string, bool> * ?description: string -> Command
    /// <summary>
    /// Output help information and exit.
    /// 
    /// Outputs built-in help, and custom text added using <c>.addHelpText()</c>.
    /// </summary>
    abstract help: ?context: HelpContext -> obj
    [<Obsolete("since v7")>]
    abstract help: ?cb: (string -> string) -> obj
    /// Add additional text to be displayed with the built-in help.
    /// 
    /// Position is 'before' or 'after' to affect just this command,
    /// and 'beforeAll' or 'afterAll' to affect this command and all its subcommands.
    abstract addHelpText: position: AddHelpTextPosition * text: string -> Command
    abstract addHelpText: position: AddHelpTextPosition * text: (AddHelpTextContext -> string) -> Command
    /// Add a listener (callback) for when events occur. (Implemented using EventEmitter.)
    abstract on: ``event``: U2<string, Symbol> * listener: (ResizeArray<obj option> -> unit) -> Command

/// <summary>
/// Typescript interface contains an <see href="https://www.typescriptlang.org/docs/handbook/2/objects.html#index-signatures">index signature</see> (like <c>{ [key:string]: string }</c>).
/// Unlike an indexer in F#, index signatures index over a type's members.
/// 
/// As such an index signature cannot be implemented via regular F# Indexer (<c>Item</c> property),
/// but instead by just specifying fields.
/// 
/// Easiest way to declare such a type is with an Anonymous Record and force it into the function.
/// For example:
/// <code lang="fsharp">
/// type I =
///     [&lt;EmitIndexer&gt;]
///     abstract Item: string -&gt; string
/// let f (i: I) = jsNative
/// 
/// let t = {| Value1 = "foo"; Value2 = "bar" |}
/// f (!! t)
/// </code>
/// </summary>
type [<AllowNullLiteral>] CommandDescriptionArgsDescription =
    [<EmitIndexer>] abstract Item: argName: string -> string with get, set

type [<AllowNullLiteral>] CommandStatic =
    [<EmitConstructor>] abstract Create: ?name: string -> Command

type [<AllowNullLiteral>] CommandOptions =
    abstract hidden: bool option with get, set
    abstract isDefault: bool option with get, set
    [<Obsolete("since v7, replaced by hidden")>]
    abstract noHelp: bool option with get, set

type [<AllowNullLiteral>] ExecutableCommandOptions =
    inherit CommandOptions
    abstract executableFile: string option with get, set

type [<AllowNullLiteral>] ParseOptionsResult =
    abstract operands: ResizeArray<string> with get, set
    abstract unknown: ResizeArray<string> with get, set

type [<StringEnum>] [<RequireQualifiedAccess>] ParseOptionsFrom =
    | Node
    | Electron
    | User
