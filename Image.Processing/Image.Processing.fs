module Image.Processing

open System.IO
open System.Drawing
open Image.Common
open Image.Color
open Image.Manipulation

/// Allows use of a single create function to create picturs based upon a 
/// single filename, a list of filenames or a folder with wildcard
type CreateWith =
    | Filename of string
    | Filenames of string list
    | Folder of string * string
    | Size of int * int * string
    | Multiple of CreateWith * CreateWith
    
/// Allows us to include or exclude color
type ColorFilterOption = 
    | Include of Color list
    | Exclude of Color list

type IterationDirection =
    | X of int * int * int
    | Y of int * int * int

let rec private forEachOnAxis current step last exit (pic : Picture) = 
    if current <= last && not(exit current pic) then
        forEachOnAxis (current + step) step last exit pic

/// Loops through each pixel until an exit condition is met
let forEachPixelUntil direction exit (pic : Picture) =
    match direction with
    | X (first, step, last) -> forEachOnAxis first step last exit pic |> ignore
    | Y (first, step, last) -> forEachOnAxis first step last exit pic |> ignore
let private createPicture (filename : string) = 
    new Picture(filename)

let private createEmptyBitmap (width : int) (height : int) name =
    [new Picture(width, height, name)]

/// Creates a single item list containing the Picture for the supplied filename
let private createBitmap (filename : string) =
    [createPicture filename]

// Creates a list of Picture types based upon the supplied filenames
let private createBitmaps filenames =
    List.map (fun filename -> (createBitmap filename).Head) filenames

/// Creates one or more images from the supplied folder using the supplied wildcard
let private createBitmapsFromFolder folder wildcard =
    Directory.GetFiles(folder, wildcard) 
    |> Array.toList
    |> createBitmaps

/// Creates a list of Picture types either from a single filename, a list of filenames 
/// or from a folder with wildcard
let create param =
    let rec build param (acc : Pictures) = 
        match param with
        | Filename filename -> createBitmap filename
        | Filenames filenames -> createBitmaps filenames
        | Folder (folder, wildcard) -> createBitmapsFromFolder folder wildcard
        | Size (width, height, name) -> createEmptyBitmap width height name
        | Multiple (a, b) -> build a [] @ build b []

    build param []
        
/// Save the Pictures
let save (pics : Picture list) =
    List.iter(fun (pic : Picture) -> pic.save) pics
    pics

/// clones the pictures and the bitmaps currently within the picture type
let clone (pics : Pictures) =
    List.map (fun (pic : Picture) -> pic.clone) pics    

/// Fill the pictures with a single colour
let fill (colour : Color) (pics : Picture list) =
    List.iter (fun (pic : Picture) -> 
        use g = Graphics.FromImage(pic.image)
        use b = new SolidBrush(colour)
        g.FillRectangle(b, 0, 0, pic.width, pic.height)) pics
    pics

/// Resize pictures to the supplied width and height
let resize (width : int) (height : int) (pics : Picture list) =
    List.map (fun (pic : Picture) ->
        let p = new Picture(width, height, pic.name)
        use g = Graphics.FromImage(p.image)
        g.DrawImage(pic.image, (width - pic.width) / 2, (height - pic.height) / 2)
        p) pics

/// Resize pictures to the supplied width and height and fill the background with the supplied colour
let resizeAndFill (width : int) (height : int) (colour: Color) (pics : Picture list) =
    List.map (fun (pic : Picture) ->
        let p = new Picture(width, height, pic.name)
        use g = Graphics.FromImage(p.image)
        use b = new SolidBrush(colour)
        g.FillRectangle(b, 0, 0, pic.width, pic.height)
        g.DrawImage(pic.image, (width - pic.width) / 2, (height - pic.height) / 2)
        p) pics

let private getColoursFromPicture (pic : Picture) =
    pic.beginUpdate
    let colours = 
        seq {for x in 0 .. pic.width - 1 do
                for y in 0 .. pic.height - 1 do
                    yield pic.getPixel x y
    }
    pic.endUpdate
    Seq.distinct colours

/// Get a list of the colours used in the image
let getColours (pics : Picture list) = 
    let colours = 
        Seq.map (fun (pic : Picture) -> getColoursFromPicture pic) pics
        |> Seq.concat

    List.ofSeq (Seq.distinct colours)

/// Iterates each pixel on each picture
let forEachPixel f (pic : Picture) =
    let width = pic.width
    let height = pic.height
    for x in 0 .. width - 1 do
        for y in 0 .. height - 1 do
            f x y pic        
    pic

/// Iterates over each pixel passing the supplied function the x, y coordinates and
/// the colour of the pixel. The supplied function may return a colour to alter the pixel
let forEachPixelDo f (pic : Picture) =
    pic.beginUpdate
    forEachPixel (fun x y (p : Picture) ->
        f x y (p.getPixel x y)
        ) pic |> ignore
    pic.endUpdate
    pic

/// alters images to a two tone where anything lighter than the midColour is set to the
/// supplied lightColour otherwise the supplied darkerColour is used
let makeTwoTone midColour lighterColour darkerColour (pics : Picture list) =
    List.map (fun (pic : Picture) -> 
        pic.beginUpdate
        forEachPixel (fun x y (p : Picture) -> 
                if (pic.getPixel x y).ToArgb() < midColour 
                then (pic.setPixel x y darkerColour)
                else (pic.setPixel x y lighterColour)
            ) pic
        ) pics

/// Specialisation of the makeTwoTone which simply turns an image into B&W
let makeBlackAndWhite midColour (pics : Picture list) =   
     makeTwoTone midColour Color.White Color.Black pics

/// Get the widths of the supplied Pictures
let getWidth (pics : Picture list) =
    List.map (fun (pic : Picture) -> pic.width) pics

/// Get the heights of the supplied Pictures
let getHeight (pics : Picture list) =
    List.map (fun (pic : Picture) -> pic.height) pics

/// Get the left coordinate of the supplied colour
/// Note: Combined option not used for the filter
let getLeft filter (pic : Picture) =
    let width = pic.width
    let height = pic.height
    let mutable left = -1
    pic.beginUpdate
    for x in 0 .. width - 1 do
        for y in 0 .. height - 1 do
            match filter with
            | Include colours -> 
                if  List.exists (fun (c : Color) -> c.ToArgb() = (pic.getPixel x y).ToArgb()) colours 
                then if left = -1 || x < left 
                        then left <- x
            | Exclude colours -> 
                if  List.exists (fun (c : Color) -> c.ToArgb() <> (pic.getPixel x y).ToArgb()) colours 
                then if left = -1 || x < left 
                        then left <- x
    pic.endUpdate
    left

/// Get the top coordinate of the supplied colour
/// Note: Combined option not used for the filter
let getTop filter (pic : Picture) =
    let width = pic.width
    let height = pic.height
    let mutable top = -1
    pic.beginUpdate
    for x in 0 .. width - 1 do
        for y in 0 .. height - 1 do
            match filter with
            | Include colours -> 
                if  List.exists (fun (c : Color) -> c.ToArgb() = (pic.getPixel x y).ToArgb()) colours 
                then if top = -1 || x < top
                        then top <- x
            | Exclude colours -> 
                if  List.exists (fun (c : Color) -> c.ToArgb() <> (pic.getPixel x y).ToArgb()) colours 
                then if top = -1 || x < top
                        then top <- x
    pic.endUpdate
    top

/// Get the top coordinate of the supplied colour
/// Note: Combined option not used for the filter
let getBottom filter (pic : Picture) =
    let width = pic.width
    let height = pic.height
    let mutable bottom = -1
    pic.beginUpdate
    for x in 0 .. width - 1 do
        for y in height - 1 .. -1 .. 0 do
            match filter with
            | Include colours -> 
                if  List.exists (fun (c : Color) -> c.ToArgb() = (pic.getPixel x y).ToArgb()) colours 
                then if bottom = -1 || y > bottom
                        then bottom <- y
            | Exclude colours -> 
                if  List.exists (fun (c : Color) -> c.ToArgb() <> (pic.getPixel x y).ToArgb()) colours 
                then if bottom = -1 || y > bottom
                        then bottom <- y
    pic.endUpdate
    bottom

/// Get the right coordinate of the supplied colour
/// Note: Combined option not used for the filter
let getRight filter (pic : Picture) =
    let width = pic.width
    let height = pic.height
    let mutable right = -1
    pic.beginUpdate
    for x in width - 1 .. -1 .. 0 do
        for y in 0 .. height - 1 do
            match filter with
            | Include colours -> 
                if  List.exists (fun (c : Color) -> c.ToArgb() = (pic.getPixel x y).ToArgb()) colours 
                then if right = -1 || x > right
                        then right <- x
            | Exclude colours -> 
                if  List.exists (fun (c : Color) -> c.ToArgb() <> (pic.getPixel x y).ToArgb()) colours 
                then if right = -1 || x > right
                        then right <- x                   
    pic.endUpdate
    right

let getBoundedRectangle filter (pics : Picture list) =
    List.map (fun pic -> 
        let left = getLeft filter pic
        let top = getTop filter pic
        let right = getRight filter pic
        let bottom = getBottom filter pic
        new Rectangle(left, top, right - left, bottom - top)) pics 

/// Get the maximum width from the list of Pictures
let getMaxWidth (pics : Picture list) =
    let result = pics |> List.maxBy (fun p -> p.width)
    result.width

/// Get the maximum height from the list of Pictures
let getMaxHeight (pics : Picture list) =
    let result = pics |> List.maxBy (fun p -> p.height)
    result.height
    /// Crop all pictures to the given width and height. Does not scale Picture, just crops it
let crop width height (pics : Picture list) =
    List.map (fun (pic : Picture) -> resize width, height, pic) pics
 
/// Copies a rectangle from the supplied picture creating a new list of pictures
let copy (r : Rectangle) (pics : Picture list) =
    List.map (fun (pic : Picture) -> pic.cloneRectangle r.Left r.Top r.Width r.Height) pics
    
/// Copies a rectangle (multiple may be supplied, i.e. one per picture) and create new pictures
let copyRectangle (rect : Rectangle list) (pics : Picture list) =
    Seq.zip rect pics 
    |> Seq.map (fun tuple -> 
                    let r, pic = tuple
                    pic.cloneRectangle r.Left r.Top r.Width r.Height)
    |> List.ofSeq

// might be worth trying this http://msdn.microsoft.com/en-us/library/4b4dc1kz.aspx
let private replaceThisColour (originalColour : Color) (newColour : Color) (pics : Picture list) =
    List.map (fun (pic : Picture) -> 
            pic.beginUpdate
            forEachPixel (fun x y (p : Picture) ->
                if (pic.getPixel x y).ToArgb() = originalColour.ToArgb() 
                then (pic.setPixel x y newColour))
                    pic |> ignore
            pic.endUpdate
        ) pics

let private replaceExceptThisColour (noReplaceColour : Color) (newColour : Color) (pics : Picture list) =
    List.map (fun (pic : Picture) -> 
            pic.beginUpdate
            forEachPixel (fun x y (p : Picture) ->
                if (pic.getPixel x y).ToArgb() <> noReplaceColour.ToArgb() 
                then (pic.setPixel x y newColour))
                    pic |> ignore
            pic.endUpdate
        ) pics

/// Replaces one or more colours with a new colour, using the filter to either Include colours to be 
/// replaced or exclude colours to be replaced
///
/// for example:     
/// let colours = Combined(Exclude [Color.Black], Include [Color.Yellow])
/// pics |> replaceColour colours Color.Red
let replaceColour filter (newColour : Color) (pics : Picture list) =
    match filter with
    | Include colours -> List.map (fun c -> replaceThisColour c newColour pics) colours |> ignore
    | Exclude colours -> List.map (fun c -> replaceExceptThisColour c newColour pics) colours |> ignore
    pics

/// lighten the whole image by a certain amount
let lighten brightness (pics : Picture list) =
    List.map (fun (pic : Picture) ->
        pic.beginUpdate
        forEachPixel (fun x y (p : Picture) ->
            let colour = lighten brightness (pic.getPixel x y)
            pic.setPixel x y colour)
                pic |> ignore
        pic.endUpdate
        ) pics

/// darken the whole image by a certain amount
let darken brightness (pics : Picture list) =
    List.map (fun (pic : Picture) ->
        pic.beginUpdate
        forEachPixel (fun x y (p : Picture) ->
            let colour = darken brightness (pic.getPixel x y)
            pic.setPixel x y colour)
                pic |> ignore
        pic.endUpdate
        ) pics
