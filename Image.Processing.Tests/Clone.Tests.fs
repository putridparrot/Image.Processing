module Clone.Tests

open System
open Xunit
open FsUnit.Xunit
open Image.Processing

[<Fact>]
let ``clone picture should not be the same instance as the original`` () =
    let pictures = create <| Size(10, 10, "test")
    let cloned = clone pictures
    Object.ReferenceEquals(pictures, cloned)
    |> should equal false
    Object.ReferenceEquals(pictures.Head, cloned.Head)
    |> should equal false

[<Fact>]
let ``clone picture a single item list is expected`` () =
    let pictures = create <| Size(10, 10, "test")
    let cloned = clone pictures
    cloned.Length 
    |> should equal 1

[<Fact>]
let ``clone picture expect correct width`` () =
    let pictures = create <| Size(10, 10, "test")
    let cloned = clone pictures
    cloned.Head.width
    |> should equal 10

[<Fact>]
let ``clone picture expect correct height`` () =
    let pictures = create <| Size(10, 10, "test")
    let picture = pictures.Head
    let cloned = clone pictures
    cloned.Head.height
    |> should equal 10
    
[<Fact>]
let ``clone picture expect correct name`` () =
    let pictures = create <| Size(10, 10, "test")
    let picture = pictures.Head
    let cloned = clone pictures
    cloned.Head.name
    |> should equal "test"
