module Create.Tests

open Xunit
open FsUnit.Xunit
open Image.Processing

[<Fact>]
let ``create picture a single item list is expected`` () =
    let pictures = create <| Size(10, 10, "test")
    pictures.Length 
    |> should equal 1

[<Fact>]
let ``create picture expect correct width`` () =
    let pictures = create <| Size(10, 10, "test")
    let picture = pictures.Head
    picture.width
    |> should equal 10

[<Fact>]
let ``create picture expect correct height`` () =
    let pictures = create <| Size(10, 10, "test")
    let picture = pictures.Head
    picture.height
    |> should equal 10
    
[<Fact>]
let ``create picture expect correct name`` () =
    let pictures = create <| Size(10, 10, "test")
    let picture = pictures.Head
    picture.name
    |> should equal "test"
