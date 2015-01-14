using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Image.Manipulation
{
    // reinterpretation of http://www.codeproject.com/Articles/1989/Image-Processing-for-Dummies-with-C-and-GDI-Part
    // with help from http://www.codeproject.com/Tips/240428/Work-with-bitmap-faster-with-Csharp
    public class FastBitmap : IDisposable
    {
	    private bool disposed;
        private int depth;
        private Bitmap bitmap;
        private BitmapData bitmapData;

        public FastBitmap(int width, int height, ImageLockMode imageLockMode = ImageLockMode.ReadWrite)
        {
            Initialize(new Bitmap(width, height), imageLockMode);
        }

        public FastBitmap(Bitmap bitmap, ImageLockMode imageLockMode = ImageLockMode.ReadWrite)
		{
            Initialize(bitmap, imageLockMode);
		}

        private void Initialize(Bitmap bmp, ImageLockMode imageLockMode)
        {
            bitmap = bmp;

            depth = System.Drawing.Image.GetPixelFormatSize(bitmap.PixelFormat);
            if (depth != 8 && depth != 24 && depth != 32)
            {
                throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
            }

            bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                imageLockMode, PixelFormat.Format32bppArgb);
        }

        public int Width
        {
            get { return bitmap != null ? bitmap.Width : 0; }
        }

        public int Height
        {
            get { return bitmap != null ? bitmap.Height : 0; }
        }

        public Color GetPixel(int x, int y)
        {
            var clr = Color.Empty;
            unsafe
            {
                var scan0 = bitmapData.Scan0;
                var p = (byte*)(void*)scan0;

                int count = depth / 8;

                int i = ((y * bitmap.Width) + x) * count;

                //if (i > (bitmap.Width * bitmap.Height * count) - count)
                //    throw new IndexOutOfRangeException();

                if (depth == 32)
                {
                    var b = p[i];
                    var g = p[i + 1];
                    var r = p[i + 2];
                    var a = p[i + 3]; 
                    clr = Color.FromArgb(a, r, g, b);
                } 
                else if (depth == 24)
                {
                    var b = p[i];
                    var g = p[i + 1];
                    var r = p[i + 2];
                    clr = Color.FromArgb(r, g, b);
                }
                else if (depth == 8)
                {
                    var c = p[i];
                    clr = Color.FromArgb(c, c, c);
                }
            }
            return clr;
        }

        public void SetPixel(int x, int y, Color color)
        {
            unsafe
            {
                var scan0 = bitmapData.Scan0;
                var p = (byte*)(void*)scan0;

                var count = depth/8;

                var i = ((y * bitmap.Width) + x) * count;

                if (depth == 32)
                {
                    p[i] = color.B;
                    p[i + 1] = color.G;
                    p[i + 2] = color.R;
                    p[i + 3] = color.A;
                }
                else if (depth == 24)
                {
                    p[i] = color.B;
                    p[i + 1] = color.G;
                    p[i + 2] = color.R;
                }
                else if (depth == 8)
                {
                    p[i] = color.B;
                }
            }
        }

    	public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					bitmap.UnlockBits(bitmapData);
				}

				disposed = true;
			}
		}

        public void Save(string filename)
        {
            bitmap.Save(filename);
        }

        public FastBitmap Clone()
        {
            var cloned = (bitmap != null)
                ? bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb)
                : null;
            return new FastBitmap(cloned);
        }

        public Bitmap Bitmap
        {
            get { return bitmap; }
        }

        public void ForEachPixel(Action<int, int, Color> action)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    action(x, y, GetPixel(x, y));
                }
            }
        }

        public void ForEachPixel(Func<int, int, Color, Color> func)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    SetPixel(x, y, func(x, y, GetPixel(x, y)));
                }
            }
        }
    }
}
