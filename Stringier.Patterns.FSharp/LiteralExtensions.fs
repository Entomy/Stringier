﻿namespace System.Text.Patterns

open System
open System.Text.Patterns.Bindings

[<AutoOpen>]
module LiteralExtensions =

    type Binder =
        static member Literl(value:String) = PatternBindings.Literal(value)
        static member Literl(value:char) = PatternBindings.Literal(value)

    let inline literl< ^t, ^a, ^b when (^t or ^a) : (static member Literl : ^a -> ^b)> value = ((^t or ^a) : (static member Literl : ^a -> ^b)(value))

    let inline p value = literl<Binder, _, _> value