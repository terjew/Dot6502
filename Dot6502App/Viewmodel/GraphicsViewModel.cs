using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dot6502App.Viewmodel
{
    public enum GraphicsMode
    {
        Palette16Color,
        RGB332
    }

    class GraphicsViewModel : BindableBase
    {
        WriteableBitmap _bitmap;
        public WriteableBitmap Bitmap
        {
            get => _bitmap;
            set => SetProperty(ref _bitmap, value);
        }

        private int width;
        private int height;
        private GraphicsMode mode;
        public GraphicsViewModel(int width, int height, GraphicsMode mode)
        {
            this.width = width;
            this.height = height;
            this.mode = mode;

            _bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
        }

        public void Update(byte[] memory, int offset)
        {
            try
            {
                // Reserve the back buffer for updates.
                _bitmap.Lock();

                unsafe
                {
                    fixed (byte* bytePtr = memory)
                    {
                        byte* srcPtr = bytePtr + offset;
                        IntPtr dstPtr = _bitmap.BackBuffer;

                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                byte src = *srcPtr;
                                var rgb = ToRgb(src);

                                *((int*)dstPtr) = rgb;

                                srcPtr += 1;
                                dstPtr += 4;
                            }
                            //srcPtr += width;
                            //dstPtr += _bitmap.BackBufferStride;
                        }
                    }

                }

                // Specify the area of the bitmap that changed.
                _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            }
            finally
            {
                // Release the back buffer and make it available for display.
                _bitmap.Unlock();
            }
        }

        private int ToRgb(byte src)
        {
            var r = src & 0b11100000;
            var g = (src & 0b00011100) << 3;
            var b = (src & 0b00000011) << 6;

            // Compute the pixel's color.
            int color_data = 255 << 24;
            color_data |= r << 16; // R
            color_data |= g << 8;   // G
            color_data |= b << 0;   // B

            return color_data;
        }
    }
}
