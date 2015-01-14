module Resize.Tests

open System
open System.Drawing
open Xunit
open FsUnit.Xunit
open Image.Processing

[<Fact>]
let ``resize picture larger - expect to find correct new dimensions`` () =
    let pictures = create <| Size(10, 10, "test")

    let resized = 
        pictures
        |> resize 20 20 

    let picture = resized.Head

    picture.height
    |> should equal 20

    picture.width
    |> should equal 20

[<Fact>]
let ``resize picture smaller - expect to find correct new dimensions`` () =
    let pictures = create <| Size(10, 10, "test")

    let resized = 
        pictures
        |> resize 3 3 

    let picture = resized.Head

    picture.height
    |> should equal 3

    picture.width
    |> should equal 3