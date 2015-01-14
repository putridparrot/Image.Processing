module Picture.Tests

open System
open Xunit
open FsUnit.Xunit
open Image.Common

[<Fact>]
let ``create picture by size - check width matches`` () =
    let picture = new Picture(20, 30)

    picture.width 
    |> should equal 20

[<Fact>]
let ``create picture by size - check height matches`` () =
    let picture = new Picture(20, 30)

    picture.height 
    |> should equal 30
