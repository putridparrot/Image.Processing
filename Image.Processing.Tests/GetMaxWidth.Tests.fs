module GetMaxWidth.Tests

open System
open System.Drawing
open Xunit
open FsUnit.Xunit
open Image.Processing

[<Fact>]
let ``get max width of pictures`` () =
    let pictures = seq {for i in 1 .. 5 do
                            yield create <| Size(i + 1, i, i.ToString())
                    }
    let width = pictures |> Seq.concat |> List.ofSeq |> getMaxWidth

    width
    |> should equal 6