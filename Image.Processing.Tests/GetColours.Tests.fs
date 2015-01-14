module GetColours.Tests

open System
open System.Drawing
open Xunit
open FsUnit.Xunit
open Image.Processing

[<Fact>]
let ``get colours from bitmap - should return 1 colour`` () =
    let pictures = create <| Size(10, 10, "test")
    
    let colours = (fill Color.Red pictures) |> getColours

    colours.Length
    |> should equal 1
    
[<Fact>]
let ``get colours from bitmap - should return 3 colour`` () =
    let pictures = create <| Size(10, 10, "test")
    
    let picture = pictures.Head
    picture.setPixel 0 0 Color.Red
    picture.setPixel 0 1 Color.Green
    picture.setPixel 0 2 Color.Blue

    let colours = pictures |> getColours

    // assuming default fill colour is not R, B or B then expect 4 colours
    colours.Length
    |> should equal 4
