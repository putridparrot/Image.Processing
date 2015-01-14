module MakeBlackAndWhite.Tests

open System
open System.Drawing
open Xunit
open FsUnit.Xunit
open Image.Processing

[<Fact>]
let ``turn a picture into a b&w picture`` () =
    let pictures = create <| Size(12, 12, "test")
    let picture = pictures.Head

    for x in 0 .. 3 do
        for y in 0 .. picture.height - 1 do
            picture.setPixel x y Color.Yellow

    for x in 3 .. 9 do
        for y in 0 .. picture.height - 1 do
            picture.setPixel x y Color.Red

    for x in 9 .. 11 do
        for y in 0 .. picture.height - 1 do
            picture.setPixel x y Color.DarkBlue

    let colours = getColours pictures
    // just ensure that the colours were applied
    colours.Length
    |> should equal 3

    let twoTone = makeBlackAndWhite (Color.Red.ToArgb() / 2) pictures
    let newColours = getColours pictures

    newColours.Length
    |> should equal 2

    newColours.Head.ToArgb()
    |> should equal (Color.White.ToArgb())

    newColours.Item(1).ToArgb()
    |> should equal (Color.Black.ToArgb())
