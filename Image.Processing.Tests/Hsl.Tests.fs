module Hsl.Tests

open System
open System.Drawing
open Xunit
open FsUnit.Xunit
open Image.Color

[<Fact>]
let ``convert Color to Hsl and back`` () =
    let hsl = toHsl Color.Red

    let colour = toColor hsl
    colour.ToArgb()
    |> should equal (Color.Red.ToArgb())

[<Fact>]
let ``change brightness and back`` () =
    let originalColor = Color.Red
    let hsl = toHsl originalColor

    let c1 = setBrightness 0.5 originalColor 
    let c2 = setBrightness hsl.l c1

    c2.ToArgb()
    |> should equal (originalColor.ToArgb())

//[<Fact>]
//let ``modify brightness and back`` () =
//    let originalColor = Color.Red
//    let hsl = toHsl originalColor
//
//    let c1 = modifyBrightness 1.5 originalColor 
//    let c2 = modifyBrightness 1.0 c1
//
//    c2.ToArgb()
//    |> should equal (originalColor.ToArgb())