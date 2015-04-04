module Image.Common

open System.Drawing
open System.IO
open Image.Manipulation

type Picture =
    val mutable _name : string
    val mutable _bitmap : Bitmap
    val mutable _fastBitmap : FastBitmap
        
    new () = { 
        _name = null
        _bitmap = null 
        _fastBitmap = null
    }

    new (width : int, height : int, name) = { 
        _bitmap = new Bitmap(width, height)
        _name = name 
        _fastBitmap = null
    }

    new (width, height) = new Picture(width, height, null)

    new (filename : string) = 
        // when creating a bitmap from a file we cannot change it, so need to clone it 
        let temp = new Bitmap(filename)
        let cloned = temp.Clone(new Rectangle(0, 0, temp.Width, temp.Height), Imaging.PixelFormat.Format32bppArgb)
        {
            _bitmap = cloned
            _name = filename
            _fastBitmap = null
        }

    private new (name, bitmap : Bitmap) = 
        {
            _bitmap = bitmap
            _name = name
            _fastBitmap = null
        }

    member this.name
        with get() = this._name
        and set(value : string) =
            this._name <- value
    member this.width
        with get() = 
            if this._bitmap <> null 
            then this._bitmap.Width else 0
    member this.height
        with get() = 
            if this._bitmap <> null 
            then this._bitmap.Height else 0
    member this.save = 
        let currentDir = Path.GetDirectoryName this.name
        let newDir = currentDir + "\\new\\"
        if not (Directory.Exists(newDir))
        then Directory.CreateDirectory(newDir) |> ignore
        let newFilename = newDir + Path.GetFileName this.name
        if File.Exists(newFilename) 
        then File.Delete(newFilename)
        this._bitmap.Save(newFilename)
    member this.cloneRectangle x y width height =
        new Picture(this._name, this._bitmap.Clone(new Rectangle(x, y, width, height), Imaging.PixelFormat.Format32bppArgb))
    member this.clone = 
        this.cloneRectangle 0 0 this.width this.height
    member this.getPixel x y =
        if this._fastBitmap <> null 
        then this._fastBitmap.GetPixel(x, y)
        else this._bitmap.GetPixel(x, y)
    member this.setPixel x y (colour : Color) =
        if this._fastBitmap <> null 
        then this._fastBitmap.SetPixel(x, y, colour)
        else this._bitmap.SetPixel(x, y, colour)
    member this.image : Image =
        this._bitmap :> Image
    member this.beginUpdate =
        if this._fastBitmap = null then
            this._fastBitmap <- new FastBitmap(this._bitmap)
    member this.endUpdate =
        if this._fastBitmap <> null then
            this._fastBitmap.Dispose()

type Pictures = Picture list