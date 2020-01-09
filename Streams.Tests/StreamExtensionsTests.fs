﻿namespace Tests.Streams

open System
open System.IO
open System.Text
open Stringier.Streams
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type StreamExtensionsTests() =
    [<TestMethod>]
    member _.``read byte - string`` () =
        use stream = new StringStream("helloworld!")
        CollectionAssert.AreEqual([|0x68uy;0x65uy;0x6Cuy;0x6Cuy;0x6Fuy|], stream.Read(5))

    [<TestMethod>]
    member _.``read/seek byte - string`` () =
        use stream = new StringStream("helloworld!")
        CollectionAssert.AreEqual([|0x68uy;0x65uy;0x6Cuy;0x6Cuy;0x6Fuy|], stream.Read(5))
        stream.Position <- 0L
        CollectionAssert.AreEqual([|0x68uy;0x65uy;0x6Cuy;0x6Cuy;0x6Fuy|], stream.Read(5))

    [<TestMethod>]
    member _.``read byte - file`` () =
        use stream = new FileStream("Test.txt", FileMode.Open)
        CollectionAssert.AreEqual([|0xEFuy;0xBBuy;0xBFuy|], stream.Read(3))
        CollectionAssert.AreEqual([|0x68uy;0x65uy;0x6Cuy;0x6Cuy;0x6Fuy|], stream.Read(5))

    [<TestMethod>]
    member _.``read/seek byte - file`` () =
        use stream = new FileStream("Test.txt", FileMode.Open)
        CollectionAssert.AreEqual([|0xEFuy;0xBBuy;0xBFuy|], stream.Read(3))
        CollectionAssert.AreEqual([|0x68uy;0x65uy;0x6Cuy;0x6Cuy;0x6Fuy|], stream.Read(5))
        stream.Position <- 0L
        CollectionAssert.AreEqual([|0xEFuy;0xBBuy;0xBFuy|], stream.Read(3))
        CollectionAssert.AreEqual([|0x68uy;0x65uy;0x6Cuy;0x6Cuy;0x6Fuy|], stream.Read(5))

    [<TestMethod>]
    member _.``read rune - string`` () =
        use stream = new StringStream("AöЖ€𝄞")
        Assert.AreEqual(Rune('A'), stream.ReadRune())
        Assert.AreEqual(Rune('ö'), stream.ReadRune())
        Assert.AreEqual(Rune('Ж'), stream.ReadRune())
        Assert.AreEqual(Rune('€'), stream.ReadRune())
        Assert.AreEqual(Rune(0x1D11E), stream.ReadRune())