using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GameOfLife.ViewModel
{
    public class QuadPixel : UIElement
    {

        protected override void OnRender(DrawingContext drawingContext)
        {
            const int width = 1;
            const int heigth = 1;

            drawingContext.DrawRectangle(Brushes.DarkBlue, null, new Rect(0, 0, width, heigth));
        }
    }
}
