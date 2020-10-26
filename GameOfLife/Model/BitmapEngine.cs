using GameOfLife.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace GameOfLife.Model
{
    class BitmapEngine:ViewModelBase
    {
        private Image _SourceImage = new Image();

        public Image SourceImage 
        {
            get { return _SourceImage; }
            set { _SourceImage = value; OnPropertyChangedEvent(nameof(SourceImage)); } 
        }

        private RenderTargetBitmap _bmp;
        public RenderTargetBitmap bmp
        {
            get { return _bmp; }
            set { _bmp = value; OnPropertyChangedEvent(nameof(bmp)); }
        }

        private DrawingVisual drawingVisual = new DrawingVisual();
        private DrawingContext drawingContext;

        public void InitRenderer(int width, int heigth)
        {
            bmp = new RenderTargetBitmap(width, heigth, 96, 96, PixelFormats.Default);
        }

        public void RenderImage(int width, int heigth, Pixel[] pixels)
        {
            //DrawingVisual drawingVisual = new DrawingVisual();
            //DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext = drawingVisual.RenderOpen();
            
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < heigth - 1; y++)
                {
                    if (pixels[y * width + x].State == 1)
                    {
                        drawingContext.DrawRectangle(Brushes.Yellow, null, new Rect(x, y, 1, 1));
                    }
                    else 
                    {
                        drawingContext.DrawRectangle(Brushes.Black, null, new Rect(x, y, 1, 1));
                    }
                }
            }

            drawingContext.Close();
            //bmp = new RenderTargetBitmap(width, heigth, 96, 96, PixelFormats.Default);
            //bmp = new RenderTargetBitmap(width, heigth, 120, 96, PixelFormats.Default);//384
            bmp.Render(drawingVisual);
            //SourceImage.Source = bmp;
            
        }
    }
}
