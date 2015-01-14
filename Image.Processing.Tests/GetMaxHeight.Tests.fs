module GetMaxHeight.Tests

open System
open System.Drawing
open Xunit
open FsUnit.Xunit
open Image.Processing

[<Fact>]
let ``get max height of pictures`` () =
    let pictures = seq {for i in 1 .. 5 do
                            yield create <| Size(i, i + 1, i.ToString())
                    }
    let height = pictures |> Seq.concat |> List.ofSeq |> getMaxHeight

    height
    |> should equal 6