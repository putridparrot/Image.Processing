module GetWidth.Tests

open System
open System.Drawing
open Xunit
open FsUnit.Xunit
open Image.Processing

[<Fact>]
let ``get widths of pictures`` () =
    let pictures = seq {for i in 1 .. 5 do
                            yield create <| Size(i, i, i.ToString())
                    }
    let widths = pictures |> Seq.concat |> List.ofSeq |> getWidth

    for i in 0 .. widths.Length - 1 do
        widths.Item(i)
        |> should equal (i + 1)


