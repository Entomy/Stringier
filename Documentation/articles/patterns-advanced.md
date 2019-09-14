﻿# Stringier.Patterns Advanced Concepts

## Checkers

Checkers are a special pattern type which are not meant for normal use. Unlike the typical case where the pattern describes what to match, a checker is a function which is ran against a candidate when determining a match. This is substantially faster and should be viewed as hand writing parts of the resulting parser.

### CharChecker

~~~~csharp
Pattern patternName = (Pattern)(nameof(patternName), (Char) => { return Boolean; });
~~~~
~~~~fsharp
// This isn't supported in F# yet
~~~~

`nameof()` can be any string, but this is recommended.

The function lambda must take a `Char` as a parameter and return a `Boolean`, but otherwise you are free to declare whatever logic desired.

A large amount of predefined patterns are actually implemented this way.

### EndChecker

~~~~csharp
Pattern endOfSource = Source.End;
~~~~
~~~~fsharp
let endOfSource = Source.End;
~~~~

Checks that the parser is currently at the end of the source, setting a `ConsumeParserError` if not. This is essentially a highly specialized lookahead.


## Self Optimization

**Stringier.Patterns** holds a unique and rare feature: it is self optimizing. This means that you do not need to be intimately aware of what the fastest way to do something is; generally speaking the pattern will become, at construction time (that is, when what you declared is initialized), the most optimal equivalent declaration. This is not an exhaustive description of these optimizations, but merely seeks to explain the approach taken.

Typically, patterns are declared with operators that combine or modify them. These operators call methods which are dispatching. These all have "reasonable default" behaviors defined, but are overridden in certain cases to provide specific optimizations.

Consider the case of a combinator of literals such as `"Hello" & ' ' & "there"`. Normally this would be implemented as the following:

~~~~
Concatenator
├─Concatenator
│ ├─"Hello"
│ └─' '
└─"there"
~~~~

Notice however, the lefthand and righthand components are actually literals. All literals are either `Char` or `String` which means instead of pattern concatenation, we can use string concatenation. Any time a concatenation of two literals is called, instead of constructing a concatenator, we can construct a `StringLiteral` which contains the lefthand and righthand components after string concatenation. This gives us the following:

~~~~
Concatenator
├─"Hello "
└─"there"
~~~~

And suddenly we have the same situation, which can further be reduced into the following:

~~~~
"Hello there"
~~~~

This has the obvious advantage of reducing memory usage, as the tree has been reduced down to a single node. There is also the not so obvious advantage in that matching a literal is slightly faster than matching a concatenator.