﻿namespace Stringier.Streams

open System
open System.IO
open Bindings

[<AutoOpen>]
module StreamExtensions =
    /// <summary>
    /// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream. Instead of calling this method, ensure that the stream is properly disposed.
    /// </summary>
    let close(stream:#stream):unit = stream.Close()

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    let dispose(stream:#IDisposable):unit = stream.Dispose()

    /// <summary>
    /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
    /// </summary>
    let flush(stream:#stream):#stream =
        stream.Flush()
        stream

    /// <summary>
    /// When overridden in a derived class, sets the position within the current stream.
    /// </summary>
    let seek(offset:int64)(origin:origin)(stream:#stream):#stream =
        match origin with
        | Begin -> stream.Seek(offset, SeekOrigin.Begin) |> ignore
        | Current -> stream.Seek(offset, SeekOrigin.Current) |> ignore
        | End -> stream.Seek(offset, SeekOrigin.End) |> ignore
        stream

    /// <summary>
    /// Writes the data to the current position in the stream, and advances the position within the stream.
    /// </summary>
    let inline (<=<)(stream:#stream)(data:^d):#stream = write<Binder, #stream, ^d>(stream)(data)

