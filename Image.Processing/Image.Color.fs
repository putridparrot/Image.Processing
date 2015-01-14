module Image.Color

open System.Drawing

(*
    Implementation of the HSL (Hue, Saturation, Luminance) colour space.
    Allows us to interacte with Color objects, allowings us to alter hue,
    saturation and luminance, to offer functionality for lightening, darkening
    and so on, colours.

    Portions of this code provided by Bob Powell. http://www.bobpowell.net
*)
type Hsl() =
    let mutable _h : double = (double)0.0
    let mutable _s : double = (double)0.0
    let mutable _l : double = (double)0.0
    member this.h
        with get() = _h
        and set(value : double) = 
            _h <- max (min value 1.0) 0.0
    member this.s
        with get() = _s
        and set(value : double) = 
            _s <- max (min value 1.0) 0.0
    member this.l
        with get() = _l
        and set(value : double) = 
            _l <- max (min value 1.0) 0.0

let toHsl (colour : Color) =
    new Hsl(h=(double)(colour.GetHue()) / (double)360, s=(double)(colour.GetSaturation()), l=(double)(colour.GetBrightness()))

let toColor (hsl : Hsl) =
    if hsl.l = (double)0.0 then 
        Color.FromArgb(0, 0, 0)
    else if hsl.s = (double)0.0 then 
            Color.FromArgb((int)hsl.l * 255, (int)hsl.l * 255, (int)hsl.l * 255)
         else
            let temp2 = if hsl.l <= (double)0.5 then hsl.l * ((double)1.0 + hsl.s) else hsl.l + hsl.s - (hsl.l * hsl.s)
            let temp1 = (double)2.0 * hsl.l - temp2

            let mutable t3 = [|hsl.h + (double)1.0 / (double)3.0; hsl.h; hsl.h - (double)1.0 / (double)3.0|];
            let mutable rgb : double array = [|(double)0.0; (double)0.0; (double)0.0|];
            for i in 0 .. 2 do
                if t3.[i] < (double)0.0 
                then t3.[i] <- t3.[i] + (double)1.0;
                if t3.[i] > (double)1.0 
                then t3.[i] <- t3.[i] - (double)1.0;

                if ((double)6.0 * t3.[i]) < (double)1.0  
                then rgb.[i] <- (temp1 + (temp2 - temp1) * t3.[i] * (double)6.0)
                else if ((double)2.0 * t3.[i]) < (double)1.0 
                then rgb.[i] <- temp2
                else if ((double)3.0 * t3.[i]) < (double)2.0 
                then rgb.[i] <- (temp1 + (temp2 - temp1) * (((double)2.0 / (double)3.0) - t3.[i]) * (double)6.0)
                else rgb.[i] <- temp1

            Color.FromArgb((int)(rgb.[0] * (double)255), (int)(rgb.[1] * (double)255), (int)(rgb.[2] * (double)255))

/// Sets the absolute brightness of the colour
let setBrightness brightness (colour : Color) =
    let hsl = toHsl colour
    toColor (new Hsl(h=hsl.h, s=hsl.s, l=brightness))

/// Modifies and existing brightness level
/// To reduce the brightness use a number < 1, to increase brightness > 1
let modifyBrightness brightness (colour : Color) = 
    let hsl = toHsl colour
    toColor (new Hsl(h=hsl.h, s=hsl.s, l=hsl.l * brightness))

/// Sets the absolute saturation level, accepted values 0-1
let setSaturation saturation (colour : Color) =
    let hsl = toHsl colour
    toColor (new Hsl(h=hsl.h, s=saturation, l=hsl.l))

/// Modifies the saturation level, to reduce use a number < 1, the increase > 1
let modifySaturation saturation (colour : Color) = 
    let hsl = toHsl colour
    toColor (new Hsl(h=hsl.h, s=hsl.s * saturation, l=hsl.l))

/// Sets the abolsute hue level, accepted values 0-1
let setHues hue (colour : Color) =
    let hsl = toHsl colour
    toColor (new Hsl(h=hue, s=hsl.s, l=hsl.l))

/// Modifies the hue, to reduce use a number < 1, to increase use > 1
let modifyHue hue (colour : Color) = 
    let hsl = toHsl colour
    toColor (new Hsl(h=hsl.h * hue, s=hsl.s, l=hsl.l))

/// Lightens the supplied colour by the given brightness value
let lighten brightness (colour : Color) =
    modifyBrightness (brightness + (double)1) colour

/// darkens the supplied colour by the given darkness value
let darken darkness (colour : Color) =
    modifyBrightness ((double)1 - darkness) colour

