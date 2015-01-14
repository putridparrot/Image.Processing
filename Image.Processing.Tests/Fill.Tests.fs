module Fill.Tests

open System
open System.Drawing
open Xunit
open FsUnit.Xunit
open Image.Processing

[<Fact>]
let ``fill picture with single colour`` () =
    let pictures = create <| Size(10, 10, "test")

    pictures
    |> fill Color.Red
    |> ignore

    let picture = pictures.Head

    for x in 0 .. picture.width - 1 do
        for y in 0 .. picture.height - 1 do
            (picture.getPixel x y).ToArgb() 
            |> should equal (Color.Red.ToArgb())