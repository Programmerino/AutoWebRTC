// ts2fable 0.8.0-build.664
module rec timers

#nowarn "3390" // disable warnings for invalid XML comments

open System
open Fable.Core
open Fable.Core.JS

[<Erase>] type KeyOf<'T> = Key of string

/// A signal object that allows you to communicate with a DOM request (such as a Fetch) and abort it if required via an AbortController object.
let [<Import("AbortSignal","module")>] AbortSignal: {| prototype: AbortSignal; Create: unit -> obj |} = jsNative

type [<AllowNullLiteral>] EventListener =
    [<Emit("$0($1...)")>] abstract Invoke: evt: Event -> unit

type [<AllowNullLiteral>] EventListenerOptions =
    abstract capture: bool option with get, set

type [<AllowNullLiteral>] AddEventListenerOptions =
    inherit EventListenerOptions
    abstract once: bool option with get, set
    abstract passive: bool option with get, set

type [<AllowNullLiteral>] EventListenerObject =
    abstract handleEvent: evt: Event -> unit

type EventListenerOrEventListenerObject =
    U2<EventListener, EventListenerObject>

type [<AllowNullLiteral>] AbortSignalEventMap =
    abstract abort: Event with get, set

/// EventTarget is a DOM interface implemented by objects that can receive events and may have listeners for them.
type [<AllowNullLiteral>] EventTarget =
    /// Appends an event listener for events whose type attribute value is type. The callback argument sets the callback that will be invoked when the event is dispatched.
    /// 
    /// The options argument sets listener-specific options. For compatibility this can be a boolean, in which case the method behaves exactly as if the value was specified as options's capture.
    /// 
    /// When set to true, options's capture prevents callback from being invoked when the event's eventPhase attribute value is BUBBLING_PHASE. When false (or not present), callback will not be invoked when event's eventPhase attribute value is CAPTURING_PHASE. Either way, callback will be invoked if event's eventPhase attribute value is AT_TARGET.
    /// 
    /// When set to true, options's passive indicates that the callback will not cancel the event by invoking preventDefault(). This is used to enable performance optimizations described in § 2.8 Observing event listeners.
    /// 
    /// When set to true, options's once indicates that the callback will only be invoked once after which the event listener will be removed.
    /// 
    /// If an AbortSignal is passed for options's signal, then the event listener will be removed when signal is aborted.
    /// 
    /// The event listener is appended to target's event listener list and is not appended if it has the same type, callback, and capture.
    abstract addEventListener: ``type``: string * callback: EventListenerOrEventListenerObject option * ?options: U2<AddEventListenerOptions, bool> -> unit
    /// Dispatches a synthetic event event to target and returns true if either event's cancelable attribute value is false or its preventDefault() method was not invoked, and false otherwise.
    abstract dispatchEvent: ``event``: Event -> bool
    /// Removes the event listener in target's event listener list with the same type, callback, and options.
    abstract removeEventListener: ``type``: string * callback: EventListenerOrEventListenerObject option * ?options: U2<EventListenerOptions, bool> -> unit

type DOMHighResTimeStamp =
    float
    
/// An event which takes place in the DOM.
type [<AllowNullLiteral>] Event =
    /// Returns true or false depending on how event was initialized. True if event goes through its target's ancestors in reverse tree order, and false otherwise.
    abstract bubbles: bool
    abstract cancelBubble: bool with get, set
    /// Returns true or false depending on how event was initialized. Its return value does not always carry meaning, but true can indicate that part of the operation during which event was dispatched, can be canceled by invoking the preventDefault() method.
    abstract cancelable: bool
    /// Returns true or false depending on how event was initialized. True if event invokes listeners past a ShadowRoot node that is the root of its target, and false otherwise.
    abstract composed: bool
    /// Returns the object whose event listener's callback is currently being invoked.
    abstract currentTarget: EventTarget option
    /// Returns true if preventDefault() was invoked successfully to indicate cancelation, and false otherwise.
    abstract defaultPrevented: bool
    /// Returns the event's phase, which is one of NONE, CAPTURING_PHASE, AT_TARGET, and BUBBLING_PHASE.
    abstract eventPhase: float
    /// Returns true if event was dispatched by the user agent, and false otherwise.
    abstract isTrusted: bool
    [<Obsolete("")>]
    abstract returnValue: bool with get, set
    [<Obsolete("")>]
    abstract srcElement: EventTarget option
    /// Returns the object to which event is dispatched (its target).
    abstract target: EventTarget option
    /// Returns the event's timestamp as the number of milliseconds measured relative to the time origin.
    abstract timeStamp: DOMHighResTimeStamp
    /// Returns the type of event, e.g. "click", "hashchange", or "submit".
    abstract ``type``: string
    /// Returns the invocation target objects of event's path (objects on which listeners will be invoked), except for any nodes in shadow trees of which the shadow root's mode is "closed" that are not reachable from event's currentTarget.
    abstract composedPath: unit -> EventTarget[]
    [<Obsolete("")>]
    abstract initEvent: ``type``: string * ?bubbles: bool * ?cancelable: bool -> unit
    /// If invoked when the cancelable attribute value is true, and while executing a listener for the event with passive set to false, signals to the operation that caused event to be dispatched that it needs to be canceled.
    abstract preventDefault: unit -> unit
    /// Invoking this method prevents event from reaching any registered event listeners after the current one finishes running and, when dispatched in a tree, also prevents event from reaching any other objects.
    abstract stopImmediatePropagation: unit -> unit
    /// When dispatched in a tree, invoking this method prevents event from reaching any objects other than the current object.
    abstract stopPropagation: unit -> unit
    abstract AT_TARGET: float
    abstract BUBBLING_PHASE: float
    abstract CAPTURING_PHASE: float
    abstract NONE: float

/// A signal object that allows you to communicate with a DOM request (such as a Fetch) and abort it if required via an AbortController object.
type [<AllowNullLiteral>] AbortSignal =
    inherit EventTarget
    /// Returns true if this AbortSignal's AbortController has signaled to abort, and false otherwise.
    abstract aborted: bool
    abstract onabort: (AbortSignal -> Event -> obj option) option with get, set
    abstract reason: obj option
    abstract throwIfAborted: unit -> unit
    /// Appends an event listener for events whose type attribute value is type. The callback argument sets the callback that will be invoked when the event is dispatched.
    /// 
    /// The options argument sets listener-specific options. For compatibility this can be a boolean, in which case the method behaves exactly as if the value was specified as options's capture.
    /// 
    /// When set to true, options's capture prevents callback from being invoked when the event's eventPhase attribute value is BUBBLING_PHASE. When false (or not present), callback will not be invoked when event's eventPhase attribute value is CAPTURING_PHASE. Either way, callback will be invoked if event's eventPhase attribute value is AT_TARGET.
    /// 
    /// When set to true, options's passive indicates that the callback will not cancel the event by invoking preventDefault(). This is used to enable performance optimizations described in § 2.8 Observing event listeners.
    /// 
    /// When set to true, options's once indicates that the callback will only be invoked once after which the event listener will be removed.
    /// 
    /// If an AbortSignal is passed for options's signal, then the event listener will be removed when signal is aborted.
    /// 
    /// The event listener is appended to target's event listener list and is not appended if it has the same type, callback, and capture.
    abstract addEventListener: ``type``: KeyOf<AbortSignalEventMap> * listener: (AbortSignal -> obj -> obj option) * ?options: U2<bool, AddEventListenerOptions> -> unit
    /// Appends an event listener for events whose type attribute value is type. The callback argument sets the callback that will be invoked when the event is dispatched.
    /// 
    /// The options argument sets listener-specific options. For compatibility this can be a boolean, in which case the method behaves exactly as if the value was specified as options's capture.
    /// 
    /// When set to true, options's capture prevents callback from being invoked when the event's eventPhase attribute value is BUBBLING_PHASE. When false (or not present), callback will not be invoked when event's eventPhase attribute value is CAPTURING_PHASE. Either way, callback will be invoked if event's eventPhase attribute value is AT_TARGET.
    /// 
    /// When set to true, options's passive indicates that the callback will not cancel the event by invoking preventDefault(). This is used to enable performance optimizations described in § 2.8 Observing event listeners.
    /// 
    /// When set to true, options's once indicates that the callback will only be invoked once after which the event listener will be removed.
    /// 
    /// If an AbortSignal is passed for options's signal, then the event listener will be removed when signal is aborted.
    /// 
    /// The event listener is appended to target's event listener list and is not appended if it has the same type, callback, and capture.
    abstract addEventListener: ``type``: string * listener: EventListenerOrEventListenerObject * ?options: U2<bool, AddEventListenerOptions> -> unit
    /// Removes the event listener in target's event listener list with the same type, callback, and options.
    abstract removeEventListener: ``type``: KeyOf<AbortSignalEventMap> * listener: (AbortSignal -> obj -> obj option) * ?options: U2<bool, EventListenerOptions> -> unit
    /// Removes the event listener in target's event listener list with the same type, callback, and options.
    abstract removeEventListener: ``type``: string * listener: EventListenerOrEventListenerObject * ?options: U2<bool, EventListenerOptions> -> unit

module Events =

    module EventEmitter =

        type [<AllowNullLiteral>] Abortable =
            /// <summary>When provided the corresponding <c>AbortController</c> can be used to cancel an asynchronous action.</summary>
            abstract signal: AbortSignal option with get, set

/// <summary>
/// The <c>timer</c> module exposes a global API for scheduling functions to
/// be called at some future period of time. Because the timer functions are
/// globals, there is no need to call <c>require('timers')</c> to use the API.
/// 
/// The timer functions within Node.js implement a similar API as the timers API
/// provided by Web Browsers but use a different internal implementation that is
/// built around the Node.js <see href="https://nodejs.org/en/docs/guides/event-loop-timers-and-nexttick/#setimmediate-vs-settimeout">Event Loop</see>.
/// </summary>
/// <seealso href="https://github.com/nodejs/node/blob/v18.0.0/lib/timers.js">source</seealso>
module Timers =
    // type Abortable = Node:events.Abortable

    type [<AllowNullLiteral>] TimerOptions =
        inherit Events.EventEmitter.Abortable
        /// <summary>
        /// Set to <c>false</c> to indicate that the scheduled <c>Timeout</c>
        /// should not require the Node.js event loop to remain active.
        /// </summary>
        /// <default>true</default>
        abstract ref: bool option with get, set
module __timers_promises =
    /// <summary>
    /// The <c>timers/promises</c> API provides an alternative set of timer functions
    /// that return <c>Promise</c> objects. The API is accessible via<c>require('timers/promises')</c>.
    /// 
    /// <code lang="js">
    /// import {
    ///    setTimeout,
    ///    setImmediate,
    ///    setInterval,
    /// } from 'timers/promises';
    /// </code>
    /// </summary>
    let [<ImportAll("timers/promises")>] ``timers/promises``: Timers_promises.IExports = jsNative

    /// <summary>
    /// The <c>timers/promises</c> API provides an alternative set of timer functions
    /// that return <c>Promise</c> objects. The API is accessible via<c>require('timers/promises')</c>.
    /// 
    /// <code lang="js">
    /// import {
    ///    setTimeout,
    ///    setImmediate,
    ///    setInterval,
    /// } from 'timers/promises';
    /// </code>
    /// </summary>
    module Timers_promises =
        type TimerOptions = Timers.TimerOptions

        type [<AllowNullLiteral>] IExports =
            /// <summary>
            /// <code lang="js">
            /// import {
            ///    setTimeout,
            /// } from 'timers/promises';
            /// 
            /// const res = await setTimeout(100, 'result');
            /// 
            /// console.log(res);  // Prints 'result'
            /// </code>
            /// </summary>
            /// <param name="delay">The number of milliseconds to wait before fulfilling the promise.</param>
            /// <param name="value">A value with which the promise is fulfilled.</param>
            abstract setTimeout: ?delay: float * ?value: 'T * ?options: TimerOptions -> Promise<'T>
