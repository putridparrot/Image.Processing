module GetHeight.Tests

open System
open System.Drawing
open Xunit
open FsUnit.Xunit
open Image.Processing

[<Fact>]
let ``get heights of pictures`` () =
    let pictures = seq {for i in 1 .. 5 do
                            yield create <| Size(i, i, i.ToString())
                    }
    let heights = pictures |> Seq.concat |> List.ofSeq |> getHeight

    for i in 0 .. heights.Length - 1 do
        heights.Item(i)
        |> should equal (i + 1)


