using Dot6502App.Model;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dot6502App.Viewmodel
{
    public enum GraphicsMode
    {
        Palette16,
        RGB332
    }

    class GraphicsViewModel : BindableBase
    {
        private EmulationModel executionModel;

        private uint[] colormap = new []
        {
            0xff000000,//Black
            0xffffffff,//White
            0xff880000,//Red
            0xffaaffee,//Cyan
            0xffcc44cc,//Purple
            0xff00cc55,//Green
            0xff0000aa,//Blue
            0xffeeee77,//Yellow
            0xffdd8855,//Orange
            0xff664400,//Brown
            0xffff7777,//Light red
            0xff333333,//Dark gray
            0xff777777,//Gray
            0xffaaff66,//Light green
            0xff0088ff,//Light blue
            0xffbbbbbb,//Light gray
        };

        WriteableBitmap _bitmap;
        public WriteableBitmap Bitmap
        {
            get => _bitmap;
            set => SetProperty(ref _bitmap, value);
        }

        public List<int> ResolutionValues { get; } = new List<int>() { 32, 64, 96, 128 };
        public List<GraphicsMode> ModeValues { get; } = new List<GraphicsMode>() { GraphicsMode.Palette16, GraphicsMode.RGB332 };
        public List<BitmapScalingMode> ScalingModes { get; } = new List<BitmapScalingMode>() { BitmapScalingMode.HighQuality, BitmapScalingMode.LowQuality, BitmapScalingMode.NearestNeighbor };

        private GraphicsMode mode;
        public GraphicsMode Mode
        {
            get => mode;
            set => SetProperty(ref mode, value);
        }

        private BitmapScalingMode scalingMode;
        public BitmapScalingMode ScalingMode
        {
            get => scalingMode;
            set => SetProperty(ref scalingMode, value);
        }

        private int width;
        public int Width
        {
            get => width;
            set => SetProperty(ref width, value);
        }

        private int height;
        public int Height
        {
            get => height;
            set => SetProperty(ref height, value);
        }

        private int offset;

        public int Offset
        {
            get => offset;
            set => SetProperty(ref offset, value);
        }

        public GraphicsViewModel(EmulationModel executionModel, int offset, int width, int height)
        {
            ScalingMode = BitmapScalingMode.NearestNeighbor;
            Mode = GraphicsMode.Palette16;

            Width = width;
            Height = height;
            Mode = mode;

            PropertyChanged += GraphicsViewModel_PropertyChanged;

            this.offset = offset;
            this.executionModel = executionModel;
            executionModel.Frame += ExecutionModel_Frame;
            executionModel.Loaded += ExecutionModel_Loaded;
        }

        private void ExecutionModel_Loaded(object sender, EventArgs e)
        {
            Initialize();
        }

        private void ExecutionModel_Frame(object sender, int e)
        {
            DrawFrame();
        }

        private void Initialize()
        {
            Bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
            DrawFrame();
        }

        private void GraphicsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "" ||
                e.PropertyName == nameof(Width) ||
                e.PropertyName == nameof(Height) ||
                e.PropertyName == nameof(Mode) ||
                e.PropertyName == nameof(EmulationModel)
            )
            {
                Initialize();
            }
        }

        public void DrawFrame()
        {
            if (executionModel?.State == null) return;
            try
            {
                // Reserve the back buffer for updates.
                _bitmap.Lock();

                unsafe
                {
                    fixed (byte* bytePtr = executionModel.State.Memory)
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
            if (mode == GraphicsMode.RGB332)
            {
                var r = src & 0b11100000;
                var g = (src & 0b00011100) << 3;
                var b = (src & 0b00000011) << 6;

                int color_data = 255 << 24; //A
                color_data |= r << 16; // R
                color_data |= g << 8; // G
                color_data |= b << 0; // B

                return color_data;
            }
            return (int)colormap[src & 0x0f];
        }
    }
}
