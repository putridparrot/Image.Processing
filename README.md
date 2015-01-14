# F# Image.Processing

The Image.Processing library was developed to solve a very specific problem. 

I was working on a bunch of bitmaps of numbers and letters. Each bitmap ended up being a slightly different in size and the characters were not centred, plus I needed to turn the images into pure black and white. Not knowing how to do these things with an application such as GIMP and being a programmer, I figured why not write my own processing code.

What I really wanted was a DSL to handle the processing of the images which could be run interactively or via a script file. F# seemed the perfect choice. Not least because I wanted to find a project to help learn it. 

So this is what I came up with. 

### Example

Let's see an example of the actual requirement I had - So I want to load all the .bmp files from a folder, turn them into black and which then resize all images to the same size. We'll need to find the largest image width and largest image height and use these to resize the images. Then save the images. The library currently saves the images to new_Filename.bmp (where Filename.bmp is obviously the original filename). Just to show one other thing, I also turn non-Black colours in the image yellow.

```
let pics = create <| Folder(@"c:\AlphaNumeric\Images", "*.bmp")

makeBlackAndWhite (Color.Black.ToArgb() / 2) pics
|> copyRectangle (getBoundedRectangle (Include [Color.Black]) pics)
|> resize (getMaxWidth pics) (getMaxHeight pics)
|> replaceColour (Exclude [Color.Black]) Color.Yellow
|> save
|> ignore
```


### Library license

The library is available under the MIT license. See the [License file][1] in the GitHub repository.

  [1]: https://github.com/putridparrot/Image.Processing/blob/master/LICENSE

